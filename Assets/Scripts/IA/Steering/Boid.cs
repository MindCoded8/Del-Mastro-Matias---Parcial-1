using UnityEngine;

public class Boid : AutonomousAgent
{
    [Header("Flocking Sensorial Ranges")]
    [SerializeField] private float neighborRadius = 5f;
    [SerializeField] private float separationRadius = 2f;

    [Header("Flocking Weights")]
    [SerializeField] private float separationWeight = 1.5f;
    [SerializeField] private float alignmentWeight = 1.0f;
    [SerializeField] private float cohesionWeight = 1.0f;

    [Header("Interaction Settings")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float interactionRadius = 1.5f;
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private LayerMask interestObjectLayer;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask boidLayer;

    private Collider[] neighborBuffer = new Collider[15];
    private Collider[] objectBuffer = new Collider[5];
    private bool isDead = false;

    public bool IsDead => isDead;

    protected override void Update()
    {
        if (isDead) return; // Si está muerto, queda totalmente inactivo[cite: 1]

        // 1. Buscamos Objetos de Interés
        int objectsFound = Physics.OverlapSphereNonAlloc(transform.position, neighborRadius, objectBuffer, interestObjectLayer);

        if (objectsFound > 0 && objectBuffer[0] != null)
        {
            InterestObject targetObject = objectBuffer[0].GetComponent<InterestObject>();
            if (targetObject != null && !targetObject.IsDestroyed)
            {
                Vector3 offset = targetObject.transform.position - transform.position;
                float sqrDist = offset.sqrMagnitude;
                float interactRadiusSqr = interactionRadius * interactionRadius;

                if (sqrDist <= interactRadiusSqr)
                {
                    // Estamos en rango: interactuamos reduciendo la vida del objeto[cite: 1]
                    targetObject.TakeDamage(damagePerSecond * Time.deltaTime);
                }
                else
                {
                    // Nos acercamos usando Arrive[cite: 1]
                    Vector3 arriveForce = SteeringBehaviours.Arrive(this, targetObject.transform.position, interactionRadius);
                    ApplySteering(arriveForce);
                }

                base.Update();
                return;
            }
        }

        // 2. Si no hay objeto de interés, aplica Flocking
        Vector3 flockingForce = CalculateFlocking();
        ApplySteering(flockingForce);

        base.Update();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;
        if (health <= 0)
        {
            health = 0;
            isDead = true; // Queda inactivo en el lugar[cite: 1]
        }
    }

    public void Revive(Vector3 newPosition)
    {
        transform.position = newPosition;
        health = 100f;
        isDead = false;
        gameObject.SetActive(true);
    }

    private Vector3 CalculateFlocking()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, neighborRadius, neighborBuffer, boidLayer);

        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesionCenter = Vector3.zero;

        int neighborCount = 0;
        int separationCount = 0;

        float separationRadiusSqr = separationRadius * separationRadius;

        for (int i = 0; i < count; i++)
        {
            Collider col = neighborBuffer[i];

            if (col.gameObject == gameObject) continue;

            Boid neighbor = col.GetComponent<Boid>();
            if (neighbor == null || neighbor.IsDead) continue;

            Vector3 offset = transform.position - neighbor.transform.position;
            float sqrDist = offset.sqrMagnitude;

            if (sqrDist < separationRadiusSqr && sqrDist > 0.0001f)
            {
                separation += offset.normalized / Mathf.Sqrt(sqrDist);
                separationCount++;
            }

            alignment += neighbor.Velocity;
            cohesionCenter += neighbor.transform.position;
            neighborCount++;
        }

        Vector3 totalForce = Vector3.zero;

        if (separationCount > 0)
        {
            separation /= separationCount;
            Vector3 desired = separation.normalized * MaxSpeed;
            totalForce += (desired - Velocity) * separationWeight;
        }

        if (neighborCount > 0)
        {
            alignment /= neighborCount;
            Vector3 desiredAlign = alignment.normalized * MaxSpeed;
            totalForce += (desiredAlign - Velocity) * alignmentWeight;

            cohesionCenter /= neighborCount;
            totalForce += SteeringBehaviours.Seek(this, cohesionCenter) * cohesionWeight;
        }

        return totalForce;
    }
}