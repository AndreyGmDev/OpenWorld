using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonMouseEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject selectionHighlight; // Objeto de seleção
    public Image buttonImage; // Imagem do botão
    public TextMeshProUGUI buttonText; // Texto do botão
    public Color highlightColor = Color.yellow; // Cor de destaque do botão
    public Color originalColor = Color.white; // Cor original do botão
    public Color textOscillationColor = Color.red; // Cor alternativa para oscilação do texto
    public Animator otherElementAnimator; // Animator de outro elemento
    public string otherElementAnimationTrigger = "PlayAnimation"; // Trigger do outro elemento
    public float textOscillationSpeed = 2f; // Velocidade de oscilação do texto

    private RectTransform selectionRectTransform; // RectTransform do objeto de seleção
    private RectTransform buttonRectTransform; // RectTransform do botão
    private bool isMouseOver; // Controle para saber se o mouse está sobre o botão

    private void Start()
    {
        // Inicializa o RectTransform do botão e do objeto de seleção
        buttonRectTransform = GetComponent<RectTransform>();

        if (selectionHighlight != null)
        {
            selectionRectTransform = selectionHighlight.GetComponent<RectTransform>();
            selectionHighlight.SetActive(false); // Esconde o objeto de seleção inicialmente
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;

        // Mostra o objeto de seleção e move instantaneamente para o botão
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(true);
            MoveSelectionToButton();
        }

        // Toca a animação de outro elemento
        if (otherElementAnimator != null)
        {
            otherElementAnimator.SetTrigger(otherElementAnimationTrigger);
        }

        // Inicia a oscilação do texto
        if (buttonText != null)
        {
            StartCoroutine(OscillateTextColor());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;

        // Esconde o objeto de seleção
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(false);
        }

        // Restaura a cor do botão
        if (buttonImage != null)
        {
            buttonImage.color = originalColor;
        }

        // Restaura a cor original do texto
        if (buttonText != null)
        {
            buttonText.color = originalColor;
        }
    }

    private void MoveSelectionToButton()
    {
        if (selectionRectTransform != null && buttonRectTransform != null)
        {
            // Move a seleção para a posição do botão
            selectionRectTransform.position = buttonRectTransform.position;
        }
    }

    private System.Collections.IEnumerator OscillateTextColor()
    {
        while (isMouseOver)
        {
            // Alterna entre a cor original e a cor de oscilação
            if (buttonText != null)
            {
                float t = (Mathf.Sin(Time.time * textOscillationSpeed) + 1) / 2; // Oscilação suave entre 0 e 1
                buttonText.color = Color.Lerp(originalColor, textOscillationColor, t);
            }

            yield return null; // Espera até o próximo frame
        }
    }
}
