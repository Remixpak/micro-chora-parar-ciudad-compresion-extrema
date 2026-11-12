using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BusController : MonoBehaviour
{
    private BusPassengerController passengerController;
    [Header("Sfx")]
    public AudioClip FrenarSfx;
    public List<AudioClip> SubirPasajeroSfx = new List<AudioClip>();

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
        List<DatosPasajero> pasajerosABajar = passengerController.pasajerosABordo.FindAll(p => p.colorDestino == paraderoLlegada.colorParadero);

        if (pasajerosABajar.Count > 0)
        {
            accionRealizada = true;
            passengerController.RemovePassengers(pasajerosABajar);
            Debug.Log($"Se bajaron {pasajerosABajar.Count} pasajeros en el paradero {paraderoLlegada.colorParadero}.");
        }

        // 2. SUBIR NUEVOS PASAJEROS DESDE EL PARADERO
        Pasajero[] pasajerosEsperando = paraderoLlegada.GetComponentsInChildren<Pasajero>();

        if (pasajerosEsperando.Length > 0)
        {
            accionRealizada = true;
            HashSet<GameObject> gruposPasajerosADestruir = new HashSet<GameObject>();

            foreach (Pasajero nuevoPasajero in pasajerosEsperando)
            {
                passengerController.addPassenger(nuevoPasajero.colorDestino, nuevoPasajero.cantidadPersonas);

                if (nuevoPasajero.transform.parent != null)
                {
                    gruposPasajerosADestruir.Add(nuevoPasajero.transform.parent.gameObject);
                }
            }

            foreach (GameObject grupo in gruposPasajerosADestruir)
            {
                if (grupo != null) Destroy(grupo);
            }

            paraderoLlegada.pasajerosEsperandoList.Clear();
            Debug.Log("Nuevos pasajeros subidos a la micro y paradero despejado.");

            // --- CORRECCIÓN: Reproducimos el audio si la lista tiene elementos ---
            if (SubirPasajeroSfx != null && SubirPasajeroSfx.Count > 0 && AudioManager.Instance != null)
            {
                AudioClip sfxSubir = SubirPasajeroSfx[Random.Range(0, SubirPasajeroSfx.Count)];
                if (sfxSubir != null)
                {
                    AudioManager.Instance.PlaySFX(sfxSubir);
                }
            }
        }

        // 3. VALIDACIÓN DE PARADA INÚTIL
        if (!accionRealizada)
        {
            RestarPuntajePorParadaInnecesaria();
        }

        // RECALCULAMOS EL CANVAS GLOBALMENTE DESPUÉS DE SUBIR/BAJAR
        GameManager.Instance.ActualizarDestinosYCanvas();
    }

    IEnumerator SubirPasajeros(Paradero paradero)
    {
        // 1. Guardamos la velocidad exacta a la que iba antes de detenerse
        int velocidadAnterior = GameManager.Instance.GeneralSpeed;

        // 2. Frena la micro
        GameManager.Instance.GeneralSpeed = 0;
        if (velocidadAnterior > 40 && AudioManager.Instance != null && FrenarSfx != null)
            AudioManager.Instance.PlayFreno(FrenarSfx);

        // 3. Ejecuta la lógica de subida y bajada
        LlegarAParadero(paradero);

        // 4. Espera el tiempo de atención en el paradero (ej. 1 segundo)
        yield return new WaitForSeconds(1f);

        // 5. Devolvemos la velocidad progresivamente hasta alcanzar la que tenía antes
        float velocidadActualFlotante = 0f;
        float tiempoAceleracion = 1f; // Tiempo que tardará en volver a su velocidad (en segundos)
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoAceleracion)
        {
            tiempoTranscurrido += Time.deltaTime;

            // Interpola suavemente desde 0 hasta la velocidad anterior
            velocidadActualFlotante = Mathf.Lerp(0f, velocidadAnterior, tiempoTranscurrido / tiempoAceleracion);

            // Actualizamos la velocidad convirtiéndola a entero
            GameManager.Instance.GeneralSpeed = Mathf.Min(Mathf.RoundToInt(velocidadActualFlotante), 100);

            yield return null; // Espera al siguiente frame
        }

        // Nos aseguramos de fijar exactamente la velocidad anterior al terminar
        GameManager.Instance.GeneralSpeed = Mathf.Min(velocidadAnterior, 100);
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