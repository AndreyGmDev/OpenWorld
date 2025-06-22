using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    private static LoadingManager loadingManager;

    public static LoadingManager Instance
    {
        get
        {
            // Confere se a instância já foi criada
            if (loadingManager == null)
            {
                // Procura o LoadingManager na cena
                loadingManager = FindFirstObjectByType<LoadingManager>();

                // Se não encontrar, cria uma nova GameObject com esse script
                if (loadingManager == null)
                {
                    // Confere se existe esse GameObject em cena, se houver, adiciona o script nele.
                    if (GameObject.Find("GameManager"))
                    {
                        GameObject obj = GameObject.Find("GameManager");
                        obj.AddComponent<LoadingManager>();
                        print("Adicione o Script LoadingManager no GameManager");
                    }
                    else
                    {
                        GameObject obj = new GameObject("GameManager");
                        loadingManager = obj.AddComponent<LoadingManager>();
                        print("Crie um GameManager e adicione o Script LoadingManager no GameManager");
                    }
                }
            }
            return loadingManager;
        }
    }
    // Finalização do Singleton.

    [Tooltip("Tempo mínimo para exibir a tela de loading (em segundos).")]
    [SerializeField] float tempoDeLoading = 2f;

    [Header("Referências")]
    [Tooltip("GameObject da tela de carregamento.")]
    [SerializeField] GameObject telaDeLoading;

    [Tooltip("Componente Animator responsável pela animação.")]
    [SerializeField] Animator animator;

    [Tooltip("Lista de objetos a serem desativados ao pressionar Novo Jogo.")]
    public GameObject[] objetosParaDesativar;

    private SaveGame saveGame;

    // Carrega as informações do save nos determinados locais.
    private void Awake()
    {
        //Temporário
        Cursor.lockState = CursorLockMode.None;

        // Permite somente uma instância de LoadingManager na cêna.
        if (loadingManager == null)
        {
            loadingManager = this;
        }
        else if (loadingManager != this)
        {
            print("Procure esses objetos e retire o script LoadingManager até sobrar apenas um: " + gameObject.name + ", " + loadingManager.name);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        saveGame = SaveGame.Instance;
    }

    // Função chamada pelo botão "Novo Jogo" no menu.
    public void NewGame()
    {
        if (!Directory.Exists(SaveGame.Instance.SAVEPATH)) 
        { 
            Directory.CreateDirectory(SaveGame.Instance.SAVEPATH);
        }
        
        File.Delete(SaveGame.Instance.SAVEPATH + SaveGame.Instance.SAVEDATA);

        // Desativa os objetos especificados
        foreach (GameObject objeto in objetosParaDesativar)
        {
            if (objeto != null)
            {
                objeto.SetActive(false);
            }
        }
        
        // Permite carregar o save do game para pegar as informações do save.
        saveGame.saveBetweenScenes.SetLoadSaveGame(true);

        // Pega o level para iniciar.
        string levelName = saveGame.LoadData().LevelName;

        // Confere se não é nulo ou vazio.
        if (!string.IsNullOrEmpty(levelName))
        {
            // Inicia o processo de carregamento da cena
            StartCoroutine(LoadScene(levelName));
        }
    }

    // Função chamada pelo botão "Continuar" no menu.
    public void ContinueGame()
    {
        // Permite carregar o save do game.
        saveGame.saveBetweenScenes.SetLoadSaveGame(true);

        // Pega o level para iniciar.
        string levelName = saveGame.LoadData().LevelName;

        // Confere se não é nulo ou vazio.
        if (!string.IsNullOrEmpty(levelName))
        {
            // Inicia o processo de carregamento da cena
            StartCoroutine(LoadScene(levelName));
        }
    }

    // Coroutine para carregar a cena de forma assíncrona
    public void LoadAsyncScene(string levelName)
    {
        // Não permite carregar o save do game.
        saveGame.saveBetweenScenes.SetLoadSaveGame(false);

        StartCoroutine(LoadScene(levelName));

        Time.timeScale = 1;
    }

    public IEnumerator LoadScene(string levelName)
    {
        // Desabilita o cursor.
        Cursor.lockState = CursorLockMode.Locked;

        // Disabilita todos os inputs.
        InputActionsManager input = InputActionsManager.Instance;
        input.DisableAllActions();
        
        // Ativa a tela de loading
        if (telaDeLoading != null)
        {
            telaDeLoading.SetActive(true);
        }

        // Toca a animação do loading, caso o Animator seja configurado
        if (animator != null)
        {
            animator.SetTrigger("IniciarLoading");
        }

        // Inicia o carregamento assíncrono da cena
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);
        operation.allowSceneActivation = false;

        // Aguarda o tempo mínimo de exibição da tela de loading
        yield return new WaitForSecondsRealtime(tempoDeLoading);

        // Aguarda até que a cena esteja completamente carregada
        while (!operation.isDone)
        {
            // Checa se o carregamento chegou a 90% (padrão para pronto, mas ainda não ativado)
            if (operation.progress >= 0.9f)
            {
                // Certifique-se de que os shaders estejam compilados
                //Shader.WarmupAllShaders();

                // Permite a ativação da cena
                Time.timeScale = 1;
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
