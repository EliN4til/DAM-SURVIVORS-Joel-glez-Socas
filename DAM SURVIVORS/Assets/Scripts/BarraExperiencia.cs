using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Controla la barra de experiencia en la parte superior de la pantalla
// Muestra el progreso de experiencia y el nivel actual
public class BarraExperiencia : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image imagenBarraExp; // La barra de relleno (fill)
    [SerializeField] private TextMeshProUGUI textoNivel; // Texto que muestra el nivel (opcional)
    [SerializeField] private PlayerLevel nivelJugador; // Referencia al sistema de nivel
    
    void Start()
    {
        // Si no se asignó nivelJugador, intentar encontrarlo
        if (nivelJugador == null)
        {
            nivelJugador = FindFirstObjectByType<PlayerLevel>();
            if (nivelJugador != null)
            {
                Debug.Log("BarraExperiencia: PlayerLevel encontrado automáticamente");
            }
            else
            {
                Debug.LogError("BarraExperiencia: ¡No se encontró PlayerLevel en la escena!");
            }
        }
        
        if (imagenBarraExp == null)
        {
            Debug.LogError("BarraExperiencia: ¡Falta asignar la imagen de la barra de experiencia!");
        }
        
        // Actualizar inmediatamente
        ActualizarBarra();
    }
    
    void Update()
    {
        if (nivelJugador == null) return;
        
        ActualizarBarra();
    }
    
    private void ActualizarBarra()
    {
        if (imagenBarraExp == null) return;
        
        // Calcular el porcentaje de experiencia (0 a 1)
        // Usamos las variables en español de PlayerLevel
        float porcentajeExp = (float)nivelJugador.experienciaActual / (float)nivelJugador.experienciaParaSiguienteNivel;
        
        // Actualizar el fillAmount de la imagen
        imagenBarraExp.fillAmount = Mathf.Clamp01(porcentajeExp);
        
        // Actualizar texto del nivel si existe
        if (textoNivel != null)
        {
            textoNivel.text = "Nivel " + nivelJugador.nivelActual;
        }
    }
}
