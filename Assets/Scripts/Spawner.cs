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
            // 1. Si la micro está detenida, esperamos de forma limpia hasta que vuelva a moverse
            yield return new WaitUntil(() => GameManager.Instance.GeneralSpeed > 0);

            // 2. Instanciamos el edificio o paradero correspondiente
            Instantiate(SelectBuild(), new Vector3(10.9f, 0f, 128.8f), Quaternion.identity);

            // 3. Calculamos el intervalo dinámico requerido según la velocidad actual
            float velocidadActual = GameManager.Instance.GeneralSpeed;
            float intervaloDinamico = intervaloBase * (velocidadBase / velocidadActual);
            intervaloDinamico = Mathf.Max(intervaloDinamico, 0.05f);

            float tiempoTranscurrido = 0f;

            // 4. Bucle de espera suave que escala el tiempo según la velocidad variable de la micro
            while (tiempoTranscurrido < intervaloDinamico)
            {
                float velocidadActualFrame = GameManager.Instance.GeneralSpeed;

                // Si la micro está en 0 (parada en el andén), simplemente pausamos este frame sin romper el cálculo
                if (velocidadActualFrame > 0)
                {
                    // Multiplicamos por un factor dinámico: si está acelerando (Lerp), 
                    // el tiempo avanza proporcionalmente para evitar retrasos al arrancar.
                    float factorAceleracion = velocidadActualFrame / velocidadBase;
                    tiempoTranscurrido += Time.deltaTime * factorAceleracion;
                }

                yield return null;
            }
        }
    }
}