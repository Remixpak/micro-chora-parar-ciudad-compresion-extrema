using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLaneMovement : MonoBehaviour
{
    [Header("Carriles")]
    [SerializeField] private Transform[] lanePoints;
    [SerializeField] private int currentLane = 1;

    [SerializeField] private Animator playerAnimator;

    [Header("Movimiento")]
    [SerializeField] private float laneChangeSpeed = 10f;

    [Header("Configuración de Velocidad Dinámica")]
    [SerializeField] private float velocidadBase = 30f; // La velocidad inicial estándar del juego

    private void Update()
    {
        // Si la micro está detenida en un paradero, pausamos la animación y no permitimos cambiar de carril
        if (GameManager.Instance.GeneralSpeed == 0)
        {
            if (playerAnimator != null) playerAnimator.speed = 0f;
            return;
        }

        // Ajustamos la velocidad del Animator de forma proporcional a la velocidad general
        if (playerAnimator != null)
        {
            float factorVelocidad = (float)GameManager.Instance.GeneralSpeed / velocidadBase;
            playerAnimator.speed = Mathf.Max(factorVelocidad, 1f);
        }

        // Detección de teclas para cambiar de carril
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame ||
                Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                if (playerAnimator != null) playerAnimator.SetTrigger("LeftSwipe");
                ChangeLane(-1);
            }

            if (Keyboard.current.dKey.wasPressedThisFrame ||
                Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                if (playerAnimator != null) playerAnimator.SetTrigger("RightSwipe");
                ChangeLane(1);
            }
        }

        MoveToLane();
    }

    private void ChangeLane(int direction)
    {
        currentLane = Mathf.Clamp(
            currentLane + direction,
            0,
            lanePoints.Length - 1
        );
    }

    private void MoveToLane()
    {
        if (lanePoints == null || lanePoints.Length == 0)
            return;

        Vector3 position = transform.position;
        float targetX = lanePoints[currentLane].position.x;

        // El movimiento se mantiene tal cual con tu laneChangeSpeed original
        position.x = Mathf.MoveTowards(
            position.x,
            targetX,
            laneChangeSpeed * Time.deltaTime
        );

        transform.position = position;
    }
}