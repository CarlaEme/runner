using UnityEngine;

public class MovimientoJugador : MonoBehaviour {
    public float velocidad = 6f;
    public float fuerzaSalto = 8f;
    public float gravedad = 20f;
    private Vector3 direccionMovimiento = Vector3.zero;
    private CharacterController controller;

    void Start() {
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        if (controller.isGrounded) {
            // Movimiento en X (Horizontal) y Z (Vertical)
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            direccionMovimiento = new Vector3(moveX, 0, moveZ);
            direccionMovimiento *= velocidad;

            // Movimiento en Y (Salto)
            if (Input.GetButton("Jump")) {
                direccionMovimiento.y = fuerzaSalto;
            }
        }

        // Aplicar gravedad constante
        direccionMovimiento.y -= gravedad * Time.deltaTime;
        controller.Move(direccionMovimiento * Time.deltaTime);
    }
}