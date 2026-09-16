using UnityEngine;

public static class SteeringBehaviours
{
    // Seek: Persigue una posición a velocidad máxima
    public static Vector3 Seek(AutonomousAgent agent, Vector3 targetPosition)
    {
        Vector3 desiredVelocity = (targetPosition - agent.transform.position).normalized * agent.MaxSpeed;
        return desiredVelocity - agent.Velocity;
    }

    // Arrive: Se acerca al objetivo y frena progresivamente al entrar al slowingRadius
    public static Vector3 Arrive(AutonomousAgent agent, Vector3 targetPosition, float slowingRadius)
    {
        Vector3 toTarget = targetPosition - agent.transform.position;
        float distanceSqr = toTarget.sqrMagnitude;

        if (distanceSqr < 0.0001f)
        {
            return -agent.Velocity;
        }

        float distance = Mathf.Sqrt(distanceSqr);
        float desiredSpeed = agent.MaxSpeed;

        if (distance < slowingRadius)
        {
            desiredSpeed = agent.MaxSpeed * (distance / slowingRadius);
        }

        Vector3 desiredVelocity = (toTarget / distance) * desiredSpeed;
        return desiredVelocity - agent.Velocity;
    }

    // Evade: Huye de la posición futura predicha de un perseguidor
    public static Vector3 Evade(AutonomousAgent agent, AutonomousAgent pursuer, float predictionTime)
    {
        Vector3 distanceToPursuer = pursuer.transform.position - agent.transform.position;
        float lookAhead = distanceToPursuer.magnitude / agent.MaxSpeed;

        Vector3 futurePosition = pursuer.transform.position + (pursuer.Velocity * Mathf.Min(lookAhead, predictionTime));

        return Flee(agent, futurePosition);
    }

    // Flee: Se aleja de una posición
    public static Vector3 Flee(AutonomousAgent agent, Vector3 targetPosition)
    {
        Vector3 desiredVelocity = (agent.transform.position - targetPosition).normalized * agent.MaxSpeed;
        return desiredVelocity - agent.Velocity;
    }
}