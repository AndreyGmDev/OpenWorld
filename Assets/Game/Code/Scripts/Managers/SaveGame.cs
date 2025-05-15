using System.IO;
using UnityEngine;

public struct SaveGameInfos
{
    // Player infos.
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;
    public Vector2 CameraControllerRotation;

    // Hotbar infos.
    public float Slot;
    public GameObject[] Itens;

    // DaylightCycle infos.
    public float Seconds;
}

public class SaveGame : MonoBehaviour
{
    // Nomes dos arquivos que serão salvos as informações
    const string FINALPATH  = "/Saves";
    const string SAVEDATA = "/game_state.txt";
    //const string SAVEPLAYER = "/player_state.txt";
    //const string SAVEDAY    = "/day_state.txt";

    [SerializeField, Tooltip("Delay between each game save")] float delaySaveGame = 30;

    // Inicia o Singleton do SaveSame.
    private static SaveGame saveGame;

    public static SaveGame Instance
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
                        obj.AddComponent<SaveGame>();
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
    }

    private void Start()
    {
        // Faz o save de tempos em tempos.
        InvokeRepeating(nameof(MakeSaves), delaySaveGame, delaySaveGame);
    }

    SaveGameInfos saveGameInfos;
    SaveGameInfos save = new SaveGameInfos();
    public void MakeSaves()
    {
        string jsonPlayerData = JsonUtility.ToJson(saveGameInfos);
        File.WriteAllText(Application.dataPath + FINALPATH + SAVEDATA, jsonPlayerData);

        string jsonHotbarData = JsonUtility.ToJson(saveGameInfos);
        File.WriteAllText(Application.dataPath + FINALPATH + SAVEDATA, jsonHotbarData);

        string jsonDaylightCycleData = JsonUtility.ToJson(saveGameInfos);
        File.WriteAllText(Application.dataPath + FINALPATH + SAVEDATA, jsonDaylightCycleData);
    }

    // Recebe o SavePlayerTransform do PlayerController.
    public void SavePlayerData(in SaveGameInfos infos)
    {
        saveGameInfos.PlayerPosition = infos.PlayerPosition;
        saveGameInfos.PlayerRotation = infos.PlayerRotation;
        saveGameInfos.CameraControllerRotation = infos.CameraControllerRotation;
    }

    public SaveGameInfos LoadPlayerData()
    {
        if (File.Exists(Application.dataPath + FINALPATH + SAVEDATA))
        {
            string jsonPlayerData = File.ReadAllText(Application.dataPath + FINALPATH + SAVEDATA);
            SaveGameInfos playerData = JsonUtility.FromJson<SaveGameInfos>(jsonPlayerData);

            return playerData;
        }
        else
        {
            NewSaveGame();
            return save;
        }
    }

    public void SaveHotbarData(in SaveGameInfos infos)
    {
        saveGameInfos.Slot = infos.Slot;
        saveGameInfos.Itens = infos.Itens;
    }

    public SaveGameInfos LoadHotbarData()
    {
       
        if (File.Exists(Application.dataPath + FINALPATH + SAVEDATA))
        {
            string jsonHotbarData = File.ReadAllText(Application.dataPath + FINALPATH + SAVEDATA);
            SaveGameInfos hotbarData = JsonUtility.FromJson<SaveGameInfos>(jsonHotbarData);

            return hotbarData;
        }
        else
        {
            NewSaveGame();
            return save;
        }
    }

    public void SaveDaylightCycleData(in SaveGameInfos infos)
    {
        saveGameInfos.Seconds = infos.Seconds;
    }

    public SaveGameInfos LoadDaylightCycleData()
    {
        if (File.Exists(Application.dataPath + FINALPATH + SAVEDATA))
        {
            string jsonDaylightCycleData = File.ReadAllText(Application.dataPath + FINALPATH + SAVEDATA);
            SaveGameInfos daylightCycleData = JsonUtility.FromJson<SaveGameInfos>(jsonDaylightCycleData);

            return daylightCycleData;
        }
        else
        {
            NewSaveGame();
            return save;
        }
    }

    private void NewSaveGame()
    {
        save.PlayerPosition = Vector3.zero;
        save.PlayerRotation = Quaternion.identity;
        save.CameraControllerRotation = Vector3.zero;
        save.Slot = 1;
        save.Itens = null;
        save.Seconds = 0;
    }
}
