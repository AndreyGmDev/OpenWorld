using Unity.Cinemachine;
using UnityEngine;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CinemachineThirdPersonFollow cinemachineTPFollow; // Referência da  de movimentação do Player.
    [SerializeField] CharacterMovement characterMovement; // Referência do  Script de movimentação do Player.
    [SerializeField] Transform cameraTarget; // Target em que a câmera está mirando.

    [Header("Actions")]
    [SerializeField] float normalTargetHeight = 1; // Altura do target da câmera (onde a câmera está mirando).
    [SerializeField] float crouchTargetHeight = 1; // Altura do target da câmera (onde a câmera está mirando).

    [Header("RotationCamera")]
    [SerializeField] Vector2 XRotationRange = new Vector2(-50, 50); // Limite de rotação no eixo x (vertical).
    [SerializeField, Range(0.1f,5)] float mouseSensitivity;

    [Header("Normal")]
    [SerializeField] float normalZoom = 5; // Distância da câmera até o Player, quando está normal.
    [SerializeField] Vector3 normalOffset = new Vector3(0.5f, 0, 0); // Offset da câmera quando está normal.
    public Orientation normalOrientation;// Pega o estado de normal do Player.

    [Header("Aiming")]
    [SerializeField] Vector3 aimingOffset = new Vector3(0.5f, 0, 0); // Offset da câmera quando está mirando.
    [SerializeField] float aimingZoom = 2; // Distância da câmera até o Player, quando está mirando.
    [HideInInspector] public bool isAiming; // Confere se o player está mirando.
    public Orientation aimingOrientation; // Pega o estado de mirar do Player.

    private Vector2 targetLook;
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
        targetLook += lookAt * mouseSensitivity;
        targetLook.x = Mathf.Clamp(targetLook.x, XRotationRange.x, XRotationRange.y);
    }

    public void IncrementZoomCamera(bool mouseLeftClick)
    {
        if (mouseLeftClick)
        {
            cinemachineTPFollow.CameraDistance = aimingZoom;
            cinemachineTPFollow.ShoulderOffset = aimingOffset;
            isAiming = true;
        }
        else
        {
            cinemachineTPFollow.CameraDistance = normalZoom;
            cinemachineTPFollow.ShoulderOffset = normalOffset;
            isAiming = false;
        }
        
    }
}
