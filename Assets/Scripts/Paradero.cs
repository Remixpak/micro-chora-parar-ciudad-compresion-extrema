using System.Collections.Generic;
using UnityEngine;

public enum colorParadero { rojo, azul, verde, amarillo };

public class Paradero : MonoBehaviour
{
    public colorParadero colorParadero;

    [SerializeField] private GameObject spawnPoint;

    [Header("Configuración de Materiales")]
    [Tooltip("El Renderer del objeto que se va a pintar (el que tiene los materiales gris y de color).")]
    [SerializeField] private Renderer renderParadero;

    [Tooltip("Índice del material en el MeshRenderer que corresponde al color variable (por ejemplo, si el gris es el 0 y el color es el 1).")]
    [SerializeField] private int materialIndexColor = 1;

    [Header("Materiales por Color")]
    [SerializeField] private Material matRojo;
    [SerializeField] private Material matAzul;
    [SerializeField] private Material matVerde;
    [SerializeField] private Material matAmarillo;

    [Header("Pasajeros esperando")]
    [Tooltip("Prefabs de pasajeros disponibles para este paradero")]
    public List<GameObject> prefabsPasajerosDisponibles;

    [HideInInspector]
    public List<Pasajero> pasajerosEsperandoList = new List<Pasajero>();

    private void Start()
    {
        AsignarColorAleatorio();
        GenerarPasajerosAleatorios();
    }

    void AsignarColorAleatorio()
    {
        // 1. Obtener todos los valores posibles del enum 'colorParadero'
        colorParadero[] valoresColores = (colorParadero[])System.Enum.GetValues(typeof(colorParadero));

        // 2. Escoger uno al azar
        colorParadero = valoresColores[Random.Range(0, valoresColores.Length)];

        // 3. Asignar el material correspondiente en base al color elegido
        if (renderParadero != null)
        {
            // Creamos un arreglo con los materiales actuales del objeto
            Material[] materiales = renderParadero.materials;

            switch (colorParadero)
            {
                case colorParadero.rojo:
                    materiales[materialIndexColor] = matRojo;
                    break;
                case colorParadero.azul:
                    materiales[materialIndexColor] = matAzul;
                    break;
                case colorParadero.verde:
                    materiales[materialIndexColor] = matVerde;
                    break;
                case colorParadero.amarillo:
                    materiales[materialIndexColor] = matAmarillo;
                    break;
            }

            // Aplicamos los materiales de vuelta al renderizador
            renderParadero.materials = materiales;
        }
        else
        {
            Debug.LogWarning($"El paradero {gameObject.name} no tiene asignado un Renderer para pintar.");
        }
    }

    void GenerarPasajerosAleatorios()
    {
        bool hayPasajeros = Random.value > 0.3f; // 70% de probabilidad

        if (hayPasajeros && prefabsPasajerosDisponibles.Count > 0)
        {
            int indicePrefab = Random.Range(0, prefabsPasajerosDisponibles.Count);
            GameObject pasajeroObj = Instantiate(prefabsPasajerosDisponibles[indicePrefab],spawnPoint.transform.position, Quaternion.identity, transform);

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

    public List<Pasajero> SubirPasajeros()
    {
        List<Pasajero> subidos = new List<Pasajero>(pasajerosEsperandoList);
        pasajerosEsperandoList.Clear();
        return subidos;
    }
}