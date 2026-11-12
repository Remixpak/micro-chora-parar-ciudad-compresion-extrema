using UnityEngine;

public class ExpandirCompresionPanel : MonoBehaviour
{
    [Header("Configuración de Pulsación")]
    [Tooltip("Velocidad con la que se expande y comprime.")]
    public float velocidad = 2f;

    // Update is called once per frame
    void Update()
    {
        // Mathf.Sin oscila entre -1 y 1. 
        // Con esta fórmula mapeamos ese resultado para que oscile exactamente entre 1 y 3.
        float escalaActual = 2f + Mathf.Sin(Time.time * velocidad) * 1f;

        // Aplicamos la escala en los ejes X, Y y Z
        transform.localScale = new Vector3(escalaActual, escalaActual, escalaActual);
    }
}