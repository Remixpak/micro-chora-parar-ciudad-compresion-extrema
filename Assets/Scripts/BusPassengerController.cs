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

    // La lista almacena datos puros sin perder referencias
    public List<DatosPasajero> pasajerosABordo = new List<DatosPasajero>();
    public int pasajerosActuales = 0;

    private void Awake()
    {
        // Buscamos el índice del BlendShape "Clave 1" automáticamente
        if (skinnedMeshRenderer != null)
        {
            blendShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(nombreBlendShape);
            if (blendShapeIndex == -1)
            {
                Debug.LogWarning($"No se encontró el BlendShape '{nombreBlendShape}' en el SkinnedMeshRenderer.");
            }
        }
        else
        {
            Debug.LogError("Falta asignar el SkinnedMeshRenderer en el Inspector.");
        }
    }

    public void addPassenger(colorParadero destino, int cantidad)
    {
        pasajerosABordo.Add(new DatosPasajero(destino, cantidad));
        ActualizarConteoYBlendShape();
    }

    public void RemovePassengers(List<DatosPasajero> aBajar)
    {
        foreach (var pasajero in aBajar)
        {
            pasajerosABordo.Remove(pasajero);
        }
        ActualizarConteoYBlendShape();
    }

    private void ActualizarConteoYBlendShape()
    {
        // Sumamos las personas reales a bordo (considerando grupos de 1, 3, 5, etc.)
        int totalPersonas = 0;
        foreach (var pasajero in pasajerosABordo)
        {
            totalPersonas += pasajero.cantidadPersonas;
        }

        pasajerosActuales = totalPersonas;

        // Actualizamos el BlendShape de la micro (de 0 a 100)
        if (skinnedMeshRenderer != null && blendShapeIndex != -1)
        {
            // Calculamos el porcentaje en base a la capacidad máxima actual
            float porcentaje = ((float)pasajerosActuales / capacidadMaxima) * 100f;

            // Limitamos el valor entre 0 y 100 por seguridad
            porcentaje = Mathf.Clamp(porcentaje, 0f, 100f);

            // Aplicamos el peso al BlendShape "Clave 1"
            skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, porcentaje);
        }
    }
}