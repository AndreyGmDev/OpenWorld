using System.Collections;
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

    
    [Header("Configurações")]
    [Tooltip("Nome da cena para carregar.")]
    public string nomeDaCena;

    [Tooltip("Tempo mínimo para exibir a tela de loading (em segundos).")]
    [SerializeField] float tempoDeLoading = 2f;

    [Header("Referências")]
    [Tooltip("GameObject da tela de carregamento.")]
    [SerializeField] GameObject telaDeLoading;

    [Tooltip("Componente Animator responsável pela animação.")]
    [SerializeField] Animator animator;

    [Tooltip("Lista de objetos a serem desativados ao pressionar Novo Jogo.")]
    public GameObject[] objetosParaDesativar;


    // Carrega as informações do save nos determinados locais.
    private void Awake()
    {
        // Permite somente uma instância de LoadingManager na cêna.
        if (loadingManager == null)
        {
            loadingManager = this;
        }
        else if (loadingManager != this)
        {

            //Temporário
            Cursor.visible = true;
            Debug.Log(Cursor.visible);
            Time.timeScale =  1;


            print("Procure esses objetos e retire o script LoadingManager até sobrar apenas um: " + gameObject.name + ", " + loadingManager.name);
            Destroy(gameObject);
        }
    }

    // Função chamada pelo botão "Novo Jogo"
    public void NovoJogo()
    {
        // Desativa os objetos especificados
        foreach (GameObject objeto in objetosParaDesativar)
        {
            if (objeto != null)
            {
                objeto.SetActive(false);
            }
        }

        // Inicia o processo de carregamento da cena
        StartCoroutine(CarregarCenaAsync());
    }

    // Coroutine para carregar a cena de forma assíncrona
    private IEnumerator CarregarCenaAsync()
    {
        // Ativa a tela de loading
        telaDeLoading.SetActive(true);

        // Toca a animação do loading, caso o Animator seja configurado
        if (animator != null)
        {
            animator.SetTrigger("IniciarLoading");
        }

        // Aguarda o tempo mínimo de exibição da tela de loading
        float tempoInicial = Time.time;

        // Inicia o carregamento assíncrono da cena
        AsyncOperation operacao = SceneManager.LoadSceneAsync(nomeDaCena);
        operacao.allowSceneActivation = false;

        // Aguarda até que a cena esteja completamente carregada
        while (!operacao.isDone)
        {
            // Checa se o carregamento chegou a 90% (padrão para pronto, mas ainda não ativado)
            if (operacao.progress >= 0.9f && Time.time >= tempoInicial + tempoDeLoading)
            {
                // Certifique-se de que os shaders estejam compilados
                Shader.WarmupAllShaders();

                // Permite a ativação da cena
                operacao.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public IEnumerator LoadAsyncScene(string nameScene)
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
        AsyncOperation operation = SceneManager.LoadSceneAsync(nameScene);
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
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
