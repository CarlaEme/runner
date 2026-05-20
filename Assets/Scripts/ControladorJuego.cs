using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class ControladorJuego : MonoBehaviour {
    public static ControladorJuego Instancia;

    [Header("Componentes de Interfaz")]
    public TextMeshProUGUI textoContador; 
    public TextMeshProUGUI textoNivel;

    [Header("Configuración Visual Dinámica")]
    public Camera camaraPrincipal;       // Arrastra aquí la Main Camera
    public GameObject objetoJugador;     // Arrastra aquí a tu Jugador

    [Header("Progreso del Jugador")]
    public int monedasRecolectadas = 0;
    public int nivelDificultad = 1;
    public int maximaPuntuacion = 0;

    [Header("Ajustes de Dificultad Progresiva")]
    public float velocidadBaseJugador = 6f;
    public float incrementoVelocidadPorMoneda = 0.5f;
    public int monedasParaSubirNivel = 5;

    private MovimientoJugador scriptMovimiento;

    void Awake() {
        if (Instancia == null) {
            Instancia = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        // 1. Primero buscamos al jugador y guardamos su script
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador == null) jugador = GameObject.Find("Jugador"); 

        if (jugador != null) {
            scriptMovimiento = jugador.GetComponent<MovimientoJugador>();
            objetoJugador = jugador; // Aseguramos la referencia física
        }

        // 2. Cargamos el progreso guardado
        CargarProgreso();
        ActualizarDificultad(); 
        ActualizarTextoUI(); 

        // Iniciamos la corrutina de espera de forma correcta
        StartCoroutine(AplicarColorConRetraso());
    }

    // Corrutina movida FUERA de Start() para que funcione legalmente en C#
    System.Collections.IEnumerator AplicarColorConRetraso() {
        // Espera 2 frames completos a que todo el entorno 3D se estabilice
        yield return null;
        yield return null;
        
        // 3. Aplicamos el color de los circuitos que el jugador eligió en el menú principal
        CargarColorElegido();
    }

    public void SumarMoneda() {
        monedasRecolectadas++;
        Debug.Log("Monedas en esta partida: " + monedasRecolectadas);

        // Lógica de niveles infinitos con cambio de iluminación cromática automática
        if (monedasRecolectadas % monedasParaSubirNivel == 0) {
            nivelDificultad++;
            Debug.Log("¡Subiste al Nivel " + nivelDificultad + "!");

            // Generamos tono de fondo dinámico (HSV)
            float tonoFondo = (nivelDificultad * 0.15f) % 1f;
            Color nuevoColorFondo = Color.HSVToRGB(tonoFondo, 0.6f, 0.3f); 
            CambiarColorDefondo(nuevoColorFondo);

            // Generamos tono de jugador contrastante de forma automática durante la carrera
            float tonoJugador = (tonoFondo + 0.5f) % 1f; 
            Color nuevoColorJugador = Color.HSVToRGB(tonoJugador, 0.8f, 0.9f); 
            CambiarColorDelJugador(nuevoColorJugador);
        }

        ActualizarDificultad();
        ActualizarTextoUI(); 

        if (monedasRecolectadas > maximaPuntuacion) {
            maximaPuntuacion = monedasRecolectadas;
            GuardarProgreso();
        }
    } // Aquí se cerraba la clase por error, corregido.
    
    void ActualizarDificultad() {
        if (scriptMovimiento != null) {
            float nuevaVelocidad = velocidadBaseJugador + (monedasRecolectadas * incrementoVelocidadPorMoneda);
            scriptMovimiento.Velocidad = nuevaVelocidad; 
            Debug.Log("Nueva velocidad del jugador: " + nuevaVelocidad);
        }
    }

    void ActualizarTextoUI() {
        if (textoContador != null) {
            textoContador.text = "Monedas: " + monedasRecolectadas;
        }

        if (textoNivel != null) {
            textoNivel.text = "Nivel: " + nivelDificultad;
        }
    }

    public void GuardarProgreso() {
        PlayerPrefs.SetInt("RecordMonedas", maximaPuntuacion);
        PlayerPrefs.Save(); 
        Debug.Log("Progreso guardado. Record histórico: " + maximaPuntuacion);
    }

    public void CargarProgreso() {
        maximaPuntuacion = PlayerPrefs.GetInt("RecordMonedas", 0);
        Debug.Log("Puntuación máxima cargada: " + maximaPuntuacion);
    }

    // ==========================================================
    // SISTEMA DE PERSONALIZACIÓN Y MEMORIA DE COLORES (MENÚ)
    // ==========================================================

    public void GuardarColorElegido(Color colorElegido) {
        string codigoColor = "#" + ColorUtility.ToHtmlStringRGBA(colorElegido);
        PlayerPrefs.SetString("ColorJugadorPersonalizado", codigoColor);
        PlayerPrefs.Save();
        CambiarColorDelJugador(colorElegido); // Cambia el color visual de inmediato en el menú
    }

    public void CargarColorElegido() {
        if (PlayerPrefs.HasKey("ColorJugadorPersonalizado")) {
            string codigoColor = PlayerPrefs.GetString("ColorJugadorPersonalizado");
            if (ColorUtility.TryParseHtmlString(codigoColor, out Color colorGuardado)) {
                CambiarColorDelJugador(colorGuardado);
            }
        }
    }

    public void CambiarColorDefondo(Color nuevoColor) {
        if (camaraPrincipal != null) {
            camaraPrincipal.clearFlags = CameraClearFlags.SolidColor;
            camaraPrincipal.backgroundColor = nuevoColor;
        }
    }

    public void CambiarColorDelJugador(Color nuevoColor) {
        if (objetoJugador != null) {
            Renderer renderizador = objetoJugador.GetComponent<Renderer>();
            if (renderizador == null) {
                renderizador = objetoJugador.GetComponentInChildren<Renderer>();
            }

            if (renderizador != null) {
                // Cambia el color clásico y el tinte de materiales con texturas URP (_BaseColor)
                renderizador.material.color = nuevoColor;
                if (renderizador.material.HasProperty("_BaseColor")) {
                    renderizador.material.SetColor("_BaseColor", nuevoColor);
                }
            }
        }
    }

    [ContextMenu("Borrar Datos Guardados")]
    public void BorrarDatos() {
        PlayerPrefs.DeleteAll();
        maximaPuntuacion = 0;
        Debug.Log("Datos eliminados correctamente.");
    }
}