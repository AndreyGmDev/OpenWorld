using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterMovement characterMovement;
    [SerializeField] CameraController cameraController;
    private bool mouseLeftClickInThisFrame;
    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool wantsToJump = Input.GetKeyDown(KeyCode.Space);
        bool wantsToCrouch = Input.GetKey(KeyCode.LeftControl);

        characterMovement.SetInput(new CharacterMovementInput()
        {
            MoveInput = new Vector2(h, v),
            WantsToJump = wantsToJump,
            WantsToCrouch = wantsToCrouch,

            LookRotation = cameraController.lookRotation,
            IsAiming = cameraController.isAiming,
            NormalOrientation = cameraController.normalOrientation,
            AimingOrientation = cameraController.aimingOrientation
        });

        float lookX = -Input.GetAxisRaw("Mouse Y");
        float lookY = Input.GetAxisRaw("Mouse X");
        
        cameraController.IncrementLookRotation(new Vector2(lookX, lookY));

        bool mouseLeftClick = Input.GetKey(KeyCode.Mouse1);
        if (mouseLeftClickInThisFrame != mouseLeftClick)
        {
            cameraController.IncrementZoomCamera(mouseLeftClick);
        }
        mouseLeftClickInThisFrame = mouseLeftClick;
    }
}
