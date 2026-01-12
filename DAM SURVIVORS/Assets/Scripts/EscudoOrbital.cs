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
        // Si no tenemos el prefab o el pivote, no podemos hacer nada
        if (orbePrefab == null) return;
        if (pivotTransform == null) return;

        DestruirOrbes(); // Borramos los anteriores primero

        // Inicializamos el array para guardar los orbes
        orbes = new GameObject[numeroDeOrbes];
        
        // Calculamos cuántos grados hay de separación entre cada orbe
        float gradosSeparacion = 360f / numeroDeOrbes;

        for (int i = 0; i < numeroDeOrbes; i++)
        {
            // Calculamos el ángulo para este orbe específico
            float anguloGrados = i * gradosSeparacion;
            
            // Convertimos a radianes porque Unity usa radianes para Seno y Coseno
            float anguloRadianes = anguloGrados * (Mathf.PI / 180f);

            // Calculamos la posición X y Z usando trigonometría
            float x = Mathf.Sin(anguloRadianes) * radio;
            float z = Mathf.Cos(anguloRadianes) * radio;

            Vector3 posicionLocal = new Vector3(x, 0, z);

            // Creamos el orbe en la posición calculada
            // Lo ponemos como hijo del 'pivotTransform' para que gire con él
            GameObject nuevoOrbe = Instantiate(orbePrefab, pivotTransform);
            nuevoOrbe.transform.localPosition = posicionLocal;

            // Le añadimos el script para que haga daño
            OrbeIndividual scriptDano = nuevoOrbe.AddComponent<OrbeIndividual>();
            
            // Calculamos el daño: Base + 20% por cada nivel extra
            float bonoNivel = (nivel - 1) * 0.2f;
            float danoTotal = dano * (1 + bonoNivel);
            
            // Asignamos el daño (redondeando a entero)
            scriptDano.dano = Mathf.RoundToInt(danoTotal);

            // Nos aseguramos de que tenga Rigidbody para las colisiones
            Rigidbody rb = nuevoOrbe.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = nuevoOrbe.AddComponent<Rigidbody>();
            }
            
            // Configuramos el Rigidbody para que no se caiga
            rb.useGravity = false;
            rb.isKinematic = true;

            // Guardamos el orbe en nuestra lista
            orbes[i] = nuevoOrbe;
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
        // Base 3 orbes. +1 orbe cada 2 niveles (Nivel 2->4, Nivel 4->5...)
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
