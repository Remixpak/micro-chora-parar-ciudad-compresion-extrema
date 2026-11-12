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
            Debug.LogError(
                "Falta el componente BusPassengerController en la micro."
            );
        }
    }

    public void LlegarAParadero(Paradero paraderoLlegada)
    {
        if (paraderoLlegada == null || passengerController == null)
            return;

        Debug.Log(
            $"La micro llegó a un paradero de color: {paraderoLlegada.colorParadero}"
        );

        bool accionRealizada = false;

        // 1. BAJAR PASAJEROS EN SU PARADERO CORRECTO

        List<DatosPasajero> pasajerosABajar =
            passengerController.pasajerosABordo.FindAll(
                p => p.colorDestino == paraderoLlegada.colorParadero
            );

        if (pasajerosABajar.Count > 0)
        {
            accionRealizada = true;

            int cantidadBajada = 0;

            foreach (DatosPasajero pasajero in pasajerosABajar)
            {
                cantidadBajada += pasajero.cantidadPersonas;
            }

            passengerController.RemovePassengers(pasajerosABajar);

            // +100 por realizar una entrega correcta.
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.RegistrarEntrega();
            }

            Debug.Log(
                $"Se bajaron {cantidadBajada} pasajeros. +100 puntos."
            );
        }

        // 2. SUBIR NUEVOS PASAJEROS

        Pasajero[] pasajerosEsperando =
            paraderoLlegada.GetComponentsInChildren<Pasajero>();

        if (pasajerosEsperando.Length > 0)
        {
            accionRealizada = true;

            HashSet<GameObject> gruposPasajerosADestruir =
                new HashSet<GameObject>();

            int totalPasajerosSubidos = 0;

            foreach (Pasajero nuevoPasajero in pasajerosEsperando)
            {
                int cantidad = nuevoPasajero.cantidadPersonas;

                passengerController.addPassenger(
                    nuevoPasajero.colorDestino,
                    cantidad
                );

                totalPasajerosSubidos += cantidad;

                // +100 por cada persona que sube.
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.RegistrarSubida(cantidad);
                }

                if (nuevoPasajero.transform.parent != null)
                {
                    gruposPasajerosADestruir.Add(
                        nuevoPasajero.transform.parent.gameObject
                    );
                }
            }

            foreach (GameObject grupo in gruposPasajerosADestruir)
            {
                if (grupo != null)
                {
                    Destroy(grupo);
                }
            }

            paraderoLlegada.pasajerosEsperandoList.Clear();

            Debug.Log(
                $"Subieron {totalPasajerosSubidos} pasajeros. " +
                $"+{totalPasajerosSubidos * 100} puntos."
            );

            // Reproducir audio de subida.
            if (SubirPasajeroSfx != null &&
                SubirPasajeroSfx.Count > 0 &&
                AudioManager.Instance != null)
            {
                AudioClip sfxSubir = SubirPasajeroSfx[
                    Random.Range(0, SubirPasajeroSfx.Count)
                ];

                if (sfxSubir != null)
                {
                    AudioManager.Instance.PlaySFX(sfxSubir);
                }
            }
        }

        // 3. PENALIZAR PARADA INNECESARIA

        if (!accionRealizada)
        {
            RestarPuntajePorParadaInnecesaria();
        }

        // 4. ACTUALIZAR DESTINOS Y CANVAS

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ActualizarDestinosYCanvas();
        }
    }

    private IEnumerator SubirPasajeros(Paradero paradero)
    {
        if (EstaAtendiendoParadero)
            yield break;

        if (GameManager.Instance == null)
            yield break;

        EstaAtendiendoParadero = true;

        int velocidadAnterior =
            GameManager.Instance.GeneralSpeed;

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
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.playerCollision.isDead)
            return;

        if (EstaAtendiendoParadero)
            return;

        if (other.CompareTag("Stop"))
        {
            if (other.TryGetComponent<Paradero>(
                out Paradero paradero))
            {
                StartCoroutine(SubirPasajeros(paradero));
            }
        }
    }

    private void RestarPuntajePorParadaInnecesaria()
    {
        // -100 por detenerse sin subir ni bajar pasajeros.
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.RegistrarParaderoIncorrecto();
        }

        Debug.Log(
            "¡PENALIZACIÓN! Parada innecesaria: -100 puntos."
        );
    }
}
