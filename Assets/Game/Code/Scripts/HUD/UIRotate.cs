using UnityEngine;

public class UIRotate : MonoBehaviour
{
    public RectTransform uiElement; // O elemento de UI que será rotacionado.
    public float rotationSpeed = 100f; // Velocidade da rotação (graus por segundo).
    public bool counterClockwise = true; // Define se a rotação será no sentido anti-horário.

    void Update()
    {
        // Calcula a direção da rotação com base na configuração.
        float direction = counterClockwise ? 1f : -1f;

        // Aplica a rotação ao elemento.
        uiElement.Rotate(0f, 0f, direction * rotationSpeed * Time.deltaTime);
    }
}
