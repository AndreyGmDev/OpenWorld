using UnityEngine;

public class UIRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f; // Velocidade de rotação em graus por segundo

    void Start()
    {
        Cursor.visible = true;
    }
    void Update()
    {
        // Calcula o ângulo de rotação baseado no tempo e velocidade
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Aplica a rotação no eixo Z
        transform.Rotate(0f, 0f, rotationAmount);
    }
}
