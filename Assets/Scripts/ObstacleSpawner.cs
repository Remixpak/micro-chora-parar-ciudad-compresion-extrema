
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

    [SerializeField] private float spawnZ = 128.8f;

    private void Start()
    {
        StartCoroutine(SpawnObstacles());
    }

    private IEnumerator SpawnObstacles()
    {
        while (true)
        {
            float interval = Random.Range(
                minSpawnInterval,
                maxSpawnInterval
            );

            float elapsed = 0f;

            while (elapsed < interval)
            {
                if (GameManager.Instance.GeneralSpeed > 0)
                {
                    elapsed += Time.deltaTime;
                }

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
        if (obstaclePrefabs.Count == 0 ||
            lanePoints.Length == 0)
            return;

        int randomObstacle = Random.Range(
            0,
            obstaclePrefabs.Count
        );

        int randomLane = Random.Range(
            0,
            lanePoints.Length
        );

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
