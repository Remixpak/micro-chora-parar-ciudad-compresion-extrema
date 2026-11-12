using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [Header("Estado del jugador")]
    [SerializeField] private bool hasShield = false;
    [SerializeField] private GameObject shieldVisual;

    [SerializeField] private BusVFXController busVFX;


    private bool isDead = false;

    private void Update()
    {
        if(hasShield)
        {
            shieldVisual.SetActive(true);
        }
        else
        {
            shieldVisual.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {   
            Debug.Log("¡La micro chocó con un obstáculo!");
            ReceiveHit();
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

        GameOver();
    }

    public void ActivateShield()
    {
        if (isDead)
            return;

        hasShield = true;
        shieldVisual.SetActive(true);
        Debug.Log("¡Escudo activado!");
    }

    private void GameOver()
    {
        isDead = true;

        Debug.Log("¡GAME OVER! La micro chocó con un obstáculo.");

    }
}