using UnityEngine;

public class JoinExitBelfry : MonoBehaviour
{
    private enum Levels { Island, Belfry1, Belfry2, Belfry3, Belfry4 }
    [SerializeField] Levels levels;

    private PlayerInteraction playerInteraction;

    private void Start()
    {
        playerInteraction = FindFirstObjectByType<PlayerInteraction>();
    }
    private void Update()
    {
        if (playerInteraction.NextInteraction() == gameObject)
        {
            InputActionsManager input = InputActionsManager.Instance;
            if (input.inputActions.Game.Interaction.WasPressedThisFrame())
            {
                string levelName = LevelName(); // Pega o nome da cena.
                LoadingManager.Instance.LoadAsyncScene(levelName); // Carrega a cena.
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
                levelName = "Belfry2";
                break;
            case Levels.Belfry4:
                levelName = "Belfry2";
                break;
        }

        return levelName;
    }
}
