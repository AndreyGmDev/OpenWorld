using UnityEngine;
using TMPro;

public class SceneTextFade : MonoBehaviour
{
    public TextMeshProUGUI transitionText; // Referência ao TextMeshPro
    public float fadeDuration = 1.0f;      // Duração do fade in/out
    public float displayTime = 2.0f;       // Tempo que o texto fica visível

    private void Start()
    {
        if (transitionText != null)
        {
            transitionText.alpha = 0; // Garante que o texto começa invisível
            StartCoroutine(ShowText());
        }
    }

    private System.Collections.IEnumerator ShowText()
    {
        // Fade In
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            transitionText.alpha = alpha;
            yield return null;
        }

        // Mantenha o texto visível por um tempo
        yield return new WaitForSeconds(displayTime);

        // Fade Out
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            transitionText.alpha = alpha;
            yield return null;
        }

        // Após o fade out, o texto fica completamente invisível
        transitionText.alpha = 0;
    }
}
