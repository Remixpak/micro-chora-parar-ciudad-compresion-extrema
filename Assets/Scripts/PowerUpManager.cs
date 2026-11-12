
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PowerUpManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BusPassengerController passengerController;
    [SerializeField] private PlayerCollision playerCollision;
    [SerializeField] private BusController busController;

    [Header("HUD")]
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private PowerUpCardUI[] cards;

    [Header("Configuración")]
    [SerializeField, Min(1f)] private float intervaloPowerUps = 60f;
    [SerializeField, Min(0.1f)] private float tiempoAceleracion = 1f;

    private float temporizador;
    private int velocidadAnterior;

    private bool seleccionActiva;
    private bool reanudando;

    private PowerUpType[] opcionesActuales;

    public bool SeleccionActiva => seleccionActiva;
    public bool ControlandoVelocidad => seleccionActiva || reanudando;

    private void Start()
    {
        if (selectionPanel != null)
            selectionPanel.SetActive(false);
    }

    private void Update()
    {
        if (ControlandoVelocidad)
            return;

        if (GameManager.Instance == null ||
            passengerController == null ||
            playerCollision == null ||
            busController == null ||
            selectionPanel == null ||
            cards == null ||
            cards.Length != 3)
            return;

        if (!passengerController.EstaOperativa ||
            playerCollision.EstaMuerto)
            return;

        // No iniciar una selección durante un paradero.
        if (busController.EstaAtendiendoParadero)
            return;

        // El tiempo solo avanza cuando la micro está moviéndose.
        if (GameManager.Instance.GeneralSpeed <= 0)
            return;

        temporizador += Time.deltaTime;

        if (temporizador >= intervaloPowerUps)
        {
            temporizador = 0f;
            MostrarSeleccion();
        }
    }

    private void MostrarSeleccion()
    {
        List<PowerUpType> disponibles = new List<PowerUpType>();

        if (passengerController.PuedeAumentarCapacidad)
            disponibles.Add(PowerUpType.Aumentar);

        // Estos poderes pueden renovarse, pero no acumularse.
        disponibles.Add(PowerUpType.Escudo);
        disponibles.Add(PowerUpType.Antiexplosion);
        disponibles.Add(PowerUpType.VaciarMicro);

        // Mezclar usando Fisher-Yates.
        for (int i = disponibles.Count - 1; i > 0; i--)
        {
            int indiceAleatorio = Random.Range(0, i + 1);

            PowerUpType temporal = disponibles[i];
            disponibles[i] = disponibles[indiceAleatorio];
            disponibles[indiceAleatorio] = temporal;
        }

        int cantidad = Mathf.Min(3, disponibles.Count);
        opcionesActuales = new PowerUpType[cantidad];

        // Guardar la velocidad exacta antes de detenerse.
        velocidadAnterior = GameManager.Instance.GeneralSpeed;

        seleccionActiva = true;

        // Detener la micro.
        GameManager.Instance.GeneralSpeed = 0;

        for (int i = 0; i < cards.Length; i++)
        {
            bool mostrar = i < cantidad;

            cards[i].gameObject.SetActive(mostrar);

            if (mostrar)
            {
                opcionesActuales[i] = disponibles[i];

                cards[i].Configurar(
                    disponibles[i],
                    i,
                    this
                );
            }
        }

        selectionPanel.SetActive(true);
    }

    public void ElegirPowerUp(int indice)
    {
        if (!seleccionActiva ||
            opcionesActuales == null ||
            indice < 0 ||
            indice >= opcionesActuales.Length)
            return;

        // Impide seleccionar dos cartas.
        seleccionActiva = false;
        reanudando = true;

        PowerUpType elegido = opcionesActuales[indice];

        switch (elegido)
        {
            case PowerUpType.Aumentar:
                passengerController.AumentarCapacidadMaxima(3);
                break;

            case PowerUpType.Escudo:
                playerCollision.ActivateShield();
                break;

            case PowerUpType.Antiexplosion:
                passengerController.ActivarProteccionExplosion();
                break;

            case PowerUpType.VaciarMicro:
                passengerController.VaciarMicro();
                break;
        }

        selectionPanel.SetActive(false);

        Debug.Log($"Power-up seleccionado: {elegido}");

        StartCoroutine(ReanudarMicro());
    }

    private IEnumerator ReanudarMicro()
    {
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

        // Igual que en BusController:
        // restaurar exactamente la velocidad anterior.
        GameManager.Instance.GeneralSpeed =
            Mathf.Min(velocidadAnterior, 100);

        reanudando = false;
    }
}
