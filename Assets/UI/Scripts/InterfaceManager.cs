using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField] private Sprite[] spritesNumbers = new Sprite[10];
    [SerializeField] private Transform container;

    [SerializeField] private int currentScore = 0;

    private List<Image> digitImages = new List<Image>();

    public int CurrentScore
    {
        get => currentScore;
        set
        {
            currentScore = Mathf.Max(0, value); // Evita puntajes negativos si no los deseas
            UpdateScoreUI();
        }
    }

    void Start()
    {
        // Actualiza el score inicial al arrancar
        UpdateScoreUI();
    }

    // Métodos públicos para modificar los puntos desde cualquier otro script
    System.Collections.IEnumerator AddScore(int amount)
    {
        CurrentScore += amount;
        yield return null;
    }

    public void ModifyScore(int amount)
    {
        CurrentScore += amount;
    }

    private void UpdateScoreUI()
    {
        // Convertimos el número a string para separar cada dígito fácilmente
        string scoreString = currentScore.ToString();

        // Asegurarnos de que tenemos suficientes componentes Image creados en pantalla
        while (digitImages.Count < scoreString.Length)
        {
            GameObject digitObj = new GameObject("Digit", typeof(RectTransform), typeof(Image));
            digitObj.transform.SetParent(container, false);

            Image img = digitObj.GetComponent<Image>();
            img.preserveAspect = true; // Mantiene la proporción original de tus números hechos a mano
            digitImages.Add(img);
        }

        // Activamos, desactivamos y asignamos el sprite correspondiente a cada dígito
        for (int i = 0; i < digitImages.Count; i++)
        {
            if (i < scoreString.Length)
            {
                digitImages[i].gameObject.SetActive(true);

                // Obtenemos el valor numérico del carácter (ej: '5' se convierte en el int 5)
                int digitValue = scoreString[i] - '0';

                if (digitValue >= 0 && digitValue < spritesNumbers.Length)
                {
                    digitImages[i].sprite = spritesNumbers[digitValue];
                }
            }
            else
            {
                // Si el número actual tiene menos dígitos que antes, ocultamos los sobrantes
                digitImages[i].gameObject.SetActive(false);
            }
        }
    }
}   