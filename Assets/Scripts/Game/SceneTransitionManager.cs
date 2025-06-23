using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

// Enum del tipo de transición
public enum TransitionType
{
    Empty, Fade, Circle
}

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Configuration Data")]
    [SerializeField] private GameConfigurationData transitionConfiguration;

    private List<string> scenesWithoutInTransition => transitionConfiguration.scenesWithoutInTransition;
    private List<string> scenesWithoutOutTransition => transitionConfiguration.scenesWithoutOutTransition;
    private TransitionType defaultTransitionType => transitionConfiguration.defaultTransitionType;

    private GameObject fadeTransition;
    private GameObject circleTransition;
    private GameObject defaultTransition;
    private GameObject outTransitionPanel;
    private TransitionType currentTransitionType = TransitionType.Empty;
    private bool inTransition;
    private bool outTransition;


    // Método que se llama cuando se activa el objeto y sirve para suscribirse a eventos
    void OnEnable()
    {
        if (this == null || gameObject == null) return;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    // Método que se llama cuando se desactiva o se destruye el objeto y sirve para desuscribirse a eventos
    void OnDisable()
    {
        if (this == null || gameObject == null) return;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // Método que se ejecuta cada vez que se carga una escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject canvas = GameObject.Find("Canvas - Immovable");

        fadeTransition = canvas?.transform.Find("Transition")?.gameObject.transform.Find("Fade")?.gameObject;
        circleTransition = canvas?.transform.Find("Transition")?.gameObject.transform.Find("CircleScale")?.gameObject;
        outTransitionPanel = canvas?.transform.Find("Out Transition Panel").gameObject;

        if (defaultTransitionType == TransitionType.Fade) defaultTransition = fadeTransition;
        else if (defaultTransitionType == TransitionType.Circle) defaultTransition = circleTransition;

        if (!scenesWithoutInTransition.Contains(scene.name)) inTransition = true;
        if (!scenesWithoutOutTransition.Contains(scene.name)) outTransition = true;
    }

    // Método que se ejecuta cada vez que se descarga una escena
    private void OnSceneUnloaded(Scene current)
    {
        inTransition = false;
        outTransition = false;
    }

    // Método para empezar la transición de comienzo de una escena 
    public void StartInTransition()
    {
        if (!inTransition) return;

        if (currentTransitionType == TransitionType.Fade)
        {
            fadeTransition.SetActive(true);
            Animator transitionAnimator = fadeTransition?.GetComponentInChildren<Animator>();
            transitionAnimator.SetTrigger("StartInTransition");
        }
        else if (currentTransitionType == TransitionType.Circle)
        {
            circleTransition.SetActive(true);
            Animator transitionAnimator = circleTransition?.GetComponentInChildren<Animator>();
            transitionAnimator.SetTrigger("StartInTransition");
        }
        else
        {
            defaultTransition.SetActive(true);
            Animator transitionAnimator = defaultTransition?.GetComponentInChildren<Animator>();
            transitionAnimator.SetTrigger("StartInTransition");
        }

        outTransitionPanel?.SetActive(true);

        if (GameStateManager.Instance.IsNewGame || GameStateManager.Instance.IsLoadGame || GameStateManager.Instance.IsMapTravel)
            PlayerEvents.StartShowingInformation();

        StartCoroutine(WaitAndCloseTransition());
    }

    // Corrutina para esperar a que se termine la transición de comienzo de una escena
    private IEnumerator WaitAndCloseTransition()
    {
        yield return new WaitForSeconds(GameStateManager.Instance.TransitionDuration);

        fadeTransition.SetActive(false);
        circleTransition.SetActive(false);

        outTransitionPanel?.SetActive(false);

        if (GameStateManager.Instance.IsNewGame || GameStateManager.Instance.IsLoadGame || GameStateManager.Instance.IsMapTravel)
        {
            PlayerEvents.FinishShowingInformation();

            GameStateManager.Instance.IsNewGame = false;
            GameStateManager.Instance.IsLoadGame = false;
            GameStateManager.Instance.IsMapTravel = false;

        }
    }

    // Método para empezar la transición de fin de una escena 
    public void StartOutTransition(TransitionType transitionType)
    {
        if (!outTransition) return;

        if (transitionType == TransitionType.Empty)
        {
            Debug.LogError("Se ha proporcionado un tipo de transición no válido.");
            return;
        }

        currentTransitionType = transitionType;

        outTransitionPanel?.SetActive(true);

        if (transitionType == TransitionType.Fade)
        {
            fadeTransition.SetActive(true);
            Animator transitionAnimator = fadeTransition?.GetComponentInChildren<Animator>();
            transitionAnimator.SetTrigger("StartOutTransition");
        }
        else if (transitionType == TransitionType.Circle)
        {
            circleTransition.SetActive(true);
            Animator transitionAnimator = circleTransition?.GetComponentInChildren<Animator>();
            transitionAnimator.SetTrigger("StartOutTransition");
        }
    }

    // Método para consultar el tipo de transición actual
    public TransitionType CurrentTransitionType
    {
        get { return currentTransitionType; }
    }
    
    // Método para consultar el tipo de transición por defecto
    public TransitionType DefaultTransitionType
    {
        get { return defaultTransitionType; }
    }
}
