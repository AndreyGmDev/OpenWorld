using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class BlinkingTMP : MonoBehaviour
{
    public float blinkInterval = 0.5f; // Tempo entre vis�vel e invis�vel
    private TextMeshProUGUI textMeshPro;
    private bool isVisible = true;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        StartCoroutine(nameof(ToggleVisibility));
    }

    IEnumerator ToggleVisibility()
    {
        Color currentColor = textMeshPro.color;

        while (true)
        {
            isVisible = Time.timeScale >= 0.5 ? true : !isVisible; // Enquanto o jogo funcionar n�o vai ficar piscando.
            
            currentColor.a = isVisible ? 1f : 0f; // 1 = vis�vel, 0 = invis�vel
            textMeshPro.color = currentColor;

            yield return new WaitForSecondsRealtime(blinkInterval);

            yield return new WaitForNextFrameUnit();
        }
    }
}
