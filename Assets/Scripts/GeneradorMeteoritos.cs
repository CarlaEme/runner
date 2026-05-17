using UnityEngine;

public class GeneradorMeteoritos : MonoBehaviour {
    [Header("Configuración del Objeto")]
    public GameObject prefabMeteorito; // Aquí arrastraremos el bloque azul del proyecto

    [Header("Ajustes de Aparición")]
    public float tiempoBaseEntreCaidas = 3f; // Segundos iniciales entre meteoritos
    public float rangoSpawnX = 10f;          // Qué tan ancho es el camino donde pueden caer
    public float alturaSpawn = 15f;          // A qué altura del cielo aparecen

    private float cronometro;
    private Transform transformJugador;

    void Start() {
        // Buscamos al jugador para que los meteoritos caigan cerca de su posición actual
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador == null) jugador = GameObject.Find("Jugador");

        if (jugador != null) {
            transformJugador = jugador.transform;
        }
        
        cronometro = tiempoBaseEntreCaidas;
    }

    void Update() {
        if (transformJugador == null) return;

        // El cronómetro avanza cuadro por cuadro
        cronometro -= Time.deltaTime;

        if (cronometro <= 0) {
            SpawnMeteorito();
            
            // --- CURVA DE DIFICULTAD ADAPTATIVA ---
            // Consultamos el nivel de dificultad actual desde tu ControladorJuego
            int nivelActual = 1;
            if (ControladorJuego.Instancia != null) {
                nivelActual = ControladorJuego.Instancia.nivelDificultad;
            }

            // Reducimos el tiempo de espera dividiendo entre el nivel. 
            // Nivel 1: espera = 3s / Nivel 2: espera = 1.5s / Nivel 3: espera = 1s... ¡caen más rápido!
            float nuevoTiempoEspera = tiempoBaseEntreCaidas / nivelActual;
            
            // Ponemos un límite mínimo para que no sea imposible de esquivar (ej. mínimo un meteorito cada 0.4 segundos)
            cronometro = Mathf.Max(nuevoTiempoEspera, 0.4f); 
        }
    }
    
void SpawnMeteorito() {
        if (transformJugador == null) return;

        // 1. Tomamos la posición actual del personaje como centro
        Vector3 posicionBaseJugador = transformJugador.position;

        // 2. NUEVO/RETORNO: Rango de dispersión hacia la izquierda o derecha (-rangoSpawnX a rangoSpawnX)
        // Asegúrate de que en el Inspector 'rangoSpawnX' tenga un valor como 6 o 8
        float posicionAleatoriaX = Random.Range(-rangoSpawnX, rangoSpawnX);
        
        // 3. Rango en Z para que caigan un poco atrás, encima o más adelante
        float posicionAleatoriaZ = Random.Range(-4f, 12f); 

        // 4. Calculamos el punto de caída final
        Vector3 posicionSpawn = new Vector3(
            posicionBaseJugador.x + posicionAleatoriaX, 
            alturaSpawn, 
            posicionBaseJugador.z + posicionAleatoriaZ
        );

        // Clonamos el meteorito
        Instantiate(prefabMeteorito, posicionSpawn, Quaternion.identity);
    }
}