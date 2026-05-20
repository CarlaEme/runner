using UnityEngine;

public class BotonSeleccionColor : MonoBehaviour {
    
    [Header("Color para este Botón")]
    public Color colorDeEsteBotón = Color.cyan;

    // Al presionar el botón, guardamos el color en el disco de inmediato
    public void SeleccionarColorPersonaje() {
        // Convertimos el color a formato de texto HTML (ej. #FF0000)
        string codigoColor = "#" + ColorUtility.ToHtmlStringRGBA(colorDeEsteBotón);
        
        // Lo guardamos directamente en la memoria de la computadora
        PlayerPrefs.SetString("ColorJugadorPersonalizado", codigoColor);
        PlayerPrefs.Save();
        
        Debug.Log("¡Color guardado con éxito en el Menú!: " + codigoColor);
    }
}