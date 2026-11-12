using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [Header("Estado del jugador")]
    [SerializeField] private bool hasShield = false;
    public float velocidadRotacion = 360f;
    [SerializeField] private GameObject shieldVisual;

    [SerializeField] private BusVFXController busVFX;

    public bool TieneEscudo => hasShield;

    public bool isDead = false;

    public bool EstaMuerto => isDead;
    


    private void Start()
    {
        if (shieldVisual != null)
            shieldVisual.SetActive(hasShield);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {   
            Debug.Log("¡La micro chocó con un obstáculo!");
            ReceiveHit();
        }
    }


    private void Update()
    {
        // Si el jugador perdió, rota el objeto continuamente en el eje Y
        if (isDead)
        {
            transform.Rotate(0f, velocidadRotacion * Time.deltaTime, 0f);
        }
    }

    private void ReceiveHit()
    {
        if (isDead)
            return;

        if (hasShield)
        {
            hasShield = false;

            if (shieldVisual != null)
                shieldVisual.SetActive(false);

            if (busVFX != null)
                busVFX.PlayShieldBreak();

            Debug.Log("¡El escudo protegió a la micro!");
            return;
        }

        if (busVFX != null)
            busVFX.PlayCollision();

        GameManager.Instance.GameOver();
    }

    public void ActivateShield()
    {
        if (isDead)
            return;

        hasShield = true;

        if (shieldVisual != null)
            shieldVisual.SetActive(true);

        Debug.Log("¡Escudo activado!");
    }

    /*private void GameOver()
    {
        isDead = true;

        Debug.Log("¡GAME OVER! La micro chocó con un obstáculo.");

    }*/
}