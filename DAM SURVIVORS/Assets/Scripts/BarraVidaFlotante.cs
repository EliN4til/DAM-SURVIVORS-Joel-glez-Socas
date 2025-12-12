using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla la barra de vida flotante sobre el jugador
/// La barra sigue al jugador y siempre mira a la cámara
/// </summary>
public class BarraVidaFlotante : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image imagenBarraVida; // La barra de relleno (fill)
    [SerializeField] private PlayerStats estadisticasJugador; // Referencia al jugador
    
    [Header("Configuración")]
    [SerializeField] private Vector3 desplazamiento = new Vector3(0, 2.5f, 0); // Altura sobre el jugador
    [SerializeField] private bool mirarCamara = true; // Si debe mirar siempre a la cámara
    
    private Camera camaraPrincipal;
    
    void Start()
    {
        camaraPrincipal = Camera.main;
        
        // Si no se asignó estadisticasJugador, intentar encontrarlo
        if (estadisticasJugador == null)
        {
            estadisticasJugador = FindFirstObjectByType<PlayerStats>();
        }
        
        if (imagenBarraVida == null)
        {
            Debug.LogError("¡Falta asignar la imagen de la barra de vida!");
        }
    }
    
    void Update()
    {
        if (estadisticasJugador == null) return;
        
        // Actualizar posición: seguir al jugador
        transform.position = estadisticasJugador.transform.position + desplazamiento;
        
        // Hacer que la barra siempre mire a la cámara (billboard)
        if (mirarCamara && camaraPrincipal != null)
        {
            transform.LookAt(transform.position + camaraPrincipal.transform.rotation * Vector3.forward,
                             camaraPrincipal.transform.rotation * Vector3.up);
        }
        
        // Actualizar el fill de la barra
        ActualizarBarra();
    }
    
    private void ActualizarBarra()
    {
        if (imagenBarraVida == null) return;
        
        // Calcular el porcentaje de vida (0 a 1)
        // Usamos las nuevas propiedades en español: VidaActual y VidaMaxima
        float porcentajeVida = (float)estadisticasJugador.VidaActual / (float)estadisticasJugador.VidaMaxima;
        
        // Actualizar el fillAmount de la imagen
        imagenBarraVida.fillAmount = Mathf.Clamp01(porcentajeVida);
    }
}
