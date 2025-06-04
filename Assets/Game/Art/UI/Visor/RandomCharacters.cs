using UnityEngine;
using TMPro;

public class RandomCharacters : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro; // Arraste o TextMeshPro pelo inspetor.
    public int characterCount = 50; // Quantidade de caracteres a exibir.
    public float updateInterval = 0.5f; // Intervalo de atualização em segundos.

    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            textMeshPro.text = GenerateRandomString(characterCount);
            timer = 0f;
        }
    }

    private string GenerateRandomString(int length)
    {
        char[] randomChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            randomChars[i] = chars[Random.Range(0, chars.Length)];
        }
        return new string(randomChars);
    }
}
