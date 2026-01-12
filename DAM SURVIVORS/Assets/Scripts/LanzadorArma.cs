using UnityEngine;
using System.Collections;

public class LanzadorArma : MonoBehaviour
{
    // --- Variables ---

    [Header("Configuración")]
    public GameObject proyectilPrefab;
    public float tiempoDeAtaque = 3f;
    public int nivel = 1;
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
            Debug.LogError("LanzadorArma: Falta asignar el prefab del proyectil.");
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
            if (this == null || gameObject == null) yield break;

            Vector3 origen = transform.position;
            if (spawnPoint != null) origen = spawnPoint.position;

            if (proyectilPrefab != null)
            {
                // Cooldown reducido por nivel (10% por nivel)
                float cooldownReal = tiempoDeAtaque * Mathf.Pow(0.9f, nivel - 1);

                // Ráfaga (Hacha): Lanzamos tantos proyectiles como nivel
                for (int i = 0; i < nivel; i++)
                {
                    LanzarUnProyectil();
                    
                    if (nivel > 1) yield return new WaitForSeconds(0.2f);
                }

                yield return new WaitForSeconds(cooldownReal);
            }
            else
            {
                yield return new WaitForSeconds(tiempoDeAtaque);
            }
        }
    }

    void LanzarUnProyectil()
    {
        Vector3 origen = transform.position;
        if (spawnPoint != null) origen = spawnPoint.position;

        GameObject proyectil = Instantiate(proyectilPrefab, origen, transform.rotation * proyectilPrefab.transform.rotation);

        // Escalado de Daño para Hacha
        ProyectilHacha scriptHacha = proyectil.GetComponent<ProyectilHacha>();
        if (scriptHacha != null)
        {
            float multiplicador = 1f + ((nivel - 1) * 0.2f);
            scriptHacha.dano = Mathf.RoundToInt(scriptHacha.dano * multiplicador);
        }
        
        // Lógica para Bumerán
        ProyectilBumeran scriptBumeran = proyectil.GetComponent<ProyectilBumeran>();
        if (scriptBumeran != null)
        {
            proyectil.transform.rotation = transform.rotation;
            scriptBumeran.nivel = nivel;
            
            float multiplicador = 1f + ((nivel - 1) * 0.2f);
            scriptBumeran.dano = Mathf.RoundToInt(scriptBumeran.dano * multiplicador);
        }

        // Lógica para Slash
        SlashAtaque scriptSlash = proyectil.GetComponent<SlashAtaque>();
        if (scriptSlash != null)
        {
            proyectil.transform.rotation = transform.rotation;
            scriptSlash.nivel = nivel;
            
            float multiplicador = 1f + ((nivel - 1) * 0.2f);
            scriptSlash.dano = Mathf.RoundToInt(scriptSlash.dano * multiplicador);
        }

        // Lógica para Rayo Luz
        RayoLuz scriptRayo = proyectil.GetComponent<RayoLuz>();
        if (scriptRayo != null)
        {
            scriptRayo.nivel = nivel;
            
            float multiplicador = 1f + ((nivel - 1) * 0.2f);
            scriptRayo.dano = Mathf.RoundToInt(scriptRayo.dano * multiplicador);
        }
    }
}
