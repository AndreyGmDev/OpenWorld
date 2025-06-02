using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Hotbar hotbar;
    public CharacterMovement characterMovement;

    [SerializeField] CameraController cameraController;
    
    private InputActionsManager input;
    private SaveGame saveGame;

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
        // Confere se o botão esquerdo e direito do mouse são pressionados.
        bool mouseRightClick = input.inputActions.Game.Aiming.IsPressed();
        bool mouseLeftClick = input.inputActions.Game.Shoot.IsPressed();

        // Pega o item que o player está segurando no momento.
        int slot = Mathf.RoundToInt(hotbar.saveSlot - 1);

        /*if (slot >= 0)
        {
            slot = Mathf.Clamp(slot, 0, hotbar.itens.Length);
        }*/
        if (slot < 0)
        {
            // Se não houver nenhum item ativado, então o player não estará mirando.
            cameraController.IncrementZoomCamera(false);
            return;
        }

        if (hotbar.itens[slot].TryGetComponent<ItemConditions>(out var itemCondition))
        {
            // Por padrão o item não pode mirar com nenhum dos botões do mouse.
            bool rightClick = false;
            bool leftClick = false;

            // Confere se o item ativado permite mirar com o botão direito.
            if (itemCondition.CheckRightClickAim())
            {
                rightClick = mouseRightClick;
            }
            
            // Confere se o item ativado permite mirar com o botão esquerdo.
            if (itemCondition.CheckLeftClickAim())
            {
                leftClick = mouseLeftClick;
            }

            // O player estará mirando se estiver segunrando pelo menos um dos botões do mouse (direito ou esquerdo).
            cameraController.IncrementZoomCamera(rightClick || leftClick);
        }
        else
        {
            // Se o item que o player usando não tiver esse script, então o player não estará mirando.
            cameraController.IncrementZoomCamera(false);
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
