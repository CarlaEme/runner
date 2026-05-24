using UnityEngine;

public class LluviaEnSeccion : MonoBehaviour
{
    private ParticleSystem sistemaLluvia;

    void Start()
    {
        // Buscamos el componente de partículas en este mismo objeto
        sistemaLluvia = GetComponent<ParticleSystem>();
        
        // En cuanto la Porción C aparezca (se clone) en el mapa, encendemos la lluvia
        if (sistemaLluvia != null && !sistemaLluvia.isPlaying)
        {
            sistemaLluvia.Play();
        }
    }
}