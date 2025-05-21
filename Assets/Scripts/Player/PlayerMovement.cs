using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Variable Section")]
    [SerializeField] private float speed = 3f;

    private SortingOrderManager sortingOrderManager;
    private Vector2 moveInput;
    private Vector2 lastPosition;
    private Rigidbody2D playerRb;
    private Animator playerAnimator;
    
    // REVISAR AUDIO
    private AudioSource footstepAudioSource;


    void Start()
    {        
        GameObject audioSourcesManager = GameLogicManager.Instance.UIManager.AudioManager;
        AudioSource[] audioSources = audioSourcesManager.GetComponents<AudioSource>();
        footstepAudioSource = audioSources[8];

        playerRb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        lastPosition = transform.position;

        sortingOrderManager = GetComponent<SortingOrderManager>();

        if (sortingOrderManager == null) 
        Debug.LogError("No existe el componente necesario para actualizar el orden de renderizado.");
    }

    void Update()
    {
        // Se recogen las entradas de movimiento (ejes horizontal y vertical) proporcionadas por el jugador    
        if(GetComponent<PlayerLogicManager>().PlayerState.CanMove())
        {  
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            moveInput = new Vector2(moveX, moveY).normalized;

            // Actualiza los parámetros de animación según las entradas del jugador
            playerAnimator.SetFloat("Horizontal", moveX);
            playerAnimator.SetFloat("Vertical", moveY);
            playerAnimator.SetFloat("Speed", moveInput.sqrMagnitude);

            if (moveInput.sqrMagnitude > 0)
            {
                if (!footstepAudioSource.isPlaying)
                    footstepAudioSource.Play();
            }
            else
            {
                if (footstepAudioSource.isPlaying)
                    footstepAudioSource.Stop();
            }
        }
        else
        {
            // Si el jugador no puede moverse, desactiva las animaciones de movimiento
            playerAnimator.SetFloat("Horizontal", 0f);
            playerAnimator.SetFloat("Vertical", 0f);
            playerAnimator.SetFloat("Speed", 0f);

            if (footstepAudioSource.isPlaying) footstepAudioSource.Stop();
        }    
    }

    // Se ejecuta a intervalos fijos, independientemente de la tasa de FPS, por lo que se usa para la lógica relacionada con la física
    private void FixedUpdate()
    {
        // Sectualiza la posición del jugador de acuerdo con la entrada de movimiento mediante la física del Rigidbody2D
        if(GetComponent<PlayerLogicManager>().PlayerState.CanMove())
        {
            Vector2 newPosition = playerRb.position + moveInput * speed * Time.fixedDeltaTime;

            if (newPosition != lastPosition)
            {
                playerRb.MovePosition(newPosition);
                sortingOrderManager.UpdateSortingOrder();
                lastPosition = newPosition;
            }
        }
    }
}
