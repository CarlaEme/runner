using UnityEngine;

public class CamaraSeguimiento : MonoBehaviour {
    public Transform objetivo; // Arrastra aquí al Jugador
    private float zFija;

    void Start() {
        // Guardamos la posición inicial en Z de la cámara
        zFija = transform.position.z;
    }

    void LateUpdate() {
        if (objetivo != null) {
            // Seguimos al jugador en X y mantenemos la Y de la cámara.
            // La Z se mantiene según el valor inicial guardado.
            transform.position = new Vector3(objetivo.position.x, transform.position.y, zFija);
        }
    }
}