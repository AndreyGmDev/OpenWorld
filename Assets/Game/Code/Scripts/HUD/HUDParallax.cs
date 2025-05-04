using UnityEngine;

public class HUDParallax : MonoBehaviour
{
    public Camera mainCamera; // A c�mera principal do jogo
    public float parallaxIntensity = 0.1f; // Intensidade do movimento (ajuste para valores menores)
    public float smoothSpeed = 5f; // Velocidade para retornar � posi��o original

    private Vector3 initialPosition;
    private RectTransform rectTransform;

    void Start()
    {
        // Pegue o RectTransform do Canvas
        rectTransform = GetComponent<RectTransform>();

        // Salvar a posi��o inicial da HUD
        initialPosition = rectTransform.localPosition;

        // Se a c�mera n�o foi configurada, buscar automaticamente
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera == null) return;

        // Calcula o deslocamento baseado no movimento da c�mera
        Vector3 cameraDelta = mainCamera.transform.position - initialPosition;
        Vector3 targetPosition = initialPosition + (cameraDelta * parallaxIntensity);

        // Lerp para suavizar o movimento
        rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, targetPosition, Time.deltaTime * smoothSpeed);

        // Gradualmente retornar � posi��o original
        rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, initialPosition, Time.deltaTime * smoothSpeed);
    }
}
