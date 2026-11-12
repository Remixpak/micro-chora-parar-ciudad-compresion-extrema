using UnityEngine;

public class GrupoPasajeros : MonoBehaviour
{
    public int cantidadPasajeros = 1; // Número de pasajeros en el grupo
    private Pasajero[] pasajerosEnGrupo;

    private void Start()
    {
        // Obtenemos los pasajeros hijos y asignamos los destinos al iniciar
        AsignarDestinosAleatoriosAIPasajeros();
    }

    void AsignarDestinosAleatoriosAIPasajeros()
    {
        // 1. Obtenemos todos los componentes 'Pasajero' en los hijos
        pasajerosEnGrupo = GetComponentsInChildren<Pasajero>();
        cantidadPasajeros = pasajerosEnGrupo.Length;

        // 2. Buscamos el componente 'Paradero' en el objeto padre
        Paradero paraderoPadre = GetComponentInParent<Paradero>();

        // Inicializamos la variable con un valor por defecto (ej. rojo) para evitar el error de compilación
        colorParadero colorDelParaderoActual = colorParadero.rojo;
        bool tieneParaderoPadre = (paraderoPadre != null);

        if (tieneParaderoPadre)
        {
            colorDelParaderoActual = paraderoPadre.colorParadero;
        }
        else
        {
            Debug.LogWarning("El grupo de pasajeros no está dentro de un Paradero. Se asignarán colores libres.");
        }

        // 3. Obtenemos todos los valores posibles del enum 'colorParadero'
        colorParadero[] valoresColores = (colorParadero[])System.Enum.GetValues(typeof(colorParadero));

        // 4. Asignamos los destinos asegurándonos de que NINGUNO coincida con el color del paradero
        foreach (Pasajero pasajero in pasajerosEnGrupo)
        {
            colorParadero colorAleatorio;

            do
            {
                // Escogemos un color al azar
                colorAleatorio = valoresColores[Random.Range(0, valoresColores.Length)];
            }
            while (tieneParaderoPadre && colorAleatorio == colorDelParaderoActual);

            pasajero.colorDestino = colorAleatorio;
        }
    }
}