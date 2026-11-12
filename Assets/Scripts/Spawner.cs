using UnityEngine;
using Unity.Collections;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> Buildings = new List<GameObject>();

    [Header("Configuración de Paraderos")]
    [Tooltip("Número de edificios normales que deben pasar obligatoriamente antes de poder generar otro paradero.")]
    [SerializeField] private int generacionesMinimasSinParadero = 5;

    [Header("Configuración de Velocidad de Generación")]
    [Tooltip("Intervalo base de espera cuando la micro está a su velocidad inicial.")]
    [SerializeField] private float intervaloBase = 0.5f;
    [SerializeField] private float velocidadBase = 30f;

    private int contadorBloqueoParadero = 0;

    void Start()
    {
        StartCoroutine(spawn());
    }

    void Update()
    {

    }

    GameObject SelectBuild()
    {
        GameObject selected = null;

        if (contadorBloqueoParadero > 0)
        {
            List<GameObject> edificiosSinParaderos = new List<GameObject>();

            foreach (GameObject obj in Buildings)
            {
                if (!obj.CompareTag("Stop"))
                {
                    edificiosSinParaderos.Add(obj);
                }
            }

            if (edificiosSinParaderos.Count > 0)
            {
                selected = edificiosSinParaderos[Random.Range(0, edificiosSinParaderos.Count)];
            }
            else
            {
                selected = Buildings[Random.Range(0, Buildings.Count)];
            }

            contadorBloqueoParadero--;
        }
        else
        {
            selected = Buildings[Random.Range(0, Buildings.Count)];

            if (selected.CompareTag("Stop"))
            {
                contadorBloqueoParadero = generacionesMinimasSinParadero;
            }
        }

        return selected;
    }

    IEnumerator spawn()
    {
        while (true)
        {
            // 1. Si la micro está detenida, esperamos limpiamente a que empiece a moverse
            yield return new WaitUntil(() => GameManager.Instance.GeneralSpeed > 0);

            // 2. Instanciamos el edificio o paradero correspondiente
            Instantiate(SelectBuild(), new Vector3(10.9f, 0f, 128.8f), Quaternion.identity);

            float tiempoTranscurrido = 0f;

            // 3. Bucle de espera donde el tiempo objetivo se recalcula frame a frame según la velocidad actual
            while (tiempoTranscurrido < 1f) // Usamos una proporción normalizada de 0 a 1
            {
                float velocidadActual = GameManager.Instance.GeneralSpeed;

                // Si la micro está detenida en el paradero, congelamos la cuenta sin romper el flujo
                if (velocidadActual > 0)
                {
                    // Calculamos el intervalo que le corresponde exactamente A ESTE FRAME
                    float intervaloDinamicoActual = intervaloBase * (velocidadBase / velocidadActual);
                    intervaloDinamicoActual = Mathf.Max(intervaloDinamicoActual, 0.05f);

                    // Avanzamos el progreso de forma proporcional al intervalo variable actual
                    tiempoTranscurrido += Time.deltaTime / intervaloDinamicoActual;
                }

                yield return null;
            }
        }
    }
}