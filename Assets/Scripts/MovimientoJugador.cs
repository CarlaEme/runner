using UnityEngine;

public class MovimientoJugador : MonoBehaviour {
    [Header("Ajustes de Movimiento")]
    public float Velocidad = 6f;
    public float fuerzaSalto = 8f;
    public float gravedad = 20f;
    
    private Vector3 direccionMovimiento = Vector3.zero;
    private CharacterController controller;

    [Header("Sistema de Salud (Vidas)")]
    public int vidasActuales = 3;
    public int vidasMaximas = 3;

    void Start() {
        controller = GetComponent<CharacterController>();
        vidasActuales = vidasMaximas;
    
        if (PlayerPrefs.HasKey("ColorJugadorPersonalizado")) {
            string codigoColor = PlayerPrefs.GetString("ColorJugadorPersonalizado");
            
            if (ColorUtility.TryParseHtmlString(codigoColor, out Color colorGuardado)) {
                Renderer renderizador = GetComponent<Renderer>();
                if (renderizador == null) renderizador = GetComponentInChildren<Renderer>();

                if (renderizador != null) {
                    Material mat = renderizador.material;

                    // 1. Pintamos el fondo base por si las dudas
                    mat.color = colorGuardado;
                    if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", colorGuardado);

                    // 2. EL CAMBIO HISTÓRICO: Forzamos la propiedad de emisión del Shader URP Lit
                    if (mat.HasProperty("_EmissionColor")) {
                        // Activamos la casilla de emisión en el motor gráfico
                        mat.EnableKeyword("_EMISSION"); 
                        
                        // Le inyectamos el color verde o naranja directamente al brillo HDR
                        mat.SetColor("_EmissionColor", colorGuardado);
                    }

                    // Forzar a Unity a actualizar los gráficos de la escena de inmediato
                    DynamicGI.SetEmissive(renderizador, colorGuardado);

                    Debug.Log("<color=green>¡Emisión URP doblegada!</color> Aplicado: " + codigoColor);
                } else {
                    Debug.LogError("No se encontró Renderer en el Jugador.");
                }
            }
        
    
    }
    
    }

    void Update() {
        if (controller.isGrounded) {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            direccionMovimiento = new Vector3(moveX, 0, moveZ);
            direccionMovimiento *= Velocidad;

            if (Input.GetButton("Jump")) {
                direccionMovimiento.y = fuerzaSalto;
            }
        }

        direccionMovimiento.y -= gravedad * Time.deltaTime;
        controller.Move(direccionMovimiento * Time.deltaTime);
    }

    public void RecibirDanio(int cantidad) {
        vidasActuales -= cantidad;
        Debug.Log("¡Ay! El jugador perdió vida. Vidas restantes: " + vidasActuales);

        if (vidasActuales <= 0) {
            Morir();
        }
    }

    void Morir() {
        Debug.Log("¡Game Over! Reiniciando partida...");
        string nombreEscenaActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscenaActual);
    }

    // Detector nativo para el Character Controller
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        Meteorito meteorito = hit.gameObject.GetComponent<Meteorito>();
        if (meteorito != null) {
            RecibirDanio(meteorito.danio);
            meteorito.Explotar();
        }
    }
}