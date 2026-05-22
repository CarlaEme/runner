using UnityEngine;
using UnityEngine.SceneManagement;

public class ManejadorEscenas : MonoBehaviour {

    [Header("Interfaz de Pausa (Solo para la Escena Principal)")]
    public GameObject panelPausa;
    private bool juegoPausado = false;

    void Update() {
        // Si el script nota que asignaste un panel de pausa y presionas Esc o P, pausa el juego
        if (panelPausa != null && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))) {
            if (juegoPausado) {
                ReanudarJuego();
            } else {
                PausarJuego();
            }
        }
    }

    // --- ENLACES DE NAVEGACIÓN ---

    public void IniciarCarrera() {
        Time.timeScale = 1f; // Asegura que el tiempo corra normalmente
        SceneManager.LoadScene("EscenaPrincipal"); // Carga tu pista infinita
    }

    public void RegresarAlMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal"); // Regresa al inicio
    }

    public void ReiniciarCarreraActual() {
        Time.timeScale = 1f;
        // Recarga la misma escena en la que estás corriendo (resetea monedas y nivel a 1)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SalirDelVideojuego() {
        Debug.Log("Cerrando el juego de forma segura...");
        Application.Quit();
    }

    // --- GESTIÓN DE TIEMPO (PAUSA) ---

    public void PausarJuego() {
        if (panelPausa != null) {
            panelPausa.SetActive(true);
            Time.timeScale = 0f; // Congela por completo las físicas, meteoritos y al jugador
            juegoPausado = true;
        }
    }

    public void ReanudarJuego() {
        if (panelPausa != null) {
            panelPausa.SetActive(false);
            Time.timeScale = 1f; // El juego continúa con su velocidad normal
            juegoPausado = false;
        }
    }
}