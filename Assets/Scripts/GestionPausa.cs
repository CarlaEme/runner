using UnityEngine;

public class GestionPausa : MonoBehaviour {

    [Header("Referencias de Interfaz")]
    public GameObject panelPausa; // Arrastra aquí tu objeto "PanelPausa" del Canvas

    private bool juegoPausado = false;

    void Update() {
        // --- DETECTOR PARA COMPUTADORA (Teclas Esc o P) ---
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            if (juegoPausado) {
                ReanudarJuego();
            } else {
                PausarJuego();
            }
        }
    }

    // --- FUNCIÓN PÚBLICA: Servirá tanto para el botón del celular como para las teclas ---
    public void AlternarPausaBoton() {
        if (juegoPausado) {
            ReanudarJuego();
        } else {
            PausarJuego();
        }
    }

    public void PausarJuego() {
        juegoPausado = true;
        if (panelPausa != null) panelPausa.SetActive(true); // Encendemos el menú de pausa
        Time.timeScale = 0f; // Congelamos el tiempo del juego por completo
        Debug.Log("Juego Pausado.");
    }

    public void ReanudarJuego() {
        juegoPausado = false;
        if (panelPausa != null) panelPausa.SetActive(false); // Apagamos el menú de pausa
        Time.timeScale = 1f; // Devolvemos el tiempo a la normalidad
        Debug.Log("Juego Reanudado.");
    }
}