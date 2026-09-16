using UnityEngine;

public class AttackState : State
{
    private HunterNPC hunter;
    private Boid targetBoid;

    public AttackState(FSM fsm, HunterNPC hunter, Boid targetBoid) : base(fsm)
    {
        this.hunter = hunter;
        this.targetBoid = targetBoid;
    }

    public override void Update()
    {
        // Si el objetivo ya no existe o murió
        if (targetBoid == null || targetBoid.IsDead)
        {
            fsm.ChangeState(new PatrolState(fsm, hunter));
            return;
        }

        Vector3 offset = targetBoid.transform.position - hunter.transform.position;
        float sqrDist = offset.sqrMagnitude;
        float visionRadiusSqr = hunter.VisionRadius * hunter.VisionRadius;

        // Si se sale del rango de visión, vuelve a Patrol SIN reiniciar el TBA
        if (sqrDist > visionRadiusSqr)
        {
            fsm.ChangeState(new PatrolState(fsm, hunter));
            return;
        }

        float meleeSqr = hunter.MeleeAttackRadius * hunter.MeleeAttackRadius;
        float rangeSqr = hunter.RangeAttackRadius * hunter.RangeAttackRadius;

        // 1. Ataque Cuerpo a Cuerpo
        if (sqrDist <= meleeSqr)
        {
            ExecuteAttack(100f); // Inflige daño suficiente para eliminarlo
            return;
        }

        // 2. Ataque a Distancia
        if (sqrDist <= rangeSqr)
        {
            ExecuteAttack(50f);
            return;
        }

        // 3. Fuera de rangos de ataque: Perseguir con Seek
        Vector3 seekForce = SteeringBehaviours.Seek(hunter, targetBoid.transform.position);
        hunter.ApplySteering(seekForce);
    }

    private void ExecuteAttack(float damage)
    {
        targetBoid.TakeDamage(damage);
        hunter.ResetAttackTimer(); // Reinicia el TBA tras un ataque exitoso
        fsm.ChangeState(new PatrolState(fsm, hunter));
    }
}