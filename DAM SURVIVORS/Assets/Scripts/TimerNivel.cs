using UnityEngine;
using TMPro;

/// <summary>
/// Muestra el tiempo transcurrido en el nivel
/// Formato: MM:SS
/// </summary>
public class TimerNivel : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TextMeshProUGUI textoTimer; // Texto que muestra el tiempo
    
    [Header("Configuración")]
    [SerializeField] private bool iniciarAutomaticamente = true; // Si debe iniciar al cargar la escena
    
    private float tiempoTranscurrido = 0f;
    private bool timerActivo = false;
    
    void Start()
    {
        if (textoTimer == null)
        {
            Debug.LogError("¡Falta asignar el texto del timer!");
        }
        
        if (iniciarAutomaticamente)
        {
            IniciarTimer();
        }
        
        ActualizarTexto();
    }
    
    void Update()
    {
        if (timerActivo)
        {
            tiempoTranscurrido += Time.deltaTime;
            ActualizarTexto();
        }
    }
    
    private void ActualizarTexto()
    {
        if (textoTimer == null) return;
        
        // Convertir a minutos y segundos
        int minutos = Mathf.FloorToInt(tiempoTranscurrido / 60f);
        int segundos = Mathf.FloorToInt(tiempoTranscurrido % 60f);
        
        // Formato MM:SS
        textoTimer.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }
    
    /// <summary>
    /// Inicia el timer
    /// </summary>
    public void IniciarTimer()
    {
        timerActivo = true;
        tiempoTranscurrido = 0f;
    }
    
    /// <summary>
    /// Pausa el timer
    /// </summary>
    public void PausarTimer()
    {
        timerActivo = false;
    }
    
    /// <summary>
    /// Reinicia el timer a cero
    /// </summary>
    public void ReiniciarTimer()
    {
        tiempoTranscurrido = 0f;
        ActualizarTexto();
    }
    
    /// <summary>
    /// Obtiene el tiempo transcurrido en segundos
    /// </summary>
    public float ObtenerTiempo()
    {
        return tiempoTranscurrido;
    }
}
