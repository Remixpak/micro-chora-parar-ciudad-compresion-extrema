
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
    [Header("Pasajeros")]
    public List<DatosPasajero> pasajerosABordo = new List<DatosPasajero>();

    [SerializeField] private int pasajerosActuales = 0;
    [SerializeField] private int capacidadMaxima = 10;

    private bool haExplotado = false;

    public int PasajerosActuales => pasajerosActuales;
    public int CapacidadMaxima => capacidadMaxima;

    private void Awake()
    {
        ActualizarCantidadPasajeros();
        VerificarCapacidad();
    }

    public void addPassenger(colorParadero destino, int cantidad)
    {
        if (haExplotado || cantidad <= 0)
            return;

        pasajerosABordo.Add(new DatosPasajero(destino, cantidad));

        ActualizarCantidadPasajeros();
        VerificarCapacidad();
    }

    public void RemovePassengers(List<DatosPasajero> aBajar)
    {
        if (aBajar == null)
            return;

        foreach (var pasajero in aBajar)
        {
            pasajerosABordo.Remove(pasajero);
        }

        ActualizarCantidadPasajeros();
    }

    private void ActualizarCantidadPasajeros()
    {
        pasajerosActuales = 0;

        foreach (var pasajero in pasajerosABordo)
        {
            pasajerosActuales += pasajero.cantidadPersonas;
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

    public void AumentarCapacidadMaxima(int cantidadExtra)
    {
        if (haExplotado || cantidadExtra <= 0)
            return;

        capacidadMaxima += cantidadExtra;

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
        // Animación de explosión.
        // Partículas.
        // Game Over.
    }
}
