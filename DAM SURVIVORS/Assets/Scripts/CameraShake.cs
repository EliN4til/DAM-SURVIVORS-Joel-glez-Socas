using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instancia;

    private void Awake()
    {
        Instancia = this;
    }

    public void Sacudir(float duracion, float magnitud)
    {
        StartCoroutine(CorrutinaSacudida(duracion, magnitud));
    }

    private IEnumerator CorrutinaSacudida(float duracion, float magnitud)
    {
        Vector3 posOriginal = transform.localPosition;
        float tiempoTranscurrido = 0.0f;

        while (tiempoTranscurrido < duracion)
        {
            float x = Random.Range(-1f, 1f) * magnitud;
            float y = Random.Range(-1f, 1f) * magnitud;

            transform.localPosition = new Vector3(posOriginal.x + x, posOriginal.y + y, posOriginal.z);

            tiempoTranscurrido += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = posOriginal;
    }
}
