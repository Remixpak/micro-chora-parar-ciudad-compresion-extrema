
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

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame ||
                Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                playerAnimator.SetTrigger("LeftSwipe");
                ChangeLane(-1);
            }

            if (Keyboard.current.dKey.wasPressedThisFrame ||
                Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                playerAnimator.SetTrigger("RightSwipe");
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

        position.x = Mathf.MoveTowards(
            position.x,
            targetX,
            laneChangeSpeed * Time.deltaTime
        );

        transform.position = position;
    }
}
