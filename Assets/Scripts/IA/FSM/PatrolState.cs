using UnityEngine;

public class PatrolState : State
{
    private HunterNPC hunter;
    private int currentWaypointIndex = 0;
    private float spawnTimer;

    public PatrolState(FSM fsm, HunterNPC hunter) : base(fsm)
    {
        this.hunter = hunter;
    }

    public override void Update()
    {
        // 1. Manejo del temporizador para generar Objetos de Interés
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= 5f) // Tiempo entre generaciones
        {
            spawnTimer = 0f;
            hunter.SpawnInterestObject();
        }

        // 2. Transición a GatherState si detecta un Boid muerto
        Collider[] deadBoids = Physics.OverlapSphere(hunter.transform.position, hunter.VisionRadius, hunter.BoidLayer);
        foreach (var col in deadBoids)
        {
            Boid boid = col.GetComponent<Boid>();
            if (boid != null && boid.IsDead)
            {
                fsm.ChangeState(new GatherState(fsm, hunter, boid));
                return;
            }
        }

        // 3. Transición a AttackState si hay Boids vivos y el TBA terminó
        if (hunter.CanAttack)
        {
            foreach (var col in deadBoids)
            {
                Boid boid = col.GetComponent<Boid>();
                if (boid != null && !boid.IsDead)
                {
                    fsm.ChangeState(new AttackState(fsm, hunter, boid));
                    return;
                }
            }
        }

        // 4. Lógica del movimiento por Waypoints
        if (hunter.Waypoints == null || hunter.Waypoints.Length == 0) return;

        Transform targetWaypoint = hunter.Waypoints[currentWaypointIndex];
        Vector3 arriveForce = SteeringBehaviours.Arrive(hunter, targetWaypoint.position, 1f);
        hunter.ApplySteering(arriveForce);

        // Si llegó al waypoint, pasa al siguiente
        if ((targetWaypoint.position - hunter.transform.position).sqrMagnitude < 1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % hunter.Waypoints.Length;
        }
    }
}