using System.IO;
using UnityEngine;

public struct SaveGameInfos
{
    // Player infos.
    public PlayerController PlayerController;
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;
    public Vector2 CameraControllerRotation;
    public float SlotPlayer;
}

public class SaveGame : MonoBehaviour
{
    // Nomes dos arquivos que ser�o salvos as informa��es
    const string SAVE = "/player_state.txt";

    // SaveGame do player em arquivo Json.
    public class PlayerData
    {
        public PlayerController playerController; // Refer�ncia do script onde ser�o descarregadas as informa��es do PlayerData.
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public Vector2 cameraControllerRotation;
        public float slotPlayer;
    }

    // Inicia o Singleton do SaveSame.
    private static SaveGame saveGame;

    public static SaveGame instance
    {
        get
        {
            // Confere se a inst�ncia j� foi criada
            if (saveGame == null)
            {
                // Procura o SaveGame na cena
                saveGame = FindFirstObjectByType<SaveGame>();

                // Se n�o encontrar, cria uma nova GameObject com esse script
                if (saveGame == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    saveGame = obj.AddComponent<SaveGame>();
                }
            }
            return saveGame;
        }
    }
    // Finaliza��o do Singleton.

    private void Awake()
    {
        // Permite somente uma inst�ncia de SaveGame na c�na.
        if (saveGame == null)
        {
            saveGame = this;
        }
        else if (saveGame != this)
        {
            Destroy(gameObject);
        }

        // Pega os saves criados anteriormente.
        LoadPlayerData();
    }
    private void LoadPlayerData()
    {
        if (File.Exists(Application.dataPath + SAVE))
        {
            string save = File.ReadAllText(Application.dataPath + SAVE);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(save);
          
            if (playerData.playerController != null)
            {
                playerData.playerController.characterMovement.motor.SetPosition(playerData.playerPosition);
                playerData.playerController.characterMovement.motor.RotateCharacter(playerData.playerRotation);
                playerData.playerController.cameraController.targetLook = playerData.cameraControllerRotation;
                playerData.playerController.hotbar.saveSlot = playerData.slotPlayer;
            }
        }
    }

    // Recebe o SavePlayerTransform do PlayerController.
    public void SavePlayerData(in SaveGameInfos infos)
    {
        PlayerData playerData = new PlayerData();

        playerData.playerController = infos.PlayerController; // Grava a refer�ncia do script que passou as informa��es pro SaveGame.
        playerData.playerPosition = infos.PlayerPosition;
        playerData.playerRotation = infos.PlayerRotation;
        playerData.cameraControllerRotation = infos.CameraControllerRotation;
        playerData.slotPlayer = infos.SlotPlayer;

        string jsonPlayerData = JsonUtility.ToJson(playerData);
        File.WriteAllText(Application.dataPath + SAVE, jsonPlayerData);
    }
}
