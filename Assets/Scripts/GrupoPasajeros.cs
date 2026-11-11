using UnityEngine;

public class GrupoPasajeros : MonoBehaviour
{
    public int cantidadPasajeros = 1; // Número de pasajeros en el grupo
    Pasajero[] pasajerosEnGrupo;
    private void Awake()
    {
        AsignarDestinosAleatoriosAIPasajeros();
    }
    private void Start()
    {
        cantidadPasajeros = pasajerosEnGrupo.Length;
    }
    void AsignarDestinosAleatoriosAIPasajeros()
    {
        // Obtenemos todos los componentes 'Pasajero' que estén en los hijos de este objeto padre
        pasajerosEnGrupo = GetComponentsInChildren<Pasajero>();

        // Obtenemos todos los colores posibles del enum 'colorParadero'
        colorParadero[] valoresColores = (colorParadero[])System.Enum.GetValues(typeof(colorParadero));

        foreach (Pasajero pasajero in pasajerosEnGrupo)
        {
            // A cada pasajero individual dentro del grupo se le asigna un color al azar
            pasajero.colorDestino = valoresColores[Random.Range(0, valoresColores.Length)];

           
        }
    }
}