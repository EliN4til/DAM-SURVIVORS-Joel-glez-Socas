using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;   

public class LanzadorArma : MonoBehaviour
{
    // --- Mis Variables ---

    // El Prefab del proyectil que vamos a disparar
    public GameObject proyectilPrefab;

    // Tiempo en segundos entre cada disparo (Base)
    public float tiempoDeAtaque = 3f;
    
    // Nivel del arma (Controla estadísticas y efectos especiales)
    public int nivel = 1;

    public Transform spawnPoint;

    // --- Funciones de Unity ---

    void Start()
    {
        // En cuanto empieza el juego, iniciamos la rutina de ataque.
        // La coroutine se encargará de su propio bucle.
        if (proyectilPrefab != null)
        {
            StartCoroutine(LanzarAtaqueRutina());
        }
        else
        {
            Debug.LogError("¡No se ha asignado un Prefab de proyectil en el LanzadorArma!");
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    // --- Mis Funciones ---

    IEnumerator LanzarAtaqueRutina()
    {
        // Un bucle infinito y seguro, gracias a la Coroutine.
        while (true)
        {
            // 1. Determinamos el punto de origen
            if (this == null || gameObject == null) yield break;

            Vector3 origen = transform.position;
            if (spawnPoint != null)
            {
                origen = spawnPoint.position;
            }
            else
            {
                // Debug.LogWarning("SpawnPoint no asignado en LanzadorArma. Usando la posición del jugador por defecto.");
            }

            // 2. Instanciamos el proyectil (o la ráfaga de proyectiles)
            if (proyectilPrefab != null)
            {
                // Calculamos el cooldown real basado en el nivel (se reduce 10% por nivel)
                float cooldownReal = tiempoDeAtaque * Mathf.Pow(0.9f, nivel - 1);

                // Lógica de Ráfaga (Efecto Único del Hacha)
                // Lanzamos tantos proyectiles como indique el nivel
                for (int i = 0; i < nivel; i++)
                {
                    LanzarUnProyectil();
                    
                    // Si es una ráfaga, esperamos un poquito entre hachas
                    if (nivel > 1)
                    {
                        yield return new WaitForSeconds(0.2f);
                    }
                }

                // Esperamos el tiempo de ataque antes de volver al inicio del bucle.
                yield return new WaitForSeconds(cooldownReal);
            }
            else
            {
                // Si no hay prefab, esperamos igual para no colgar el juego
                yield return new WaitForSeconds(tiempoDeAtaque);
            }
        }
    }

    void LanzarUnProyectil()
    {
        Vector3 origen = transform.position;
        if (spawnPoint != null) origen = spawnPoint.position;

        GameObject proyectil = Instantiate(proyectilPrefab, origen, transform.rotation * proyectilPrefab.transform.rotation);

        // Escalado de Daño (Task 6)
        // Buscamos si es un Hacha para subirle el daño
        ProyectilHacha scriptHacha = proyectil.GetComponent<ProyectilHacha>();
        if (scriptHacha != null)
        {
            // Daño base * (1 + 20% por cada nivel extra)
            float multiplicador = 1f + ((nivel - 1) * 0.2f);
            scriptHacha.dano = Mathf.RoundToInt(scriptHacha.dano * multiplicador);
        }
        
        // También aplicamos lógica para el Slash si fuera este lanzador (por si acaso)
        SlashAtaque scriptSlash = proyectil.GetComponent<SlashAtaque>();
        if (scriptSlash != null)
        {
            scriptSlash.nivel = nivel;
            // El daño del slash también sube
            float multiplicador = 1f + ((nivel - 1) * 0.2f);
            scriptSlash.dano = Mathf.RoundToInt(scriptSlash.dano * multiplicador);
        }

        // Lógica para Rayo Luz
        RayoLuz scriptRayo = proyectil.GetComponent<RayoLuz>();
        if (scriptRayo != null)
        {
            scriptRayo.nivel = nivel;
            // El daño del rayo también sube
            float multiplicador = 1f + ((nivel - 1) * 0.2f);
            scriptRayo.dano = Mathf.RoundToInt(scriptRayo.dano * multiplicador);

        }
        
        // LOG PARA VERIFICAR TASK 6
        Debug.Log($"[LanzadorArma] Disparo! Nivel: {nivel} | Cooldown: {tiempoDeAtaque * Mathf.Pow(0.9f, nivel - 1):F2}s");
    }
}
