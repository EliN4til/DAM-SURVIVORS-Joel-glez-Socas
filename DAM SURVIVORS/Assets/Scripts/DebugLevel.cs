using UnityEngine;
using UnityEngine.InputSystem;

public class DebugLevel : MonoBehaviour
{
    // --- Mis Variables ---
    public GameObject prefabHacha;
    public GameObject prefabVarita;

    // Referencia a la clase generada por el Input System
    private Controles controles;

    // --- Funciones de Unity ---
    void Awake()
    {
        // Inicializamos los controles
        controles = new Controles();
    }

    void OnEnable()
    {
        Debug.Log("DebugLevel: OnEnable llamado. Activando controles de Debug...");
        // Activamos el mapa de Debug
        controles.Debug.Enable();

        // Nos suscribimos a los eventos de pulsación
        controles.Debug.SubirNivel.performed += ctx => {
            Debug.Log("DebugLevel: Evento SubirNivel recibido!");
            SubirNivelTodasLasArmas();
        };
        controles.Debug.AnadirArma.performed += ctx => {
            Debug.Log("DebugLevel: Evento AnadirArma recibido!");
            AnadirNuevaArma();
        };
    }

    void OnDisable()
    {
        // Es buena práctica desuscribirse y desactivar
        controles.Debug.SubirNivel.performed -= ctx => SubirNivelTodasLasArmas();
        controles.Debug.AnadirArma.performed -= ctx => AnadirNuevaArma();
        controles.Debug.Disable();
    }

    void Update()
    {
        // Debugging Polling
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("HARDWARE CHECK: Tecla '1' presionada (Keyboard.current)");
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("HARDWARE CHECK: Tecla '2' presionada (Keyboard.current)");
        }
        
        if (controles.Debug.SubirNivel.WasPerformedThisFrame())
        {
             Debug.Log("INPUT SYSTEM CHECK: Acción SubirNivel activada (WasPerformedThisFrame)");
        }
    }

    // --- Mis Funciones ---

    void SubirNivelTodasLasArmas()
    {
        Debug.Log("--- COMANDO DEBUG: SUBIENDO NIVEL ---");

        // 1. Buscamos Lanzadores de Armas
        LanzadorArma[] lanzadores = FindObjectsByType<LanzadorArma>(FindObjectsSortMode.None);
        foreach (var lanzador in lanzadores)
        {
            lanzador.nivel++;
            Debug.Log($"Arma {lanzador.name} subida a Nivel {lanzador.nivel}");
        }

        // 2. Buscamos Lanzadores de Varitas
        LanzadorVarita[] varitas = FindObjectsByType<LanzadorVarita>(FindObjectsSortMode.None);
        foreach (var varita in varitas)
        {
            varita.nivel++;
            Debug.Log($"Varita {varita.name} subida a Nivel {varita.nivel}");
        }

        // 3. Buscamos Escudos Orbitales
        EscudoOrbital[] escudos = FindObjectsByType<EscudoOrbital>(FindObjectsSortMode.None);
        foreach (var escudo in escudos)
        {
            escudo.nivel++;
            Debug.Log($"Escudo {escudo.name} subido a Nivel {escudo.nivel}");
        }

        // 4. Buscamos Rayos de Luz
        RayoLuz[] rayos = FindObjectsByType<RayoLuz>(FindObjectsSortMode.None);
        foreach (var rayo in rayos)
        {
            rayo.nivel++;
            Debug.Log($"Rayo {rayo.name} subido a Nivel {rayo.nivel}");
        }
    }

    void AnadirNuevaArma()
    {
        Debug.Log("--- COMANDO DEBUG: SUBIR NIVEL DEL JUGADOR ---");
        
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null)
        {
            Debug.LogError("No se encontro al jugador!");
            return;
        }

        // Buscar el componente PlayerLevel
        PlayerLevel playerLevel = jugador.GetComponent<PlayerLevel>();
        if (playerLevel != null)
        {
            // Dar suficiente experiencia para subir de nivel
            int expNecesaria = playerLevel.experienciaParaSiguienteNivel - playerLevel.experienciaActual + 1;
            playerLevel.GanarExperiencia(expNecesaria);
            
            Debug.Log($"Experiencia anadida: {expNecesaria}. Nivel actual: {playerLevel.nivelActual}");
        }
        else
        {
            Debug.LogError("El jugador no tiene el componente PlayerLevel!");
        }
    }
}
