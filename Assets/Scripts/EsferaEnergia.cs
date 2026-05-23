using UnityEngine;

public class EsferaEnergia : MonoBehaviour {

    [Header("Ajustes Visuales")]
    public float velocidadRotacion = 60f;
    public float amplitudFlote = 0.2f;
    public float velocidadFlote = 2f;

    private Vector3 posicionInicial;

    void Start() {
        posicionInicial = transform.position;
    }

    void Update() {
        // Efecto visual: La esfera gira en el espacio y flota elegantemente de arriba a abajo
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);
        
        float nuevoY = posicionInicial.y + Mathf.Sin(Time.time * velocidadFlote) * amplitudFlote;
        transform.position = new Vector3(transform.position.x, nuevoY, transform.position.z);
    }

    // DETECTOR DE CHOQUE: Activa el turbo cuando el jugador la atraviesa
    private void OnTriggerEnter(Collider other) {
        // Buscamos si el objeto que chocó tiene el componente de movimiento del jugador
        MovimientoJugador jugador = other.GetComponent<MovimientoJugador>();

        if (jugador != null) {
            // Le ordenamos al jugador que encienda su Turbo y su estela combinada
            jugador.ActivarTurboEsfera();

            // Desaparecemos la esfera de la pista con un efecto limpio
            Destroy(gameObject);
        }
    }
}