using UnityEngine;

public class Boid : AutonomousAgent
{
    [Header("Flocking Sensorial Ranges")]
    [SerializeField] private float neighborRadius = 5f;
    [SerializeField] private float separationRadius = 2f; // Deberá ser menor que neighborRadius

    [Header("Flocking Weights")]
    [SerializeField] private float separationWeight = 1.5f;
    [SerializeField] private float alignmentWeight = 1.0f;
    [SerializeField] private float cohesionWeight = 1.0f;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask boidLayer;

    private Collider[] neighborBuffer = new Collider[15];

    protected override void Update()
    {
        Vector3 flockingForce = CalculateFlocking();
        ApplySteering(flockingForce);

        base.Update();
    }

    private Vector3 CalculateFlocking()
    {
        // Detectamos colisionadores cercanos usando el buffer local (evita GC Alloc y Find)
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

            // Ignoramos a sí mismo
            if (col.gameObject == gameObject) continue;

            Boid neighbor = col.GetComponent<Boid>();
            if (neighbor == null) continue;

            Vector3 offset = transform.position - neighbor.transform.position;
            float sqrDist = offset.sqrMagnitude;

            // 1. SEPARACIÓN (Radio Menor)
            if (sqrDist < separationRadiusSqr && sqrDist > 0.0001f)
            {
                separation += offset.normalized / Mathf.Sqrt(sqrDist); // Inversamente proporcional a la distancia
                separationCount++;
            }

            // 2 y 3. ALINEACIÓN Y COHESIÓN (Radio Normal)
            alignment += neighbor.Velocity;
            cohesionCenter += neighbor.transform.position;
            neighborCount++;
        }

        Vector3 totalForce = Vector3.zero;

        // Cálculo final de Separación
        if (separationCount > 0)
        {
            separation /= separationCount;
            Vector3 desired = separation.normalized * MaxSpeed;
            totalForce += (desired - Velocity) * separationWeight;
        }

        // Cálculo final de Alineación y Cohesión
        if (neighborCount > 0)
        {
            // Alineación
            alignment /= neighborCount;
            Vector3 desiredAlign = alignment.normalized * MaxSpeed;
            totalForce += (desiredAlign - Velocity) * alignmentWeight;

            // Cohesión
            cohesionCenter /= neighborCount;
            totalForce += SteeringBehaviours.Seek(this, cohesionCenter) * cohesionWeight;
        }

        return totalForce;
    }
}