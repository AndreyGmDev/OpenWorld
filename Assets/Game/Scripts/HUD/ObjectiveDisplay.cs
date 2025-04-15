using System.Collections;
using TMPro;
using UnityEngine;

public class ObjectiveDisplay : MonoBehaviour
{
    public TextMeshProUGUI newObjectiveText; // Referência para o texto "NOVO OBJETIVO!"
    public TextMeshProUGUI objectiveText;   // Referência para o texto do objetivo
    public string objectiveMessage = "Complete a primeira missão"; // Texto do objetivo
    public float letterDelay = 0.1f; // Tempo entre o surgimento de cada letra
    public float fadeDuration = 1f; // Duração do fade-in e expansão

    private void Start()
    {
        StartCoroutine(DisplayObjective());
    }

    private IEnumerator DisplayObjective()
    {
        // Efeito do texto "NOVO OBJETIVO!"
        yield return ShowTextRandomly(newObjectiveText, "NOVO OBJETIVO!");

        // Pequena pausa antes de mostrar o objetivo
        yield return new WaitForSeconds(0.5f);

        // Efeito de fade-in e expansão do objetivo
        yield return FadeInAndExpand(objectiveText, objectiveMessage);
    }

    private IEnumerator ShowTextRandomly(TextMeshProUGUI textComponent, string message)
    {
        textComponent.text = ""; // Limpar o texto inicialmente
        char[] letters = message.ToCharArray();
        bool[] revealed = new bool[letters.Length];
        int remaining = letters.Length;

        while (remaining > 0)
        {
            // Escolher uma letra aleatória ainda não exibida
            int index = Random.Range(0, letters.Length);
            if (!revealed[index])
            {
                revealed[index] = true;
                remaining--;
                textComponent.text = ReplaceWithRevealed(letters, revealed);
                yield return new WaitForSeconds(letterDelay);
            }
        }
    }

    private string ReplaceWithRevealed(char[] letters, bool[] revealed)
    {
        string result = "";
        for (int i = 0; i < letters.Length; i++)
        {
            result += revealed[i] ? letters[i].ToString() : " ";
        }
        return result;
    }

    private IEnumerator FadeInAndExpand(TextMeshProUGUI textComponent, string message)
    {
        textComponent.text = message;
        textComponent.alpha = 0;
        textComponent.transform.localScale = Vector3.zero;

        float timer = 0f;
        while (timer <= fadeDuration)
        {
            float progress = timer / fadeDuration;
            textComponent.alpha = progress;
            textComponent.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);

            timer += Time.deltaTime;
            yield return null;
        }

        // Certificar-se de que o texto está completamente visível e em escala 1
        textComponent.alpha = 1f;
        textComponent.transform.localScale = Vector3.one;
    }
}
