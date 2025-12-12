using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "OleadaNueva", menuName="Oleadas")]
public class DataOleada : ScriptableObject
{
    [System.Serializable]
    public class SegmentoOleada
    {
        [Tooltip("Prefab del enemigo a generar")]
        public GameObject PrefabEnemigo;
        
        [Tooltip("Cantidad total de enemigos en este segmento")]
        public int CantidadEnemigos;
        
        [Tooltip("Tiempo entre cada enemigo (segundos)")]
        public float IntervaloGeneracion;
        
        [Tooltip("Retraso inicial antes de empezar a spawnear este segmento")]
        public float RetrasoInicial;
    }

    [Header("Configuración de la Oleada")]
    [Tooltip("Tiempo de espera antes de iniciar la siguiente oleada")]
    public float TiempoEntreOleadas;
    
    public List<SegmentoOleada> Segmentos;
}
