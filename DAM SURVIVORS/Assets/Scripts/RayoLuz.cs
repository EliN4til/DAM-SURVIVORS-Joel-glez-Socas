using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayoLuz : MonoBehaviour
{
    public int dano = 2;
    public float tiempoVida = 3f;
    public float tiempoEntreDano = 0.25f;
    
    // Nivel del rayo
    public int nivel = 1;

    // --- Movimiento ---
    public float offsetAdelante = 6f;
    public float velocidadRotacion = 5f;

    private List<EnemyController> enemigosEnRayo = new List<EnemyController>();
    private Transform transformJugador;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true; 
        }
    }

    void Start()
    {
        Destroy(gameObject, tiempoVida);
        
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            transformJugador = jugador.transform;
            ActualizarTransform(100f); 
        }

        // Arrancamos la máquina de hacer daño
        StartCoroutine(BucleDano());

        // Asignar capa "PlayerAttack" y evitar colisiones entre ataques
        int capaAtaqueJugador = LayerMask.NameToLayer("PlayerAttack");
        if (capaAtaqueJugador != -1)
        {
            gameObject.layer = capaAtaqueJugador;
            Physics.IgnoreLayerCollision(capaAtaqueJugador, capaAtaqueJugador);
        }

        // Efecto Único: Aumentar ancho y velocidad de daño (Task 8)
        // Ancho: +50% por nivel
        float escalaAncho = 1f + ((nivel - 1) * 0.5f);
        transform.localScale = new Vector3(transform.localScale.x * escalaAncho, transform.localScale.y, transform.localScale.z * escalaAncho);

        // Velocidad de daño: Se reduce un 20% por nivel (mínimo 0.05s)
        tiempoEntreDano = Mathf.Max(0.05f, tiempoEntreDano * Mathf.Pow(0.8f, nivel - 1));
    }

    void Update()
    {
        // Si el jugador murió o no existe, no hacemos nada
        if (transformJugador == null) return;
        
        // Actualizamos la posición y rotación suavemente cada frame
        ActualizarTransform(Time.deltaTime * velocidadRotacion);
    }

    private void ActualizarTransform(float t)
    {
        // 1. Calculamos hacia dónde queremos mirar: la rotación del jugador + 90 grados
        Quaternion rotacionObjetivo = transformJugador.rotation * Quaternion.Euler(90, 0, 0);
        
        // 2. Giramos poquito a poco hacia esa dirección
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, t);
        
        // 3. Calculamos la distancia para que el rayo empiece JUSTO delante del jugador
        // La altura por defecto del cilindro es 2. La mitad es 1. Multiplicamos por la escala Y.
        float mitadLongitud = transform.localScale.y; 
        float margenJugador = 1.0f; // Un poco de espacio para no atravesar el cuerpo
        float distanciaSegura = mitadLongitud + margenJugador;

        // 4. Nos colocamos en la posición correcta
        transform.position = transformJugador.position + transform.up * distanciaSegura;
    }

    // Esta es la corrutina PRINCIPAL. Solo hay UNA y se encarga de todo el daño.
    private IEnumerator BucleDano()
    {
        // Bucle infinito que se ejecuta mientras el rayo exista
        while (true)
        {
            // 1. Esperamos el tiempo de "tick" (ej. 0.25 segundos)
            yield return new WaitForSeconds(tiempoEntreDano);

            // 2. Repartimos dolor a TODOS los enemigos que estén dentro del rayo
            foreach (var enemigo in enemigosEnRayo)
            {
                if (enemigo != null)
                {
                    enemigo.RecibirDano(dano);
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // ¡Alguien ha entrado en el rayo!
        // Si tiene el script de enemigo...
        EnemyController enemigo = other.GetComponent<EnemyController>();
        if (enemigo != null)
        {
            // ...lo apuntamos en la lista negra para hacerle daño luego.
            enemigosEnRayo.Add(enemigo);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // El enemigo ha logrado escapar del rayo...
        EnemyController enemigo = other.GetComponent<EnemyController>();
        if (enemigo != null)
        {
            // ...lo borramos de la lista.
            enemigosEnRayo.Remove(enemigo);
        }
    }
}