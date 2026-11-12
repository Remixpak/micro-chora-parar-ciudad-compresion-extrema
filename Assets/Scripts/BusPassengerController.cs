
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DatosPasajero
{
    public colorParadero colorDestino;
    public int cantidadPersonas;

    public DatosPasajero(colorParadero destino, int cantidad)
    {
        colorDestino = destino;
        cantidadPersonas = cantidad;
    }
}

public class BusPassengerController : MonoBehaviour
{
    [Header("Configuración de Capacidad")]
    [Tooltip("Capacidad máxima de pasajeros. Puede modificarse mediante power-ups.")]
    public int capacidadMaxima = 25;

    [Header("Configuración Visual (BlendShapes)")]
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private string nombreBlendShape = "Clave 1";

    private int blendShapeIndex = -1;

    [Header("Pasajeros")]
    public List<DatosPasajero> pasajerosABordo = new List<DatosPasajero>();
    public int pasajerosActuales = 0;

    private bool haExplotado = false;

    public int PasajerosActuales => pasajerosActuales;
    public int CapacidadMaxima => capacidadMaxima;

    private void Awake()
    {
        // Buscar el índice del BlendShape.
        if (skinnedMeshRenderer != null &&
            skinnedMeshRenderer.sharedMesh != null)
        {
            blendShapeIndex = skinnedMeshRenderer.sharedMesh
                .GetBlendShapeIndex(nombreBlendShape);

            if (blendShapeIndex == -1)
            {
                Debug.LogWarning(
                    $"No se encontró el BlendShape '{nombreBlendShape}'."
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "Falta asignar el SkinnedMeshRenderer o su malla."
            );
        }

        ActualizarConteoYBlendShape();
        VerificarCapacidad();
    }

    public void addPassenger(colorParadero destino, int cantidad)
    {
        if (haExplotado || cantidad <= 0)
            return;

        pasajerosABordo.Add(new DatosPasajero(destino, cantidad));

        ActualizarConteoYBlendShape();
        VerificarCapacidad();
    }

    public void RemovePassengers(List<DatosPasajero> aBajar)
    {
        if (haExplotado || aBajar == null)
            return;

        foreach (var pasajero in aBajar)
        {
            pasajerosABordo.Remove(pasajero);
        }

        ActualizarConteoYBlendShape();
    }

    private void ActualizarConteoYBlendShape()
    {
        int totalPersonas = 0;

        foreach (var pasajero in pasajerosABordo)
        {
            totalPersonas += pasajero.cantidadPersonas;
        }

        pasajerosActuales = totalPersonas;

        if (skinnedMeshRenderer != null &&
            blendShapeIndex != -1 &&
            capacidadMaxima > 0)
        {
            float porcentaje =
                ((float)pasajerosActuales / capacidadMaxima) * 100f;

            porcentaje = Mathf.Clamp(porcentaje, 0f, 100f);

            skinnedMeshRenderer.SetBlendShapeWeight(
                blendShapeIndex,
                porcentaje
            );
        }
    }

    private void VerificarCapacidad()
    {
        if (haExplotado)
            return;

        if (pasajerosActuales > capacidadMaxima)
        {
            ExplotarMicro();
        }
    }

    // Método que llamará el power-up de capacidad.
    public void AumentarCapacidadMaxima(int cantidadExtra)
    {
        if (haExplotado || cantidadExtra <= 0)
            return;

        capacidadMaxima += cantidadExtra;

        // Recalcular la deformación según el nuevo límite.
        ActualizarConteoYBlendShape();

        Debug.Log(
            $"¡Capacidad aumentada! Nuevo máximo: {capacidadMaxima}"
        );
    }

    private void ExplotarMicro()
    {
        if (haExplotado)
            return;

        haExplotado = true;

        Debug.LogError(
            $"¡BOOM! La micro explotó por exceso de pasajeros. " +
            $"Pasajeros: {pasajerosActuales}/{capacidadMaxima}"
        );

        // FUTURO:
        // Reproducir animación de explosión.
        // Activar partículas.
        // Ejecutar Game Over.
    }
}
