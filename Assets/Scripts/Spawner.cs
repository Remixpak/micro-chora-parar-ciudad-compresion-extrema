using UnityEngine;
using Unity.Collections;
using System.Collections;
using System.Collections.Generic;
public class Spawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> Buildings = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
        StartCoroutine(spawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    GameObject SelectBuild()
    {
        GameObject selected;
        return selected = Buildings[Random.Range(0, Buildings.Count)];
    }

    IEnumerator spawn()
    {
        while (true)
        {
            Debug.Log("Intanciando");
            Instantiate(SelectBuild(), new Vector3(10.9f, 0f, 128.8f), Quaternion.identity);
            Debug.Log("Instanciado");
            yield return new WaitForSeconds(0.5f); // Wait for 1 second before spawning the next object
            Debug.Log("Pasaron los 5 segundos");

        }
    }
}
