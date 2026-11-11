using UnityEngine;
using Unity.Collections;
using System.Collections;
public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(spawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator spawn()
    {
        while (true)
        {
            Debug.Log("Intanciando");
            Instantiate(objectToSpawn);
            Debug.Log("Instanciado");
            yield return new WaitForSeconds(5f); // Wait for 1 second before spawning the next object
            Debug.Log("Pasaron los 5 segundos");

        }
    }
}
