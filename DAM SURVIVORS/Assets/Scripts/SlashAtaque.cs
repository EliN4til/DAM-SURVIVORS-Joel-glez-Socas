using UnityEngine;

public class SlashAtaque : MonoBehaviour
{
    // --- Variables ---

    [Header("Configuración del Ataque")]
    [Tooltip("Daño que inflige el tajo")]
    public int dano = 10;

    [Tooltip("Distancia máxima que recorre el tajo")]
    public float distanciaMaxima = 5f;

    [Tooltip("Velocidad a la que avanza el tajo")]
    public float velocidad = 10f;

    // Nivel del ataque (para tamaño)
    public int nivel = 1;

    private Vector3 posicionInicial;

    // --- Funciones de Unity ---

    void Start()
    {
        // Guardamos la posición inicial
        posicionInicial = transform.position;

        // 1. Buscar al jugador por su tag
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            // 2. Hacemos que el Slash sea hijo del jugador para que se mueva con él
            transform.SetParent(jugador.transform);
        }

        // 3. Asignar capa "PlayerAttack" y evitar colisiones entre ataques
        int capaAtaqueJugador = LayerMask.NameToLayer("PlayerAttack");
        if (capaAtaqueJugador != -1)
        {
            gameObject.layer = capaAtaqueJugador;
            Physics.IgnoreLayerCollision(capaAtaqueJugador, capaAtaqueJugador);
        }

        // Efecto Único: Aumentar tamaño con el nivel
        // Aumentamos ancho (X) y largo (Z) un 25% por nivel extra
        float escalaExtra = 1f + ((nivel - 1) * 0.25f);
        transform.localScale = new Vector3(transform.localScale.x * escalaExtra, transform.localScale.y, transform.localScale.z * escalaExtra);
    }

    void Update()
    {
        // Mover el slash hacia adelante (en su propia dirección local)
        transform.Translate(Vector3.forward * velocidad * Time.deltaTime);

        // Calcular distancia recorrida desde el punto de inicio
        float distanciaRecorrida = Vector3.Distance(posicionInicial, transform.position);

        // Si ha recorrido la distancia máxima, lo destruimos
        if (distanciaRecorrida >= distanciaMaxima)
        {
            Destroy(gameObject);
        }
    }

    // Esta función se llama AUTOMÁTICAMENTE cuando otro collider entra en nuestro sensor
    private void OnTriggerEnter(Collider otro)
    {
        // Intentamos obtener el componente EnemyController del objeto con el que chocamos
        EnemyController enemigo = otro.GetComponent<EnemyController>();

        // Si 'enemigo' no es null, ¡significa que hemos chocado con un enemigo!
        if (enemigo != null)
        {
            // Le decimos al enemigo que reciba daño
            enemigo.RecibirDano(dano);
        }
    }
}
