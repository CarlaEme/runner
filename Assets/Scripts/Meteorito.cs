using UnityEngine;

public class Meteorito : MonoBehaviour {
    [Header("Ajustes de Daño")]
    public int danio = 1;
    private bool yaDestruido = false;

    private void OnCollisionEnter(Collision collision) {
        if (yaDestruido) return;

        // Si choca contra el jugador
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.name == "Jugador") {
            yaDestruido = true;
            
            MovimientoJugador jugador = collision.gameObject.GetComponent<MovimientoJugador>();
            if (jugador != null) {
                jugador.RecibirDanio(danio);
            }
            
            Debug.Log("¡Meteorito destruido por chocar con el Jugador!");
            Destroy(gameObject);
            return;
        }

        // Si choca contra el suelo, un obstáculo o la zona de muerte
        // Pasamos el nombre a minúsculas (.ToLower()) para que detecte "Caja", "caja", "Obstaculo", etc.
        string nombreMinusculas = collision.gameObject.name.ToLower();

        if (nombreMinusculas.Contains("suelo") || 
            nombreMinusculas.Contains("plane") || 
            nombreMinusculas.Contains("floor") || 
            nombreMinusculas.Contains("obstaculo") || 
            nombreMinusculas.Contains("caja") || 
            nombreMinusculas.Contains("cube") || 
            collision.gameObject.CompareTag("Obstaculo") || 
            collision.gameObject.name == "ZonaMuerte") 
        {
            yaDestruido = true;
            Debug.Log("Meteorito destruido de inmediato contra el entorno: " + collision.gameObject.name);
            Destroy(gameObject);
        }
    }
    // Respaldo público
    public void Explotar() {
        if (!yaDestruido) {
            yaDestruido = true;
            Destroy(gameObject);
        }
    }
}