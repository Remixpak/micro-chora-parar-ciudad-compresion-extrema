using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstáculos")]
    [SerializeField] private List<GameObject> obstaclePrefabs = new List<GameObject>();

    [Header("Carriles")]
    [SerializeField] private Transform[] lanePoints;

    [Header("Generación")]
    [SerializeField] private float minSpawnInterval = 1.5f;
    [SerializeField] private float maxSpawnInterval = 3f;

    [Header("Configuración de Velocidad")]
    [SerializeField] private float velocidadBase = 30f; // La velocidad inicial estándar

    [SerializeField] private float spawnZ = 128.8f;

    private void Start()
    {
        StartCoroutine(SpawnObstacles());
    }

    private IEnumerator SpawnObstacles()
    {
        while (true)
        {
            // 1. Si la micro está detenida (GeneralSpeed == 0), pausamos la generación
            yield return new WaitUntil(() => GameManager.Instance.GeneralSpeed > 0);

            // 2. Calculamos un intervalo base aleatorio entre los rangos configurados
            float intervaloBaseAleatorio = Random.Range(minSpawnInterval, maxSpawnInterval);

            // 3. Ajustamos el intervalo inversamente proporcional a la velocidad actual del GameManager
            float velocidadActual = GameManager.Instance.GeneralSpeed;
            float interval = intervaloBaseAleatorio * (velocidadBase / velocidadActual);

            // Evitamos que el intervalo sea peligrosamente bajo
            interval = Mathf.Max(interval, 0.2f);

            float elapsed = 0f;

            while (elapsed < interval)
            {
                // Si durante la espera la micro frena, pausamos el conteo del temporizador
                if (GameManager.Instance.GeneralSpeed == 0)
                {
                    yield return new WaitUntil(() => GameManager.Instance.GeneralSpeed > 0);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (GameManager.Instance.GeneralSpeed > 0)
            {
                SpawnObstacle();
            }
        }
    }

    private void SpawnObstacle()
    {
        if (obstaclePrefabs.Count == 0 || lanePoints.Length == 0)
            return;

        int randomObstacle = Random.Range(0, obstaclePrefabs.Count);
        int randomLane = Random.Range(0, lanePoints.Length);

        Vector3 spawnPosition = new Vector3(
            lanePoints[randomLane].position.x,
            lanePoints[randomLane].position.y,
            spawnZ
        );

        Instantiate(
            obstaclePrefabs[randomObstacle],
            spawnPosition,
            Quaternion.identity
        );
    }
}