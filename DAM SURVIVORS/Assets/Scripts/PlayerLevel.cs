using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    /////////////////////////////////////////////////////////////////// VARIABLES ///////////////////////////////////////////

    [Header("Estado del Jugador")]
    public int nivelActual = 1; // Antes currentLevel
    public int experienciaActual = 0; // Antes currentExperience
    public int experienciaParaSiguienteNivel = 100; // Antes experienceToNextLevel

    ///////////////////////////////////////////////////////////////// FUNCIONES UNITY /////////////////////////////////////////

    void Start()
    {
        // (Opcional) Inicializar UI aquí en el futuro
    }

    // Se ejecuta automáticamente cuando un objeto entra en nuestro Sphere Collider (Trigger)
    private void OnTriggerEnter(Collider other)
    {
        // 1. ¿Es lo que hemos tocado un Orbe de Experiencia?
        // Usamos el Tag que configuramos en la Parte 1.
        if (other.CompareTag("OrbeEXP"))
        {
            // 2. Intentamos leer el script del orbe para saber cuánta exp da
            ExpOrb orbeScript = other.GetComponent<ExpOrb>();

            if (orbeScript != null)
            {
                // 3. Sumamos la experiencia
                GanarExperiencia(orbeScript.cantidadExp);

                // 4. Destruimos el orbe (llamando a su función Recoger)
                orbeScript.Recoger();
            }
        }
    }

    /////////////////////////////////////////////////////////////// FUNCIONES PROPIAS /////////////////////////////////////////

    // Esta función será llamada cuando recojamos un orbe
    public void GanarExperiencia(int cantidad) // Antes GainExperience
    {
        experienciaActual += cantidad;
        Debug.Log("Experiencia ganada: " + cantidad + ". Total: " + experienciaActual + "/" + experienciaParaSiguienteNivel);

        // Comprobamos si hemos alcanzado el umbral para subir de nivel
        if (experienciaActual >= experienciaParaSiguienteNivel)
        {
            SubirNivel();
        }
    }

    private void SubirNivel() // Antes LevelUp
    {
        nivelActual++;
        
        // La experiencia sobrante se mantiene para el siguiente nivel
        experienciaActual -= experienciaParaSiguienteNivel;

        // Aumentamos la dificultad para el siguiente nivel (ej. necesitas un 20% más de exp)
        // Usamos Mathf.RoundToInt para mantener números enteros limpios.
        experienciaParaSiguienteNivel = Mathf.RoundToInt(experienciaParaSiguienteNivel * 1.2f);

        Debug.Log("<color=yellow>¡SUBIDA DE NIVEL! Nivel actual: " + nivelActual + "</color>");
        
        // Mostrar panel de selección de armas
        if (LevelUpManager.Instancia != null)
        {
            LevelUpManager.Instancia.MostrarPanelLevelUp(nivelActual);
        }
        else
        {
            Debug.LogWarning("LevelUpManager.Instancia es null! No se mostrara el panel de level up.");
        }
    }
}