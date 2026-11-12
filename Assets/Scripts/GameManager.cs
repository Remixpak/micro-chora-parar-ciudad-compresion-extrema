using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject StopImage;
    public List<Paradero> paraderosDestino = new List<Paradero>();
    public int GeneralSpeed = 30;

    [Header("Aceleraci�n Progresiva")]
    [SerializeField] private float intervaloAceleracion = 30f; // Segundos entre cada aumento
    [SerializeField] private int incrementoVelocidad = 10;     // Cu�nto aumenta la velocidad
    private float temporizadorAceleracion = 0f;

    [Header("Sistema de Power-ups"  )]
    [SerializeField] private PowerUpManager powerUpManager;

    [Header("Imagenes de Paraderos")]
    [SerializeField] private Sprite rojoSprite;
    [SerializeField] private Sprite azulSprite;
    [SerializeField] private Sprite verdeSprite;
    [SerializeField] private Sprite amarilloSprite;

    [Header("Sfx")]
    public List<AudioClip> ClipsBajada = new List<AudioClip>();
    public AudioClip rojoSfx;
    public AudioClip azulSfx;
    public AudioClip verdeSfx;
    public AudioClip amarilloSfx;
    public PlayerCollision playerCollision;
    [Header("Canvases")]
    [SerializeField] private GameObject GameOverCanvas;
    [SerializeField] private GameObject Pause;
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
        playerCollision = FindFirstObjectByType<PlayerCollision>();
    }


    private void Update()
    {
        // No acelerar durante la selección ni su reanudación.
        if (powerUpManager != null &&
            powerUpManager.ControlandoVelocidad)
        {
            return;
        }

        // Tampoco acelerar mientras la micro está detenida.
        if (GeneralSpeed <= 0)
            return;

        temporizadorAceleracion += Time.deltaTime;

        if (temporizadorAceleracion >= intervaloAceleracion)
        {
            GeneralSpeed += incrementoVelocidad;

            GeneralSpeed = Mathf.Min(GeneralSpeed, 100);

            temporizadorAceleracion = 0f;

            Debug.Log(
                $"¡La micro ha acelerado! Nueva velocidad: {GeneralSpeed}"
            );
        }
    }


    // M�todo centralizado para evaluar pasajeros a bordo y paraderos activos en la escena
    public void ActualizarDestinosYCanvas()
    {
        BusPassengerController controller = FindObjectOfType<BusPassengerController>();
        if (controller == null) return;

        // Limpiamos la lista actual de destinos para recalcularla limpiamente
        paraderosDestino.Clear();

        // Buscamos todos los paraderos que est�n vivos actualmente en la escena
        Paradero[] paraderosEnEscena = FindObjectsOfType<Paradero>();

        // Cruzamos los datos: Paraderos en escena vs Pasajeros a bordo
        foreach (Paradero paradero in paraderosEnEscena)
        {
            foreach (DatosPasajero pasajero in controller.pasajerosABordo)
            {
                // Si el color del paradero coincide con el destino del pasajero y a�n no est� en la lista
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
            AudioClip sfxCorrespondiente = null;

            switch (paraderosDestino[0].colorParadero)
            {
                case colorParadero.rojo:
                    spriteAAsignar = rojoSprite;
                    sfxCorrespondiente = rojoSfx;
                    break;
                case colorParadero.azul:
                    spriteAAsignar = azulSprite;
                    sfxCorrespondiente = azulSfx;
                    break;
                case colorParadero.verde:
                    spriteAAsignar = verdeSprite;
                    sfxCorrespondiente = verdeSfx;
                    break;
                case colorParadero.amarillo:
                    spriteAAsignar = amarilloSprite;
                    sfxCorrespondiente = amarilloSfx;
                    break;
            }

            if (spriteRenderer != null) spriteRenderer.sprite = spriteAAsignar;
            if (uiImage != null) uiImage.sprite = spriteAAsignar;

            // --- SELECCI�N ALEATORIA DE AUDIO (SFX del color o de la lista general) ---
            ReproducirAudioAleatorioParada(sfxCorrespondiente);
        }
    }

    private void ReproducirAudioAleatorioParada(AudioClip sfxColorEspecifico)
    {
        // Creamos una lista temporal que contenga el audio del color y todos los de la lista general
        List<AudioClip> audiosDisponibles = new List<AudioClip>();

        if (sfxColorEspecifico != null)
        {
            audiosDisponibles.Add(sfxColorEspecifico);
        }

        if (ClipsBajada != null && ClipsBajada.Count > 0)
        {
            // A�adimos todos los clips de la lista general
            audiosDisponibles.AddRange(ClipsBajada);
        }

        // Si tenemos al menos un audio disponible, elegimos uno al azar y lo reproducimos mediante el AudioManager
        if (audiosDisponibles.Count > 0 && AudioManager.Instance != null)
        {
            int indiceAleatorio = Random.Range(0, audiosDisponibles.Count);
            AudioClip audioSeleccionado = audiosDisponibles[indiceAleatorio];

            AudioManager.Instance.PlaySFX(audioSeleccionado);
            Debug.Log($"Reproduciendo sonido de parada aleatorio: {audioSeleccionado.name}");
        }
    }
    public void Reinciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu Inicial");
    }

    public void GameOver()
    {
        
        playerCollision.isDead = true;
        GameOverCanvas.gameObject.SetActive(true);
        Debug.Log("¡GAME OVER! La micro chocó con un obstáculo.");

    }
}