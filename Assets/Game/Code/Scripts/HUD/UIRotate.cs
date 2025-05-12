using UnityEngine;

public class UIRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f; // Velocidade de rota��o em graus por segundo

    void Start()
    {
        Cursor.visible = true;
    }
    void Update()
    {
        // Calcula o �ngulo de rota��o baseado no tempo e velocidade
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Aplica a rota��o no eixo Z
        transform.Rotate(0f, 0f, rotationAmount);
    }
}
