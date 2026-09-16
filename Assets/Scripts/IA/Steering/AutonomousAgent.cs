using UnityEngine;

public class AutonomousAgent : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float maxForce = 10f;
    [SerializeField] private float mass = 1f;

    private Vector3 velocity;

    public Vector3 Velocity => velocity;
    public float MaxSpeed => maxSpeed;

    protected virtual void Update()
    {
        // La lógica de decisión de cada agente llamará a ApplySteering
    }

    public void ApplySteering(Vector3 steeringForce)
    {
        // Limitamos la fuerza de direccionamiento a la fuerza máxima
        Vector3 steeringClamped = Vector3.ClampMagnitude(steeringForce, maxForce);

        // F = m * a  =>  a = F / m
        Vector3 acceleration = steeringClamped / mass;

        // Actualizamos la velocidad considerando el tiempo transcurrido
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // Movemos al agente
        transform.position += velocity * Time.deltaTime;

        // Orientamos el objeto visual hacia la dirección del movimiento
        if (velocity.sqrMagnitude > 0.001f)
        {
            transform.forward = velocity.normalized;
        }
    }
}