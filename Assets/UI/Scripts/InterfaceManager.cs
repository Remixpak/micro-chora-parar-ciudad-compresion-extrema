
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    [Header("Números personalizados")]
    [SerializeField] private Sprite[] spritesNumbers = new Sprite[10];
    [SerializeField] private Transform container;

    [Header("Puntaje")]
    [SerializeField] private int currentScore = 0;

    [Header("Referencias")]
    [SerializeField] private ScoreManager scoreManager;

    private List<Image> digitImages = new List<Image>();

    public int CurrentScore
    {
        get => currentScore;
        set
        {
            currentScore = Mathf.Max(0, value);
            UpdateScoreUI();
        }
    }


    private void OnEnable()
    {
        // Si ya tenemos la referencia, nos suscribimos.
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged += ActualizarPuntaje;
            ActualizarPuntaje(scoreManager.PuntajeActual);
        }
    }

    private void Start()
    {
        // Si no se asignó desde el Inspector,
        // intentamos encontrarlo automáticamente.
        if (scoreManager == null)
        {
            scoreManager = ScoreManager.Instance;
        }

        if (scoreManager != null)
        {
            // Evita suscripciones duplicadas.
            scoreManager.OnScoreChanged -= ActualizarPuntaje;
            scoreManager.OnScoreChanged += ActualizarPuntaje;

            ActualizarPuntaje(scoreManager.PuntajeActual);
        }
        else
        {
            Debug.LogError(
                "InterfaceManager: No se encontró ScoreManager."
            );
        }

        UpdateScoreUI();
    }

    private void OnDisable()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= ActualizarPuntaje;
        }
    }


    private void ActualizarPuntaje(int nuevoPuntaje)
    {
        CurrentScore = nuevoPuntaje;
    }

    public void ModifyScore(int amount)
    {
        if (ScoreManager.Instance != null)
        {
            if (amount >= 0)
                ScoreManager.Instance.SumarPuntos(amount);
            else
                ScoreManager.Instance.RestarPuntos(-amount);
        }
    }

    private void UpdateScoreUI()
    {
        if (container == null)
            return;

        string scoreString = currentScore.ToString();

        while (digitImages.Count < scoreString.Length)
        {
            GameObject digitObj = new GameObject(
                "Digit",
                typeof(RectTransform),
                typeof(Image)
            );

            digitObj.transform.SetParent(container, false);

            Image img = digitObj.GetComponent<Image>();
            img.preserveAspect = true;

            digitImages.Add(img);
        }

        for (int i = 0; i < digitImages.Count; i++)
        {
            if (i < scoreString.Length)
            {
                digitImages[i].gameObject.SetActive(true);

                int digitValue = scoreString[i] - '0';

                if (digitValue >= 0 &&
                    digitValue < spritesNumbers.Length)
                {
                    digitImages[i].sprite =
                        spritesNumbers[digitValue];
                }
            }
            else
            {
                digitImages[i].gameObject.SetActive(false);
            }
        }
    }
}
