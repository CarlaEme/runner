using UnityEngine;

public class ItemColeccionable : MonoBehaviour {
    public float velocidadRotacion = 100f;

    void Update() {
        // Hace que la moneda gire en el espacio 3D para llamar la atención
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other) {
    if (other.GetComponent<CharacterController>() != null) {
        // Le avisamos al cerebro del juego que sume una moneda y procese la dificultad
        if (ControladorJuego.Instancia != null) {
            ControladorJuego.Instancia.SumarMoneda();
        }

        Debug.Log("¡Moneda recolectada con éxito!");
        Destroy(gameObject); // Borra la moneda de la escena
    }
}

    
}


