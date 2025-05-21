using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelected : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Variable Section")]
    [SerializeField] private bool hasSelectedMark;

    private MainMenuLogic mainMenuLogic;


    void Start()
    {
        mainMenuLogic = GameObject.Find("Main Menu Manager").GetComponent<MainMenuLogic>();
    }

    // Método que se llama al entrar el cursor en el área del UI y sirve para activar la marca de selección y el cursor correspondiente
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hasSelectedMark) transform.GetChild(1).gameObject.SetActive(true);
        mainMenuLogic.SetCursor(mainMenuLogic.InteractCursor);
    }

    // Método que se llama al salir el cursor en el área del UI y sirve para desactivar la marca de selección y el cursor correspondiente
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hasSelectedMark) transform.GetChild(1).gameObject.SetActive(false);
        mainMenuLogic.SetCursor(mainMenuLogic.DefaultCursor);
    }

    // Método que se llama cuando se desactiva o se destruye el objeto y sirve para volver a poner la imagen por defecto en el cursor
    private void OnDisable()
    {
        if (mainMenuLogic != null)
        {
            if (hasSelectedMark) transform.GetChild(1).gameObject.SetActive(false);
            mainMenuLogic.SetCursor(mainMenuLogic.DefaultCursor);
        }
    }
}
