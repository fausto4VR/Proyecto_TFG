using System;

// Enum de las fases en las que se puede encontrar el jugador
public enum PlayerStatePhase
{
    Default, Idle, Inspection, Talking, ShowingInformation
}

// Clase para gestionar los eventos de estado del jugador desde otros scripts
public static class PlayerEvents
{
    // Eventos a los que se pueden suscribir/desuscribir otras clases 
    public static event Action OnInspectionStarted = delegate { };
    public static event Action OnTalkingStarted = delegate { };
    public static event Action OnShowingInformationStarted = delegate { };
    public static event Action OnInspectionFinished = delegate { };
    public static event Action OnInspectionAborted = delegate { };
    public static event Action OnTalkingFinishedWithoutClue = delegate { };
    public static event Action OnTalkingFinishedWithClue = delegate { };
    public static event Action OnShowingInformationFinished = delegate { };

    // Métodos que lanzan los eventos correspondientes 
    public static void StartInspection() => OnInspectionStarted?.Invoke();
    public static void StartTalking() => OnTalkingStarted?.Invoke();
    public static void StartShowingInformation() => OnShowingInformationStarted?.Invoke();
    public static void FinishInspection() => OnInspectionFinished?.Invoke();
    public static void AbortInspection() => OnInspectionAborted?.Invoke();
    public static void FinishTalkingWithoutClue() => OnTalkingFinishedWithoutClue?.Invoke();
    public static void FinishTalkingWithClue() => OnTalkingFinishedWithClue?.Invoke();
    public static void FinishShowingInformation() => OnShowingInformationFinished?.Invoke();
}

// Clase general que representa el estado del personaje del jugador
public class PlayerState
{
    public PlayerStatePhase NextPlayerState { get; protected set; } = PlayerStatePhase.Default;

    public virtual PlayerStatePhase StateName => PlayerStatePhase.Default;
    public virtual bool CanMove() => false;

    // Método para suscribirse a los eventos necesarios al entrar en un estado
    public virtual void OnEnter() { }

    // Método para desuscribirse a los eventos necesarios al salir en un estado
    public virtual void OnExit() { }

    // Método que actualiza el estado actual del jugador
    public virtual PlayerState HandleInput()
    {
        return this;
    }

    // Método que gestiona el cambio de un estado a otro
    public PlayerState TransitionTo(PlayerState nextState)
    {
        this.OnExit();
        nextState.OnEnter();
        return nextState;
    }
}

// Clase que representa el estado base del jugador 
public class IdleState : PlayerState
{
    public override PlayerStatePhase StateName => PlayerStatePhase.Idle;

    public override bool CanMove() => true;

    public override void OnEnter()
    {
        PlayerEvents.OnInspectionStarted += HandleInspectionStarted;
        PlayerEvents.OnTalkingStarted += HandleTalkingStarted;
        PlayerEvents.OnShowingInformationStarted += HandleShowingInformationStarted;
    }

    public override void OnExit()
    {
        PlayerEvents.OnInspectionStarted -= HandleInspectionStarted;
        PlayerEvents.OnTalkingStarted -= HandleTalkingStarted;
        PlayerEvents.OnShowingInformationStarted -= HandleShowingInformationStarted;
    }

    private void HandleInspectionStarted() => NextPlayerState = PlayerStatePhase.Inspection;
    private void HandleTalkingStarted() => NextPlayerState = PlayerStatePhase.Talking;
    private void HandleShowingInformationStarted() => NextPlayerState = PlayerStatePhase.ShowingInformation;

    public override PlayerState HandleInput()
    {
        switch (NextPlayerState)
        {
            case PlayerStatePhase.Inspection:
                return TransitionTo(new InspectionState());

            case PlayerStatePhase.Talking:
                return TransitionTo(new TalkingState());

            case PlayerStatePhase.ShowingInformation:
                return TransitionTo(new ShowingInformationState());

            default:
                return this;
        }
    }
}

// Clase que representa el estado de inspección 
public class InspectionState : PlayerState
{
    public override PlayerStatePhase StateName => PlayerStatePhase.Inspection;

    public override void OnEnter()
    {
        PlayerEvents.OnInspectionFinished += HandleInspectionFinished;
        PlayerEvents.OnInspectionAborted += HandleInspectionAborted;
    }

    public override void OnExit()
    {
        PlayerEvents.OnInspectionFinished -= HandleInspectionFinished;
        PlayerEvents.OnInspectionAborted -= HandleInspectionAborted;
    }

    private void HandleInspectionFinished() => NextPlayerState = PlayerStatePhase.Talking;
    private void HandleInspectionAborted() => NextPlayerState = PlayerStatePhase.Idle;

    public override PlayerState HandleInput()
    {
        switch (NextPlayerState)
        {
            case PlayerStatePhase.Talking:
                return TransitionTo(new TalkingState());

            case PlayerStatePhase.Idle:
                return TransitionTo(new IdleState());

            default:
                return this;
        }
    }
}

// Clase que representa el estado de conversación 
public class TalkingState : PlayerState
{
    public override PlayerStatePhase StateName => PlayerStatePhase.Talking;

    public override void OnEnter()
    {
        PlayerEvents.OnTalkingFinishedWithoutClue += HandleTalkingFinishedWithoutClue;
        PlayerEvents.OnTalkingFinishedWithClue += HandleTalkingFinishedWithClue;
    }

    public override void OnExit()
    {
        PlayerEvents.OnTalkingFinishedWithoutClue -= HandleTalkingFinishedWithoutClue;
        PlayerEvents.OnTalkingFinishedWithClue -= HandleTalkingFinishedWithClue;
    }

    private void HandleTalkingFinishedWithoutClue() => NextPlayerState = PlayerStatePhase.Idle;
    private void HandleTalkingFinishedWithClue() => NextPlayerState = PlayerStatePhase.ShowingInformation;

    public override PlayerState HandleInput()
    {
        switch (NextPlayerState)
        {
            case PlayerStatePhase.Idle:
                return TransitionTo(new IdleState());

            case PlayerStatePhase.ShowingInformation:
                return TransitionTo(new ShowingInformationState());

            default:
                return this;
        }
    }
}

// Clase que representa el estado en el que se muestra información 
public class ShowingInformationState : PlayerState
{
    public override PlayerStatePhase StateName => PlayerStatePhase.ShowingInformation;

    public override void OnEnter()
    {
        PlayerEvents.OnShowingInformationFinished += HandleShowingInformationFinished;
    }

    public override void OnExit()
    {
        PlayerEvents.OnShowingInformationFinished -= HandleShowingInformationFinished;
    }

    private void HandleShowingInformationFinished() => NextPlayerState = PlayerStatePhase.Idle;

    public override PlayerState HandleInput()
    {
        switch (NextPlayerState)
        {
            case PlayerStatePhase.Idle:
                return TransitionTo(new IdleState());

            default:
                return this;
        }
    }
}