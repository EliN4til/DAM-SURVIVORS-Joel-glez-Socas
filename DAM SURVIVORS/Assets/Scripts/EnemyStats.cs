using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Stats/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("Configuración de Estadísticas")]
    [Tooltip("Vida máxima del enemigo")]
    public int vida;

    [Tooltip("Daño que inflige al jugador")]
    public int dano;

    [Tooltip("Defensa contra ataques")]
    public int defensa;

    [Tooltip("Velocidad de movimiento")]
    public float velocidad;
}
