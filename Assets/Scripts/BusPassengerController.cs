
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
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
    public int capacidadMaxima = 15;

    [Header("Configuración Visual (BlendShapes)")]
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private string nombreBlendShape = "Clave 1";

    [SerializeField] private BusVFXController busVFX;

    [Header("Power-ups")]
    [SerializeField] private bool proteccionExplosion = false;

    public const int CapacidadInicial = 15;
    public const int LimiteCapacidad = 50;

    public bool TieneProteccionExplosion => proteccionExplosion;
    public bool EstaOperativa => !haExplotado;

    public bool PuedeAumentarCapacidad => !haExplotado && capacidadMaxima < LimiteCapacidad;

    private int blendShapeIndex = -1;

    [Header("Pasajeros")]
    public List<DatosPasajero> pasajerosABordo = new List<DatosPasajero>();
    public int pasajerosActuales = 0;

    private bool haExplotado = false;

    public int PasajerosActuales => pasajerosActuales;
    public int CapacidadMaxima => capacidadMaxima;

    public GameObject CompresionPanel;

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

        // 1. Si estamos exactamente al límite de la capacidad
        if (pasajerosActuales == capacidadMaxima)
        {
            Debug.Log("Activando panel por llegar al límite");
            StartCoroutine(ActivarPanel());
            return; // Salimos porque está lleno pero operativo
        }

        // 2. Si estamos por debajo de la capacidad, no hay problema
        if (pasajerosActuales < capacidadMaxima)
            return;

        // 3. Si pasamos la capacidad máxima (pasajerosActuales > capacidadMaxima)
        if (proteccionExplosion)
        {
            proteccionExplosion = false;

            // La explosión ocurre visualmente.
            if (busVFX != null)
                busVFX.PlayExplosion();

            // Se pierden todos los pasajeros.
            pasajerosABordo.Clear();

            // Se conserva la capacidad máxima actual.
            ActualizarConteoYBlendShape();

            if (GameManager.Instance != null)
                GameManager.Instance.ActualizarDestinosYCanvas();

            Debug.Log(
                "¡La micro sobrevivió a la explosión! " +
                $"Capacidad conservada: {capacidadMaxima}"
            );

            return;
        }

        // Sin protección y superando el límite, ocurre la explosión normal.
        ExplotarMicro();
    }

    // Método que llamará el power-up de capacidad.

    public void AumentarCapacidadMaxima(int cantidadExtra)
    {
        if (!PuedeAumentarCapacidad || cantidadExtra <= 0)
            return;

        capacidadMaxima = Mathf.Min(
            capacidadMaxima + cantidadExtra,
            LimiteCapacidad
        );

        ActualizarConteoYBlendShape();

        Debug.Log($"Capacidad aumentada a {capacidadMaxima}");
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

        if (busVFX != null)
        {
            busVFX.PlayExplosion();
        }

        GameManager.Instance.GameOver();
    }


    public void ActivarProteccionExplosion()
    {
        if (haExplotado)
            return;

        proteccionExplosion = true;

        Debug.Log("¡Protección antiexplosión activada!");
    }

    public void VaciarMicro()
    {
        if (haExplotado)
            return;

        pasajerosABordo.Clear();

        // Solo este power-up reinicia la capacidad.
        capacidadMaxima = CapacidadInicial;

        ActualizarConteoYBlendShape();

        if (GameManager.Instance != null)
            GameManager.Instance.ActualizarDestinosYCanvas();

        Debug.Log("Micro vacía. Capacidad restaurada a 25.");
    }

    private IEnumerator ActivarPanel()
    {
        if (CompresionPanel != null)
        {
            CompresionPanel.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            CompresionPanel.SetActive(false);
        }
    }
}
