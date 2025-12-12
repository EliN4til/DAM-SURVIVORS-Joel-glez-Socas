using UnityEngine;

public class ProyectilHacha : MonoBehaviour
{
    // --- Configuración del Hacha ---
    [Header("Estadísticas")]
    [Tooltip("Velocidad a la que viaja el hacha")]
    public float velocidad = 10f;
    
    [Tooltip("Daño que causa al impactar")]
    public int dano = 2; 
    
    [Tooltip("Tiempo en segundos antes de que el hacha desaparezca")]
    public float tiempoDeVida = 1.5f;

    // --- Funciones de Unity ---

    void Start()
    {
        // Programamos la autodestrucción del hacha para que no vuele infinitamente y consuma memoria
        Destroy(this.gameObject, tiempoDeVida);
    }

    void Update()
    {
        // Movemos el hacha hacia adelante (eje Z local) en cada frame
        transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
    }

    // Esta función se llama AUTOMÁTICAMENTE cuando el Trigger del hacha toca otro Collider
    void OnTriggerEnter(Collider otro)
    {
        // Intentamos obtener el script del enemigo del objeto que hemos tocado
        EnemyController enemigo = otro.GetComponent<EnemyController>();

        // Si encontramos el script, significa que hemos golpeado a un enemigo
        if (enemigo != null)
        {
            // Le causamos daño
            enemigo.RecibirDano(dano);
            
            // Destruimos el hacha tras el impacto (para que no atraviese a todos infinitamente)
            // Si quisieras que atraviese, podrías quitar esta línea o usar un contador de "penetracion".
            Destroy(this.gameObject); 
        }
    }
}