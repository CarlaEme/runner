using UnityEngine;

public class CamaraSeguimiento : MonoBehaviour {
    public Transform objetivo; // Arrastra aquí al Jugador
    
    // Cambiamos la variable de un solo número (float) a un vector 3D para calcular la distancia completa
    private Vector3 distanciaSeparacion; 

    void Start() {
        if (objetivo != null) {
            // Guardamos la distancia exacta original que hay entre la cámara y el jugador al arrancar
            distanciaSeparacion = transform.position - objetivo.position;
        }
    }

    void LateUpdate() {
        if (objetivo != null) {
            // 1. Calculamos la posición ideal sumando la posición actual del jugador + la distancia que calculamos en el Start
            Vector3 posicionDeseada = objetivo.position + distanciaSeparacion;
            
            // 2. Conservamos la altura original (Y) fija de la cámara para que no dé saltos extraños
            posicionDeseada.y = transform.position.y; 

            // 3. Aplicamos el movimiento final (Ahora se moverá en X y avanzará infinitamente en Z)
            transform.position = posicionDeseada;
        }
    }
}