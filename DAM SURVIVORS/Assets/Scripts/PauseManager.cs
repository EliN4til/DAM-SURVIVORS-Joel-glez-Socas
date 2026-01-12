using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private GameObject menuPausaUI;
    
    private bool estaPausado = false;

    void Update()
    {
        // Detectar tecla ESC para pausar/reanudar
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Si el menú de Level Up está abierto, NO permitimos pausar ni reanudar (el Level Up tiene prioridad)
            if (LevelUpManager.Instancia != null && LevelUpManager.Instancia.IsMenuOpen)
            {
                return;
            }

            if (estaPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Reanudar()
    {
        Debug.Log("Reanudar juego");
        if (menuPausaUI != null) menuPausaUI.SetActive(false);
        
        Time.timeScale = 1f;
        estaPausado = false;
    }

    void Pausar()
    {
        Debug.Log("Pausar juego");
        if (menuPausaUI != null) menuPausaUI.SetActive(true);
        
        Time.timeScale = 0f;
        estaPausado = true;
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo al menú principal...");
        Time.timeScale = 1f;
        
        SceneManager.LoadScene("MainMenu"); 
    }
}
