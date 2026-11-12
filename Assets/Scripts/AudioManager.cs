using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Playlist de Música de Fondo")]
    [SerializeField] private List<AudioClip> playlistMusica = new List<AudioClip>();
    private int indiceCancionActual = 0;
    private Coroutine corrutinaPlaylist;

    private void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Si hay canciones en la playlist al iniciar, comenzamos la playlist aleatoriamente
        if (playlistMusica != null && playlistMusica.Count > 0)
        {
            IniciarPlaylistAleatoria();
        }
    }

    public void IniciarPlaylistAleatoria()
    {
        if (playlistMusica.Count == 0) return;

        if (corrutinaPlaylist != null)
        {
            StopCoroutine(corrutinaPlaylist);
        }

        // Elegimos un índice inicial completamente al azar entre las canciones disponibles
        indiceCancionActual = Random.Range(0, playlistMusica.Count);
        corrutinaPlaylist = StartCoroutine(ReproducirSecuenciaMusical());
    }

    private IEnumerator ReproducirSecuenciaMusical()
    {
        while (true)
        {
            AudioClip cancionActual = playlistMusica[indiceCancionActual];

            if (cancionActual != null)
            {
                musicSource.clip = cancionActual;
                musicSource.loop = false; // Desactivamos el bucle individual para permitir avanzar
                musicSource.Play();

                // Esperamos exactamente lo que dura la canción actual
                yield return new WaitForSecondsRealtime(cancionActual.length);
            }

            // Avanzamos al siguiente índice de forma circular (0 -> 1 -> 2 -> 0 -> ...)
            // Si empezó en la 1, seguirá con la 2, luego la 0, y así sucesivamente.
            indiceCancionActual = (indiceCancionActual + 1) % playlistMusica.Count;
        }
    }

    /// <summary>
    /// Reproduce una canción específica de forma individual (interrumpiendo la playlist).
    /// </summary>
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null) return;

        if (corrutinaPlaylist != null)
        {
            StopCoroutine(corrutinaPlaylist);
            corrutinaPlaylist = null;
        }

        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    /// <summary>
    /// Reproduce un efecto de sonido (SFX) por un instante. 
    /// Puede ser llamado desde cualquier otro script.
    /// </summary>
    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip == null) return;

        sfxSource.PlayOneShot(sfxClip);
    }
}