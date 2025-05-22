using UnityEngine.UI;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    // NewGame Buttons.
    [SerializeField] Button newGameButton;
    [SerializeField] Button newGameButtonYes;
    [SerializeField] Button newGameButtonNo;

    // Continue Button.
    [SerializeField] Button continueButton;

    // Setting Button.
    [SerializeField] Button settingsButton;

    // Credits Buttons.
    [SerializeField] Button creditsButton;
    [SerializeField] Button backCreditsButton;

    // Exit Buttons.
    [SerializeField] Button exitButton;
    [SerializeField] Button exitButtonYes;
    [SerializeField] Button exitButtonNo;

    [Header("Canvas")]
    [SerializeField] GameObject menuOptionsPopUp;
    [SerializeField] GameObject newGamePopUp;
    [SerializeField] GameObject settingsPopUp;
    [SerializeField] GameObject creditsPopUp;
    [SerializeField] GameObject exitPopUp;

    InputActionsManager input;
    void Start()
    {
        //input = InputActionsManager.Instance;

        // Ação dos botões do MainMenuCanvas.
        // NewGame.
        if (newGameButton != null && newGamePopUp != null)
        {
            newGameButton.onClick.AddListener(() => newGamePopUp.SetActive(true)); // Ativa o NewGame.
            newGameButton.onClick.AddListener(() => menuOptionsPopUp.SetActive(false)); // Desativa o MenuOptions.
        }

        if (newGameButtonYes != null)
        {
            newGameButtonYes.onClick.AddListener(() => LoadingManager.Instance.NovoJogo()); // Inicia um novo jogo.
        }

        if (newGameButtonNo != null && newGamePopUp != null)
        {
            newGameButtonNo.onClick.AddListener(() => menuOptionsPopUp.SetActive(true)); // Ativa o MenuOptions.
            newGameButtonNo.onClick.AddListener(() => newGamePopUp.SetActive(false)); // Desativa o NewGame.
        }

        // Continue.
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(() => StartCoroutine(LoadingManager.Instance.LoadAsyncScene("OpenWorld")));
        }

        // Settings.
        if (settingsButton != null && settingsPopUp != null)
        {
            settingsButton.onClick.AddListener(() => settingsPopUp.SetActive(true));
        }

        // Credits.
        if (creditsButton != null && creditsPopUp != null)
        {
            creditsButton.onClick.AddListener(() => creditsPopUp.SetActive(true)); // Abre os créditos.
            creditsButton.onClick.AddListener(() => menuOptionsPopUp.SetActive(false)); // Desativa o MenuOptions.
        }

        if (backCreditsButton != null && creditsPopUp != null)
        {
            backCreditsButton.onClick.AddListener(() => menuOptionsPopUp.SetActive(true)); // Ativa o MenuOptions.
            backCreditsButton.onClick.AddListener(() => creditsPopUp.SetActive(false)); // Desativa os créditos.
        }

        // Exit.
        if (exitButton != null && exitPopUp != null)
        {
            exitButton.onClick.AddListener(() => exitPopUp.SetActive(true)); // Ativa o ExitGame.
            exitButton.onClick.AddListener(() => menuOptionsPopUp.SetActive(false)); // Desativa o MenuOptions.
        }

        if (exitButtonYes != null)
        {
            exitButtonYes.onClick.AddListener(Application.Quit); // Fecha o jogo.
        }

        if (exitButtonNo != null && exitPopUp != null)
        {
            exitButtonNo.onClick.AddListener(() => menuOptionsPopUp.SetActive(true)); // Ativa o MenuOptions.
            exitButtonNo.onClick.AddListener(() => exitPopUp.SetActive(false)); // Desativa o ExitGame.
        }

    }
}
