using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusController : MonoBehaviour
{
    private BusPassengerController passengerController;

    private void Awake()
    {
        passengerController = GetComponent<BusPassengerController>();
        if (passengerController == null)
        {
            Debug.LogError("Falta el componente BusPassengerController en la micro.");
        }
    }

    public void LlegarAParadero(Paradero paraderoLlegada)
    {
        Debug.Log($"La micro llegó a un paradero de color: {paraderoLlegada.colorParadero}");

        bool accionRealizada = false;

        // 1. REVISAR SI SE BAJAN PASAJEROS
        if (GameManager.Instance != null && GameManager.Instance.paraderosDestino.Contains(paraderoLlegada))
        {
            // Buscamos en la lista de datos los pasajeros cuyo destino coincida con el paradero
            List<DatosPasajero> pasajerosABajar = passengerController.pasajerosABordo.FindAll(p => p.colorDestino == paraderoLlegada.colorParadero);

            if (pasajerosABajar.Count > 0)
            {
                accionRealizada = true;

                // Removemos los datos del controlador de la micro
                passengerController.RemovePassengers(pasajerosABajar);

                Debug.Log($"Se bajaron {pasajerosABajar.Count} pasajeros en el paradero {paraderoLlegada.colorParadero}.");
            }

            // Quitamos el paradero de la lista de destinos del GameManager
            GameManager.Instance.paraderosDestino.Remove(paraderoLlegada);
        }

        // 2. SUBIR NUEVOS PASAJEROS DESDE EL PARADERO
        Pasajero[] pasajerosEsperando = paraderoLlegada.GetComponentsInChildren<Pasajero>();

        if (pasajerosEsperando.Length > 0)
        {
            accionRealizada = true;
            HashSet<GameObject> gruposPasajerosADestruir = new HashSet<GameObject>();

            foreach (Pasajero nuevoPasajero in pasajerosEsperando)
            {
                // Guardamos sus datos puros en la micro
                passengerController.addPassenger(nuevoPasajero.colorDestino, nuevoPasajero.cantidadPersonas);

                // Ubicamos el objeto contenedor (el grupo) para destruirlo visualmente del andén
                if (nuevoPasajero.transform.parent != null)
                {
                    gruposPasajerosADestruir.Add(nuevoPasajero.transform.parent.gameObject);
                }
            }

            // Destruimos el grupo visual del paradero
            foreach (GameObject grupo in gruposPasajerosADestruir)
            {
                if (grupo != null)
                {
                    Destroy(grupo);
                }
            }

            paraderoLlegada.pasajerosEsperandoList.Clear();
            Debug.Log("Nuevos pasajeros subidos a la micro y paradero despejado.");
        }

        // 3. VALIDACIÓN DE PARADA INÚTIL
        if (!accionRealizada)
        {
            RestarPuntajePorParadaInnecesaria();
        }
    }

    IEnumerator SubirPasajeros(Paradero paradero)
    {
        GameManager.Instance.GeneralSpeed = 0;
        LlegarAParadero(paradero);
        yield return new WaitForSeconds(1f);
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
        }
    }

    void RestarPuntajePorParadaInnecesaria()
    {
        Debug.Log("¡PENALIZACIÓN! La micro se detuvo en un paradero donde nadie se bajaba ni subía.");
    }
}