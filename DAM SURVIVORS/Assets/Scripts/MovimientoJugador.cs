using UnityEngine;
using UnityEngine.AI; // ¡Necesario para NavMeshAgent!

public class MovimientoJugador : MonoBehaviour
{
    ///////////////////////////////////// VARIABLES /////////////////////////////////
    private bool puedeMoverse = true;
    private float velocidadMovimiento = 5f;
    private Vector2 direccionPlana;
    private float velocidadRotacion = 10f; // Velocidad de rotación suave

    public Controles control;

    // NavMeshAgent para movimiento restringido al NavMesh
    private NavMeshAgent agente;

    ///////////////////////////////////// FUNCIONES UNITY /////////////////////////////////
    private void Awake()
    {
        control = new Controles();
        
        // Obtener o añadir NavMeshAgent
        agente = GetComponent<NavMeshAgent>();
        if (agente == null)
        {
            agente = gameObject.AddComponent<NavMeshAgent>();
            Debug.Log("NavMeshAgent añadido automáticamente al jugador");
        }
        
        // Configurar NavMeshAgent
        ConfigurarNavMeshAgent();
    }

    // Configura el NavMeshAgent con los valores correctos para el jugador
    private void ConfigurarNavMeshAgent()
    {
        agente.speed = velocidadMovimiento;
        agente.acceleration = 20f; // Aceleración rápida para respuesta inmediata
        agente.angularSpeed = 360f; // Rotación rápida
        agente.stoppingDistance = 0f; // Sin distancia de frenado
        agente.autoBraking = false; // No frenar automáticamente
        agente.radius = 0.5f; // Radio de colisión
        agente.height = 2f; // Altura del agente
        agente.baseOffset = 1f; // Levantar al jugador para que no esté atravesado por el suelo
        
        Debug.Log("NavMeshAgent configurado correctamente para el jugador");
    }

    private void OnEnable()
    {
        control.Enable();
    }
    
    private void OnDisable()
    {
        control.Disable();
    }
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (puedeMoverse && agente != null)
        {
            // Leer input del jugador
            direccionPlana = control.Player.Move.ReadValue<Vector2>();

            // Convertir a Vector3 (dirección en el mundo)
            Vector3 direccionMovimiento = new Vector3(direccionPlana.x, 0f, direccionPlana.y);

            // Solo mover si hay input
            if (direccionMovimiento.magnitude > 0.1f)
            {
                direccionMovimiento.Normalize();
                
                // Usar Move() para movimiento continuo que RESPETA el NavMesh
                // Move() no deja que el agente salga del NavMesh (agua, montañas, etc.)
                Vector3 movimiento = direccionMovimiento * velocidadMovimiento * Time.deltaTime;
                agente.Move(movimiento);

                // Rotar suavemente hacia la dirección de movimiento
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
            }
        }
    }
    
    ///////////////////////////////////// FUNCIONES PROPIAS /////////////////////////////////

    // Habilita o deshabilita el movimiento del jugador
    public void PermitirMovimiento(bool permitir)
    {
        puedeMoverse = permitir;
    }
}
