using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    /// <summary>
    /// Inicia el juego cargando la escena del primer nivel
    /// </summary>
    public void Jugar()
    {
        Debug.Log("Cargando escena: Nivel1");
        SceneManager.LoadScene("Nivel1");
    }

    /// <summary>
    /// Cierra la aplicación
    /// </summary>
    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        
        #if UNITY_EDITOR
        // En el Editor de Unity, detenemos el modo Play
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // En una build compilada, cerramos la aplicación
        Application.Quit();
        #endif
    }
}
