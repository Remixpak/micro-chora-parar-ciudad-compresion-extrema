using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject StopImage;
    public List<Paradero> paraderosDestino = new List<Paradero>();
    public int GeneralSpeed = 30;

    [Header("Aceleración Progresiva")]
    [SerializeField] private float intervaloAceleracion = 30f; // Segundos entre cada aumento
    [SerializeField] private int incrementoVelocidad = 10;     // Cuánto aumenta la velocidad
    private float temporizadorAceleracion = 0f;

    [Header("Imagenes de Paraderos")]
    [SerializeField] private Sprite rojoSprite;
    [SerializeField] private Sprite azulSprite;
    [SerializeField] private Sprite verdeSprite;
    [SerializeField] private Sprite amarilloSprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Acumulamos el tiempo transcurrido
        temporizadorAceleracion += Time.deltaTime;

        // Si pasan 30 segundos (o el intervalo configurado)
        if (temporizadorAceleracion >= intervaloAceleracion)
        {
            GeneralSpeed += incrementoVelocidad;

            // Limitamos la velocidad para que no pase de 100
            GeneralSpeed = Mathf.Min(GeneralSpeed, 100);

            temporizadorAceleracion = 0f; // Reiniciamos el temporizador
            Debug.Log($"¡La micro ha acelerado! Nueva velocidad general: {GeneralSpeed}");
        }
    }

    // Método centralizado para evaluar pasajeros a bordo y paraderos activos en la escena
    public void ActualizarDestinosYCanvas()
    {
        BusPassengerController controller = FindObjectOfType<BusPassengerController>();
        if (controller == null) return;

        // Limpiamos la lista actual de destinos para recalcularla limpiamente
        paraderosDestino.Clear();

        // Buscamos todos los paraderos que están vivos actualmente en la escena
        Paradero[] paraderosEnEscena = FindObjectsOfType<Paradero>();

        // Cruzamos los datos: Paraderos en escena vs Pasajeros a bordo
        foreach (Paradero paradero in paraderosEnEscena)
        {
            foreach (DatosPasajero pasajero in controller.pasajerosABordo)
            {
                // Si el color del paradero coincide con el destino del pasajero y aún no está en la lista
                if (pasajero.colorDestino == paradero.colorParadero && !paraderosDestino.Contains(paradero))
                {
                    paraderosDestino.Add(paradero);
                }
            }
        }

        // Evaluamos si debemos mostrar u ocultar el Canvas de Stop
        if (paraderosDestino.Count > 0)
        {
            ActivateStopCanvas();
        }
        else
        {
            DeactivateStopCanvas();
        }
    }

    private void ActivateStopCanvas()
    {
        UpdateStopCanvas();
        if (StopImage != null && !StopImage.activeSelf)
        {
            StopImage.SetActive(true);
        }
    }

    public void DeactivateStopCanvas()
    {
        if (StopImage != null && StopImage.activeSelf)
        {
            StopImage.SetActive(false);
        }
    }

    public void UpdateStopCanvas()
    {
        if (paraderosDestino.Count > 0 && paraderosDestino[0] != null)
        {
            // Verificamos si usa SpriteRenderer o UI Image regular para evitar errores
            var spriteRenderer = StopImage.GetComponent<SpriteRenderer>();
            var uiImage = StopImage.GetComponent<Image>();

            Sprite spriteAAsignar = null;

            switch (paraderosDestino[0].colorParadero)
            {
                case colorParadero.rojo: spriteAAsignar = rojoSprite; break;
                case colorParadero.azul: spriteAAsignar = azulSprite; break;
                case colorParadero.verde: spriteAAsignar = verdeSprite; break;
                case colorParadero.amarillo: spriteAAsignar = amarilloSprite; break;
            }

            if (spriteRenderer != null) spriteRenderer.sprite = spriteAAsignar;
            if (uiImage != null) uiImage.sprite = spriteAAsignar;
        }
    }
}