using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    // Variable pública para decidir cuánta experiencia da este orbe.
    // PlayerLevel busca específicamente este nombre: "cantidadExp"
    public int cantidadExp = 10;

    // Función pública para destruir el orbe cuando el jugador lo toca.
    // PlayerLevel busca específicamente este nombre: "Recoger"
    public void Recoger()
    {
        // Aquí puedes añadir sonido o partículas antes de destruir si quieres.
        
        // Destruye el objeto del orbe de la escena
        Destroy(this.gameObject);
    }
}
