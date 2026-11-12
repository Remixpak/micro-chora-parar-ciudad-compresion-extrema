using UnityEngine;

public class StopAnimation : MonoBehaviour
{
    [Header("Configuración de Animación")]
    [SerializeField] private float velocidadAnimacionBase = 4f; // Velocidad base del loop
    [SerializeField] private float velocidadBaseJuego = 30f;     // La velocidad inicial estándar del GameManager

    private void Update()
    {
        // Si la micro está detenida, podemos pausar la animación o dejarla quieta
        if (GameManager.Instance != null && GameManager.Instance.GeneralSpeed == 0)
        {
            return; // Opcional: si quieres que se detenga al frenar
        }

        // Calculamos la velocidad dinámica basada en la GeneralSpeed actual del GameManager
        float velocidadActualJuego = (GameManager.Instance != null) ? GameManager.Instance.GeneralSpeed : velocidadBaseJuego;
        float factorVelocidad = velocidadActualJuego / velocidadBaseJuego;
        float velocidadEfectiva = velocidadAnimacionBase * factorVelocidad;

        // 1. Oscilación en bucle usando Seno con la velocidad adaptada
        float seno = Mathf.Sin(Time.time * velocidadEfectiva);

        // 2. Rotación en el eje Z (interpolando de 0 a -2)
        float rotacionZ = Mathf.Lerp(0f, -2f, (seno + 1f) / 2f);

        Vector3 rotacionActual = transform.localEulerAngles;
        rotacionActual.z = rotacionZ;
        transform.localEulerAngles = rotacionActual;

        // 3. Escalado en todos los ejes (interpolando de 0.3 a 0.4)
        float escalaActual = Mathf.Lerp(0.3f, 0.4f, (seno + 1f) / 2f);
        transform.localScale = new Vector3(escalaActual, escalaActual, escalaActual);
    }
}