using System.Collections.Generic;
using UnityEngine;
public enum colorParadero { rojo, azul, verde, amarillo };

public class Paradero : MonoBehaviour
{
    public colorParadero colorParadero;

    [Header("Pasajeros esperando")]
    [Tooltip("Prefabs de pasajeros disponibles para este paradero (pueden ser grupos de 1, 3 o 5)")]
    public List<GameObject> prefabsPasajerosDisponibles;

    [HideInInspector]
    public List<Pasajero> pasajerosEsperandoList = new List<Pasajero>();

    private void Start()
    {
        GenerarPasajerosAleatorios();
    }

    void GenerarPasajerosAleatorios()
    {
        // Opcional: Decidir de forma aleatoria si hay pasajeros o está vacío (ningún pasajero)
        bool hayPasajeros = Random.value > 0.3f; // 70% de probabilidad de que haya gente

        if (hayPasajeros && prefabsPasajerosDisponibles.Count > 0)
        {
            // Instanciar aleatoriamente 1 o un par de grupos de pasajeros en el paradero
            int indicePrefab = Random.Range(0, prefabsPasajerosDisponibles.Count);
            GameObject pasajeroObj = Instantiate(prefabsPasajerosDisponibles[indicePrefab], transform.position, Quaternion.identity, transform);

            Pasajero pasajeroScript = pasajeroObj.GetComponent<Pasajero>();
            if (pasajeroScript != null)
            {
                pasajerosEsperandoList.Add(pasajeroScript);
            }
        }
        else
        {
            Debug.Log($"El paradero {gameObject.name} está vacío (ningún pasajero).");
        }
    }

    // Método que llama la micro al subirse gente
    public List<Pasajero> SubirPasajeros()
    {
        List<Pasajero> subidos = new List<Pasajero>(pasajerosEsperandoList);
        pasajerosEsperandoList.Clear(); // Ya se subieron todos
        return subidos;
    }
}
