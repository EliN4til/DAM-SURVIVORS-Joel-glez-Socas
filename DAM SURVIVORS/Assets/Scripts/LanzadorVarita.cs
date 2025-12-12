using UnityEngine;
using System.Collections;

public class LanzadorVarita : MonoBehaviour
{
    // --- Mis Variables ---
    
    // El prefab del proyectil que vamos a disparar
    public GameObject proyectilPrefab;

    // Cuántos segundos esperamos entre cada ataque (Base)
    public float tiempoDeAtaque = 2.5f;

    // Nivel de la varita
    public int nivel = 1;

    // Cuántos misiles lanzamos en cada tanda (Base)
    public int numeroMisiles = 2;

    // Cuánto esperamos entre cada misil de la misma tanda
    public float retardoEntreMisiles = 0.15f;

    // Hasta dónde vemos a los enemigos
    public float radioBusqueda = 15f;

    public Transform spawnPoint;

    // --- Funciones de Unity ---

    void Start()
    {
        // Iniciamos el ciclo de ataque automático
        if (proyectilPrefab != null)
        {
            StartCoroutine(LanzarAtaqueRutina());
        }
        else
        {
            Debug.LogError("No se ha asignado el prefab del proyectil en LanzadorVarita!");
        }
    }

    void OnDisable()
    {
        // Detenemos las corrutinas si el objeto se desactiva
        StopAllCoroutines();
    }

    // --- Mis Funciones ---

    IEnumerator LanzarAtaqueRutina()
    {
        // Bucle infinito de ataque
        while (true)
        {
            // 1. Disparamos los misiles
            if (proyectilPrefab != null)
            {
                // Efecto Único: Disparamos a más enemigos según el nivel
                // Nivel 1 = 2 misiles (base). Nivel 2 = 3 misiles, etc.
                int misilesA_Lanzar = numeroMisiles + (nivel - 1);

                // Determinamos desde dónde buscar enemigos
                Vector3 origen = transform.position;
                if (spawnPoint != null)
                {
                    origen = spawnPoint.position;
                }

                // Buscamos enemigos cercanos
                Collider[] enemigosCercanos = Physics.OverlapSphere(origen, radioBusqueda);
                
                // (Opcional: Ordenar por cercanía si quisiéramos, pero simple es mejor)
                
                int disparados = 0;
                foreach (var col in enemigosCercanos)
                {
                    if (disparados >= misilesA_Lanzar) break;
                    
                    // Verificar que el collider aún existe (puede haberse destruido)
                    if (col == null) continue;

                    EnemyController enemigo = col.GetComponent<EnemyController>();
                    if (enemigo != null)
                    {
                        DispararMisil(enemigo.transform);
                        disparados++;
                        yield return new WaitForSeconds(retardoEntreMisiles);
                    }
                }
            }
            
            // 2. Pausamos la coroutine (Cooldown reducido por nivel)
            float cooldownReal = tiempoDeAtaque * Mathf.Pow(0.9f, nivel - 1);
            yield return new WaitForSeconds(cooldownReal);
        }
    }

    void DispararMisil(Transform objetivo)
    {
        // Determinamos desde dónde disparar
        Vector3 origen = transform.position;
        if (spawnPoint != null)
        {
            origen = spawnPoint.position;
        }

        // Calculamos la dirección hacia el objetivo
        Vector3 direccion = (objetivo.position - origen).normalized;

        // Instanciamos el proyectil
        GameObject misil = Instantiate(proyectilPrefab, origen, Quaternion.identity);

        // Escalado de Daño (Task 6)
        ProyectilVarita scriptMisil = misil.GetComponent<ProyectilVarita>();
        if (scriptMisil != null)
        {
            scriptMisil.EstablecerDireccion(direccion);
            
            // Aumentamos daño: Base * (1 + 20% por nivel)
            float multiplicador = 1f + ((nivel - 1) * 0.2f);
            scriptMisil.dano = Mathf.RoundToInt(scriptMisil.dano * multiplicador);
            
            // LOG PARA VERIFICAR TASK 6
            Debug.Log($"[LanzadorVarita] Misil disparado! Nivel: {nivel} | Daño: {scriptMisil.dano}");
        }

        // Orientamos el misil hacia donde va
        if (direccion != Vector3.zero)
        {
            misil.transform.rotation = Quaternion.LookRotation(direccion);
        }
    }
}
