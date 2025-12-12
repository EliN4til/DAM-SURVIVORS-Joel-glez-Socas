using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Controla el menú de Game Over
/// Se activa cuando el jugador muere
/// </summary>
public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instancia { get; private set; }
    
    [Header("Referencias UI")]
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private TextMeshProUGUI textoGameOver;
    [SerializeField] private TextMeshProUGUI textoTiempoSobrevivido;
    
    private void Awake()
    {
        // Singleton
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Asegurar que el panel esté oculto al inicio
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }
    }
    
    /// <summary>
    /// Muestra el menú de Game Over
    /// </summary>
    public void MostrarGameOver(float tiempoSobrevivido)
    {
        Debug.Log($"MostrarGameOver llamado con tiempo: {tiempoSobrevivido} segundos");
        
        if (panelGameOver == null)
        {
            Debug.LogError("Panel Game Over no asignado!");
            return;
        }
        
        // Mostrar panel PRIMERO
        panelGameOver.SetActive(true);
        Debug.Log("Panel Game Over activado");
        
        // Actualizar texto de tiempo sobrevivido si existe
        if (textoTiempoSobrevivido != null)
        {
            int minutos = Mathf.FloorToInt(tiempoSobrevivido / 60f);
            int segundos = Mathf.FloorToInt(tiempoSobrevivido % 60f);
            string textoFormateado = $"Sobreviviste: {minutos:00}:{segundos:00}";
            textoTiempoSobrevivido.text = textoFormateado;
            Debug.Log($"Texto actualizado a: {textoFormateado}");
        }
        else
        {
            Debug.LogError("textoTiempoSobrevivido es NULL!");
        }
        
        // Pausar el juego AL FINAL (después de actualizar UI)
        Time.timeScale = 0f;
        Debug.Log("Juego pausado (timeScale = 0)");
    }
    
    /// <summary>
    /// Botón: Volver al menú principal
    /// </summary>
    public void VolverAlMenu()
    {
        // Resetear el time scale
        Time.timeScale = 1f;
        
        // Cargar escena del menú principal
        SceneManager.LoadScene("MainMenu"); 
    }
    
    /// <summary>
    /// Botón: Reintentar (opcional)
    /// </summary>
    public void Reintentar()
    {
        // Resetear el time scale
        Time.timeScale = 1f;
        
        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
