using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Gestiona el panel de selección de armas al subir de nivel
/// Pausa el juego y permite elegir entre 2 opciones
/// </summary>
public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instancia { get; private set; }
    
    [Header("Referencias UI")]
    [SerializeField] private GameObject panelLevelUp;
    [SerializeField] private Button boton1;
    [SerializeField] private Button boton2;
    [SerializeField] private TextMeshProUGUI textoBoton1;
    [SerializeField] private TextMeshProUGUI textoBoton2;
    [SerializeField] private TextMeshProUGUI textoNivel;
    
    [Header("Prefabs de Armas")]
    [SerializeField] private GameObject prefabVarita;
    [SerializeField] private GameObject prefabHacha;
    [SerializeField] private GameObject prefabRayo; 
    [SerializeField] private GameObject prefabEscudo; 
    
    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Ocultar panel al inicio
        if (panelLevelUp != null)
        {
            panelLevelUp.SetActive(false);
        }
        
        // Configurar botones
        if (boton1 != null) boton1.onClick.AddListener(() => ElegirOpcion1());
        if (boton2 != null) boton2.onClick.AddListener(() => ElegirOpcion2());
    }
    
    // Clase para representar una opción
    private class OpcionArma
    {
        public string nombre;
        public bool esNuevaArma; // true = conseguir nueva, false = subir nivel
        public System.Action accion; // Qué hacer al elegirla
    }
    
    private OpcionArma opcion1Actual;
    private OpcionArma opcion2Actual;
    
    /// <summary>
    /// Muestra el panel de level up con 2 opciones ALEATORIAS
    /// </summary>
    public void MostrarPanelLevelUp(int nivelActual)
    {
        if (panelLevelUp == null)
        {
            Debug.LogError("Panel Level Up no asignado!");
            return;
        }
        
        // Pausar el juego
        Time.timeScale = 0f;
        
        // Mostrar panel
        panelLevelUp.SetActive(true);
        
        // Actualizar texto de nivel
        if (textoNivel != null)
        {
            textoNivel.text = "NIVEL " + nivelActual;
        }
        
        // Generar opciones aleatorias
        GenerarOpcionesAleatorias();
        
        // Actualizar textos de los botones
        if (textoBoton1 != null && opcion1Actual != null)
        {
            textoBoton1.text = opcion1Actual.esNuevaArma ? 
                $"CONSEGUIR\n{opcion1Actual.nombre}" : 
                $"MEJORAR\n{opcion1Actual.nombre}";
        }
        
        if (textoBoton2 != null && opcion2Actual != null)
        {
            textoBoton2.text = opcion2Actual.esNuevaArma ? 
                $"CONSEGUIR\n{opcion2Actual.nombre}" : 
                $"MEJORAR\n{opcion2Actual.nombre}";
        }
    }
    
    private void GenerarOpcionesAleatorias()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null) return;
        
        List<OpcionArma> opcionesDisponibles = new List<OpcionArma>();
        
        // --- ARMAS QUE TIENE (mejorar nivel) ---
        LanzadorArma[] lanzadores = jugador.GetComponentsInChildren<LanzadorArma>();
        foreach (var lanzador in lanzadores)
        {
            // IMPORTANTE: Crear variables locales para capturar en la lambda
            var lanzadorCapturado = lanzador;
            string nombreArma = "Arma"; // Nombre por defecto
            
            // Intentar obtener nombre del prefab del proyectil
            if (lanzador.proyectilPrefab != null)
            {
                nombreArma = lanzador.proyectilPrefab.name.Replace("Proyectil", "").Replace("proyectil", "");
            }
            else
            {
                // Si no hay prefab, usar nombre del GameObject
                nombreArma = lanzador.gameObject.name.Replace("Lanzador_", "").Replace("Player", "Hacha");
            }
            
            // Capturar el nombre en una variable local
            var nombreCapturado = nombreArma;
            
            opcionesDisponibles.Add(new OpcionArma
            {
                nombre = nombreCapturado,
                esNuevaArma = false,
                accion = () => { 
                    lanzadorCapturado.nivel++; 
                    Debug.Log($"{nombreCapturado} mejorada a nivel {lanzadorCapturado.nivel}"); 
                }
            });
        }
        
        LanzadorVarita varita = jugador.GetComponentInChildren<LanzadorVarita>();
        if (varita != null)
        {
            opcionesDisponibles.Add(new OpcionArma
            {
                nombre = "Varita Magica",
                esNuevaArma = false,
                accion = () => { varita.nivel++; Debug.Log($"Varita mejorada a nivel {varita.nivel}"); }
            });
        }
        
        EscudoOrbital escudo = jugador.GetComponent<EscudoOrbital>();
        if (escudo != null)
        {
            opcionesDisponibles.Add(new OpcionArma
            {
                nombre = "Escudo Orbital",
                esNuevaArma = false,
                accion = () => { escudo.nivel++; Debug.Log($"Escudo mejorado a nivel {escudo.nivel}"); }
            });
        }
        
        RayoLuz rayo = jugador.GetComponentInChildren<RayoLuz>();
        if (rayo != null)
        {
            opcionesDisponibles.Add(new OpcionArma
            {
                nombre = "Rayo de Luz",
                esNuevaArma = false,
                accion = () => { rayo.nivel++; Debug.Log($"Rayo mejorado a nivel {rayo.nivel}"); }
            });
        }
        
        // --- ARMAS QUE NO TIENE (conseguir nueva) ---
        if (varita == null && prefabVarita != null)
        {
            opcionesDisponibles.Add(new OpcionArma
            {
                nombre = "Varita Magica",
                esNuevaArma = true,
                accion = () => { Instantiate(prefabVarita, jugador.transform); Debug.Log("Varita Magica conseguida!"); }
            });
        }
        
        if (escudo == null)
        {
            opcionesDisponibles.Add(new OpcionArma
            {
                nombre = "Escudo Orbital",
                esNuevaArma = true,
                accion = () => { jugador.AddComponent<EscudoOrbital>(); Debug.Log("Escudo Orbital conseguido!"); }
            });
        }
        
        if (rayo == null && prefabRayo != null)
        {
            opcionesDisponibles.Add(new OpcionArma
            {
                nombre = "Rayo de Luz",
                esNuevaArma = true,
                accion = () => { 
                    // Crear un nuevo Lanzador para el Rayo
                    GameObject lanzadorGO = new GameObject("Lanzador_RayoLuz");
                    lanzadorGO.transform.SetParent(jugador.transform);
                    lanzadorGO.transform.localPosition = Vector3.zero;
                    
                    LanzadorArma lanzador = lanzadorGO.AddComponent<LanzadorArma>();
                    lanzador.proyectilPrefab = prefabRayo;
                    lanzador.tiempoDeAtaque = 5.0f; // Dispara cada 5s (el rayo dura 3s)
                    
                    Debug.Log("Rayo de Luz conseguido!"); 
                }
            });
        }
        
        // Elegir 2 opciones al azar
        if (opcionesDisponibles.Count >= 2)
        {
            int indice1 = Random.Range(0, opcionesDisponibles.Count);
            opcion1Actual = opcionesDisponibles[indice1];
            opcionesDisponibles.RemoveAt(indice1);
            
            int indice2 = Random.Range(0, opcionesDisponibles.Count);
            opcion2Actual = opcionesDisponibles[indice2];
        }
        else if (opcionesDisponibles.Count == 1)
        {
            opcion1Actual = opcionesDisponibles[0];
            opcion2Actual = opcionesDisponibles[0]; // Misma opción en ambos
        }
    }
    
    private void ElegirOpcion1()
    {
        if (opcion1Actual != null && opcion1Actual.accion != null)
        {
            opcion1Actual.accion();
        }
        CerrarPanel();
    }
    
    private void ElegirOpcion2()
    {
        if (opcion2Actual != null && opcion2Actual.accion != null)
        {
            opcion2Actual.accion();
        }
        CerrarPanel();
    }
    
    private void CerrarPanel()
    {
        // Ocultar panel
        if (panelLevelUp != null)
        {
            panelLevelUp.SetActive(false);
        }
        
        // Reanudar el juego
        Time.timeScale = 1f;
        
        Debug.Log("Panel cerrado. Juego reanudado.");
    }
}
