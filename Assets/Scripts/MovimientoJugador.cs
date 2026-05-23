using System.Collections; // ¡AÑADIDO! Necesario para el control de tiempo (Corrutinas)
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

    [Header("Efecto de Estela y Velocidad (Dash)")] // ¡AÑADIDO!
    private TrailRenderer estelaEfecto;
    private float velocidadOriginal;
    private bool estaEnDash = false;

    void Start() {
        controller = GetComponent<CharacterController>();
        vidasActuales = vidasMaximas;

        // Guardamos la velocidad base asignada en el Inspector
        velocidadOriginal = Velocidad;

        // Buscamos el componente Trail Renderer de forma automática en la cápsula
        estelaEfecto = GetComponent<TrailRenderer>();
        if (estelaEfecto != null) {
            estelaEfecto.enabled = false; // Iniciamos con el rastro apagado
        }
    
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
        // DETECTOR DEL EVENTO: Si presionas Shift Izquierdo y no estás acelerando actualmente
        if (Input.GetKeyDown(KeyCode.LeftShift) && !estaEnDash) {
            StartCoroutine(ActivarEfectoVelocidad());
        }

        // Lógica de movimiento nativa sobre el suelo
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

    // ================================================================
    // TEMPORIZADOR DEL EFECTO VISUAL Y CAMBIO DE VELOCIDAD
    // ================================================================
  IEnumerator ActivarEfectoVelocidad() {
        estaEnDash = true;
        Debug.Log("<color=lime>¡POWER-UP ACTIVADO!</color> Turbo iniciado.");

        // 1. OBTENER EL COLOR ACTUAL: Le robamos el color al material del cuerpo
        Renderer renderizador = GetComponent<Renderer>();
        if (renderizador == null) renderizador = GetComponentInChildren<Renderer>();
        
        Color colorDelJugador = Color.white; // Color por defecto por si falla
        if (renderizador != null) {
            colorDelJugador = renderizador.material.color;
        }

        // 2. CONFIGURAR LA ESTELA DINÁMICAMENTE: Forzamos a la estela a usar ese color
        if (estelaEfecto != null) {
            // Creamos un gradiente de color que va desde el color del jugador hasta transparente
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(colorDelJugador, 0.0f), new GradientColorKey(colorDelJugador, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) } // Se desvanece al final
            );
            estelaEfecto.colorGradient = gradient;
            estelaEfecto.enabled = true;
        }

        // 3. SUPER VELOCIDAD: Multiplicamos por 3.5 para que se note un cambio drástico de velocidad
        Velocidad = velocidadOriginal * 3.5f;

        // 4. TIEMPO REQUERIDO: Mantenemos el turbo por 2.5 segundos
        yield return new WaitForSeconds(2.5f);

        // 5. RESTABLECER: Regresamos a la normalidad de forma limpia
        Velocidad = velocidadOriginal;

        if (estelaEfecto != null) {
            estelaEfecto.Clear(); 
            estelaEfecto.enabled = false; 
        }

        estaEnDash = false;
        Debug.Log("Fin del efecto Turbo. Velocidad normalizada.");
    }

    // Función pública para que las esferas espaciales puedan activar el Turbo
    public void ActivarTurboEsfera() {
        if (!estaEnDash) {
            StartCoroutine(ActivarEfectoVelocidad());
        }
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

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        Meteorito meteorito = hit.gameObject.GetComponent<Meteorito>();
        if (meteorito != null) {
            RecibirDanio(meteorito.danio);
            meteorito.Explotar();
        }
    }
}