using UnityEngine;

public class JoinExitBelfry : MonoBehaviour
{
    private enum Levels { Island, Belfry1, Belfry2, Belfry3, Belfry4, Menu }
    [SerializeField] Levels levels;

    private enum Mode { Interaction, Automatic }
    [SerializeField] Mode mode;
    
    [Header("PlayerInfosNextScene")]
    [SerializeField, Tooltip("Se ativo, a unity gravará as informações do player em tempo real")] bool recordInformations;
    [SerializeField] Vector3 playerPosition;
    [SerializeField] Quaternion playerRotation;
    [SerializeField] Vector2 cameraRotation;

    private PlayerInteraction playerInteraction;
    private InputActionsManager input;

    private void Start()
    {
        // Prevenir que o jogo não inicie com isso ligado.
        recordInformations = false;

        playerInteraction = FindFirstObjectByType<PlayerInteraction>();
        input = InputActionsManager.Instance;

        if (mode == Mode.Automatic)
        {
            PassScene();
        }
    }
    private void Update()
    {
        // Editor.
        if (recordInformations)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            playerPosition = player.characterMovement.transform.position;
            playerRotation = player.characterMovement.transform.rotation;
            cameraRotation = player.cameraController.targetLook;
        }

        if (mode == Mode.Interaction)
        {
            // Interação.
            if (playerInteraction.NextInteraction() == gameObject)
            {
                if (input.inputActions.Game.Interaction.WasPressedThisFrame())
                {
                    PassScene();
                }
            }
        }
    }

    private string LevelName()
    {
        string levelName = "OpenWorld";

        switch (levels)
        {
            case Levels.Island:
                levelName = "OpenWorld";
                break;
            case Levels.Belfry1:
                levelName = "Belfry1";
                break;
            case Levels.Belfry2:
                levelName = "Belfry2";
                break;
            case Levels.Belfry3:
                levelName = "Belfry3";
                break;
            case Levels.Belfry4:
                levelName = "Belfry4";
                break;
            case Levels.Menu:
                levelName = "MainMenu";
                break;
        }

        return levelName;
    }

    private void PassScene()
    {
        float seconds = SaveGame.Instance.saveBetweenScenes.saveGameInfos.Seconds; // Garante que o tempo será o mesmo de quando entrou na cena.
        if (FindFirstObjectByType<DaylightCycle>())
        {
            seconds = FindFirstObjectByType<DaylightCycle>().seconds; // Passa o tempo atual da cena para a próxima cena.
        }

        SaveGame saveGame = SaveGame.Instance;

        // Altera as informações do save na hora de passar de cena. O que não estiver alterando aqui mantem como estava antes de passar de cena.
        saveGame.saveBetweenScenes.BetweenScenesPlayerInfos(new SaveGameInfos()
        {
            PlayerPosition = playerPosition,
            PlayerRotation = playerRotation,
            CameraControllerRotation = cameraRotation,
            Seconds = seconds,
        });

        string levelName = LevelName(); // Pega o nome da cena.
        LoadingManager.Instance.LoadAsyncScene(levelName); // Carrega a cena.
    }
}