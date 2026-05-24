using System.Collections; // Necesario para el control de tiempo (Corrutinas)
using UnityEngine;
using UnityEngine.UI;    // Necesario para el control de la interfaz (Slider/Botones)

public class MovimientoJugador : MonoBehaviour {
    [Header("Ajustes de Movimiento")]
    public float Velocidad = 6f;
    public float fuerzaSalto = 8f;
    public float gravity = 20f; 
    
    private Vector3 direccionMovimiento = Vector3.zero;
    private CharacterController controller;

    [Header("Sistema de Salud (Vidas Persistentes)")]
    public int vidasActuales = 3;
    public int vidasMaximas = 3;
    public GameObject panelGameOver; // Arrastra tu PanelGameOver aquí en el Inspector
    private TMPro.TextMeshProUGUI textoVidasUI; // Se vincula automáticamente a TextoContador
    private bool yaMurio = false; // Candado de seguridad para evitar restas infinitas en la caída

    [Header("Efecto de Estela y Velocidad (Dash)")]
    private TrailRenderer estelaEfecto;
    private float velocidadOriginal;
    private bool estaEnDash = false;

    void Start() {
        controller = GetComponent<CharacterController>();
        velocidadOriginal = Velocidad;
        yaMurio = false; // Reiniciamos el candado de muerte al cargar la escena

        // 1. BUSCAR EL TEXTO DE VIDAS EN EL CANVAS AUTOMÁTICAMENTE
        GameObject objTexto = GameObject.Find("TextoContador");
        if (objTexto != null) {
            textoVidasUI = objTexto.GetComponent<TMPro.TextMeshProUGUI>();
        }

        // 2. LÓGICA DE VIDAS PERSISTENTES ENTRE RECARGAS (Guardado Permanente)
        if (!PlayerPrefs.HasKey("VidasGuardadas")) {
            PlayerPrefs.SetInt("VidasGuardadas", vidasMaximas);
        }
        
        vidasActuales = PlayerPrefs.GetInt("VidasGuardadas");
        ActualizarInterfazVidas();

        // 3. CONFIGURACIÓN AUTOMÁTICA DE LA ESTELA (TRAIL RENDERER)
        estelaEfecto = GetComponent<TrailRenderer>();
        if (estelaEfecto != null) {
            estelaEfecto.enabled = false; 
        }
    
        // 4. PERSONALIZACIÓN DE COLOR EMISIVO (URP) DESDE EL MENÚ
        if (PlayerPrefs.HasKey("ColorJugadorPersonalizado")) {
            string codigoColor = PlayerPrefs.GetString("ColorJugadorPersonalizado");
            
            if (ColorUtility.TryParseHtmlString(codigoColor, out Color colorGuardado)) {
                Renderer renderizador = GetComponent<Renderer>();
                if (renderizador == null) renderizador = GetComponentInChildren<Renderer>();

                if (renderizador != null) {
                    Material mat = renderizador.material;
                    mat.color = colorGuardado;
                    if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", colorGuardado);

                    if (mat.HasProperty("_EmissionColor")) {
                        mat.EnableKeyword("_EMISSION"); 
                        mat.SetColor("_EmissionColor", colorGuardado);
                    }

                    DynamicGI.SetEmissive(renderizador, colorGuardado);
                }
            }
        }
    }

    void Update() {
        float moveX = 0f;

        // --- 1. CONTROLES PARA COMPUTADORA (TECLADO / EDITOR) ---
        #if UNITY_EDITOR || UNITY_STANDALONE
        moveX = Input.GetAxis("Horizontal");
        
        if (controller.isGrounded && Input.GetButton("Jump")) {
            direccionMovimiento.y = fuerzaSalto;
        }
        #endif

        // --- 2. CONTROLES PARA DISPOSITIVOS MÓVILES (PANTALLA TÁCTIL) ---
        if (Input.touchCount > 0) {
            Touch toque = Input.GetTouch(0); 

            if (toque.position.x < Screen.width / 2) {
                moveX = -1f; 
            } else {
                moveX = 1f;  
            }

            if (controller.isGrounded && toque.phase == TouchPhase.Began) {
                if (toque.position.y > Screen.height * 0.2f) { 
                    direccionMovimiento.y = fuerzaSalto;
                }
            }
        }

        // --- 3. APLICAR EL MOVIMIENTO TRIDIMENSIONAL EN EL ESPACIO ---
        Vector3 movimientoHorizontal = new Vector3(moveX * Velocidad, 0, Velocidad);
        
        direccionMovimiento.x = movimientoHorizontal.x;
        direccionMovimiento.z = movimientoHorizontal.z;

        direccionMovimiento.y -= gravity * Time.deltaTime;
        
        controller.Move(direccionMovimiento * Time.deltaTime);

        // --- 4. VALIDADOR DE CAÍDA AL VACÍO CON CANDADO DE SEGURIDAD ---
        if (transform.position.y < -10f && !yaMurio) { 
            yaMurio = true; // Cerramos el candado inmediatamente para congelar el cálculo
            Debug.Log("<color=red>¡Caída al vacío detectada!</color> Restando una vida.");
            RecibirDanio(1); 
        }
    }

    // --- MÉTODOS AUXILIARES DEL SISTEMA DE VIDAS ---
    void ActualizarInterfazVidas() {
        if (textoVidasUI != null) {
            textoVidasUI.text = "Vidas: " + vidasActuales;
        }
    }

    public void RecibirDanio(int cantidad) {
        // Si ya entramos en proceso de Game Over, ignoramos cualquier impacto extra retrasado
        if (vidasActuales <= 0) return; 

        vidasActuales -= cantidad;
        
        // Forzamos matemáticamente a que no baje de cero bajo ninguna circunstancia
        if (vidasActuales < 0) vidasActuales = 0;

        PlayerPrefs.SetInt("VidasGuardadas", vidasActuales);
        PlayerPrefs.Save();
        ActualizarInterfazVidas();

        Debug.Log("¡Impacto detectado! Vidas restantes registradas: " + vidasActuales);

        if (vidasActuales <= 0) {
            // DETENER AL JUGADOR EN EL ACTO: Frenamos el avance automático en Z
            Velocidad = 0f;
            direccionMovimiento = Vector3.zero;
            
            GameOverGlobal();
        } else {
            Morir(); 
        }
    }

    void Morir() {
        Debug.Log("<color=yellow>Reintentando nivel. Restableciendo constantes de tiempo...</color>");
        Time.timeScale = 1f; 

        string nombreEscenaActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscenaActual);
    }

    void GameOverGlobal() {
        Debug.Log("<color=red>¡GAME OVER GLOBAL!</color> Borrando memoria de progreso y activando Canvas.");
        
        // 1. Limpiamos los datos del intento actual para la nueva partida
        PlayerPrefs.DeleteKey("VidasGuardadas");
        PlayerPrefs.DeleteKey("ProgresoEjeZ");          // Borra la distancia recorrida
        PlayerPrefs.DeleteKey("MonedasActualesGuardadas"); // Borra las monedas de esta carrera
        PlayerPrefs.DeleteKey("NivelActualGuardado");     // Borra el nivel alcanzado en este intento
        
        // Guardamos físicamente los cambios en el almacenamiento del dispositivo
        PlayerPrefs.Save();

        // 2. Encendemos el panel visual en tu pantalla
        if (panelGameOver != null) {
            panelGameOver.SetActive(true);
        } else {
            Debug.LogError("No has asignado el PanelGameOver en el Inspector del Jugador.");
        }

        // 3. Congelamos las físicas por completo para pausar la acción de fondo
        Time.timeScale = 0f; 
    }

    // FUNCIÓN PÚBLICA PARA EL BOTÓN INTERACTIVO DE REGRESO AL MENÚ
    public void VolverAlMenuDesdeGameOver() {
        Time.timeScale = 1f; // Muy importante devolver el tiempo a la normalidad antes del salto
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipal");
    }

    // ================================================================
    // TEMPORIZADOR DEL POWER-UP (CORRUTINA)
    // ================================================================
    // ================================================================
    // TEMPORIZADOR DEL POWER-UP (CORRUTINA CORREGIDA PARA NIVELES ALTOS)
    // ================================================================
    IEnumerator ActivarEfectoVelocidad() {
        estaEnDash = true;

        Renderer renderizador = GetComponent<Renderer>();
        if (renderizador == null) renderizador = GetComponentInChildren<Renderer>();
        
        Color colorDelJugador = Color.white; 
        if (renderizador != null) {
            colorDelJugador = renderizador.material.color;
        }

        if (estelaEfecto != null) {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(colorDelJugador, 0.0f), new GradientColorKey(colorDelJugador, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) } 
            );
            estelaEfecto.colorGradient = gradient;
            estelaEfecto.enabled = true;
        }

        // --- SOLUCIÓN: Multiplicamos la velocidad ACTUAL del nivel, no la base ---
        float velocidadAntesDelTurbo = Velocidad; 
        Velocidad = velocidadAntesDelTurbo * 2.0f; // Multiplica por 2 tu velocidad actual (puedes ajustar el 2.0f)

        yield return new WaitForSeconds(2.5f);

        // --- SOLUCIÓN: Le devolvemos exactamente la velocidad que le correspondía por su nivel ---
        Velocidad = velocidadAntesDelTurbo;

        if (estelaEfecto != null) {
            estelaEfecto.Clear(); 
            estelaEfecto.enabled = false; 
        }

        estaEnDash = false;
    }

    public void ActivarTurboEsfera() {
        if (!estaEnDash) {
            StartCoroutine(ActivarEfectoVelocidad());
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        Meteorito meteorito = hit.gameObject.GetComponent<Meteorito>();
        if (meteorito != null) {
            // Si el jugador ya está muerto por caída, ignoramos colisiones de meteoritos fantasmas
            if (!yaMurio) {
                RecibirDanio(meteorito.danio);
                meteorito.Explotar();
            }
        }
    }
}