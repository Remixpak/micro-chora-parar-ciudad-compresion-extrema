using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class BusController : MonoBehaviour
{
    [Header("Estado de la Micro")]
    public List<Pasajero> pasajerosABordo = new List<Pasajero>();

    // Método para simular la llegada a un paradero (puedes llamarlo desde un OnTriggerEnter con el paradero)
    public void LlegarAParadero(Paradero paraderoLlegada)
    {
        Debug.Log($"La micro llegó a un paradero de color: {paraderoLlegada.colorParadero}");

        bool alguienSeBajaAqui = false;

        // 1. Revisar pasajeros que quieren bajarse en este paradero
        // Usamos una lista temporal para recorrer y remover sin romper el bucle
        List<Pasajero> pasajerosABajar = pasajerosABordo.FindAll(p => p.colorDestino == paraderoLlegada.colorParadero);

        if (pasajerosABajar.Count > 0)
        {
            alguienSeBajaAqui = true;
            foreach (var pasajero in pasajerosABajar)
            {
                pasajerosABordo.Remove(pasajero);
                // Aquí destruirías el objeto visual del pasajero que bajó o lo liberarías
                Destroy(pasajero.gameObject);
                Debug.Log($"Un pasajero se bajó correctamente en el paradero {paraderoLlegada.colorParadero}.");
            }
        }

        // 2. Subir nuevos pasajeros que estaban esperando en el paradero
        List<Pasajero> nuevosPasajeros = paraderoLlegada.SubirPasajeros();
        if (nuevosPasajeros.Count > 0)
        {
            pasajerosABordo.AddRange(nuevosPasajeros);
            alguienSeBajaAqui = true;
            Debug.Log($"Se subieron {nuevosPasajeros.Count} grupos de pasajeros al bus.");
        }

        // 3. Validación de parada inútil (nadie se bajaba ni subía / o específicamente si nadie se baja según tu regla)
        if (!alguienSeBajaAqui)
        {
            RestarPuntajePorParadaInnecesaria();
        }
    }

    IEnumerator SubirPasajeros(Paradero paradero)
    {
        // 1. Frena la micro
        GameManager.Instance.GeneralSpeed = 0;

        // 2. Ejecuta la lógica de subida y bajada
        LlegarAParadero(paradero);

        // 3. Espera los 2 segundos (la corrutina descansa aquí sin gastar recursos de Update)
        yield return new WaitForSeconds(1f);

        // 4. Devuelve la velocidad para que continúe la marcha
        GameManager.Instance.GeneralSpeed = 30;

        // Al llegar aquí, la corrutina finaliza y Unity la elimina automáticamente de la memoria.
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        if (other.CompareTag("Stop"))
        {
            Debug.Log("pico");
            Paradero paradero = other.GetComponent<Paradero>();
            Debug.Log("paradero: " + other.GetComponent<Paradero>().ToString());
            if (paradero != null)
            {
                StartCoroutine(SubirPasajeros(paradero));
            }
            else
            {
                Debug.Log("ES NULLLLLLLLLLLLLLLL");
            }
        }
        else
        {
            Debug.Log("Nico marambio");
        }
    }

    void RestarPuntajePorParadaInnecesaria()
    {
        // Por ahora un Debug.Log como pediste
        Debug.Log("¡PENALIZACIÓN! La micro se detuvo en un paradero donde nadie se bajaba.");
        // TODO: Aqui restarás puntos de tu GameManager en el futuro, ej: GameManager.Instance.RestarPuntos(10);
    }
}