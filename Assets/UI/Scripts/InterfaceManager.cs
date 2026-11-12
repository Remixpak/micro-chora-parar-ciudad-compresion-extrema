
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    [Header("Números personalizados")]
    [SerializeField] private Sprite[] spritesNumbers = new Sprite[10];

    [Header("Puntaje")]
    [SerializeField] private Transform container;
    [SerializeField] private int currentScore = 0;
    [SerializeField] private ScoreManager scoreManager;

    [Header("Pasajeros")]
    [SerializeField] private BusPassengerController passengerController;
    [SerializeField] private Transform pasajerosContainer;
    [SerializeField] private Transform capacidadContainer;

    [Header("Iconos de Power-ups")]
    [SerializeField] private GameObject iconoEscudo;
    [SerializeField] private GameObject iconoAntiexplosion;

    private readonly List<Image> digitImages = new List<Image>();
    private readonly List<Image> pasajerosImages = new List<Image>();
    private readonly List<Image> capacidadImages = new List<Image>();

    private PlayerCollision playerCollision;

    private int ultimosPasajeros = -1;
    private int ultimaCapacidad = -1;
    private bool ultimoEscudo;
    private bool ultimaAntiexplosion;

    public int CurrentScore
    {
        get => currentScore;
        set
        {
            currentScore = Mathf.Max(0, value);
            UpdateScoreUI();
        }
    }

    private void Awake()
    {
        if (passengerController != null)
        {
            playerCollision =
                passengerController.GetComponent<PlayerCollision>();
        }
    }

    private void OnEnable()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= ActualizarPuntaje;
            scoreManager.OnScoreChanged += ActualizarPuntaje;

            ActualizarPuntaje(scoreManager.PuntajeActual);
        }
    }

    private void Start()
    {
        if (scoreManager == null)
            scoreManager = ScoreManager.Instance;

        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= ActualizarPuntaje;
            scoreManager.OnScoreChanged += ActualizarPuntaje;

            ActualizarPuntaje(scoreManager.PuntajeActual);
        }
        else
        {
            Debug.LogWarning(
                "InterfaceManager: No se encontró ScoreManager."
            );
        }

        if (passengerController != null && playerCollision == null)
        {
            playerCollision =
                passengerController.GetComponent<PlayerCollision>();
        }

        UpdateScoreUI();
        ActualizarPasajeros(true);
        ActualizarPowerUps(true);
    }

    private void OnDisable()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= ActualizarPuntaje;
        }
    }

    private void Update()
    {
        ActualizarPasajeros(false);
        ActualizarPowerUps(false);
    }

    // -----------------------------
    // PUNTAJE
    // -----------------------------

    private void ActualizarPuntaje(int nuevoPuntaje)
    {
        CurrentScore = nuevoPuntaje;
    }

    public void ModifyScore(int amount)
    {
        if (scoreManager == null)
            return;

        if (amount >= 0)
            scoreManager.SumarPuntos(amount);
        else
            scoreManager.RestarPuntos(-amount);
    }

    private void UpdateScoreUI()
    {
        MostrarNumero(currentScore, container, digitImages);
    }

    // -----------------------------
    // PASAJEROS
    // -----------------------------

    private void ActualizarPasajeros(bool forzar)
    {
        if (passengerController == null)
            return;

        int pasajeros = passengerController.PasajerosActuales;
        int capacidad = passengerController.CapacidadMaxima;

        if (forzar || pasajeros != ultimosPasajeros)
        {
            MostrarNumero(
                pasajeros,
                pasajerosContainer,
                pasajerosImages
            );

            ultimosPasajeros = pasajeros;
        }

        if (forzar || capacidad != ultimaCapacidad)
        {
            MostrarNumero(
                capacidad,
                capacidadContainer,
                capacidadImages
            );

            ultimaCapacidad = capacidad;
        }
    }

    // -----------------------------
    // POWER-UPS
    // -----------------------------

    private void ActualizarPowerUps(bool forzar)
    {
        if (playerCollision != null)
        {
            bool escudoActivo = playerCollision.TieneEscudo;

            if (forzar || escudoActivo != ultimoEscudo)
            {
                if (iconoEscudo != null)
                    iconoEscudo.SetActive(escudoActivo);

                ultimoEscudo = escudoActivo;
            }
        }

        if (passengerController != null)
        {
            bool antiexplosionActiva =
                passengerController.TieneProteccionExplosion;

            if (forzar ||
                antiexplosionActiva != ultimaAntiexplosion)
            {
                if (iconoAntiexplosion != null)
                    iconoAntiexplosion.SetActive(
                        antiexplosionActiva
                    );

                ultimaAntiexplosion = antiexplosionActiva;
            }
        }
    }

    // -----------------------------
    // NÚMEROS PERSONALIZADOS
    // -----------------------------

    private void MostrarNumero(
        int numero,
        Transform contenedor,
        List<Image> imagenes)
    {
        if (contenedor == null)
            return;

        string numeroString = Mathf.Max(0, numero).ToString();

        while (imagenes.Count < numeroString.Length)
        {
            GameObject digitObj = new GameObject(
                "Digit",
                typeof(RectTransform),
                typeof(Image)
            );

            digitObj.transform.SetParent(contenedor, false);

            Image img = digitObj.GetComponent<Image>();
            img.preserveAspect = true;

            imagenes.Add(img);
        }

        for (int i = 0; i < imagenes.Count; i++)
        {
            if (i < numeroString.Length)
            {
                imagenes[i].gameObject.SetActive(true);

                int digitValue = numeroString[i] - '0';

                if (digitValue >= 0 &&
                    digitValue < spritesNumbers.Length)
                {
                    imagenes[i].sprite =
                        spritesNumbers[digitValue];
                }
            }
            else
            {
                imagenes[i].gameObject.SetActive(false);
            }
        }
    }
}
