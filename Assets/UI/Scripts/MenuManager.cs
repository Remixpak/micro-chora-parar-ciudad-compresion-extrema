
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    //[SerializeField] private Button PlayButton;
    //[SerializeField] private Button QuitButton;

    [SerializeField] private Button[] ButtonsMenuInitial;
    [SerializeField] private Button[] ButtonsMenuPauseAndDeath;

    [SerializeField] private string PrincipalScene;

    [SerializeField] private int ListOfButtons;

    private GameObject LastButtonSelected;

    private Button[] Buttons;

    private void Start()
    {
        switch (ListOfButtons)
        {
            case 0: Buttons = ButtonsMenuInitial; break;

            case 1: Buttons = ButtonsMenuPauseAndDeath; break;

            default:
                Buttons = new Button[0]; // Evita que sea null
                Debug.LogWarning("ListOfButtons tiene un valor no válido.");
                break;
        }

        foreach (Button button in Buttons)
        {
            //Animator animator = button.GetComponent<Animator>();
            
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

            if (trigger == null)
            {
                trigger = button.gameObject.AddComponent<EventTrigger>();
            }

            Button actualButton = button;

            EventTrigger.Entry Selection = new EventTrigger.Entry();
            Selection.eventID = EventTriggerType.PointerEnter;
            Selection.callback.AddListener((data) => { ActivateButton(actualButton); });
            trigger.triggers.Add(Selection);
        }
    }

    private void ActivateButton(Button buttonActive)
    {
        foreach (Button button in Buttons)
        {
            Animator animator = button.GetComponent<Animator>();

            if (animator != null)
            {
                bool selected = (button == buttonActive);
                animator.SetBool("IsSelected", selected);
            }
        }
    }

    // Añadir nombre de la escena
    public void ButtonPlay()
    {
        if (PrincipalScene != null)
        {
            SceneManager.LoadScene(PrincipalScene);
        }
        Debug.Log("Entrando a la Escena Principal");
    }

    public void ButtonQuit()
    {
        Application.Quit();
        Debug.Log("Salio del juego");
    }

}
