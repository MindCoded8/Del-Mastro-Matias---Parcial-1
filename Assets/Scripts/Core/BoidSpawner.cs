using System.Collections;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField] private GameObject boidPrefab;
    [SerializeField] private int totalBoids = 6; // Mínimo 6 agentes requeridos
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float respawnDelay = 3f;

    private void Start()
    {
        for (int i = 0; i < totalBoids; i++)
        {
            SpawnBoid();
        }
    }

    public void SpawnBoid()
    {
        Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
        randomPos.y = 0; // Mantener en el plano del suelo

        Instantiate(boidPrefab, randomPos, Quaternion.identity);
    }

    public void RequestRespawn(Boid deadBoid)
    {
        StartCoroutine(RespawnRoutine(deadBoid));
    }

    private IEnumerator RespawnRoutine(Boid deadBoid)
    {
        yield return new WaitForSeconds(respawnDelay);

        Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
        randomPos.y = 0;

        deadBoid.Revive(randomPos);
    }
}