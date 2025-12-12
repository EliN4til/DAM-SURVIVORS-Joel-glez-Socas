using UnityEngine;

public class ProyectilBumeran : MonoBehaviour
{
    // --- Variables ---

    [Header("Estadísticas de Combate")]
    [Tooltip("Velocidad de vuelo del bumerán")]
    public float velocidad = 15f;

    [Tooltip("Daño que causa a los enemigos")]
    public int dano = 3; 

    [Tooltip("Tiempo en segundos antes de que empiece a volver")]
    public float tiempoAntesDevolverse = 1.2f;

    [Tooltip("Distancia al jugador para considerarse 'recogido'")]
    public float distanciaRecogida = 0.5f;

    [Header("Nivel y Mejoras")]
    public int nivel = 1;
    private int rebotesActuales = 0;

    // Referencias internas
    private Transform jugador;
    private bool volviendoAlJugador = false;
    private float tiempoTranscurrido = 0f;

    // --- Funciones de Unity ---

    void Awake()
    {
        // Aseguramos que tenga Rigidbody para colisiones, pero sin físicas realistas
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        
        rb.useGravity = false;
        rb.isKinematic = true; // Controlamos el movimiento por código, no por física
    }

    void Start()
    {
        // Buscamos al jugador para saber a dónde volver
        GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");
        if (objetoJugador != null)
        {
            jugador = objetoJugador.transform;
        }
        else
        {
            // Si no hay jugador, el bumerán no tiene propósito :(
            Debug.LogWarning("¡No se encontró al Jugador! El bumerán se autodestruirá.");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Contamos el tiempo
        tiempoTranscurrido += Time.deltaTime;

        // Si ya pasó el tiempo de ida, activamos el modo vuelta
        if (tiempoTranscurrido >= tiempoAntesDevolverse)
        {
            volviendoAlJugador = true;
        }

        if (volviendoAlJugador)
        {
            MoverHaciaJugador();
        }
        else
        {
            // Modo IDA: Simplemente avanzamos hacia adelante
            transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
        }
    }

    // Cuando tocamos algo con el Trigger
    void OnTriggerEnter(Collider otro)
    {
        // Intentamos obtener el script del enemigo
        EnemyController enemigo = otro.GetComponent<EnemyController>();
        
        // Si no está en el objeto principal, buscamos en el padre (por si acaso)
        if (enemigo == null) enemigo = otro.GetComponentInParent<EnemyController>(); 

        // Si es un enemigo válido
        if (enemigo != null)
        {
            enemigo.RecibirDano(dano);
            // El bumerán NO se destruye al impactar, atraviesa a los enemigos
        }
    }

    // --- Funciones Propias ---

    private void MoverHaciaJugador()
    {
        if (jugador != null)
        {
            // 1. Calculamos la dirección hacia el jugador
            Vector3 direccion = (jugador.position - transform.position).normalized;
            
            // 2. Nos movemos en esa dirección
            transform.position += direccion * velocidad * Time.deltaTime;

            // 3. Rotamos para mirar hacia donde vamos (efecto visual)
            if (direccion != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direccion);
            }

            // 4. Comprobamos si hemos llegado al jugador
            if (Vector3.Distance(transform.position, jugador.position) < distanciaRecogida)
            {
                GestionarLlegadaAlJugador();
            }
        }
        else
        {
            // Si el jugador murió o desapareció, el bumerán también se va
            Destroy(gameObject);
        }
    }

    private void GestionarLlegadaAlJugador()
    {
        // Efecto Especial: Rebote
        // Si el nivel lo permite, el bumerán vuelve a salir en vez de destruirse
        // Nivel 1 = 0 rebotes extra. Nivel 2 = 1 rebote extra.
        if (rebotesActuales < (nivel - 1))
        {
            // ¡Rebote!
            rebotesActuales++;
            volviendoAlJugador = false; // Volvemos a modo IDA
            tiempoTranscurrido = 0f; // Reiniciamos el temporizador
            
            // Importante: Apuntamos hacia donde mira el jugador ahora
            transform.forward = jugador.forward; 
        }
        else
        {
            // Fin del trayecto
            Destroy(gameObject);
        }
    }
}
