using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BusController : MonoBehaviour
{
    private BusPassengerController passengerController;
    [Header("Sfx")]
    public AudioClip FrenarSfx;
    public List<AudioClip> SubirPasajeroSfx = new List<AudioClip>();

    public bool EstaAtendiendoParadero { get; private set; }

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
        Debug.Log($"La micro lleg� a un paradero de color: {paraderoLlegada.colorParadero}");

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

            // --- CORRECCI�N: Reproducimos el audio si la lista tiene elementos ---
            if (SubirPasajeroSfx != null && SubirPasajeroSfx.Count > 0 && AudioManager.Instance != null)
            {
                AudioClip sfxSubir = SubirPasajeroSfx[Random.Range(0, SubirPasajeroSfx.Count)];
                if (sfxSubir != null)
                {
                    AudioManager.Instance.PlaySFX(sfxSubir);
                }
            }
        }

        // 3. VALIDACI�N DE PARADA IN�TIL
        if (!accionRealizada)
        {
            RestarPuntajePorParadaInnecesaria();
        }

        // RECALCULAMOS EL CANVAS GLOBALMENTE DESPU�S DE SUBIR/BAJAR
        GameManager.Instance.ActualizarDestinosYCanvas();
    }


    private IEnumerator SubirPasajeros(Paradero paradero)
    {
        if (EstaAtendiendoParadero)
            yield break;

        EstaAtendiendoParadero = true;

        int velocidadAnterior = GameManager.Instance.GeneralSpeed;

        // Detener la micro.
        GameManager.Instance.GeneralSpeed = 0;

        if (velocidadAnterior > 40 &&
            AudioManager.Instance != null &&
            FrenarSfx != null)
        {
            AudioManager.Instance.PlayFreno(FrenarSfx);
        }

        LlegarAParadero(paradero);

        yield return new WaitForSeconds(1f);

        // Recuperar velocidad progresivamente.
        float tiempoAceleracion = 1f;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoAceleracion)
        {
            tiempoTranscurrido += Time.deltaTime;

            float velocidad = Mathf.Lerp(
                0f,
                velocidadAnterior,
                Mathf.Clamp01(
                    tiempoTranscurrido / tiempoAceleracion
                )
            );

            GameManager.Instance.GeneralSpeed = Mathf.Min(
                Mathf.RoundToInt(velocidad),
                100
            );

            yield return null;
        }

        GameManager.Instance.GeneralSpeed =
            Mathf.Min(velocidadAnterior, 100);

        EstaAtendiendoParadero = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stop") && !GameManager.Instance.playerCollision.isDead)
        {
            if (other.TryGetComponent<Paradero>(out Paradero paradero))
            {
                StartCoroutine(SubirPasajeros(paradero));
            }
        }
    }

    void RestarPuntajePorParadaInnecesaria()
    {
        Debug.Log("�PENALIZACI�N! La micro se detuvo en un paradero donde nadie se bajaba ni sub�a.");
    }
}