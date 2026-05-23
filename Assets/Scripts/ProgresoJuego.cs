using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necesario si usas TextMeshPro para los textos

public class ProgresoJuego : MonoBehaviour {

    [Header("Referencias de Objetos")]
    public Transform jugador;          // Arrastra aquí a tu Jugador
    public Slider barraProgreso;       // Arrastra aquí el Slider de la UI
    public TextMeshProUGUI textoDistancia; // Opcional: Texto para ver los metros (ej. "150m")

    [Header("Configuración del Nivel")]
    public float distanciaMeta = 500f; // La distancia en metros para llenar la barra o cambiar de nivel

    private float posicionInicialZ;
    private float recordDistancia;

    void Start() {
        if (jugador != null) {
            // Guardamos el punto de partida exacto del jugador en el eje Z
            posicionInicialZ = jugador.position.z;
        }

        // Recuperamos el récord anterior guardado en la memoria del juego (PlayerPrefs)
        recordDistancia = PlayerPrefs.GetFloat("RecordDistancia", 0f);
        Debug.Log("Tu récord actual a batir es: " + recordDistancia.ToString("F0") + " metros.");

        // Configuramos los límites de la barra en base a la meta establecida
        if (barraProgreso != null) {
            barraProgreso.minValue = 0f;
            barraProgreso.maxValue = distanciaMeta;
            barraProgreso.value = 0f;
        }
    }

    void Update() {
        if (jugador == null) return;

        // 1. Calculamos cuántos metros reales ha avanzado el personaje desde el inicio
        float distanciaRecorrida = jugador.position.z - posicionInicialZ;

        // Evitamos que marque números negativos si se mueve un poco hacia atrás al inicio
        if (distanciaRecorrida < 0) distanciaRecorrida = 0; 

        // 2. Actualizamos el valor visual de la barra de progreso
        if (barraProgreso != null) {
            barraProgreso.value = distanciaRecorrida;
        }

        // 3. Opcional: Actualizamos el marcador de texto en la pantalla
        if (textoDistancia != null) {
            textoDistancia.text = distanciaRecorrida.ToString("F0") + " m / " + distanciaMeta + " m";
        }

        // 4. Si el jugador supera su récord actual en tiempo real, guardamos el nuevo récord
        if (distanciaRecorrida > recordDistancia) {
            recordDistancia = distanciaRecorrida;
            PlayerPrefs.SetFloat("RecordDistancia", recordDistancia);
        }

        // 5. EVENTO DE META: Si llega al final de la distancia propuesta
        if (distanciaRecorrida >= distanciaMeta) {
            LograrMeta();
        }
    }

    void LograrMeta() {
        Debug.Log("<color=green>¡Felicidades! Has completado la distancia del nivel.</color>");
        // Aquí puedes activar tu pantalla de "Nivel Completado" o llamar a tu ManejadorEscenas
    }
}