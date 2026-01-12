using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [Header("Estado del Jugador")]
    public int nivelActual = 1;
    public int experienciaActual = 0;
    public int experienciaParaSiguienteNivel = 100;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OrbeEXP"))
        {
            ExpOrb orbeScript = other.GetComponent<ExpOrb>();

            if (orbeScript != null)
            {
                GanarExperiencia(orbeScript.cantidadExp);
                orbeScript.Recoger();
            }
        }
    }

    public void GanarExperiencia(int cantidad)
    {
        experienciaActual += cantidad;

        if (experienciaActual >= experienciaParaSiguienteNivel)
        {
            SubirNivel();
        }
    }

    private void SubirNivel()
    {
        nivelActual++;
        experienciaActual -= experienciaParaSiguienteNivel;

        // Aumentar dificultad (20% más necesario para el siguiente)
        experienciaParaSiguienteNivel = Mathf.RoundToInt(experienciaParaSiguienteNivel * 1.2f);

        Debug.Log("¡SUBIDA DE NIVEL! Nivel actual: " + nivelActual);
        
        if (LevelUpManager.Instancia != null)
        {
            LevelUpManager.Instancia.MostrarPanelLevelUp(nivelActual);
        }
    }
}