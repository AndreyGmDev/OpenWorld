using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PauseMenuController : MonoBehaviour
{
    public List<RectTransform> menuOptions; // Refer�ncia �s op��es do menu
    public float spacing = 800f; // Espa�o entre as op��es (ajustado para deixar fora da tela)
    public float centerScale = 1.5f; // Escala para a op��o central
    public float sideScale = 0.7f; // Escala para op��es laterais
    public float lerpSpeed = 10f; // Velocidade de anima��o
    public Canvas pauseCanvas; // Canvas do menu de pausa
    public KeyCode pauseKey = KeyCode.Escape; // Tecla para ativar/desativar o menu

    private int currentIndex = 0;
    private bool isPaused = false;

    void Start()
    {
        ArrangeOptions();
        pauseCanvas.enabled = false; // Desativa o menu inicialmente

        //Button sla;
       // sla.onClick.AddListener(csasa);
    }

    private void csasa(string map)
    {

    }
    void Update()
    {
        // Ativar/Desativar o menu de pausa
        if (Input.GetKeyDown(pauseKey))
        {
            isPaused = !isPaused;
            pauseCanvas.enabled = isPaused;
            Time.timeScale = isPaused ? 0 : 1;
        }

        if (!isPaused) return;

        // Navega��o pelo teclado
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

        // Navega��o por arrasto do mouse
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

            // Calcula a posi��o-alvo
            float targetPositionX = (i - currentIndex) * spacing;
            float targetScale = (i == currentIndex) ? centerScale : sideScale;

            // Suaviza a movimenta��o e escala
            option.anchoredPosition = Vector2.Lerp(option.anchoredPosition, new Vector2(targetPositionX, 0), Time.deltaTime * lerpSpeed);
            option.localScale = Vector3.Lerp(option.localScale, Vector3.one * targetScale, Time.deltaTime * lerpSpeed);
        }
    }
}
