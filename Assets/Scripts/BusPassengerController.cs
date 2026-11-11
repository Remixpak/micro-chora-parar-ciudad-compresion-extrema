using UnityEngine;

public class BusPassengerController : MonoBehaviour
{
    public int pasajerosActuales = 0; // Número actual de pasajeros en el bus
    public void addPassengers(int cantidad)
    {
        pasajerosActuales += cantidad;
    }
}
