using System.IO;
using UnityEngine;

public struct SaveGameInfos
{
    // Player infos.
    public PlayerController PlayerController;
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;
    public Vector2 CameraControllerRotation;

    // Hotbar infos.
    public Hotbar Hotbar;
    public float Slot;
    public GameObject[] Itens;

    // DaylightCycle infos.
    public DaylightCycle DaylightCycle;
    public float Seconds;
}

public class SaveGame : MonoBehaviour
{
    // Nomes dos arquivos que serão salvos as informações
    const string FINALPATH  = "/Saves";
    const string SAVEPLAYER = "/player_state.txt";
    const string SAVEHOTBAR = "/hotbar_state.txt";
    const string SAVEDAY    = "/day_state.txt";

    [SerializeField, Tooltip("Delay between each game save")] float delaySaveGame = 30;

    // SaveGame do player em arquivo Json.
    public class PlayerData
    {
        public PlayerController playerController; // Referência do script onde serão descarregadas as informações do PlayerData.
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public Vector2 cameraControllerRotation;
    }

    // SaveGame da Hotbar em arquivo Json.
    public class HotbarData
    {
        public Hotbar hotbar;
        public float slot;
        public GameObject[] itens;
    }

    // SaveGame do DaylightCycle em arquivo Json.
    public class DaylightCycleData
    {
        public DaylightCycle daylightCycle;
        public float seconds;
    }

    // Inicia o Singleton do SaveSame.
    private static SaveGame saveGame;

    public static SaveGame instance
    {
        get
        {
            // Confere se a instância já foi criada
            if (saveGame == null)
            {
                // Procura o SaveGame na cena
                saveGame = FindFirstObjectByType<SaveGame>();

                // Se não encontrar, cria uma nova GameObject com esse script
                if (saveGame == null)
                {
                    // Confere se existe esse GameObject em cena, se houver, adiciona o script nele.
                    if (GameObject.Find("GameManager"))
                    {
                        GameObject obj = GameObject.Find("GameManager");
                        obj.AddComponent<MixerManager>();
                        print("Adicione o Script SaveGame no GameManager");
                    }
                    else
                    {
                        GameObject obj = new GameObject("GameManager");
                        saveGame = obj.AddComponent<SaveGame>();
                        print("Crie um GameManager e adicione o Script SaveGame no GameManager");
                    }
                }
            }
            return saveGame;
        }
    }
    // Finalização do Singleton.

    // Carrega as informações do save nos determinados locais.
    private void Awake()
    {
        // Permite somente uma instância de SaveGame na cêna.
        if (saveGame == null)
        {
            saveGame = this;
        }
        else if (saveGame != this)
        {
            print("Procure esses objetos e retire o script SaveGame até sobrar apenas um: " + gameObject.name + ", " + saveGame.name);
            Destroy(gameObject);
        }

        // Pega os saves criados anteriormente.
        LoadPlayerData();
        LoadHotbarData();
        LoadDaylightCycleData();
    }

    private void Start()
    {
        // Faz o save de tempos em tempos.
        InvokeRepeating(nameof(MakeSaves), delaySaveGame, delaySaveGame);
    }

    PlayerData playerData = new PlayerData();
    HotbarData hotbarData = new HotbarData();
    DaylightCycleData daylightCycleData = new DaylightCycleData();
    public void MakeSaves()
    {
        if (playerData.playerController != null)
        {
            string jsonPlayerData = JsonUtility.ToJson(playerData);
            File.WriteAllText(Application.dataPath + FINALPATH + SAVEPLAYER, jsonPlayerData);
        }

        if (hotbarData.hotbar != null)
        {
            string jsonHotbarData = JsonUtility.ToJson(hotbarData);
            File.WriteAllText(Application.dataPath + FINALPATH + SAVEHOTBAR, jsonHotbarData);
        }

        if (daylightCycleData.daylightCycle != null)
        {
            string jsonDaylightCycleData = JsonUtility.ToJson(daylightCycleData);
            File.WriteAllText(Application.dataPath + FINALPATH + SAVEDAY, jsonDaylightCycleData);
        }
    }

    // Recebe o SavePlayerTransform do PlayerController.
    public void SavePlayerData(in SaveGameInfos infos)
    {
        playerData.playerController = infos.PlayerController; // Grava a referência do script que passou as informações pro SaveGame.
        playerData.playerPosition = infos.PlayerPosition;
        playerData.playerRotation = infos.PlayerRotation;
        playerData.cameraControllerRotation = infos.CameraControllerRotation;
    }

    private void LoadPlayerData()
    {
        if (playerData == null) return;

        if (File.Exists(Application.dataPath + FINALPATH + SAVEPLAYER))
        {
            string jsonPlayerData = File.ReadAllText(Application.dataPath + FINALPATH + SAVEPLAYER);
            PlayerData _playerData = JsonUtility.FromJson<PlayerData>(jsonPlayerData);
          
            if (_playerData.playerController != null)
            {
                _playerData.playerController.characterMovement.motor.SetPosition(_playerData.playerPosition);
                _playerData.playerController.characterMovement.motor.RotateCharacter(_playerData.playerRotation);
                _playerData.playerController.cameraController.targetLook = _playerData.cameraControllerRotation;
            }
        }
    }

    public void SaveHotbarData(in SaveGameInfos infos)
    {
        hotbarData.hotbar = infos.Hotbar;
        hotbarData.slot = infos.Slot;
        hotbarData.itens = infos.Itens;
    }

    private void LoadHotbarData()
    {
        if (hotbarData == null) return;

        if (File.Exists(Application.dataPath + FINALPATH + SAVEHOTBAR))
        {
            string jsonHotbarData = File.ReadAllText(Application.dataPath + FINALPATH + SAVEHOTBAR);
            HotbarData _hotbarData = JsonUtility.FromJson<HotbarData>(jsonHotbarData);

            if (_hotbarData.hotbar != null)
            {
                _hotbarData.hotbar.saveSlot = _hotbarData.slot;
                _hotbarData.hotbar.itens = _hotbarData.itens;
            }
        }
    }

    public void SaveDaylightCycleData(in SaveGameInfos infos)
    {
        daylightCycleData.daylightCycle = infos.DaylightCycle;
        daylightCycleData.seconds = infos.Seconds;
    }

    private void LoadDaylightCycleData()
    {
        if (daylightCycleData == null) return;

        if (File.Exists(Application.dataPath + FINALPATH + SAVEDAY))
        {
            string jsonDaylightCycleData = File.ReadAllText(Application.dataPath + FINALPATH + SAVEDAY);
            DaylightCycleData _daylightCycleData = JsonUtility.FromJson<DaylightCycleData>(jsonDaylightCycleData);

            if (_daylightCycleData.daylightCycle != null)
            {
                _daylightCycleData.daylightCycle.seconds = _daylightCycleData.seconds;
            }
        }
    }
}
