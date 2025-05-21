using UnityEngine;
using UnityEngine.EventSystems;

public class CursorChangeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Método que se llama al entrar el cursor en el área del UI y sirve para cambiar su imagen
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameLogicManager.Instance.UIManager.SetCursor(GameLogicManager.Instance.UIManager.InteractCursor);
    }

    // Método que se llama al salir el cursor en el área del UI y sirve para cambiar su imagen
    public void OnPointerExit(PointerEventData eventData)
    {
        GameLogicManager.Instance.UIManager.SetCursor(GameLogicManager.Instance.UIManager.DefaultCursor);
    }

    // Método que se llama cuando se desactiva o se destruye el objeto y sirve para volver a poner la imagen por defecto en el cursor
    private void OnDisable()
    {
        if (GameLogicManager.Instance != null && GameLogicManager.Instance.UIManager != null)
        {
            GameLogicManager.Instance.UIManager.SetCursor(GameLogicManager.Instance.UIManager.DefaultCursor);
        }
    }
}