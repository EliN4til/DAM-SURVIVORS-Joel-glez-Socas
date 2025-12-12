using UnityEngine;
using System.Collections;

public class EscudoOrbital : MonoBehaviour
{
    // --- Variables ---

    [Header("Configuración del Escudo")]
    [Tooltip("Prefab de la esfera que girará")]
    public GameObject orbePrefab;

    [Tooltip("Número de orbes activos simultáneamente")]
    public int numeroDeOrbes = 3;

    [Tooltip("Distancia desde el jugador")]
    public float radio = 2.5f;

    [Tooltip("Velocidad de giro (grados por segundo)")]
    public float velocidadRotacion = 90f;

    [Tooltip("Daño por contacto")]
    public int dano = 3; 

    [Tooltip("Duración del escudo activo (segundos)")]
    public float duracion = 5f;

    [Tooltip("Tiempo de espera entre activaciones")]
    public float cooldown = 5f;

    [Header("Estado")]
    public int nivel = 1;
    private int nivelAnterior = 1;

    // Referencias
    private Transform jugador;
    private Transform pivotTransform; // El objeto invisible centro de rotación
    private GameObject[] orbes;
    private float anguloActual = 0f;
    private bool escudoActivo = false;

    // --- Funciones de Unity ---

    void Start()
    {
        // Identificar al jugador
        if (gameObject.CompareTag("Player"))
        {
            jugador = transform;
        }
        else
        {
            GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");
            if (objetoJugador != null) jugador = objetoJugador.transform;
        }

        if (jugador != null)
        {
            ConfigurarPivote();
            StartCoroutine(RutinaCicloEscudo());
        }
        else
        {
            Debug.LogError("EscudoOrbital: No se encontró al Jugador.");
        }
    }

    void Update()
    {
        if (jugador == null) return;

        // Mantener posición junto al jugador (si no somos hijos)
        if (transform.parent != jugador)
        {
            transform.position = jugador.position;
        }
        
        // Actualizar estadísticas si cambia el nivel en tiempo real
        if (nivel != nivelAnterior)
        {
            AplicarEstadisticasNivel();
            if (escudoActivo) ReiniciarEscudo();
        }

        // Rotación del escudo
        if (escudoActivo && pivotTransform != null)
        {
            anguloActual += velocidadRotacion * Time.deltaTime;
            pivotTransform.rotation = Quaternion.Euler(0, anguloActual, 0);
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
        DestruirOrbes();
        if (pivotTransform != null) Destroy(pivotTransform.gameObject);
    }

    // --- Funciones Propias ---

    private void ConfigurarPivote()
    {
        // Creamos un objeto vacío invisible que servirá de eje de rotación
        GameObject pivotObj = new GameObject("EscudoPivot");
        pivotTransform = pivotObj.transform;
        pivotTransform.SetParent(transform); 
        pivotTransform.localPosition = Vector3.zero; 
        pivotTransform.localRotation = Quaternion.identity;
    }

    IEnumerator RutinaCicloEscudo()
    {
        while (true)
        {
            // 1. ACTIVAR
            AplicarEstadisticasNivel();
            CrearOrbes();
            escudoActivo = true;

            // 2. ESPERAR DURACIÓN
            yield return new WaitForSeconds(duracion);

            // 3. DESACTIVAR
            DestruirOrbes();
            escudoActivo = false;

            // 4. ESPERAR COOLDOWN
            yield return new WaitForSeconds(cooldown);
        }
    }

    void CrearOrbes()
    {
        if (orbePrefab == null || pivotTransform == null) return;

        DestruirOrbes(); // Limpieza previa

        orbes = new GameObject[numeroDeOrbes];
        float anguloPaso = 360f / numeroDeOrbes; // Distribución equitativa

        for (int i = 0; i < numeroDeOrbes; i++)
        {
            float angulo = i * anguloPaso * Mathf.Deg2Rad;
            Vector3 posLocal = new Vector3(Mathf.Sin(angulo) * radio, 0, Mathf.Cos(angulo) * radio);

            // Crear orbe como hijo del pivote
            GameObject orbe = Instantiate(orbePrefab, pivotTransform.position + posLocal, Quaternion.identity, pivotTransform);
            orbe.transform.localPosition = posLocal;

            // Añadir componente de daño
            OrbeIndividual componenteOrbe = orbe.AddComponent<OrbeIndividual>();
            
            // Calcular daño según nivel (+20% por nivel extra)
            float multiplicador = 1f + ((nivel - 1) * 0.2f);
            componenteOrbe.dano = Mathf.RoundToInt(dano * multiplicador);

            // Asegurar configuración física
            Rigidbody rb = orbe.GetComponent<Rigidbody>();
            if (rb == null) rb = orbe.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            orbes[i] = orbe;
        }
    }

    void DestruirOrbes()
    {
        if (orbes != null)
        {
            foreach (var orbe in orbes)
            {
                if (orbe != null) Destroy(orbe);
            }
            orbes = null;
        }
    }

    void AplicarEstadisticasNivel()
    {
        nivelAnterior = nivel;
        // Más velocidad y más orbes al subir de nivel
        velocidadRotacion = 90f + ((nivel - 1) * 30f);
        numeroDeOrbes = 3 + (nivel / 2);
    }

    void ReiniciarEscudo()
    {
        DestruirOrbes();
        CrearOrbes();
    }
}

// Componente auxiliar para gestionar el daño de cada orbe individualmente
public class OrbeIndividual : MonoBehaviour
{
    public int dano = 10;

    void OnTriggerEnter(Collider otro)
    {
        EnemyController enemigo = otro.GetComponent<EnemyController>();
        if (enemigo == null) enemigo = otro.GetComponentInParent<EnemyController>(); 

        if (enemigo != null)
        {
            enemigo.RecibirDano(dano);
        }
    }
}
