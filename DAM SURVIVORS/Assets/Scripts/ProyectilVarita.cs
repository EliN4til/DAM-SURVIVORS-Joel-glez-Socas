using UnityEngine;
 
 public class ProyectilVarita : MonoBehaviour
{
    // --- Mis Variables ---

    // La velocidad a la que viaja el misil
    public float velocidad = 12f;

    // El daño que hace al impactar
    public int dano = 4; // Reducido para balance

    // Cuánto tiempo vive antes de desaparecer si no choca
    public float tiempoDeVida = 3f;

    private Vector3 direccion;
    private bool direccionEstablecida = false;

    // --- Funciones de Unity ---

    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void Start()
    {
        // Programamos la autodestrucción por si no impacta nada
        Destroy(gameObject, tiempoDeVida);

        // Asignar capa de ataque del jugador
        // (Código de capas eliminado para asegurar colisión con enemigos)
    }

    void Update()
    {
        // Solo nos movemos si ya tenemos una dirección asignada
        if (direccionEstablecida)
        {
            // Nos movemos en la dirección que nos indicaron
            transform.position += direccion * velocidad * Time.deltaTime;
        }
    }

    // --- Mis Funciones ---

    // Esta función la llama el lanzador para decirnos hacia dónde volar
    public void EstablecerDireccion(Vector3 nuevaDireccion)
    {
        direccion = nuevaDireccion.normalized;
        direccionEstablecida = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Comprobamos si hemos impactado con un enemigo
        // Comprobamos si hemos impactado con un enemigo
        EnemyController enemigo = other.GetComponent<EnemyController>();
        if (enemigo == null) enemigo = other.GetComponentInParent<EnemyController>(); // Fallback por si el collider está en un hijo

        if (enemigo != null)
        {
            // Le hacemos daño
            enemigo.RecibirDano(dano);
            
            // Nos destruimos al impactar
            Destroy(gameObject);
        }
    }
}
