using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int vidaActual;
    [SerializeField] private int vidaMaxima = 100;
    
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

    private void Awake() 
    {
        vidaActual = vidaMaxima;
        estaVivo = true;
    }
    
    public void RecibirDano(int dano)
    {
        if (!estaVivo) return;

        if (dano > defensa)
        {
            vidaActual -= (dano - defensa);

            if (CameraShake.Instancia != null)
            {
                CameraShake.Instancia.Sacudir(0.15f, 0.3f); 
            }

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
        StartCoroutine(AnimacionMuerte());
    }
    
    private System.Collections.IEnumerator AnimacionMuerte()
    {
        MovimientoJugador movimiento = GetComponent<MovimientoJugador>();
        if (movimiento != null) movimiento.enabled = false;
        
        Renderer playerRenderer = GetComponentInChildren<Renderer>();
        Color colorOriginal = Color.white;
        
        if (playerRenderer != null)
        {
            Material matInstancia = new Material(playerRenderer.material);
            playerRenderer.material = matInstancia;
            colorOriginal = matInstancia.color;
            
            matInstancia.SetFloat("_Surface", 1);
            matInstancia.SetFloat("_Blend", 0);
            matInstancia.SetOverrideTag("RenderType", "Transparent");
            matInstancia.renderQueue = 3000;
            matInstancia.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            matInstancia.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }
        
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
        
        EnemyController[] enemigos = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        foreach (EnemyController enemigo in enemigos)
        {
            if (enemigo != null)
            {
                enemigo.enabled = false;
            }
        }
        
        TimerNivel timer = FindFirstObjectByType<TimerNivel>();
        float tiempoSobrevivido = 0f;
        
        if (timer != null)
        {
            tiempoSobrevivido = timer.ObtenerTiempo();
        }
        
        if (GameOverManager.Instancia != null)
        {
            GameOverManager.Instancia.MostrarGameOver(tiempoSobrevivido);
        }
        
        gameObject.SetActive(false);
    }
}
