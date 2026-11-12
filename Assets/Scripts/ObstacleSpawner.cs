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
            // 1. Si la micro está detenida (GeneralSpeed == 0), esperamos limpiamente
            yield return new WaitUntil(() => GameManager.Instance.GeneralSpeed > 0);

            // 2. Seleccionamos un intervalo base aleatorio dentro del rango configurado
            float intervaloBaseAleatorio = Random.Range(minSpawnInterval, maxSpawnInterval);

            float tiempoTranscurrido = 0f;

            // 3. Bucle de espera normalizado de 0 a 1 que se adapta en tiempo real a la velocidad variable
            while (tiempoTranscurrido < 1f)
            {
                float velocidadActual = GameManager.Instance.GeneralSpeed;

                // Si la micro está en movimiento, calculamos el intervalo dinámico correspondiente a este frame
                if (velocidadActual > 0)
                {
                    float intervaloDinamicoActual = intervaloBaseAleatorio * (velocidadBase / velocidadActual);

                    // Límite de seguridad para evitar divisiones por cero o intervalos ultra-congelados
                    intervaloDinamicoActual = Mathf.Max(intervaloDinamicoActual, 0.1f);

                    // Avanzamos el progreso porcentualmente por cada frame
                    tiempoTranscurrido += Time.deltaTime / intervaloDinamicoActual;
                }

                yield return null;
            }

            // 4. Al terminar el ciclo, spawneamos el obstáculo si la micro sigue en marcha
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