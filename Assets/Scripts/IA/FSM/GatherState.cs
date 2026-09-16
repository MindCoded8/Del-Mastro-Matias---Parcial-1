using UnityEngine;

public class GatherState : State
{
    private HunterNPC hunter;
    private Boid targetBoid;
    private float gatherTimer;

    public GatherState(FSM fsm, HunterNPC hunter, Boid targetBoid) : base(fsm)
    {
        this.hunter = hunter;
        this.targetBoid = targetBoid;
    }

    public override void Update()
    {
        if (targetBoid == null || !targetBoid.IsDead)
        {
            fsm.ChangeState(new PatrolState(fsm, hunter));
            return;
        }

        Vector3 offset = targetBoid.transform.position - hunter.transform.position;
        float sqrDist = offset.sqrMagnitude;

        // Si aún no está a distancia de recolección, se acerca con Arrive
        if (sqrDist > 1.5f)
        {
            Vector3 arriveForce = SteeringBehaviours.Arrive(hunter, targetBoid.transform.position, 1f);
            hunter.ApplySteering(arriveForce);
        }
        else
        {
            // Ejecuta la acción de recolección durante un tiempo
            gatherTimer += Time.deltaTime;
            if (gatherTimer >= hunter.GatherDuration)
            {
                // Al finalizar, el agente recolectado desaparece para el respawn
                targetBoid.gameObject.SetActive(false);
                fsm.ChangeState(new PatrolState(fsm, hunter));
            }
        }
    }
}