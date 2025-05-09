using UnityEngine;
using UnityEngine.EventSystems;

public class CursorChangePuzzle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PuzzleUIManager puzzleUIManager;


    void Start()
    {
        puzzleUIManager = GameObject.Find("Puzzle Logic Manager").GetComponent<PuzzleUIManager>();        
    }
    
    // Método que se llama al entrar el cursor en el área del UI y sirve para activar la marca de selección y el cursor correspondiente
    public void OnPointerEnter(PointerEventData eventData)
    {
        puzzleUIManager.SetCursor(puzzleUIManager.InteractCursor);
    }

    // Método que se llama al salir el cursor en el área del UI y sirve para desactivar la marca de selección y el cursor correspondiente
    public void OnPointerExit(PointerEventData eventData)
    {
        puzzleUIManager.SetCursor(puzzleUIManager.DefaultCursor);
    }

    // Método que se llama cuando se desactiva o se destruye el objeto y sirve para volver a poner la imagen por defecto en el cursor
    private void OnDisable()
    {
        if (puzzleUIManager != null)
        {
            puzzleUIManager.SetCursor(puzzleUIManager.DefaultCursor);
        }
    }
}
