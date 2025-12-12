using UnityEngine;

public class KeepPlayerSpawnPosition : MonoBehaviour
{
    // --- Variables ---
    [Tooltip("Referencia al GameObject del Jugador")]
    public GameObject jugador;
    
    private Transform transformPosicionSpawn;

    // --- Funciones de Unity ---
    void Start()
    {
        // Buscamos al jugador si no ha sido asignado manualmente
        if (jugador == null)
        {
            jugador = GameObject.FindGameObjectWithTag("Player");
        }

        if (jugador != null)
        {
            // Asumimos que el punto de spawn es el primer hijo del jugador
            // (Esto es útil si el jugador tiene un objeto vacío que marca dónde deben aparecer cosas)
            if (jugador.transform.childCount > 0)
            {
                transformPosicionSpawn = jugador.transform.GetChild(0);
            }
            else
            {
                Debug.LogWarning("El jugador no tiene hijos. Usando su propia transformación como punto de spawn.");
                transformPosicionSpawn = jugador.transform;
            }
        }
        else
        {
            Debug.LogError("No se encontró al Jugador (Tag 'Player'). Este script necesita al jugador para funcionar.");
        }
    }

    void Update()
    {
        // Si no tenemos las referencias necesarias, no hacemos nada
        if (jugador == null || transformPosicionSpawn == null) return;

        // Mantenemos este objeto en la posición del spawn del jugador
        transform.position = transformPosicionSpawn.position;
        
        // Copiamos también la rotación del jugador
        transform.rotation = jugador.transform.rotation;
    }
}
