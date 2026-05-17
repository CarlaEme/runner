using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class ControladorJuego : MonoBehaviour {
    public static ControladorJuego Instancia;

    [Header("Componentes de Interfaz")]
    public TextMeshProUGUI textoContador; 

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
        Debug.Log("Monedas totales en esta partida: " + monedasRecolectadas);

        // Cada 5 monedas sube un nivel automáticamente
        if (monedasRecolectadas % monedasParaSubirNivel == 0) {
            nivelDificultad++;
            Debug.Log("¡La dificultad ha subido! Nivel actual: " + nivelDificultad);
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
        if (textoContador != null) {
            textoContador.text = "Monedas: " + monedasRecolectadas;
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