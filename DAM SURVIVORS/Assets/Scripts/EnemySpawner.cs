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
            Vector3 posicionSpawn = ObtenerPosicionSpawnValida();
            
            if (segmento.PrefabEnemigo != null && posicionSpawn != Vector3.zero)
            {
                Instantiate(segmento.PrefabEnemigo, posicionSpawn, Quaternion.identity);
            }
            
            yield return new WaitForSeconds(segmento.IntervaloGeneracion);
        }
    }
    
    // Obtiene una posición válida para spawn (evita agua y áreas sin terreno)
    private Vector3 ObtenerPosicionSpawnValida()
    {
        int intentosMaximos = 10;
        
        for (int intento = 0; intento < intentosMaximos; intento++)
        {
            Vector2 puntoAleatorio = Random.insideUnitCircle * radioGeneracion;
            Vector3 posicionCandidato = jugador.position + new Vector3(puntoAleatorio.x, 50f, puntoAleatorio.y);
            
            RaycastHit hit;
            if (Physics.Raycast(posicionCandidato, Vector3.down, out hit, 100f))
            {
                // Evitamos spawnear en agua
                if (hit.collider != null && hit.collider.tag != "Water" && hit.collider.tag != "Agua")
                {
                    return hit.point + Vector3.up * 0.5f; 
                }
            }
        }
        
        // Plan B: Spawn cerca del jugador si falla
        return jugador.position + Vector3.forward * 2f;
    }
    
    public IEnumerator GenerarOleadas()
    {
        if (oleadas == null || oleadas.Count == 0)
        {
            Debug.LogError("EnemySpawner: No hay oleadas configuradas.");
            yield break;
        }

        if (jugador == null)
        {
            Debug.LogError("EnemySpawner: Falta asignar al jugador.");
            yield break;
        }

        foreach(DataOleada oleadaActual in oleadas)
        {
            if (oleadaActual == null) continue;

            yield return new WaitForSeconds(oleadaActual.TiempoEntreOleadas);

            if (oleadaActual.Segmentos != null)
            {
                List<Coroutine> segmentosActivos = new List<Coroutine>();
                foreach(var segmento in oleadaActual.Segmentos)
                {
                    segmentosActivos.Add(StartCoroutine(GenerarSegmento(segmento)));
                }

                // Esperar a que terminen todos los segmentos
                foreach(var c in segmentosActivos)
                {
                    yield return c;
                }
            }
        }
    }
}
