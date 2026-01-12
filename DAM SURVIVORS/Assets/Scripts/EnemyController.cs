using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private GameObject jugador;
    private Transform objetivo;

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
        [Range(0, 100)] public float Probabilidad;
    }
    public System.Collections.Generic.List<ItemBotin> TablaBotin;

    private NavMeshAgent agente;

    [Header("Configuración Enjambre")]
    public GameObject MinionPrefab;
    public int CantidadMinions = 0; 
    public float RadioSpawnMinion = 2f; 

    private SpriteRenderer spriteRenderer;
    private Renderer meshRenderer;
    private Material materialOriginal;
    private Color colorOriginal;
    private bool estaMuriendo = false;

    private void Awake()
    {
        if (estadisticas != null)
        {
            vidaActual = estadisticas.vida;
            dano = estadisticas.dano;
            defensa = estadisticas.defensa;
            velocidad = estadisticas.velocidad;
        }

        agente = GetComponent<NavMeshAgent>();
        
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            colorOriginal = spriteRenderer.color;
        }
        else
        {
            meshRenderer = GetComponentInChildren<Renderer>();
            if (meshRenderer != null)
            {
                materialOriginal = meshRenderer.material;
                colorOriginal = materialOriginal.color;
            }
        }
    }

    void Start()
    {
        if (estadisticas == null)
        {
            return;
        }

        jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null) objetivo = jugador.transform;
        
        if (agente != null)
        {
            agente.speed = velocidad;
            agente.autoBraking = false;
            agente.stoppingDistance = 0f;
        }

        if (MinionPrefab != null && CantidadMinions > 0)
        {
            GenerarEsbirros();
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (estaMuriendo) return;
        if (agente == null || !agente.enabled) return;

        if (objetivo != null)
        {
            Vector3 direccion = (objetivo.position - transform.position).normalized;
            agente.Move(direccion * agente.speed * Time.deltaTime);
            
            if (direccion != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, 10f * Time.deltaTime);
            }
        }
    }    

    private float tiempoUltimoAtaque = 0f;
    private float intervaloAtaque = 1.0f;

    private void OnTriggerStay(Collider other)
    {
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

    public void RecibirDano(int cantidadDano)
    {
        if (spriteRenderer != null || meshRenderer != null) 
        {
            StartCoroutine(FeedbackParpadeo());
            StartCoroutine(FeedbackSacudida());
        }

        vidaActual -= cantidadDano;

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        estaMuriendo = true;
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
    }

    private System.Collections.IEnumerator AnimacionMuerte()
    {
        if (agente != null) agente.enabled = false;

        if (meshRenderer != null)
        {
            Material matInstancia = new Material(meshRenderer.material);
            meshRenderer.material = matInstancia;
            
            matInstancia.SetFloat("_Surface", 1);
            matInstancia.SetFloat("_Blend", 0);
            matInstancia.SetOverrideTag("RenderType", "Transparent");
            matInstancia.renderQueue = 3000;
            matInstancia.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            matInstancia.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        SoltarBotin();

        float duracion = 0.8f;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, tiempoTranscurrido / duracion);

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
        Color colorFlash = Color.red;
        
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
        float duracion = 0.15f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            if (this == null) yield break;
            
            transform.Rotate(0, Random.Range(-10f, 10f), 0);
            
            tiempo += Time.deltaTime;
            yield return null;
        }
    }
}
