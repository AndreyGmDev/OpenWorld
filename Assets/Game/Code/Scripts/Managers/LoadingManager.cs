using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Nome da cena para carregar.")]
    public string nomeDaCena;

    [Tooltip("Tempo mínimo para exibir a tela de loading (em segundos).")]
    public float tempoDeLoading = 2f;

    [Header("Referências")]
    [Tooltip("GameObject da tela de carregamento.")]
    public GameObject telaDeLoading;

    [Tooltip("Componente Animator responsável pela animação.")]
    public Animator animator;

    [Tooltip("Lista de objetos a serem desativados ao pressionar Novo Jogo.")]
    public GameObject[] objetosParaDesativar;

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
}
