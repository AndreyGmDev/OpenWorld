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
    private const string FINALPATH  = "/Saves";
    private const string SAVEDATA = "/game_state.txt";

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
                        GameObject obj = new("GameManager");
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
    public void MakeSaves()
    {
        string jsonPlayerData = JsonUtility.ToJson(saveGameInfos);
        File.WriteAllText(Application.dataPath + FINALPATH + SAVEDATA, jsonPlayerData);

        string jsonHotbarData = JsonUtility.ToJson(saveGameInfos);
        File.WriteAllText(Application.dataPath + FINALPATH + SAVEDATA, jsonHotbarData);

        string jsonDaylightCycleData = JsonUtility.ToJson(saveGameInfos);
        File.WriteAllText(Application.dataPath + FINALPATH + SAVEDATA, jsonDaylightCycleData);
    }

    // Save do Player - Script PlayerController.
    public void SavePlayerData(in SaveGameInfos infos)
    {
        saveGameInfos.PlayerPosition = infos.PlayerPosition;
        saveGameInfos.PlayerRotation = infos.PlayerRotation;
        saveGameInfos.CameraControllerRotation = infos.CameraControllerRotation;
    }

    // Save da Hotbar - Scrip PlayerController.
    public void SaveHotbarData(in SaveGameInfos infos)
    {
        saveGameInfos.Slot = infos.Slot;
        saveGameInfos.Itens = infos.Itens;
    }

    // Save do DaylightCycle - Script DaylightCycle.
    public void SaveDaylightCycleData(in SaveGameInfos infos)
    {
        saveGameInfos.Seconds = infos.Seconds;
    }

    // Função para o carregar o jogo.
    public SaveGameInfos LoadData()
    {
        if (File.Exists(Application.dataPath + FINALPATH + SAVEDATA))
        {
            string jsonData = File.ReadAllText(Application.dataPath + FINALPATH + SAVEDATA);
            SaveGameInfos data = JsonUtility.FromJson<SaveGameInfos>(jsonData);

            return data;
        }
        else
        {
            return NewSaveGame();
        }
    }

    // Função para criar um novo save.
    private SaveGameInfos NewSaveGame()
    {
        SaveGameInfos save = new()
        {
            PlayerPosition = Vector3.zero,
            PlayerRotation = Quaternion.identity,
            CameraControllerRotation = Vector3.zero,
            Slot = 0,
            Itens = new GameObject[4],
            Seconds = 28800 // 08:00 horas.
        };
        return save;
    }
}
