using UnityEngine;

public class Hacha : MonoBehaviour
{
    [Header("Datos del Hacha")]
    [Tooltip("Velocidad de movimiento del hacha")]
    public float velocidad = 10f; 
    
    [Tooltip("Tiempo antes de destruirse")]
    public float tiempoVida = 5f;
    
    [Tooltip("Daño al impactar")]
    public int dano = 2; 

    // --- Funciones de Unity ---
    void Start()
    {
        // Destruir automáticamente después del tiempo de vida
        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        // Mover hacia adelante
        transform.position += transform.forward * velocidad * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider otro) 
    {
        // Verificar si chocamos con un enemigo
        if (otro.CompareTag("Enemy"))
        {
            EnemyController enemigo = otro.GetComponent<EnemyController>();
            if (enemigo != null)
            {
                enemigo.RecibirDano(dano);
                // Opcional: Destruir el hacha al impactar
                // Destroy(gameObject);
            }
        }
    }
}
