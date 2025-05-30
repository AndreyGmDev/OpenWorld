using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterMovement characterMovement;
    public CameraController cameraController;
    public Hotbar hotbar;
    public SaveGame saveGame;
    private InputActionsManager input;

    //private InputSystem_Actions inputActions;
    Vector2 moveInput = Vector2.zero;
    private void Awake()
    {
        // Inicializando o NewInputSystem.
        input = InputActionsManager.Instance;

        // Carregar as informações do SaveGame.
        saveGame = SaveGame.Instance;
    }

    private void Start()
    {
        Load();
    }

    private void Update()
    {
        // Inputs player.
        Inputs();

        // Passar informações para o SaveGame.
        PassToSaveGame();
    }

    private void Inputs()
    {
        // Passar informações para CharacterMovement.

        moveInput = input.inputActions.Game.Move.ReadValue<Vector2>();
        bool wantsToJump = input.inputActions.Game.Jump.WasPressedThisFrame();
        bool wantsToCrouch = input.inputActions.Game.Crouch.IsPressed();

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

        Vector2 look = input.inputActions.Game.Look.ReadValue<Vector2>();

        cameraController.IncrementLookRotation(new Vector2(look.y, look.x));

        AllowIncrementZoomCamera();
    }

    private void AllowIncrementZoomCamera()
    {
        bool mouseRightClick = input.inputActions.Game.Aiming.IsPressed();
        bool mouseLeftClick = input.inputActions.Game.Shoot.IsPressed();

        int slot = Mathf.RoundToInt(hotbar.saveSlot - 1);

        if (slot >= 0)
        {
            slot = Mathf.Clamp(slot, 0, hotbar.itens.Length);
        }
        else
        {
            return;
        }

        if (hotbar.itens[slot].TryGetComponent<ItemConditions>(out var itemCondition))
        {
            bool rightClick = false;
            bool leftClick = false;

            if (itemCondition.CheckRightClickAim())
            {
                rightClick = mouseRightClick;
            }
            

            if (itemCondition.CheckLeftClickAim())
            {
                leftClick = mouseLeftClick;
            }

            cameraController.IncrementZoomCamera(rightClick || leftClick);
        }
    }

    // Passa as informações para o SaveGame.
    private void PassToSaveGame()
    {
        // Passar informações para o SaveGame.

        saveGame.SavePlayerData(new SaveGameInfos
        {
            PlayerPosition = characterMovement.transform.position,
            PlayerRotation = characterMovement.transform.rotation,
            CameraControllerRotation = cameraController.targetLook,
        });

        saveGame.SaveHotbarData(new SaveGameInfos
        {
            Slot = hotbar.saveSlot,
            Itens = hotbar.itens,
        });
    }

    // Carrega as informações do SaveGame.
    private void Load()
    {
        if (saveGame != null)
        {
            SaveGameInfos save = saveGame.LoadData();

            //characterMovement.motor.SetPosition(save.PlayerPosition);
            characterMovement.motor.RotateCharacter(save.PlayerRotation);
            cameraController.targetLook = save.CameraControllerRotation;
        }
    }
}
