using UnityEngine;

public class ControladorLluvia : MonoBehaviour
{
    [Header("Referencias")]
    public ParticleSystem sistemaLluvia;
    public Transform jugador;

    [Header("Ajustes de Persecución")]
    public bool seguirAlJugador = true;
    public float alturaLluvia = 10f; // Qué tan arriba del jugador flotarán las nubes

    [Header("Ajustes de Posición Aleatoria")]
    public Vector3 rangoMinimoPista; // Coordenadas X, Y, Z mínimas de tu mapa
    public Vector3 rangoMaximoPista; // Coordenadas X, Y, Z máximas de tu mapa

    void Start()
    {
        // Si no arrastraste al jugador, intentamos buscarlo por Tag o por nombre
        if (jugador == null)
        {
            GameObject jugadorObj = GameObject.FindWithTag("Player");
            if (jugadorObj == null) jugadorObj = GameObject.Find("Jugador");
            if (jugadorObj != null) jugador = jugadorObj.transform;
        }

        // Si olvidaste asignar las partículas, las buscamos en el mismo objeto
        if (sistemaLluvia == null)
        {
            sistemaLluvia = GetComponent<ParticleSystem>();
        }
    }

    void LateUpdate()
    {
        // Si está marcado seguir al jugador, la lluvia se mueve con él en tiempo real
        if (seguirAlJugador && jugador != null && sistemaLluvia != null)
        {
            Vector3 nuevaPosicion = jugador.position;
            nuevaPosicion.y += alturaLluvia; // La mantenemos en el cielo
            sistemaLluvia.transform.position = nuevaPosicion;
        }
    }

    // Método público para teletransportar la lluvia a un punto aleatorio de la pista
    [ContextMenu("Mover a Posición Aleatoria")]
    public void ReposicionarLluviaAleatoria()
    {
        if (sistemaLluvia == null) return;

        // Desactivamos la persecución para que se quede estática en el punto aleatorio
        seguirAlJugador = false;

        float aleatorioX = Random.Range(rangoMinimoPista.x, rangoMaximoPista.x);
        float aleatorioY = alturaLluvia; // Mantiene la altura del cielo
        float aleatorioZ = Random.Range(rangoMinimoPista.z, rangoMaximoPista.z);

        sistemaLluvia.transform.position = new Vector3(aleatorioX, aleatorioY, aleatorioZ);
        
        // Aseguramos que esté encendida
        if (!sistemaLluvia.isPlaying) sistemaLluvia.Play();
        
        Debug.Log("La tormenta se movió a las coordenadas: " + sistemaLluvia.transform.position);
    }

    // Métodos para encender y apagar la lluvia por código (ej. al subir de nivel)
    public void ActivarLluvia(bool activar)
    {
        if (sistemaLluvia == null) return;

        if (activar && !sistemaLluvia.isPlaying)
            sistemaLluvia.Play();
        else if (!activar && sistemaLluvia.isPlaying)
            sistemaLluvia.Stop();
    }
}