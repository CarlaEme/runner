using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class ControladorJuego : MonoBehaviour {
    public static ControladorJuego Instancia;

    [Header("Componentes de Interfaz")]
    public TextMeshProUGUI textoContador; 
    public TextMeshProUGUI textoNivel;

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
        }

        // 2. Ahora que ya tenemos al jugador referenciado, cargamos y aplicamos todo lo demás
        CargarProgreso();
        ActualizarDificultad(); // Ahora sí aplicará la velocidad correctamente sin fallar
        ActualizarTextoUI(); 
    }

    public void SumarMoneda() {
    monedasRecolectadas++;
    Debug.Log("Monedas en esta partida: " + monedasRecolectadas);

    // LÓGICA DIRECTA: Si las monedas de esta partida llegan al límite configurado (ej. 5 o 50)
    // y todavía estás en el nivel 1, te sube obligatoriamente al nivel 2.
    if (monedasRecolectadas >= monedasParaSubirNivel && nivelDificultad == 1) {
        nivelDificultad = 2;
        Debug.Log("¡Subiste a Nivel 2!");
    } 
    // Si quieres dejar listo el Nivel 3 para cuando llegue al doble de monedas:
    else if (monedasRecolectadas >= (monedasParaSubirNivel * 2) && nivelDificultad == 2) {
        nivelDificultad = 3;
        Debug.Log("¡Subiste a Nivel 3!");
    }

    ActualizarDificultad();
    ActualizarTextoUI(); 

    if (monedasRecolectadas > maximaPuntuacion) {
        maximaPuntuacion = monedasRecolectadas;
        GuardarProgreso();
    }


    }

    void ActualizarDificultad() {
        if (scriptMovimiento != null) {
            // El jugador también se vuelve un poco más rápido con cada moneda para poder huir mejor
            float nuevaVelocidad = velocidadBaseJugador + (monedasRecolectadas * incrementoVelocidadPorMoneda);
            scriptMovimiento.Velocidad = nuevaVelocidad; 
            
            Debug.Log("Nueva velocidad del jugador: " + nuevaVelocidad);
        }
    } // Llave de cierre corregida aquí

   
    void ActualizarTextoUI() {
    // Esto actualiza el contador de monedas en pantalla
    if (textoContador != null) {
        textoContador.text = "Monedas: " + monedasRecolectadas;
    }

    // ¡NUEVO! Esto es lo que te faltaba para que cambie el nivel en la pantalla
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

    [ContextMenu("Borrar Datos Guardados")]
    public void BorrarDatos() {
        PlayerPrefs.DeleteAll();
        maximaPuntuacion = 0;
        Debug.Log("Datos eliminados correctamente.");
    }
}