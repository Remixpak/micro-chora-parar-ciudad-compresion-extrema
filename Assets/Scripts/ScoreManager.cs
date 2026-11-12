
using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Puntaje")]
    [SerializeField] private int puntajeActual = 0;

    [Header("Bonificaciones")]
    [SerializeField] private int puntosPorEntrega = 100;
    [SerializeField] private int puntosPorPasajero = 100;

    [Header("Penalizaciones")]
    [SerializeField] private int penalizacionPorPasajero = 50;
    [SerializeField] private int penalizacionParaderoIncorrecto = 100;

    public int PuntajeActual => puntajeActual;

    public event Action<int> OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        OnScoreChanged?.Invoke(puntajeActual);
    }

    public void SumarPuntos(int cantidad)
    {
        if (cantidad <= 0)
            return;

        puntajeActual += cantidad;
        NotificarCambio();
    }

    public void RestarPuntos(int cantidad)
    {
        if (cantidad <= 0)
            return;

        puntajeActual = Mathf.Max(0, puntajeActual - cantidad);
        NotificarCambio();
    }

    public void RegistrarEntrega()
    {
        SumarPuntos(puntosPorEntrega);
    }

    public void RegistrarSubida(int cantidadPasajeros)
    {
        SumarPuntos(cantidadPasajeros * puntosPorPasajero);
    }

    public void RegistrarPasajerosNoRecogidos(int cantidad)
    {
        RestarPuntos(cantidad * penalizacionPorPasajero);
    }

    public void RegistrarParaderoIncorrecto()
    {
        RestarPuntos(penalizacionParaderoIncorrecto);
    }

    public void RegistrarHitoKilometros(int puntos)
    {
        SumarPuntos(puntos);
    }

    private void NotificarCambio()
    {
        OnScoreChanged?.Invoke(puntajeActual);
        Debug.Log($"Puntaje actual: {puntajeActual}");
    }
}
