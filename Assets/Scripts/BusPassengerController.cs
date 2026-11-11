using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class BusPassengerController : MonoBehaviour
{
    public List<Pasajero> pasajerosABordo = new List<Pasajero>(); // Lista de pasajeros actualmente en el bus
    public int pasajerosActuales = 0; // Número actual de pasajeros en el bus
    public void addPassengers(Pasajero p)
    {
        pasajerosABordo.Add(p);
    }
}
