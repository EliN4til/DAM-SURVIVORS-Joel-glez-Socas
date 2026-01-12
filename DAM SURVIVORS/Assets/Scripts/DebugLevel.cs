using UnityEngine;
using UnityEngine.InputSystem;

public class DebugLevel : MonoBehaviour
{
    // --- Variables ---
    public GameObject prefabHacha;
    public GameObject prefabVarita;

    private Controles controles;

    // --- Funciones de Unity ---
    void Awake()
    {
        controles = new Controles();
    }

    void OnEnable()
    {
        controles.Debug.Enable();

        controles.Debug.SubirNivel.performed += ctx => SubirNivelTodasLasArmas();
        controles.Debug.AnadirArma.performed += ctx => AnadirNuevaArma();
    }

    void OnDisable()
    {
        controles.Debug.SubirNivel.performed -= ctx => SubirNivelTodasLasArmas();
        controles.Debug.AnadirArma.performed -= ctx => AnadirNuevaArma();
        controles.Debug.Disable();
    }

    // --- Funciones Propias ---

    void SubirNivelTodasLasArmas()
    {
        Debug.Log("--- DEBUG: SUBIENDO NIVEL ARMAS ---");

        // 1. Lanzadores de Armas
        LanzadorArma[] lanzadores = FindObjectsByType<LanzadorArma>(FindObjectsSortMode.None);
        foreach (var lanzador in lanzadores)
        {
            lanzador.nivel++;
        }

        // 2. Varitas
        LanzadorVarita[] varitas = FindObjectsByType<LanzadorVarita>(FindObjectsSortMode.None);
        foreach (var varita in varitas)
        {
            varita.nivel++;
        }

        // 3. Escudos
        EscudoOrbital[] escudos = FindObjectsByType<EscudoOrbital>(FindObjectsSortMode.None);
        foreach (var escudo in escudos)
        {
            escudo.nivel++;
        }

        // 4. Rayos
        RayoLuz[] rayos = FindObjectsByType<RayoLuz>(FindObjectsSortMode.None);
        foreach (var rayo in rayos)
        {
            rayo.nivel++;
        }
    }

    void AnadirNuevaArma()
    {
        Debug.Log("--- DEBUG: SUBIR NIVEL JUGADOR ---");
        
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null) return;

        PlayerLevel playerLevel = jugador.GetComponent<PlayerLevel>();
        if (playerLevel != null)
        {
            // Dar suficiente experiencia para subir de nivel
            int expNecesaria = playerLevel.experienciaParaSiguienteNivel - playerLevel.experienciaActual + 1;
            playerLevel.GanarExperiencia(expNecesaria);
        }
    }
}
