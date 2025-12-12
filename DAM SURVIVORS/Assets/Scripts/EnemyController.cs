using UnityEngine;
using UnityEngine.AI; // ¡Necesario para usar NavMeshAgent!

public class EnemyController : MonoBehaviour
{
    // --- Mis Variables ---
    private GameObject jugador;
    private Transform objetivo;

    // Estadísticas del Enemigo
    [Header("Configuración")]
    public EnemyStats estadisticas;
    private int vidaActual;
    private int dano;
    private int defensa;
    private float velocidad;

    [System.Serializable]
    public class ItemBotin
    {
        public GameObject Prefab;
        [Range(0, 100)] public float Probabilidad; // 0 a 100%
    }
    public System.Collections.Generic.List<ItemBotin> TablaBotin;

    // Variable para el movimiento
    private NavMeshAgent agente;

    [Header("Configuración Enjambre (Solo para Enemigo 4)")]
    public GameObject MinionPrefab;
    public int CantidadMinions = 0; 
    public float RadioSpawnMinion = 2f; 

    // Visuals (soporte para 2D y 3D)
    private SpriteRenderer spriteRenderer;
    private Renderer meshRenderer;
    private Material materialOriginal;
    private Color colorOriginal;
    private bool estaMuriendo = false; // Bandera para controlar el estado

    // --- Funciones de Unity ---
    private void Awake()
    {
        // Inicializamos las estadísticas del enemigo desde el ScriptableObject
        // Nota: Es importante usar las variables en español como definimos en EnemyStats
        if (estadisticas != null)
        {
            vidaActual = estadisticas.vida;
            dano = estadisticas.dano;
            defensa = estadisticas.defensa;
            velocidad = estadisticas.velocidad;
        }

        // Obtenemos el componente del Agente
        agente = GetComponent<NavMeshAgent>();
        
        // Soporte para enemigos 2D (Sprite) y 3D (Mesh)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            colorOriginal = spriteRenderer.color;
        }
        else
        {
            // Si no es sprite, buscar MeshRenderer
            meshRenderer = GetComponentInChildren<Renderer>();
            if (meshRenderer != null)
            {
                // Crear copia del material para no afectar a otros enemigos
                materialOriginal = meshRenderer.material;
                colorOriginal = materialOriginal.color;
            }
        }
    }

    void Start()
    {
        // Se asegura de que las estadísticas estén asignadas
        if (estadisticas == null)
        {
            Debug.LogError("¡Error! No se han asignado las estadísticas del enemigo en " + this.gameObject.name);
            return;
        }

        // Encuentra al jugador en la escena
        jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null) objetivo = jugador.transform;
        
        // CONFIGURAMOS AL AGENTE CON NUESTROS DATOS
        if (agente != null)
        {
            agente.speed = velocidad; // El agente usará la velocidad de la ficha
            agente.autoBraking = false; // No frenar automáticamente (movimiento continuo)
            agente.stoppingDistance = 0f; // Sin distancia de frenado
        }

        if (MinionPrefab != null && CantidadMinions > 0)
        {
            GenerarEsbirros();
        }
    }

    void Update()
    {
        // Si el juego está pausado (Game Over), no hacer nada
        if (Time.timeScale == 0f) return;
        
        // No ejecutar lógica si está muriendo
        if (estaMuriendo) return;
        
        // Verificar que el agente esté activo antes de usarlo
        if (agente == null || !agente.enabled) return;

        // El enemigo siempre sigue al jugador si está vivo
        if (objetivo != null)
        {
            // Calcular dirección hacia el jugador
            Vector3 direccion = (objetivo.position - transform.position).normalized;
            
            // Mover directamente hacia el jugador (sin pathfinding complejo, más simple y rápido)
            agente.Move(direccion * agente.speed * Time.deltaTime);
            
            // Rotar suavemente hacia el jugador
            if (direccion != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, 10f * Time.deltaTime);
            }
        }
    }    

    // --- Lógica de Ataque (NO afecta el movimiento) ---
    private float tiempoUltimoAtaque = 0f;
    private float intervaloAtaque = 1.0f; // Daño cada 1 segundo

    private void OnTriggerStay(Collider other)
    {
        // Si toca al jugador y ha pasado suficiente tiempo desde el último ataque
        if (other.CompareTag("Player") && Time.time >= tiempoUltimoAtaque + intervaloAtaque)
        {
            PlayerStats estadisticasJugador = other.GetComponent<PlayerStats>();
            if (estadisticasJugador != null)
            {
                estadisticasJugador.RecibirDano(dano);
                tiempoUltimoAtaque = Time.time;
            }
        }
    }

    // --- Mis Funciones ---
    public void RecibirDano(int cantidadDano)
    {
        // Feedback visual (Parpadeo y Sacudida)
        if (spriteRenderer != null || meshRenderer != null) 
        {
            StartCoroutine(FeedbackParpadeo());
            StartCoroutine(FeedbackSacudida());
        }

        // (En el futuro, aquí podríamos usar 'defensa' para reducir el daño)
        vidaActual -= cantidadDano;

        // Comprobamos si el enemigo ha muerto
        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        // Marcar como muriendo para detener Update
        estaMuriendo = true;
        
        // Iniciar animación de desvanecimiento
        StartCoroutine(AnimacionMuerte());
    }

    private void SoltarBotin()
    {
        if (TablaBotin != null && TablaBotin.Count > 0)
        {
            float valorAleatorio = Random.Range(0f, 100f);
            float acumulado = 0f;
            foreach(var item in TablaBotin)
            {
                acumulado += item.Probabilidad;
                if (valorAleatorio <= acumulado)
                {
                    if (item.Prefab != null)
                        Instantiate(item.Prefab, transform.position, Quaternion.identity);
                    break;
                }
            }
        }
        Debug.Log("¡Enemigo " + this.gameObject.name + " derrotado!");
    }

    private System.Collections.IEnumerator AnimacionMuerte()
    {
        // Desactivar movimiento
        if (agente != null) agente.enabled = false;

        // Configurar material para transparencia si es 3D
        if (meshRenderer != null)
        {
            // Crear una instancia del material para no afectar a otros
            Material matInstancia = new Material(meshRenderer.material);
            meshRenderer.material = matInstancia;
            
            // Configurar para modo transparente (URP)
            matInstancia.SetFloat("_Surface", 1); // Transparent
            matInstancia.SetFloat("_Blend", 0); // Alpha blending
            matInstancia.SetOverrideTag("RenderType", "Transparent");
            matInstancia.renderQueue = 3000;
            matInstancia.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            matInstancia.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        // Soltar botín antes de desvanecerse
        SoltarBotin();

        // Desvanecimiento gradual durante 0.8 segundos
        float duracion = 0.8f;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, tiempoTranscurrido / duracion);

            // Aplicar transparencia según el tipo de renderer
            if (spriteRenderer != null)
            {
                Color nuevoColor = colorOriginal;
                nuevoColor.a = alpha;
                spriteRenderer.color = nuevoColor;
            }
            else if (meshRenderer != null)
            {
                Color nuevoColor = colorOriginal;
                nuevoColor.a = alpha;
                meshRenderer.material.color = nuevoColor;
            }

            yield return null;
        }

        // Destruir el objeto después de la animación
        Destroy(gameObject);
    }

    private void GenerarEsbirros()
    {
        for(int i=0; i<CantidadMinions; i++)
        {
             Vector2 puntoAleatorio = Random.insideUnitCircle * RadioSpawnMinion;
             Vector3 posSpawn = transform.position + new Vector3(puntoAleatorio.x, 0, puntoAleatorio.y);
             Instantiate(MinionPrefab, posSpawn, Quaternion.identity);
        }
    }

    private System.Collections.IEnumerator FeedbackParpadeo()
    {
        Color colorFlash = Color.red; // Color rojo para daño
        
        // Aplicar feedback según el tipo de renderer
        if (spriteRenderer != null)
        {
            spriteRenderer.color = colorFlash;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = colorOriginal;
        }
        else if (meshRenderer != null)
        {
            meshRenderer.material.color = colorFlash;
            yield return new WaitForSeconds(0.1f);
            meshRenderer.material.color = colorOriginal;
        }
    }

    private System.Collections.IEnumerator FeedbackSacudida()
    {
        // Nota: NavMeshAgent controla la posición, así que hacemos una rotación rápida (Wiggle)
        float duracion = 0.15f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            if (this == null) yield break; // Seguridad si muere
            
            // Rotación aleatoria rápida
            transform.Rotate(0, Random.Range(-10f, 10f), 0);
            
            tiempo += Time.deltaTime;
            yield return null;
        }
    }
}
