using UnityEngine;

public class TransitionComponent : MonoBehaviour
{
    void Start()
    {        
        GameStateManager.Instance.TransitionManager.StartInTransition();     
    }
}
