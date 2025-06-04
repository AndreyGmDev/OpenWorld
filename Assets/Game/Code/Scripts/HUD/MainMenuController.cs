using UnityEngine.UI;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    private const int VOL = 1; // Volume.

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

    AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.Instance;

        // Setup hover sound effects for all buttons
        SetupButtonHoverSFX(newGameButton);
        SetupButtonHoverSFX(newGameButtonYes);
        SetupButtonHoverSFX(newGameButtonNo);
        SetupButtonHoverSFX(continueButton);
        SetupButtonHoverSFX(settingsButton);
        SetupButtonHoverSFX(creditsButton);
        SetupButtonHoverSFX(backCreditsButton);
        SetupButtonHoverSFX(exitButton);
        SetupButtonHoverSFX(exitButtonYes);
        SetupButtonHoverSFX(exitButtonNo);

        // Ação dos botões do MainMenuCanvas.
        // NewGame.
        if (newGameButton != null && newGamePopUp != null)
        {
            newGameButton.onClick.AddListener(() => newGamePopUp.SetActive(true)); // Ativa o NewGame.
            newGameButton.onClick.AddListener(() => menuOptionsPopUp.SetActive(false)); // Desativa o MenuOptions.
            newGameButton.onClick.AddListener(() => audioManager.PlaySoundFXClip(clickSFX, transform, VOL, false)); // SFX
        }
        
        if (newGameButtonYes != null)
        {
            newGameButtonYes.onClick.AddListener(() => LoadingManager.Instance.NovoJogo()); // Inicia um novo jogo.
            newGameButtonYes.onClick.AddListener(() => audioManager.PlaySoundFXClip(bigClickSFX, transform, VOL, false)); // SFX
        }

        if (newGameButtonNo != null && newGamePopUp != null)
        {
            newGameButtonNo.onClick.AddListener(() => menuOptionsPopUp.SetActive(true)); // Ativa o MenuOptions.
            newGameButtonNo.onClick.AddListener(() => newGamePopUp.SetActive(false)); // Desativa o NewGame.
            newGameButtonNo.onClick.AddListener(() => audioManager.PlaySoundFXClip(clickSFX, transform, VOL, false)); // SFX
        }

        // Continue.
        if (continueButton != null)
        {
            // Se houver um save
            if (!File.Exists(SaveGame.Instance.SAVEPATH + SaveGame.Instance.SAVEDATA))
            {
                continueButton.GetComponent<ButtonMouseEffects>().enabled = false;
                continueButton.interactable = false;

                Color disableColor = Color.white;
                disableColor = new Color(0.192f, 0.192f, 0.192f, 255);
                continueButton.GetComponentInChildren<TextMeshProUGUI>().color = disableColor;
            }
            else
            {
                continueButton.onClick.AddListener(() => StartCoroutine(LoadingManager.Instance.LoadAsyncScene("OpenWorld")));
                continueButton.onClick.AddListener(() => audioManager.PlaySoundFXClip(bigClickSFX, transform, VOL, false));
            }
        }

        // Settings.
        if (settingsButton != null && settingsPopUp != null)
        {
            settingsButton.onClick.AddListener(() => settingsPopUp.SetActive(true));
            settingsButton.onClick.AddListener(() => audioManager.PlaySoundFXClip(clickSFX, transform, VOL, false));
        }

        // Credits.
        if (creditsButton != null && creditsPopUp != null)
        {
            creditsButton.onClick.AddListener(() => creditsPopUp.SetActive(true)); // Abre os cr�ditos.
            creditsButton.onClick.AddListener(() => menuOptionsPopUp.SetActive(false)); // Desativa o MenuOptions.
            creditsButton.onClick.AddListener(() => audioManager.PlaySoundFXClip(clickSFX, transform, VOL, false));
        }

        if (backCreditsButton != null && creditsPopUp != null)
        {
            backCreditsButton.onClick.AddListener(() => menuOptionsPopUp.SetActive(true)); // Ativa o MenuOptions.
            backCreditsButton.onClick.AddListener(() => creditsPopUp.SetActive(false)); // Desativa os cr�ditos.
            backCreditsButton.onClick.AddListener(() => audioManager.PlaySoundFXClip(clickSFX, transform, VOL, false));
        }

        // Exit.
        if (exitButton != null && exitPopUp != null)
        {
            exitButton.onClick.AddListener(() => exitPopUp.SetActive(true)); // Ativa o ExitGame.
            exitButton.onClick.AddListener(() => menuOptionsPopUp.SetActive(false)); // Desativa o MenuOptions.
            exitButton.onClick.AddListener(() => audioManager.PlaySoundFXClip(clickSFX, transform, VOL, false));
        }

        if (exitButtonYes != null)
        {
            exitButtonYes.onClick.AddListener(Application.Quit); // Fecha o jogo.
        }

        if (exitButtonNo != null && exitPopUp != null)
        {
            exitButtonNo.onClick.AddListener(() => menuOptionsPopUp.SetActive(true)); // Ativa o MenuOptions.
            exitButtonNo.onClick.AddListener(() => exitPopUp.SetActive(false)); // Desativa o ExitGame.
            exitButtonNo.onClick.AddListener(() => audioManager.PlaySoundFXClip(clickSFX, transform, VOL, false));
        }
    }

    /// <summary>
    /// O botão para adicionar o hoverSFX
    /// </summary>
    /// <param name="button"></param>
    private void SetupButtonHoverSFX(Button button)
    {
        if (button == null || hoverSFX == null) return;

        // Pega ou adiciona o componente EventTrigger
        EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
        if (!eventTrigger)
        {
            eventTrigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // Cria o evento de hover
        EventTrigger.Entry hoverEnter = new()
        {
            eventID = EventTriggerType.PointerEnter // Seta o tipo de evento(PointerEnter).
        };
        
        // Toca o som quando a condição do evento é atendida.
        hoverEnter.callback.AddListener((eventData) => {
            if (button.interactable && audioManager != null)
            {
                audioManager.PlaySoundFXClip(hoverSFX, transform, VOL, false);
            }
        });

        // Adiciona o evento ao trigger
        eventTrigger.triggers.Add(hoverEnter);
    }
}
