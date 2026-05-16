using UnityEngine;
using UnityEngine.SceneManagement;

public class Navegacion : MonoBehaviour {

    // 1. Función para el botón Jugar
    public void IrAlJuego() {
        SceneManager.LoadScene("EscenaPrincipal");
    }

    // 2. Función para el botón Opciones (abre una escena o un panel)
    public void AbrirOpciones() {
        // Por ahora, como es la Fase 1, pondremos un mensaje en consola para verificar que funciona.
        // En la siguiente fase aquí puedes cargar otra escena o activar un panel.
        Debug.Log("Abriendo el menú de Opciones...");
    }

    // 3. Función para el botón Salir
    public void SalirDelJuego() {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Cierra el juego (funciona en el juego ya exportado/.exe)
    }
}