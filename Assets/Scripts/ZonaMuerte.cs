using UnityEngine;

public class ZonaMuerte : MonoBehaviour {
    // Posición a la que volverá el jugador (la inicial)
    public Vector3 puntoDeInicio = new Vector3(0, 2, 0); 

    private void OnTriggerEnter(Collider other) {
        // Verificamos si lo que cayó fue el jugador
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null) {
            CharacterController cc = other.GetComponent<CharacterController>();
            
            // Truco de Unity: Para teletransportar un CharacterController, hay que apagarlo un milisegundo
            if (cc != null) cc.enabled = false;
            other.transform.position = puntoDeInicio;
            if (cc != null) cc.enabled = true;
        }
    }
}