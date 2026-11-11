using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusController : MonoBehaviour
{
    private BusPassengerController passengerController;

    private void Awake()
    {
        // Obtenemos el componente que gestiona los pasajeros en la misma micro
        passengerController = GetComponent<BusPassengerController>();
        if (passengerController == null)
        {
            Debug.LogError("Falta el componente BusPassengerController en la micro.");
        }
    }

    // Método principal al llegar a un paradero
    public void LlegarAParadero(Paradero paraderoLlegada)
    {
        Debug.Log($"La micro llegó a un paradero de color: {paraderoLlegada.colorParadero}");

        bool accionRealizada = false;

        // 1. REVISAR SI SE BAJAN PASAJEROS
        // Verificamos si este paradero está en la lista de destinos del GameManager 
        // o si hay pasajeros a bordo cuyo color de destino coincida con el paradero.
        if (GameManager.Instance != null && GameManager.Instance.paraderosDestino.Contains(paraderoLlegada))
        {
            List<Pasajero> pasajerosABajar = passengerController.pasajerosABordo.FindAll(p => p.colorDestino == paraderoLlegada.colorParadero);

            if (pasajerosABajar.Count > 0)
            {
                accionRealizada = true;
                foreach (var pasajero in pasajerosABajar)
                {
                    passengerController.pasajerosABordo.Remove(pasajero);
                    passengerController.pasajerosActuales = passengerController.pasajerosABordo.Count;

                    // Destruimos el objeto visual del pasajero que bajó
                    Destroy(pasajero.gameObject);
                    Debug.Log($"Un pasajero se bajó correctamente en el paradero {paraderoLlegada.colorParadero}.");
                }
            }

            // Quitamos el paradero de la lista de destinos del GameManager ya que fue atendido
            GameManager.Instance.paraderosDestino.Remove(paraderoLlegada);
        }

        // 2. SUBIR NUEVOS PASAJEROS DESDE EL PARADERO
        Pasajero[] pasajerosEsperando = paraderoLlegada.GetComponentsInChildren<Pasajero>();

        if (pasajerosEsperando.Length > 0)
        {
            accionRealizada = true;

            // Usamos un HashSet para almacenar los objetos padre (el EmptyObject del grupo) 
            // y evitar borrarlos varias veces si el grupo tiene múltiples pasajeros dentro.
            HashSet<GameObject> gruposPasajerosADestruir = new HashSet<GameObject>();

            foreach (Pasajero nuevoPasajero in pasajerosEsperando)
            {
                // Lo añadimos al controlador de pasajeros del bus
                passengerController.addPassengers(nuevoPasajero);

                // Verificamos que el pasajero tenga un padre directo (el EmptyObject contenedor)
                if (nuevoPasajero.transform.parent != null)
                {
                    // Agregamos el objeto padre inmediato a la lista para destruirlo después
                    gruposPasajerosADestruir.Add(nuevoPasajero.transform.parent.gameObject);
                }
            }

            // Destruimos únicamente el EmptyObject contenedor (el grupo) de cada uno
            foreach (GameObject grupo in gruposPasajerosADestruir)
            {
                Destroy(grupo);
            }

            // Limpiamos la lista interna del paradero
            paraderoLlegada.pasajerosEsperandoList.Clear();
            Debug.Log($"Se subieron los pasajeros y se eliminó el grupo de espera del paradero.");
        }

        // 3. VALIDACIÓN DE PARADA INÚTIL
        if (!accionRealizada)
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

        // 3. Espera el tiempo de atención en el paradero (ej. 1 segundo)
        yield return new WaitForSeconds(1f);

        // 4. Devuelve la velocidad para que continúe la marcha
        GameManager.Instance.GeneralSpeed = 30;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stop"))
        {
            if (other.TryGetComponent<Paradero>(out Paradero paradero))
            {
                StartCoroutine(SubirPasajeros(paradero));
            }
            else
            {
                Debug.Log("El objeto con tag Stop no tiene el script Paradero.");
            }
        }
    }

    void RestarPuntajePorParadaInnecesaria()
    {
        Debug.Log("¡PENALIZACIÓN! La micro se detuvo en un paradero donde nadie se bajaba ni subía.");
        // GameManager.Instance.RestarPuntaje(10);
    }
}