using System.Collections.Generic;
using UnityEngine;

public class HunterNPC : AutonomousAgent
{
    [Header("Hunter Required Variables")]
    [SerializeField] private float tba = 3f; // Time Between Attacks
    [SerializeField] private float rangeAttackRadius = 8f;
    [SerializeField] private float meleeAttackRadius = 2f;

    [Header("Sensory Ranges")]
    [SerializeField] private float visionRadius = 10f;
    [SerializeField] private LayerMask boidLayer;
    [SerializeField] private LayerMask interestObjectLayer;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private GameObject interestObjectPrefab;
    [SerializeField] private float spawnObjectInterval = 5f;

    [Header("Gather Settings")]
    [SerializeField] private float gatherDuration = 2f;

    private FSM fsm;
    private float attackTimer;
    private List<InterestObject> activeInterestObjects = new List<InterestObject>();

    // Propiedades públicas para acceso de los Estados
    public float TBA => tba;
    public float RangeAttackRadius => rangeAttackRadius;
    public float MeleeAttackRadius => meleeAttackRadius;
    public float VisionRadius => visionRadius;
    public Transform[] Waypoints => waypoints;
    public LayerMask BoidLayer => boidLayer;
    public float GatherDuration => gatherDuration;
    public bool CanAttack => attackTimer <= 0;

    private void Start()
    {
        fsm = new FSM();
        fsm.ChangeState(new PatrolState(fsm, this));
    }

    protected override void Update()
    {
        // Actualizamos el temporizador del TBA
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // Ejecutamos la FSM
        fsm.Update();

        base.Update();
    }

    public void ResetAttackTimer()
    {
        attackTimer = tba;
    }

    private void OnDrawGizmosSelected()
    {
        // Rango de Visión (Amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        // Rango de Ataque a Distancia (Naranja)
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, rangeAttackRadius);

        // Rango de Ataque Cuerpo a Cuerpo (Rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRadius);
    }

    public void SpawnInterestObject()
    {
        // Limpiamos la lista de objetos destruidos
        activeInterestObjects.RemoveAll(item => item == null);

        // Solo genera si hay menos de 5 activos
        if (activeInterestObjects.Count < 5 && interestObjectPrefab != null)
        {
            GameObject obj = Instantiate(interestObjectPrefab, transform.position, Quaternion.identity);
            InterestObject interestObj = obj.GetComponent<InterestObject>();
            if (interestObj != null)
            {
                activeInterestObjects.Add(interestObj);
            }
        }
    }
}