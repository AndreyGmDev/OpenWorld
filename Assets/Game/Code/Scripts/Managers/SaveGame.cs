using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct SaveGameInfos
{
    // Player infos.
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;
    public Vector2 CameraControllerRotation;

    // Hotbar infos.
    public float Slot;
    public string[] ItensID;

    // DaylightCycle infos.
    public float Seconds;

    // Game infos.
    public string LevelName;

    // Hat_Quest.
    public int HatsCollected;
}

public class SaveGame : MonoBehaviour
{
    // Nomes dos arquivos que serão salvos as informações
    // Caminho personalizado: Pasta "Saves" dentro de "Meus Documentos"
    public readonly string SAVEPATH = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Saves");
    public readonly string SAVEDATA = "/game_state.txt";

    [Header("References")]
    public SaveBetweenScenes saveBetweenScenes;

    [Header("SaveGame")]
    [SerializeField, Tooltip("Delay between each game save. If delay is 0, the save will not be performed")] float delaySaveGame = 30;
    private SaveGameInfos saveGameInfos;

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
        // Pega o nome do level atual
        SaveCurrentLevel(new SaveGameInfos
        {
            LevelName = SceneManager.GetActiveScene().name,
        });

        // Faz um save sempre que uma nova cena é carregada.
        //StartCoroutine(MakeSaves(0, false)); 

        // Confere se pode fazer save na cena.   
        if (delaySaveGame > 0)
        {
            StartCoroutine(MakeSaves(5, true)); // Faz o save de tempos em tempos.
        }
    }

    private void Update()
    {
        // Passa todas as informações que normalmente são salvas no jogo para o SaveBetweenScenes.
        saveBetweenScenes.saveGameInfos = saveGameInfos;
    }

    // Confere todas as condições para saber se o save do game pode ser feito.
    public bool CanMakeSaves()
    {
        bool condition = true;

        // 1° Condição - A cena carregada é Ilha.

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            condition &= false;
        }

        // Final da 1° condição.

        // 2° Condição - A cena carregada é Ilha.

        /*if (!(SceneManager.GetActiveScene().name == "OpenWorld"))
        {
            condition &= false;
        }*/

        // Final da 2° condição.

        // Se nenhuma das condições forem atendidas, return false.
        return condition;
    }

    public IEnumerator MakeSaves(float delay, bool makeSaveAgain)
    {
        // delay - Tempo para nova tentativa de fazer o save, caso não possa realiza-lo no momento.
        // makeSaveAgain - Confere se essa coroutine será chamada novamente. Serve para o botão de save que chamará a função uma única vez a cada clique.

        // Confere se é para fazer o save instantaneamente.
        if (makeSaveAgain)
        {
            // Espera o tempo para tentar fazer o save.
            yield return new WaitForSeconds(delaySaveGame);
        }

        // Confere se pode fazer o save.
        if (CanMakeSaves())
        {
            // Faz o Save.
            if (!Directory.Exists(SAVEPATH))
            {
                Directory.CreateDirectory(SAVEPATH);
            }

            string jsonPlayerData = JsonUtility.ToJson(saveGameInfos);
            File.WriteAllText(SAVEPATH + SAVEDATA, jsonPlayerData);

            // Se for para fazer o save novamente.
            if (makeSaveAgain)
            {
                // Inicia o novo save.
                StartCoroutine(MakeSaves(delay, true));
            }
        }
        // Se não puder fazer o save e se for para tentar fazer o save novamente.
        else if (makeSaveAgain)
        {
            // Espero o delay para a nova tentativa de save.
            yield return new WaitForSeconds(delay);

            // Faz a nova tentativa de save.
            StartCoroutine(MakeSaves(delay, true));
        }

        yield return null;
    }

    // Save do Player - Script PlayerController.
    public void SavePlayerData(in SaveGameInfos infos)
    {
        saveGameInfos.PlayerPosition = infos.PlayerPosition;
        saveGameInfos.PlayerRotation = infos.PlayerRotation;
        saveGameInfos.CameraControllerRotation = infos.CameraControllerRotation;
    }

    // Save da Hotbar - Scrip Hotbar.
    public void SaveHotbarData(in SaveGameInfos infos)
    {
        saveGameInfos.Slot = infos.Slot;
        saveGameInfos.ItensID = infos.ItensID;
    }

    // Save do DaylightCycle - Script DaylightCycle.
    public void SaveDaylightCycleData(in SaveGameInfos infos)
    {
        saveGameInfos.Seconds = infos.Seconds;
    }

    // Save do CurrentLevel - Script SaveGame.
    public void SaveCurrentLevel(in SaveGameInfos infos)
    {
        saveGameInfos.LevelName = infos.LevelName;
    }

    // Save da HatQuest - Script Hat_Quest.
    public void SaveHatQuest(in SaveGameInfos infos)
    {
        saveGameInfos.HatsCollected = infos.HatsCollected;
    }

    // Função para o carregar o jogo.
    public SaveGameInfos LoadData()
    {
        // Confere se é para carregar o save game.
        if (saveBetweenScenes.CanLoadSaveGame())
        {
            // Se um save existir, carrega o save.
            if (File.Exists(SAVEPATH + SAVEDATA))
            {
                string jsonData = File.ReadAllText(SAVEPATH + SAVEDATA);
                SaveGameInfos data = JsonUtility.FromJson<SaveGameInfos>(jsonData);

                return data;
            }
            // Se não houver um save, chama a função NewSaveGame.
            else
            {
                return NewSaveGame();
            }
        }
        // Se não for para carregar o save game, carrega save between scenes.
        else
        {
            return saveBetweenScenes.newSaveGameInfos;
        }
     }

    // Função para criar um novo save.
    private SaveGameInfos NewSaveGame()
    {
        SaveGameInfos save = new()
        {
            
            PlayerPosition = new Vector3(-86.1620026f, 26.3700008f, 229.979996f),
            PlayerRotation = Quaternion.identity,
            CameraControllerRotation = Vector3.zero,
            Slot = 0,
            ItensID = new string[3],
            Seconds = 28800, // 08:00 horas.
            LevelName = "OpenWorld",
            HatsCollected = 0,
        };
        return save;
    }
}
