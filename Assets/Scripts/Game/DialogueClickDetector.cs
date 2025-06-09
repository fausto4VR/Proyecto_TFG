using UnityEngine;
using System.Collections;

public class DialogueClickDetector : MonoBehaviour
{
    private Coroutine detectionCoroutine;
    private bool isHoveringInteractable = false;


    // Método que se llama para empezar a detectar si se clica sobre un personaje para mantener una conversación 
    public void StartDetection()
    {
        if (detectionCoroutine == null) detectionCoroutine = StartCoroutine(DetectDialogueClick());
    }

    // Corrutina que se llama para detectar el clic sobre un personaje y para cambiar el cursor 
    private IEnumerator DetectDialogueClick()
    {
        while (true)
        {
            // Se convierte la posición del ratón en pantalla a una posición en el mundo 2D
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Se lanzan rayos desde la posición del ratón hacia adelante (dirección cero en 2D)
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);

            bool foundInteractable = false;

            foreach (var hit in hits)
            {
                // Se ignoran los colliders que sean triggers
                if (hit.collider != null && !hit.collider.isTrigger
                    && hit.collider.gameObject.name == GetComponent<DialogueManager>().ObjectNameInRange
                    && !GameLogicManager.Instance.UIManager.OutDetectionPanel.activeInHierarchy)
                {
                    IDialogueLogic dialogue = hit.collider.GetComponent<IDialogueLogic>();

                    if (dialogue != null)
                    {
                        foundInteractable = true;

                        // Se cambia el cursor si aún no se ha hecho
                        if (!isHoveringInteractable)
                        {
                            GameLogicManager.Instance.UIManager.SetCursor(GameLogicManager.Instance.UIManager.InteractCursor);
                            isHoveringInteractable = true;
                        }

                        // Se inicia el diálogo si se hace clic
                        if (Input.GetMouseButtonDown(0)
                            && GetComponent<DialogueManager>().CurrentConversationPhase == ConversationPhase.None)
                        {
                            dialogue.TryStartDialogueOnClick();
                        }

                        break;
                    }
                }
            }
            
            if (!foundInteractable && isHoveringInteractable)
            {
                // Se restaura el cursor si no estamos sobre ningún objeto interactuable válido
                GameLogicManager.Instance.UIManager.SetCursor(GameLogicManager.Instance.UIManager.DefaultCursor);
                isHoveringInteractable = false;
            }

            yield return null;
        }
    }    

    // Método que se llama para terminar de detectar si se clica sobre un personaje para mantener una conversación 
    public void StopDetection()
    {
        StopAllCoroutines();
        if (detectionCoroutine != null) detectionCoroutine = null;

        GameLogicManager.Instance.UIManager.SetCursor(GameLogicManager.Instance.UIManager.DefaultCursor);
        isHoveringInteractable = false;
    }
}