using UnityEngine;

public class SeguimientoCamara : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    [Tooltip("El objeto que la cámara debe seguir. Arrastra al Jugador aquí.")]
    public Transform objetivo;

    [Tooltip("Qué tan suavemente sigue la cámara al objetivo. Un valor más bajo es más 'flotante', un valor más alto es más rígido.")]
    public float velocidadSuavizado = 5f;

    // Esta variable privada guardará la distancia y el ángulo inicial
    // entre la cámara y el jugador.
    private Vector3 offset;
    public Controles control;


    [Header("Configuración del zoom")]
    private float zoom = 1f;
    [Tooltip("Sensibilidad de la rueda del ratón.")]
    public float sensibilidadZoom = 0.1f;

    [Tooltip("Qué tan cerca podemos estar del objetivo.")]
    public float zoomMinimo = 0.5f;

    [Tooltip("Qué tan lejos podemos estar del objetivo.")]
    public float zoomMaximo = 2f;
    ///////////////////////////////////////////////////////////////// FUNCIONES UNITY /////////////////////////////////////////


    void Awake()
    {
        control = new Controles();
    }

    void OnEnable()
    {
        control.Enable();
    }

    void OnDisable()
    {
        control.Disable();
    }

    void Start()
    {
        // Calculamos el 'offset' (desplazamiento) inicial una sola vez.
        // Esta es la distancia y ángulo que queremos mantener SIEMPRE.
        // offset = PosiciónCámara - PosiciónJugador
        offset = transform.position - objetivo.position;
    }

    void Update()
    {
        SetZoom();
    }

    // ¡Usamos LateUpdate() para todo lo relacionado con cámaras!
    void LateUpdate()
    {
        // Si no tenemos objetivo (ej. el jugador muere), no hacemos nada.
        if (objetivo == null) return;

        // 1. Calcular la posición a la que la cámara QUIERE estar.
        //    PosiciónDeseada = PosiciónActualDelJugador + DistanciaGuardada
        Vector3 posicionDeseada = objetivo.position + offset * zoom;

        // 2. Mover la cámara suavemente.
        //    Usamos Vector3.Lerp (que ya vimos en nuestra guía) para
        //    movernos desde nuestra posición actual (transform.position)
        //    hacia la posición deseada, a una velocidad suave.
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, velocidadSuavizado * Time.deltaTime);
    }

    /////////////////////////////////////////////////////////////// FUNCIONES PROPIAS /////////////////////////////////////////
    
    
    public void SetZoom()
    {
       float valorRueda = control.Camara.Zoom.ReadValue<float>();
       zoom -= valorRueda * sensibilidadZoom;
       zoom = Mathf.Clamp(zoom, zoomMinimo, zoomMaximo);
    }
}
