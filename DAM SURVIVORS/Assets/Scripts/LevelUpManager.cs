using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instancia { get; private set; }
    public bool IsMenuOpen { get; private set; } = false;
    
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
    [SerializeField] private GameObject prefabBumeran;
    [SerializeField] private GameObject prefabSlash; 
    
    // Clase interna para gestionar opciones
    private class OpcionArma
    {
        public string nombre;
        public bool esNuevaArma;
        public System.Action accion;
    }
    
    private OpcionArma opcion1Actual;
    private OpcionArma opcion2Actual;

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }
    
    private void Start()
    {
        if (panelLevelUp != null) panelLevelUp.SetActive(false);
        
        if (boton1 != null) boton1.onClick.AddListener(() => ElegirOpcion1());
        if (boton2 != null) boton2.onClick.AddListener(() => ElegirOpcion2());
    }
    
    public void MostrarPanelLevelUp(int nivelActual)
    {
        if (panelLevelUp == null) return;
        
        // Pausar juego
        Time.timeScale = 0f;
        panelLevelUp.SetActive(true);
        IsMenuOpen = true;
        
        if (textoNivel != null) textoNivel.text = "NIVEL " + nivelActual;
        
        GenerarOpcionesAleatorias();
        ActualizarBotones();
    }

    private void ActualizarBotones()
    {
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
        
        // 1. Buscar armas actuales para ofrecer mejoras
        LanzadorArma[] lanzadores = jugador.GetComponentsInChildren<LanzadorArma>();
        foreach (var lanzador in lanzadores)
        {
            var lanzadorRef = lanzador;
            string nombreArma = ObtenerNombreArma(lanzador);
            
            opcionesDisponibles.Add(new OpcionArma
            {
                nombre = nombreArma,
                esNuevaArma = false,
                accion = () => { 
                    lanzadorRef.nivel++; 
                    Debug.Log($"{nombreArma} mejorada a nivel {lanzadorRef.nivel}"); 
                }
            });
        }
        
        // Varita Mágica (Script específico)
        LanzadorVarita varita = jugador.GetComponentInChildren<LanzadorVarita>();
        if (varita != null)
        {
            opcionesDisponibles.Add(new OpcionArma
            {
                nombre = "Varita Magica",
                esNuevaArma = false,
                accion = () => { varita.nivel++; }
            });
        }
        
        // Escudo Orbital (Script específico)
        EscudoOrbital escudo = jugador.GetComponent<EscudoOrbital>();
        if (escudo != null)
        {
            opcionesDisponibles.Add(new OpcionArma
            {
                nombre = "Escudo Orbital",
                esNuevaArma = false,
                accion = () => { escudo.nivel++; }
            });
        }
        
        // 2. Buscar armas nuevas para ofrecer
        AgregarOpcionNuevaArma(opcionesDisponibles, jugador, varita == null, prefabVarita, "Varita Magica", () => {
            CrearLanzadorVarita(jugador);
        });

        AgregarOpcionNuevaArma(opcionesDisponibles, jugador, escudo == null, prefabEscudo, "Escudo Orbital", () => {
            EscudoOrbital nuevoEscudo = jugador.AddComponent<EscudoOrbital>();
            nuevoEscudo.orbePrefab = prefabEscudo;
        });

        // Verificar si tiene Rayo, Hacha, Bumerán, Slash
        bool tieneRayo = TieneArma(lanzadores, prefabRayo);
        bool tieneHacha = TieneArma(lanzadores, prefabHacha);
        bool tieneBumeran = TieneArma(lanzadores, prefabBumeran);
        bool tieneSlash = TieneArma(lanzadores, prefabSlash);

        AgregarOpcionNuevaArma(opcionesDisponibles, jugador, !tieneRayo, prefabRayo, "Rayo de Luz", () => CrearLanzadorGenerico(jugador, "Lanzador_RayoLuz", prefabRayo, 5.0f));
        AgregarOpcionNuevaArma(opcionesDisponibles, jugador, !tieneHacha, prefabHacha, "Hacha Arrojadiza", () => CrearLanzadorGenerico(jugador, "Lanzador_Hacha", prefabHacha, 1.5f));
        AgregarOpcionNuevaArma(opcionesDisponibles, jugador, !tieneBumeran, prefabBumeran, "Bumerán", () => CrearLanzadorGenerico(jugador, "Lanzador_Bumeran", prefabBumeran, 2.0f));
        AgregarOpcionNuevaArma(opcionesDisponibles, jugador, !tieneSlash, prefabSlash, "Corte Lateral", () => CrearLanzadorGenerico(jugador, "Lanzador_Slash", prefabSlash, 2.5f));
        
        // Seleccionar 2 opciones aleatorias
        if (opcionesDisponibles.Count > 0)
        {
            opcion1Actual = opcionesDisponibles[Random.Range(0, opcionesDisponibles.Count)];
            // Intentar que la segunda sea diferente si hay suficientes
            if (opcionesDisponibles.Count > 1)
            {
                opcionesDisponibles.Remove(opcion1Actual);
                opcion2Actual = opcionesDisponibles[Random.Range(0, opcionesDisponibles.Count)];
            }
            else
            {
                opcion2Actual = opcion1Actual;
            }
        }
    }

    // --- Helpers para limpiar el código ---

    private string ObtenerNombreArma(LanzadorArma lanzador)
    {
        if (lanzador.proyectilPrefab != null)
        {
            return lanzador.proyectilPrefab.name
                .Replace("Proyectil", "")
                .Replace("proyectil", "")
                .Replace("SlashHitbox", "Corte Lateral")
                .Replace("SlashWarrior", "Corte Lateral")
                .Replace("Boomerang", "Bumerán")
                .Replace("boomerang", "Bumerán")
                .Replace("Bumeran", "Bumerán")
                .Replace("bumeran", "Bumerán");
        }
        return "Arma Desconocida";
    }

    private bool TieneArma(LanzadorArma[] lanzadores, GameObject prefab)
    {
        if (prefab == null) return false;
        foreach (var l in lanzadores)
        {
            if (l.proyectilPrefab == prefab) return true;
        }
        return false;
    }

    private void AgregarOpcionNuevaArma(List<OpcionArma> lista, GameObject jugador, bool condicion, GameObject prefab, string nombre, System.Action onElegir)
    {
        if (condicion && prefab != null)
        {
            lista.Add(new OpcionArma
            {
                nombre = nombre,
                esNuevaArma = true,
                accion = onElegir
            });
        }
    }

    private void CrearLanzadorVarita(GameObject jugador)
    {
        GameObject go = new GameObject("Lanzador_Varita");
        go.transform.SetParent(jugador.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        
        LanzadorVarita l = go.AddComponent<LanzadorVarita>();
        l.proyectilPrefab = prefabVarita;
    }

    private void CrearLanzadorGenerico(GameObject jugador, string nombreGO, GameObject prefab, float cooldown)
    {
        GameObject go = new GameObject(nombreGO);
        go.transform.SetParent(jugador.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        
        LanzadorArma l = go.AddComponent<LanzadorArma>();
        l.proyectilPrefab = prefab;
        l.tiempoDeAtaque = cooldown;
    }
    
    private void ElegirOpcion1()
    {
        opcion1Actual?.accion?.Invoke();
        CerrarPanel();
    }
    
    private void ElegirOpcion2()
    {
        opcion2Actual?.accion?.Invoke();
        CerrarPanel();
    }
    
    private void CerrarPanel()
    {
        if (panelLevelUp != null)
        {
            panelLevelUp.SetActive(false);
            IsMenuOpen = false;
        }
        Time.timeScale = 1f;
    }
}
