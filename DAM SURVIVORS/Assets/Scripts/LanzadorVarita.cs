using UnityEngine;
using System.Collections;

public class LanzadorVarita : MonoBehaviour
{
    // --- Variables ---
    
    [Header("Configuración")]
    public GameObject proyectilPrefab;
    public float tiempoDeAtaque = 2.5f;
    public int nivel = 1;
    public int numeroMisiles = 2;
    public float retardoEntreMisiles = 0.15f;
    public float radioBusqueda = 15f;
    public Transform spawnPoint;

    // --- Funciones de Unity ---

    void Start()
    {
        if (proyectilPrefab != null)
        {
            StartCoroutine(LanzarAtaqueRutina());
        }
        else
        {
            Debug.LogError("LanzadorVarita: Falta asignar el prefab del proyectil.");
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    // --- Funciones Propias ---

    IEnumerator LanzarAtaqueRutina()
    {
        while (true)
        {
            if (proyectilPrefab != null)
            {
                // Nivel 1 = 2 misiles. Nivel 2 = 3 misiles, etc.
                int misilesA_Lanzar = numeroMisiles + (nivel - 1);

                Vector3 origen = transform.position;
                if (spawnPoint != null) origen = spawnPoint.position;

                Collider[] enemigosCercanos = Physics.OverlapSphere(origen, radioBusqueda);
                
                int disparados = 0;
                foreach (var col in enemigosCercanos)
                {
                    if (disparados >= misilesA_Lanzar) break;
                    if (col == null) continue;

                    EnemyController enemigo = col.GetComponent<EnemyController>();
                    if (enemigo != null)
                    {
                        DispararMisil(enemigo.transform);
                        disparados++;
                        // Disparo simultáneo (sin espera)
                    }
                }
            }
            
            // Reducción de cooldown por nivel
            float cooldownReal = tiempoDeAtaque * Mathf.Pow(0.9f, nivel - 1);
            yield return new WaitForSeconds(cooldownReal);
        }
    }

    void DispararMisil(Transform objetivo)
    {
        Vector3 origen = transform.position;
        if (spawnPoint != null) origen = spawnPoint.position;

        Vector3 direccion = (objetivo.position - origen).normalized;

        GameObject misil = Instantiate(proyectilPrefab, origen, Quaternion.identity);

        ProyectilVarita scriptMisil = misil.GetComponent<ProyectilVarita>();
        if (scriptMisil != null)
        {
            scriptMisil.EstablecerDireccion(direccion);
            
            // Aumentamos daño: Base * (1 + 20% por nivel)
            float multiplicador = 1f + ((nivel - 1) * 0.2f);
            scriptMisil.dano = Mathf.RoundToInt(scriptMisil.dano * multiplicador);
        }

        if (direccion != Vector3.zero)
        {
            misil.transform.rotation = Quaternion.LookRotation(direccion);
        }
    }
}
