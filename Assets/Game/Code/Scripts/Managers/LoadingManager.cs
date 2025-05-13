using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [Header("Configura��es")]
    [Tooltip("Nome da cena para carregar.")]
    public string nomeDaCena;

    [Tooltip("Tempo m�nimo para exibir a tela de loading (em segundos).")]
    public float tempoDeLoading = 2f;

    [Header("Refer�ncias")]
    [Tooltip("GameObject da tela de carregamento.")]
    public GameObject telaDeLoading;

    [Tooltip("Componente Animator respons�vel pela anima��o.")]
    public Animator animator;

    [Tooltip("Lista de objetos a serem desativados ao pressionar Novo Jogo.")]
    public GameObject[] objetosParaDesativar;

    // Fun��o chamada pelo bot�o "Novo Jogo"
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

    // Coroutine para carregar a cena de forma ass�ncrona
    private IEnumerator CarregarCenaAsync()
    {
        // Ativa a tela de loading
        telaDeLoading.SetActive(true);

        // Toca a anima��o do loading, caso o Animator seja configurado
        if (animator != null)
        {
            animator.SetTrigger("IniciarLoading");
        }

        // Aguarda o tempo m�nimo de exibi��o da tela de loading
        float tempoInicial = Time.time;

        // Inicia o carregamento ass�ncrono da cena
        AsyncOperation operacao = SceneManager.LoadSceneAsync(nomeDaCena);
        operacao.allowSceneActivation = false;

        // Aguarda at� que a cena esteja completamente carregada
        while (!operacao.isDone)
        {
            // Checa se o carregamento chegou a 90% (padr�o para pronto, mas ainda n�o ativado)
            if (operacao.progress >= 0.9f && Time.time >= tempoInicial + tempoDeLoading)
            {
                // Certifique-se de que os shaders estejam compilados
                Shader.WarmupAllShaders();

                // Permite a ativa��o da cena
                operacao.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
