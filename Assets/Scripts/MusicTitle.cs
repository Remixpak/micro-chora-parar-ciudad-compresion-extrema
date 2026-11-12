using UnityEngine;

public class MusicTitle : MonoBehaviour
{
    public AudioClip currentMusicClip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlayMusic(currentMusicClip); // Reproduce la primera canción de la playlist al iniciar el juego
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
