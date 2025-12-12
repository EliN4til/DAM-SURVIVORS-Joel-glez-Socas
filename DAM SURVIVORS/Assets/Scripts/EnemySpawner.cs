using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // --- Variables ---
    [Header("Configuración de Generación")]
    [Tooltip("Radio alrededor del jugador donde pueden aparecer enemigos")]
    [SerializeField]
    private float radioGeneracion = 10f; 

    [Header("Referencias")]
    [SerializeField]
    private Transform jugador; 

    [SerializeField]
    private List<DataOleada> oleadas;

    // --- Funciones de Unity ---
    void Start()
    {
        StartCoroutine(GenerarOleadas());
    }

    // --- Funciones Propias ---

    private IEnumerator GenerarSegmento(DataOleada.SegmentoOleada segmento) 
    {
        if (segmento.RetrasoInicial > 0)
            yield return new WaitForSeconds(segmento.RetrasoInicial);

        for (int i = 0; i < segmento.CantidadEnemigos; i++)
        {
            // Intentar encontrar una posición válida (no en agua)
            Vector3 posicionSpawn = ObtenerPosicionSpawnValida();
            
            if (segmento.PrefabEnemigo != null && posicionSpawn != Vector3.zero)
            {
                Instantiate(segmento.PrefabEnemigo, posicionSpawn, Quaternion.identity);
            }
            
            yield return new WaitForSeconds(segmento.IntervaloGeneracion);
        }
    }
    
    /// <summary>
    /// Obtiene una posición válida para spawn (evita agua y áreas sin terreno)
    /// </summary>
    private Vector3 ObtenerPosicionSpawnValida()
    {
        int intentosMaximos = 10; // Intentar hasta 10 veces encontrar una posición válida
        
        for (int intento = 0; intento < intentosMaximos; intento++)
        {
            // Generar posición aleatoria alrededor del jugador
            Vector2 puntoAleatorio = Random.insideUnitCircle * radioGeneracion;
            Vector3 posicionCandidato = jugador.position + new Vector3(puntoAleatorio.x, 50f, puntoAleatorio.y);
            
            // Lanzar raycast hacia abajo para encontrar el suelo
            RaycastHit hit;
            if (Physics.Raycast(posicionCandidato, Vector3.down, out hit, 100f))
            {
                // Verificar si el punto golpeado NO tiene el tag "Water" o "Agua"
                if (hit.collider != null && !hit.collider.CompareTag("Water") && !hit.collider.CompareTag("Agua"))
                {
                    // Posición válida encontrada
                    return hit.point + Vector3.up * 0.5f; // Elevar un poco para que no quede enterrado
                }
            }
        }
        
        // Si no se encontró posición válida después de 10 intentos,
        // devolver posición del jugador (como plan B)
        Debug.LogWarning("No se pudo encontrar posición de spawn válida, usando posición del jugador");
        return jugador.position + Vector3.forward * 2f;
    }
    
    public IEnumerator GenerarOleadas()
    {
        // Validación: verificar que haya oleadas configuradas
        if (oleadas == null || oleadas.Count == 0)
        {
            Debug.LogError("¡ERROR! No hay oleadas configuradas en el EnemySpawner. Ve al Inspector y arrastra las oleadas desde Assets/Datos/Oleadas/Level1/");
            yield break;
        }

        // Validación: verificar que el jugador esté asignado
        if (jugador == null)
        {
            Debug.LogError("¡ERROR! No hay jugador asignado en el EnemySpawner. Arrastra el GameObject del jugador en el Inspector.");
            yield break;
        }

        Debug.Log($"Iniciando sistema de oleadas. Total de oleadas: {oleadas.Count}");

        foreach(DataOleada oleadaActual in oleadas)
        {
            // Validación: verificar que la oleada actual no sea null
            if (oleadaActual == null)
            {
                Debug.LogError("¡ERROR! Una de las oleadas está vacía (null). Revisa la lista de oleadas en el Inspector.");
                continue; // Saltar esta oleada y continuar con la siguiente
            }

            Debug.Log("Preparando siguiente oleada...");
            yield return new WaitForSeconds(oleadaActual.TiempoEntreOleadas);

            if (oleadaActual.Segmentos != null)
            {
                List<Coroutine> segmentosActivos = new List<Coroutine>();
                foreach(var segmento in oleadaActual.Segmentos)
                {
                    segmentosActivos.Add(StartCoroutine(GenerarSegmento(segmento)));
                }

                // Esperamos a que terminen todos los segmentos de esta oleada antes de pasar a la siguiente
                foreach(var c in segmentosActivos)
                {
                    yield return c;
                }
            }
            Debug.Log("Oleada completada.");
        }

        Debug.Log("¡Todas las oleadas completadas!");
    }
}
