using UnityEngine;
using System.Collections.Generic;

public class GeneradorCaminoInfinito : MonoBehaviour {
    [Header("Configuración del Desarrollador")]
    public List<GameObject> prefabsPorciones; // Aquí arrastras tus diferentes diseños de nivel (Tipo A, B, C...)
    public float largoDePorcion = 30f;        // El tamaño en Z de cada bloque (configurable en el Inspector)
    public int porcionesEnPantalla = 5;       // Cuántos bloques quieres ver clonados a la vez

    [Header("Referencias")]
    public Transform transformJugador;

    private List<GameObject> porcionesActivas = new List<GameObject>();
    private float siguientePuntoZ = 0f; // Lleva la cuenta de dónde debe nacer el siguiente bloque

    void Start() {
        // Al arrancar, generamos los primeros bloques para que el jugador no caiga al vacío
        for (int i = 0; i < porcionesEnPantalla; i++) {
            // El primer bloque suele ser vacío (sin obstáculos) para empezar tranquilo
            if (i == 0) {
                GenerarBloque(0); // Genera el primer prefab de la lista (haz que sea el más fácil)
            } else {
                GenerarBloqueRandom();
            }
        }
    }

    void Update() {
        if (transformJugador == null) return;

        // Si el jugador avanzó lo suficiente y ya pasó el primer bloque...
        if (transformJugador.position.z - largoDePorcion > (siguientePuntoZ - (porcionesEnPantalla * largoDePorcion))) {
            GenerarBloqueRandom();  // Creamos un bloque nuevo al frente
            BorrarBloqueViejo();    // Eliminamos el bloque de atrás para ahorrar memoria
        }
    }

    void GenerarBloqueRandom() {
        // Elige un diseño de nivel al azar de la lista que configuró el desarrollador
        int indiceAleatorio = Random.Range(0, prefabsPorciones.Count);
        GenerarBloque(indiceAleatorio);
    }

    void GenerarBloque(int indicePrefab) {
        // Calculamos la posición exacta donde debe encajar la porción
        Vector3 posicionSpawn = new Vector3(0, 0, siguientePuntoZ);
        
        // Clonamos la porción de nivel
        GameObject bloqueClonado = Instantiate(prefabsPorciones[indicePrefab], posicionSpawn, Quaternion.identity);
        
        // Lo guardamos en la lista para poder borrarlo después
       porcionesActivas.Add(bloqueClonado);
       
        // Recorremos el punto de aparición para el siguiente bloque
        siguientePuntoZ += largoDePorcion;
    }

    void BorrarBloqueViejo() {
        // Destruye el bloque más antiguo (el primero de la lista)
        Destroy(porcionesActivas[0]);
        porcionesActivas.RemoveAt(0);
    }
}