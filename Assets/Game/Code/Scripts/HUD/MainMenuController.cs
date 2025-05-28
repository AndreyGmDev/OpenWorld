using UnityEngine.UI;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
//

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
  
    [Header("SFX")]
    [SerializeField] AudioClip hoverSFX;
    [SerializeField] AudioClip clickSFX;
    [SerializeField] AudioClip bigClickSFX;

    float vol = 1;
    AudioManager audioManager;
    private void Start()
    {
        audioManager = AudioManager.Instance;
        // Ação dos botões do MainMenuCanvas.
        // NewGame.
        if (newGameButton != null && newGamePopUp != null)
        {
            newGameButton.onClick.AddListener(() => audioManager.PlaySoundFXClip(clickSFX, transform, 1, false)); // SFX
            newGameButton.onClick.AddListener(() => newGamePopUp.SetActive(true)); // Ativa o NewGame.
            newGameButton.onClick.AddListener(() => menuOptionsPopUp.SetActive(false)); // Desativa o MenuOptions.
        }
        
        if (newGameButtonYes != null)
        {
            newGameButtonYes.onClick.AddListener(() => LoadingManager.Instance.NovoJogo()); // Inicia um novo jogo.
            newGameButtonYes.onClick.AddListener(() => AudioManager.Instance.PlaySoundFXClip(clickSFX, transform, vol, false));
            newGameButtonYes.onClick.AddListener(() => AudioManager.Instance.PlaySoundFXClip(bigClickSFX, transform, vol, false)); // SFX
        }

        if (newGameButtonNo != null && newGamePopUp != null)
        {
            newGameButtonNo.onClick.AddListener(() => menuOptionsPopUp.SetActive(true)); // Ativa o MenuOptions.
            newGameButtonNo.onClick.AddListener(() => newGamePopUp.SetActive(false)); // Desativa o NewGame.
            newGameButtonNo.onClick.AddListener(() => AudioManager.Instance.PlaySoundFXClip(clickSFX, transform, vol, false)); // SFX
        }

        // Continue.
        if (continueButton != null)
        {
            // Se houver um save
            if (!File.Exists(Application.dataPath + "/Saves/game_state.txt"))
            {
                continueButton.GetComponent<ButtonMouseEffects>().enabled = false;
                continueButton.interactable = false;

                Color disableColor = new Color(0.192f, 0.192f, 0.192f, 255);
                continueButton.GetComponentInChildren<TextMeshProUGUI>().color = disableColor;
            }
            else
            {
                continueButton.onClick.AddListener(() => StartCoroutine(LoadingManager.Instance.LoadAsyncScene("OpenWorld")));
                continueButton.onClick.AddListener(() => AudioManager.Instance.PlaySoundFXClip(bigClickSFX, transform, vol, false));
            }
        }

        // Settings.
        if (settingsButton != null && settingsPopUp != null)
        {
            settingsButton.onClick.AddListener(() => settingsPopUp.SetActive(true));
            settingsButton.onClick.AddListener(() => AudioManager.Instance.PlaySoundFXClip(clickSFX, transform, vol, false));
        }

        // Credits.
        if (creditsButton != null && creditsPopUp != null)
        {
            creditsButton.onClick.AddListener(() => creditsPopUp.SetActive(true)); // Abre os créditos.
            creditsButton.onClick.AddListener(() => menuOptionsPopUp.SetActive(false)); // Desativa o MenuOptions.
            creditsButton.onClick.AddListener(() => AudioManager.Instance.PlaySoundFXClip(clickSFX, transform, vol, false));
        }

        if (backCreditsButton != null && creditsPopUp != null)
        {
            backCreditsButton.onClick.AddListener(() => menuOptionsPopUp.SetActive(true)); // Ativa o MenuOptions.
            backCreditsButton.onClick.AddListener(() => creditsPopUp.SetActive(false)); // Desativa os créditos.
            backCreditsButton.onClick.AddListener(() => AudioManager.Instance.PlaySoundFXClip(clickSFX, transform, vol, false));
        }

        // Exit.
        if (exitButton != null && exitPopUp != null)
        {
            exitButton.onClick.AddListener(() => exitPopUp.SetActive(true)); // Ativa o ExitGame.
            exitButton.onClick.AddListener(() => menuOptionsPopUp.SetActive(false)); // Desativa o MenuOptions.
            exitButton.onClick.AddListener(() => AudioManager.Instance.PlaySoundFXClip(clickSFX, transform, vol, false));
        }

        if (exitButtonYes != null)
        {
            exitButtonYes.onClick.AddListener(Application.Quit); // Fecha o jogo.
        }

        if (exitButtonNo != null && exitPopUp != null)
        {
            exitButtonNo.onClick.AddListener(() => menuOptionsPopUp.SetActive(true)); // Ativa o MenuOptions.
            exitButtonNo.onClick.AddListener(() => exitPopUp.SetActive(false)); // Desativa o ExitGame.
            exitButtonNo.onClick.AddListener(() => AudioManager.Instance.PlaySoundFXClip(clickSFX, transform, vol, false));
        }
    }
}
