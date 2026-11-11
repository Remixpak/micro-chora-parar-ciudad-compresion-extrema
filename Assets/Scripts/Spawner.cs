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

    private int contadorBloqueoParadero = 0;

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
        GameObject selected = null;

        // Si el bloqueo está activo, filtramos la lista para excluir los objetos que tengan el tag "Stop"
        if (contadorBloqueoParadero > 0)
        {
            List<GameObject> edificiosSinParaderos = new List<GameObject>();

            foreach (GameObject obj in Buildings)
            {
                // Verificamos si NO tiene el tag "Stop"
                if (!obj.CompareTag("Stop"))
                {
                    edificiosSinParaderos.Add(obj);
                }
            }

            // Si por alguna razón la lista filtrada no tiene elementos, usamos la lista completa por seguridad
            if (edificiosSinParaderos.Count > 0)
            {
                selected = edificiosSinParaderos[Random.Range(0, edificiosSinParaderos.Count)];
            }
            else
            {
                selected = Buildings[Random.Range(0, Buildings.Count)];
            }

            // Reducimos el contador de bloqueo ya que ha pasado una generación sin paradero
            contadorBloqueoParadero--;
        }
        else
        {
            // El bloqueo terminó, podemos elegir cualquier edificio libremente
            selected = Buildings[Random.Range(0, Buildings.Count)];

            // Comprobamos si el objeto seleccionado ES un paradero
            if (selected.CompareTag("Stop"))
            {
                // ¡Acabamos de spawnear un paradero! Activamos el bloqueo por X generaciones
                contadorBloqueoParadero = generacionesMinimasSinParadero;
            }
        }

        return selected;
    }

    IEnumerator spawn()
    {
        while (true)
        {
            // 1. Si la micro está detenida (GeneralSpeed == 0), pausamos todo aquí mismo
            yield return new WaitUntil(() => GameManager.Instance.GeneralSpeed > 0);

            // 2. Instanciamos el edificio o paradero correspondiente
            Debug.Log("Instanciando");
            Instantiate(SelectBuild(), new Vector3(10.9f, 0f, 128.8f), Quaternion.identity);
            Debug.Log("Instanciado");

            // 3. Esperamos el intervalo, pero volvemos a vigilar por si la micro se detiene a mitad de la espera
            float tiempoTranscurrido = 0f;
            float intervalo = 0.5f;

            while (tiempoTranscurrido < intervalo)
            {
                // Si durante la espera la micro frena, pausamos el contador de tiempo del spawn
                if (GameManager.Instance.GeneralSpeed == 0)
                {
                    yield return new WaitUntil(() => GameManager.Instance.GeneralSpeed > 0);
                }

                tiempoTranscurrido += Time.deltaTime;
                yield return null; // Espera al siguiente frame
            }

            Debug.Log("Pasaron los 0.5 segundos");
        }
    }
}