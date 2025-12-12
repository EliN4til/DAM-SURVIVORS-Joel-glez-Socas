using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // --- Variables ---
    [SerializeField] private int vidaActual;
    [SerializeField] private int vidaMaxima = 100;
    
    // Propiedades públicas para acceso desde UI (Encapsulamiento)
    public int VidaActual => vidaActual;
    public int VidaMaxima => vidaMaxima;
    
    [Header("Estadísticas de Combate")]
    [SerializeField] private int ataque = 5;
    public int Ataque => ataque;

    [SerializeField] private int defensa = 0;
    public int Defensa => defensa;

    [SerializeField] private float velocidadMovimiento = 5f;
    public float VelocidadMovimiento => velocidadMovimiento;

    [SerializeField] private float velocidadAtaque = 1f;
    public float VelocidadAtaque => velocidadAtaque;

    private bool estaVivo;

    // --- Funciones de Unity ---
    private void Awake() 
    {
        vidaActual = vidaMaxima;
        estaVivo = true;
    }

    // --- Funciones Propias ---
    
    public void RecibirDano(int dano)
    {
        if (!estaVivo) return;

        // Solo recibe daño si el daño es mayor que la defensa.
        if (dano > defensa)
        {
            // Le quitamos el daño menos la defensa
            vidaActual -= (dano - defensa);

            // Sacudida de cámara (muy sutil)
            if (CameraShake.Instancia != null)
            {
                CameraShake.Instancia.Sacudir(0.15f, 0.3f); 
            }

            // Si la vida es menor o igual que 0, muere
            if (vidaActual <= 0)
            {
                estaVivo = false;
                Morir();
            }
        }
    }

    private void Morir()
    {
        estaVivo = false;
        
        // Iniciar animación de muerte
        StartCoroutine(AnimacionMuerte());
    }
    
    private System.Collections.IEnumerator AnimacionMuerte()
    {
        // Desactivar movimiento del jugador
        MovimientoJugador movimiento = GetComponent<MovimientoJugador>();
        if (movimiento != null) movimiento.enabled = false;
        
        // Obtener renderer para el efecto de desvanecimiento
        Renderer playerRenderer = GetComponentInChildren<Renderer>();
        Color colorOriginal = Color.white;
        
        if (playerRenderer != null)
        {
            // Crear instancia del material para no afectar otros objetos
            Material matInstancia = new Material(playerRenderer.material);
            playerRenderer.material = matInstancia;
            colorOriginal = matInstancia.color;
            
            // Configurar material para transparencia
            matInstancia.SetFloat("_Surface", 1); // Transparent
            matInstancia.SetFloat("_Blend", 0); // Alpha blending
            matInstancia.SetOverrideTag("RenderType", "Transparent");
            matInstancia.renderQueue = 3000;
            matInstancia.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            matInstancia.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }
        
        // Desvanecimiento gradual durante 1 segundo
        float duracion = 1.0f;
        float tiempoTranscurrido = 0f;
        
        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, tiempoTranscurrido / duracion);
            
            if (playerRenderer != null)
            {
                Color nuevoColor = colorOriginal;
                nuevoColor.a = alpha;
                playerRenderer.material.color = nuevoColor;
            }
            
            yield return null;
        }
        
        // Detener TODOS los enemigos antes de mostrar Game Over
        EnemyController[] enemigos = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        foreach (EnemyController enemigo in enemigos)
        {
            if (enemigo != null)
            {
                enemigo.enabled = false; // Desactivar el script para detener Update()
            }
        }
        
        // Obtener tiempo sobrevivido del timer
        TimerNivel timer = FindFirstObjectByType<TimerNivel>();
        float tiempoSobrevivido = 0f;
        
        if (timer != null)
        {
            tiempoSobrevivido = timer.ObtenerTiempo();
            Debug.Log($"Timer encontrado. Tiempo sobrevivido: {tiempoSobrevivido} segundos");
        }
        else
        {
            Debug.LogError("¡TimerNivel NO encontrado! El tiempo será 0.");
        }
        
        // Mostrar Game Over
        if (GameOverManager.Instancia != null)
        {
            GameOverManager.Instancia.MostrarGameOver(tiempoSobrevivido);
        }
        else
        {
            Debug.LogError("¡GameOverManager.Instancia es null!");
        }
        
        // Ocultar el jugador (no destruir para evitar errores)
        gameObject.SetActive(false);
    }
}
