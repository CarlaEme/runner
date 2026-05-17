using UnityEngine;

public class MovimientoJugador : MonoBehaviour {
    [Header("Ajustes de Movimiento")]
    public float Velocidad = 6f;
    public float fuerzaSalto = 8f;
    public float gravedad = 20f;
    
    private Vector3 direccionMovimiento = Vector3.zero;
    private CharacterController controller;

    [Header("Sistema de Salud (Vidas)")]
    public int vidasActuales = 3;
    public int vidasMaximas = 3;

    void Start() {
        controller = GetComponent<CharacterController>();
        vidasActuales = vidasMaximas;
    }

    void Update() {
        if (controller.isGrounded) {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            direccionMovimiento = new Vector3(moveX, 0, moveZ);
            direccionMovimiento *= Velocidad;

            if (Input.GetButton("Jump")) {
                direccionMovimiento.y = fuerzaSalto;
            }
        }

        direccionMovimiento.y -= gravedad * Time.deltaTime;
        controller.Move(direccionMovimiento * Time.deltaTime);
    }

    public void RecibirDanio(int cantidad) {
        vidasActuales -= cantidad;
        Debug.Log("¡Ay! El jugador perdió vida. Vidas restantes: " + vidasActuales);

        if (vidasActuales <= 0) {
            Morir();
        }
    }

    void Morir() {
        Debug.Log("¡Game Over! Reiniciando partida...");
        string nombreEscenaActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscenaActual);
    }

    // Detector nativo para el Character Controller
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        Meteorito meteorito = hit.gameObject.GetComponent<Meteorito>();
        if (meteorito != null) {
            RecibirDanio(meteorito.danio);
            meteorito.Explotar();
        }
    }
}