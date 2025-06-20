using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PauseMenuController : MonoBehaviour
{
    private const int VOL = 1; // Volume.

    [Header("Components")]
    [SerializeField] List<RectTransform> menuOptions; // Referência às opções do menu
    [SerializeField] Canvas pauseCanvas; // Canvas do menu de pausa.

    [Header("Infos")]
    [SerializeField] float spacing = 800f; // Espaço entre as opções (ajustado para deixar fora da tela)
    [SerializeField] float centerScale = 1.5f; // Escala para a opção central
    [SerializeField] float sideScale = 0.7f; // Escala para opções laterais
    [SerializeField] float lerpSpeed = 10f; // Velocidade de animação

    [Header("Buttons")]
    [SerializeField] Button settingsButton;
    [SerializeField] Button resumeButton;
    [SerializeField] Button saveButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button exitButtonYes;
    [SerializeField] Button exitButtonNo;

    [Header("Canvas")]
    [SerializeField] GameObject settingsCanvas;
    [SerializeField] GameObject resumeCanvas;
    [SerializeField] GameObject exitCanvas;

    [Header("SFX")]
    [SerializeField] AudioClip hoverSFX;
    [SerializeField] AudioClip clickSFX;

    private int currentIndex = 0;
    public bool isPaused;

    private InputActionsManager input;
    private SaveGame saveGame;
    private AudioManager audioManager; // Manager do Audio.

    void Start()
    {
        // Garante que as cenas irão iniciar normalmente.
        Time.timeScale = 1;

        input = InputActionsManager.Instance;
        saveGame = SaveGame.Instance;
        audioManager = AudioManager.Instance;

        ArrangeOptions();
        pauseCanvas.enabled = false; // Desativa o menu inicialmente

        // Setup hover sound effects for all buttons
        SetupButtonHoverSFX(resumeButton);
        SetupButtonHoverSFX(settingsButton);
        SetupButtonHoverSFX(saveButton);
        SetupButtonHoverSFX(exitButton);
        SetupButtonHoverSFX(exitButtonYes);
        SetupButtonHoverSFX(exitButtonNo);

        // Ação dos botões do PauseCanvas.
        if (resumeButton != null && resumeCanvas != null)
        {
            resumeButton.onClick.AddListener(DisablePauseCanvas);
            resumeButton.onClick.AddListener(() => {
                if (audioManager != null && clickSFX != null)
                    audioManager.PlaySoundFXClipUnscaled(clickSFX, transform, VOL, false);
            }); // SFX
        }

        if (settingsButton != null && settingsCanvas != null)
        {
            settingsButton.onClick.AddListener(() => settingsCanvas.SetActive(true));
            settingsButton.onClick.AddListener(() => {
                if (audioManager != null && clickSFX != null)
                    audioManager.PlaySoundFXClipUnscaled(clickSFX, transform, VOL, false);
            }); // SFX
        }
        
        if (exitButton != null && exitCanvas != null)
        {
            exitButton.onClick.AddListener(() => exitCanvas.SetActive(true));
            exitButton.onClick.AddListener(() => {
                if (audioManager != null && clickSFX != null)
                    audioManager.PlaySoundFXClipUnscaled(clickSFX, transform, VOL, false);
            }); // SFX
        }
        
        if (exitButtonNo != null && exitCanvas != null)
        {
            exitButtonNo.onClick.AddListener(() => exitCanvas.SetActive(false));
            exitButtonNo.onClick.AddListener(() => {
                if (audioManager != null && clickSFX != null)
                    audioManager.PlaySoundFXClipUnscaled(clickSFX, transform, VOL, false);
            }); // SFX
        }

        if (exitButtonYes != null) 
        { 
            exitButtonYes.onClick.AddListener(() => LoadingManager.Instance.LoadAsyncScene("MainMenu"));
            exitButtonYes.onClick.AddListener(() => {
                if (audioManager != null && clickSFX != null)
                    audioManager.PlaySoundFXClipUnscaled(clickSFX, transform, VOL, false);
            }); // SFX
        }

        if (saveButton != null)
        {
            saveButton.onClick.AddListener(() => StartCoroutine(saveGame.MakeSaves(0, false)));
            saveButton.onClick.AddListener(() => {
                if (audioManager != null && clickSFX != null)
                    audioManager.PlaySoundFXClipUnscaled(clickSFX, transform, VOL, false);
            }); // SFX
        }
    }

    private void DisablePauseCanvas()
    {
        // Habilita ou desabilita o botão de save game.
        saveButton.interactable = saveGame.CanMakeSaves();
        saveButton.GetComponent<ButtonMouseEffects>().enabled = saveButton.interactable;

        pauseCanvas.enabled = !pauseCanvas.enabled;
        isPaused = pauseCanvas.enabled;
        Time.timeScale = isPaused ? 0 : 1;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

        // Notifica o AudioManager sobre o estado de pausa
        if (audioManager != null)
        {
            audioManager.SetPauseState(isPaused);
        }

        if (isPaused)
            input.inputActions.Game.Disable();
        else
            input.inputActions.Game.Enable();

        SetActive(settingsCanvas, false);
        SetActive(exitCanvas, false);
    }

    private void SetActive(GameObject canvas, bool t)
    {
        canvas.SetActive(t);
    }

    void Update()
    {
        // Ativar/Desativar o menu de pausa
        if (input.inputActions.UI.Pause.WasPressedThisFrame() && !DialogueManager.Instance.IsInDialogue())
        {
            DisablePauseCanvas();
        }

        if (!isPaused) return;

        // Navegação por arrasto do mouse
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            if (mouseX > 0.1f)
            {
                currentIndex = Mathf.Min(menuOptions.Count - 1, currentIndex + 1);
                ArrangeOptions();
            }
            else if (mouseX < -0.1f)
            {
                currentIndex = Mathf.Max(0, currentIndex - 1);
                ArrangeOptions();
            }
        }
    }

    void ArrangeOptions()
    {
        for (int i = 0; i < menuOptions.Count; i++)
        {
            RectTransform option = menuOptions[i];

            // Calcula a posição-alvo
            float targetPositionX = (i - currentIndex) * spacing;
            float targetScale = (i == currentIndex) ? centerScale : sideScale;

            // Suaviza a movimentação e escala
            option.anchoredPosition = Vector2.Lerp(option.anchoredPosition, new Vector2(targetPositionX, 0), Time.deltaTime * lerpSpeed);
            option.localScale = Vector3.Lerp(option.localScale, Vector3.one * targetScale, Time.deltaTime * lerpSpeed);
        }
    }

    /// <summary>
    /// O botão para adicionar o hoverSFX
    /// </summary>
    /// <param name="button"></param>
    private void SetupButtonHoverSFX(Button button)
    {
        if (button == null || hoverSFX == null || audioManager == null) return;

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
                audioManager.PlaySoundFXClipUnscaled(hoverSFX, transform, VOL, false);
            }
        });

        // Adiciona o evento ao trigger
        eventTrigger.triggers.Add(hoverEnter);
    }
}
