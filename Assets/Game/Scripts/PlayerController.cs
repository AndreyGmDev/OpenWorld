using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerController : MonoBehaviour
{
    public CharacterMovement characterMovement;
    public CameraController cameraController;
    public Hotbar hotbar;
    private SaveGame saveGame;
    private bool mouseLeftClickInThisFrame;
    private InputSystem_Actions inputActions;
    Vector2 moveInput = Vector2.zero;
    private void Awake()
    {
        // Inicializando o NewInputSystem.
        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        // Carregar as informações do SaveGame.
        saveGame = SaveGame.instance;
    }

    private void Update()
    {
        // Passar informações para CharacterMovement.

        moveInput = inputActions.Game.Move.ReadValue<Vector2>();
        bool wantsToJump = inputActions.Game.Jump.WasPressedThisFrame();
        bool wantsToCrouch = inputActions.Game.Crouch.IsPressed();

        characterMovement.SetInput(new CharacterMovementInput()
        {
            MoveInput = moveInput,
            WantsToJump = wantsToJump,
            WantsToCrouch = wantsToCrouch,

            LookRotation = cameraController.lookRotation,
            IsAiming = cameraController.isAiming,
            NormalOrientation = cameraController.normalOrientation,
            AimingOrientation = cameraController.aimingOrientation
        });

        // Passar informações para câmera.

        Vector2 look = inputActions.Game.Look.ReadValue<Vector2>();
        
        cameraController.IncrementLookRotation(new Vector2(look.y,look.x));

        AllowIncrementZoomCamera();

        // Passar informações para o SaveGame.

        saveGame.SavePlayerData(new SaveGameInfos
        {
            PlayerController = this,
            PlayerPosition = characterMovement.transform.position,
            PlayerRotation = characterMovement.transform.rotation,
            CameraControllerRotation = cameraController.targetLook,
            //SlotPlayer = hotbar.saveSlot
        });

        saveGame.SaveHotbarData(new SaveGameInfos
        {
            Hotbar = hotbar,
            Slot = hotbar.saveSlot,
            Itens = hotbar.itens,
        });
    }

    private void AllowIncrementZoomCamera()
    {
        bool mouseLeftClick = inputActions.Game.Aiming.IsPressed();
        int slot = Convert.ToInt32(hotbar.saveSlot - 1);
        slot = Mathf.Clamp(slot, 0, hotbar.itens.Length);

        if (hotbar.itens[slot].CompareTag("CanAim"))
        {
            if (mouseLeftClickInThisFrame != mouseLeftClick)
            {
                cameraController.IncrementZoomCamera(mouseLeftClick);
            }
            mouseLeftClickInThisFrame = mouseLeftClick;
        }
        else
        {
            if (mouseLeftClickInThisFrame != mouseLeftClick)
            {
                cameraController.IncrementZoomCamera(false);
            }
            mouseLeftClickInThisFrame = mouseLeftClick;
        }
    }
}
