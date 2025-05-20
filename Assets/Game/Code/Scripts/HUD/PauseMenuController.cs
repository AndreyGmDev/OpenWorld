using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PauseMenuController : MonoBehaviour
{
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

    private int currentIndex = 0;
    private bool isPaused = false;

    InputActionsManager input;
    void Start()
    {
        input = InputActionsManager.Instance;

        ArrangeOptions();
        pauseCanvas.enabled = false; // Desativa o menu inicialmente


        // Ação dos botões do PauseCanvas.
        if (resumeButton != null && resumeCanvas != null)
        {
            resumeButton.onClick.AddListener(DisablePauseCanvas);
        }

        if (settingsButton != null && settingsCanvas != null)
        {
            settingsButton.onClick.AddListener(() => settingsCanvas.SetActive(true));
        }
        
        if (exitButton != null && exitCanvas != null)
        {
            exitButton.onClick.AddListener(() => exitCanvas.SetActive(true));
        }
        
        if (exitButtonNo != null && exitCanvas != null)
        {
            exitButtonNo.onClick.AddListener(() => exitCanvas.SetActive(false));
        }

        if (exitButtonYes != null) { 

            
            exitButtonYes.onClick.AddListener(() => StartCoroutine(LoadingManager.Instance.LoadAsyncScene("MainMenu")));
        }

        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveGame.Instance.MakeSaves);
        }
    }
    private void DisablePauseCanvas()
    {
        isPaused = !isPaused;
        pauseCanvas.enabled = isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

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
        if (input.inputActions.UI.Pause.WasPressedThisFrame())
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
}
