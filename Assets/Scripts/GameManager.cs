using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<Paradero> paraderosDestino = new List<Paradero>();
    public int GeneralSpeed = 30;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddDestiny(Paradero p)
    {
        BusPassengerController controller = FindObjectOfType<BusPassengerController>();

        if (controller == null) return;

        // Recorremos la lista usando 'DatosPasajero' en lugar de 'Pasajero'
        foreach (DatosPasajero pasajero in controller.pasajerosABordo)
        {
            if (pasajero.colorDestino == p.colorParadero && !paraderosDestino.Contains(p))
            {
                Debug.Log($"Agregando paradero de destino: {p.colorParadero}");
                paraderosDestino.Add(p);
            }
        }
    }

}
