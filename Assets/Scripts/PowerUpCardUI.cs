using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpCardUI : MonoBehaviour
{
    [Header("Elementos UI")]
    [SerializeField] private TMP_Text titulo;
    [SerializeField] private TMP_Text descripcion;
    [SerializeField] private Image icono;
    [SerializeField] private Button boton;

    [Header("Iconos de power-ups")]
    [SerializeField] private Sprite iconoAumentar;
    [SerializeField] private Sprite iconoEscudo;
    [SerializeField] private Sprite iconoAntiexplosion;
    [SerializeField] private Sprite iconoVaciar;

    public void Configurar(
        PowerUpType tipo,
        int indice,
        PowerUpManager manager)
    {
        Sprite sprite = null;

        switch (tipo)
        {
            case PowerUpType.Aumentar:
                titulo.text = "MÁS ESPACIO";
                descripcion.text =
                    "Aumenta la capacidad máxima en 3 pasajeros.";
                sprite = iconoAumentar;
                break;

            case PowerUpType.Escudo:
                titulo.text = "ESCUDO";
                descripcion.text =
                    "Protege la micro del próximo choque.";
                sprite = iconoEscudo;
                break;

            case PowerUpType.Antiexplosion:
                titulo.text = "ANTIEXPLOSIÓN";
                descripcion.text =
                    "Sobrevive a una explosión, pero pierde todos los pasajeros.";
                sprite = iconoAntiexplosion;
                break;

            case PowerUpType.VaciarMicro:
                titulo.text = "VACIAR MICRO";
                descripcion.text =
                    "Baja a todos los pasajeros y restaura la capacidad a 25.";
                sprite = iconoVaciar;
                break;
        }

        if (icono != null)
        {
            icono.sprite = sprite;
            icono.enabled = sprite != null;
        }

        boton.onClick.RemoveAllListeners();
        boton.onClick.AddListener(
            () => manager.ElegirPowerUp(indice)
        );
    }
}
