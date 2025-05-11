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

    [Header("Canvas")]
    [SerializeField] GameObject settingsCanvas;
    [SerializeField] GameObject resumeCanvas;
    [SerializeField] GameObject exitCanvas;

    private int currentIndex = 0;
    private bool isPaused = false;

    InputSystem_Actions inputActions;
    void Start()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
        ArrangeOptions();
        pauseCanvas.enabled = false; // Desativa o menu inicialmente

        resumeButton.onClick.AddListener(DisablePauseCanvas);

        settingsButton.onClick.AddListener(() => SetActive(settingsCanvas, true));
        saveButton.onClick.AddListener(SaveGame.instance.MakeSaves);
        exitButton.onClick.AddListener(() => SetActive(exitCanvas, true));
    }
    private void DisablePauseCanvas()
    {
        isPaused = !isPaused;
        pauseCanvas.enabled = isPaused;
        Time.timeScale = isPaused ? 0 : 1;

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
        if (inputActions.UI.Pause.WasPressedThisFrame())
        {
            DisablePauseCanvas();
        }

        if (!isPaused) return;

        // Navegação pelo teclado
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex = Mathf.Max(0, currentIndex - 1);
            ArrangeOptions();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex = Mathf.Min(menuOptions.Count - 1, currentIndex + 1);
            ArrangeOptions();
        }

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
