using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public static FPSDisplay Instance { get; private set; }
    private float deltaTime = 0.0f;

    // En el Awake se define su comportamiento como singleton 
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Calcula el tiempo aplicando un suavizado entre frames. 
        // La variable deltaTime es el tiempo que ha transcurrido entre un frame y el siguiente
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        if (this == null || gameObject == null) return;
        
        int w = Screen.width;
        int h = Screen.height;

        // Se crea un estilo de GUI personalizado
        GUIStyle style = new GUIStyle();

        // Se define el área donde se dibujará el texto (esquina superior derecha)
        Rect rect = new Rect(w - 110, 10, 100, h * 2 / 100); 

        // Se configura la alineación, el tamaño de la fuente y el color del texto
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = Color.white;

         // Se calcula los FPS reales y se establece el formato del número
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.} FPS", fps);

         // Se dibuja el borde negro alrededor del texto
        Vector2 shadowOffset = new Vector2(2, 2);

        // Se dibuja el borde negro (con desplazamientos)
        GUIStyle borderStyle = new GUIStyle(style);
        borderStyle.normal.textColor = Color.black;

        // Se dibujar el texto desplazado para crear el borde negro
        GUI.Label(new Rect(rect.x + shadowOffset.x, rect.y + shadowOffset.y, rect.width, rect.height), text, borderStyle);
        GUI.Label(new Rect(rect.x - shadowOffset.x, rect.y + shadowOffset.y, rect.width, rect.height), text, borderStyle);
        GUI.Label(new Rect(rect.x + shadowOffset.x, rect.y - shadowOffset.y, rect.width, rect.height), text, borderStyle);
        GUI.Label(new Rect(rect.x - shadowOffset.x, rect.y - shadowOffset.y, rect.width, rect.height), text, borderStyle);

        // Se dibuja el texto en pantalla (encima del borde)
        GUI.Label(rect, text, style);
    }
}
