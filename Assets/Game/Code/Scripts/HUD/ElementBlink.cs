using UnityEngine;
using TMPro;

public class BlinkingTMP : MonoBehaviour
{
    public float blinkInterval = 0.5f; // Tempo entre vis�vel e invis�vel
    private TextMeshProUGUI textMeshPro;
    private bool isVisible = true;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        InvokeRepeating(nameof(ToggleVisibility), 0f, blinkInterval);
    }

    void ToggleVisibility()
    {
        isVisible = !isVisible;
        Color currentColor = textMeshPro.color;
        currentColor.a = isVisible ? 1f : 0f; // 1 = vis�vel, 0 = invis�vel
        textMeshPro.color = currentColor;
    }
}
