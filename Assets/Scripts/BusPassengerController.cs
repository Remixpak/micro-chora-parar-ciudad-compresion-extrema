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
    // Ahora la lista almacena datos puros, por lo que nunca dará "Missing" en el Inspector
    public List<DatosPasajero> pasajerosABordo = new List<DatosPasajero>();
    public int pasajerosActuales = 0;

    public void addPassenger(colorParadero destino, int cantidad)
    {
        pasajerosABordo.Add(new DatosPasajero(destino, cantidad));
        pasajerosActuales = pasajerosABordo.Count;
    }

    public void RemovePassengers(List<DatosPasajero> aBajar)
    {
        foreach (var pasajero in aBajar)
        {
            pasajerosABordo.Remove(pasajero);
        }
        pasajerosActuales = pasajerosABordo.Count;
    }
}