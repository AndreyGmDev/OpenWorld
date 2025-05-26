using UnityEngine;

public class UISwing : MonoBehaviour
{
    [Header("Swing Settings")]
    [Tooltip("Velocidade do movimento para cima e para baixo.")]
    public float speed = 1f;

    [Tooltip("Amplitude do movimento (distância máxima para cima e para baixo).")]
    public float amplitude = 10f;

    private Vector3 startPosition;

    private void Start()
    {
        // Armazena a posição inicial do elemento
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        // Calcula o deslocamento vertical com base no tempo e na velocidade
        float offset = Mathf.Sin(Time.time * speed) * amplitude;

        // Aplica o deslocamento à posição inicial
        transform.localPosition = startPosition + new Vector3(0, offset, 0);
    }
}
