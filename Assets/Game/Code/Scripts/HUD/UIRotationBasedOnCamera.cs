using UnityEngine;
using Unity.Cinemachine;

public class UIRotationBasedOnHorizontalCameraMovement : MonoBehaviour
{
    [SerializeField] private CinemachineCamera virtualCamera; // A Cinemachine Virtual Camera
    [SerializeField] private RectTransform uiElement; // O elemento de UI que será rotacionado

    private float previousCameraYRotation; // Armazena a rotação anterior da câmera
    private float currentCameraYRotation; // Armazena a rotação atual da câmera

    private void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogWarning("Virtual Camera não configurada!");
            return;
        }

        // Inicializa a rotação inicial
        previousCameraYRotation = virtualCamera.transform.eulerAngles.y;
    }

    private void Update()
    {
        if (virtualCamera == null || uiElement == null)
        {
            Debug.LogWarning("Virtual Camera ou UI Element não configurado!");
            return;
        }

        // Atualiza a rotação atual da câmera
        currentCameraYRotation = virtualCamera.transform.eulerAngles.y;

        // Calcula a diferença de rotação (movimento horizontal)
        float deltaY = Mathf.DeltaAngle(previousCameraYRotation, currentCameraYRotation);

        // Aplica a diferença de rotação ao elemento de UI no eixo Z
        uiElement.Rotate(0, 0, deltaY);

        // Atualiza a rotação anterior para a próxima iteração
        previousCameraYRotation = currentCameraYRotation;
    }
}
