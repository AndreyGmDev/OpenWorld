using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonMouseEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject selectionHighlight; // Objeto de sele��o
    public Image buttonImage; // Imagem do bot�o
    public TextMeshProUGUI buttonText; // Texto do bot�o
    public Color highlightColor = Color.yellow; // Cor de destaque do bot�o
    public Color originalColor = Color.white; // Cor original do bot�o
    public Color textOscillationColor = Color.red; // Cor alternativa para oscila��o do texto
    public Animator otherElementAnimator; // Animator de outro elemento
    public string otherElementAnimationTrigger = "PlayAnimation"; // Trigger do outro elemento
    public float textOscillationSpeed = 2f; // Velocidade de oscila��o do texto

    private RectTransform selectionRectTransform; // RectTransform do objeto de sele��o
    private RectTransform buttonRectTransform; // RectTransform do bot�o
    private bool isMouseOver; // Controle para saber se o mouse est� sobre o bot�o

    private void Start()
    {
        // Inicializa o RectTransform do bot�o e do objeto de sele��o
        buttonRectTransform = GetComponent<RectTransform>();

        if (selectionHighlight != null)
        {
            selectionRectTransform = selectionHighlight.GetComponent<RectTransform>();
            selectionHighlight.SetActive(false); // Esconde o objeto de sele��o inicialmente
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;

        // Mostra o objeto de sele��o e move instantaneamente para o bot�o
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(true);
            MoveSelectionToButton();
        }

        // Toca a anima��o de outro elemento
        if (otherElementAnimator != null)
        {
            otherElementAnimator.SetTrigger(otherElementAnimationTrigger);
        }

        // Inicia a oscila��o do texto
        if (buttonText != null)
        {
            StartCoroutine(OscillateTextColor());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;

        // Esconde o objeto de sele��o
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(false);
        }

        // Restaura a cor do bot�o
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
            // Move a sele��o para a posi��o do bot�o
            selectionRectTransform.position = buttonRectTransform.position;
        }
    }

    private System.Collections.IEnumerator OscillateTextColor()
    {
        while (isMouseOver)
        {
            // Alterna entre a cor original e a cor de oscila��o
            if (buttonText != null)
            {
                float t = (Mathf.Sin(Time.time * textOscillationSpeed) + 1) / 2; // Oscila��o suave entre 0 e 1
                buttonText.color = Color.Lerp(originalColor, textOscillationColor, t);
            }

            yield return null; // Espera at� o pr�ximo frame
        }
    }
}
