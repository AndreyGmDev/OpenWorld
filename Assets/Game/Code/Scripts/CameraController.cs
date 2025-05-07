using Unity.Cinemachine;
using UnityEngine;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CinemachineThirdPersonFollow playerFollowCamera; // Referência da  de movimentação do Player.
    [SerializeField] CinemachineThirdPersonFollow playerAimCamera; // Referência da  de movimentação da mira.
    [SerializeField] CharacterMovement characterMovement; // Referência do  Script de movimentação do Player.
    [SerializeField] Transform cameraTarget; // Target em que a câmera está mirando.

    [Header("Actions")]
    [SerializeField] float normalTargetHeight = 1; // Altura do target da câmera (onde a câmera está mirando).
    [SerializeField] float crouchTargetHeight = 1; // Altura do target da câmera (onde a câmera está mirando).

    [Header("RotationCamera")]
    [SerializeField] Vector2 XRotationRange = new Vector2(-50, 50); // Limite de rotação no eixo x (vertical).
    [Range(0.1f,5)] static float normalSensitivity;
    [Range(0.1f,5)] static float aimSensitivity;
    private float sensitivity = 1; 

    [Header("Orientation")]
    public Orientation normalOrientation;// Pega o estado de normal do Player.
    public Orientation aimingOrientation; // Pega o estado de mirar do Player.
    [HideInInspector] public bool isAiming; // Confere se o player está mirando.

    public Vector2 targetLook;
    private float targetHeight = 1;
    public Quaternion lookRotation => cameraTarget.rotation;
    public enum Orientation {towardsCamera, towardMovement}; // Enum para orientação do Player.

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        IncrementZoomCamera(false);
    }

    private void LateUpdate()
    {
        if (characterMovement == null) return;

        if (characterMovement.isCrouching)
        {
            targetHeight = crouchTargetHeight;
        }
        else
        {
            targetHeight = normalTargetHeight;
        }

        cameraTarget.position = characterMovement.transform.position + Vector3.up * targetHeight;
        cameraTarget.rotation = Quaternion.Euler(targetLook.x, targetLook.y, 0);
    }

    public void IncrementLookRotation(Vector2 lookAt)
    {
        targetLook += lookAt * sensitivity;
        targetLook.x = Mathf.Clamp(targetLook.x, XRotationRange.x, XRotationRange.y);
    }

    public void IncrementZoomCamera(bool mouseLeftClick)
    {
        if (mouseLeftClick)
        {
            sensitivity = aimSensitivity;
            isAiming = true;
            playerAimCamera.gameObject.SetActive(true);
        }
        else
        {
            sensitivity = normalSensitivity;
            isAiming = false;
            playerAimCamera.gameObject.SetActive(false);
        }
        
    }

    public static void SetNormalSensitivity(float value)
    {
        normalSensitivity = value;
    }

    public static void SetAimSensitivity(float value)
    {
        aimSensitivity = value;
    }
}
