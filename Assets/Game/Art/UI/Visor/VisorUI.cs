using UnityEngine;
using TMPro;

using UnityEngine.UI;

public class VisorUI : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro; // Arraste o TextMeshPro pelo inspetor.
    public int characterCount = 50; // Quantidade de caracteres a exibir.
    public float updateInterval = 0.5f; // Intervalo de atualização em segundos.


    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private float timer = 0f;

    //pOSIÇÃO
    public Transform targetObject;

    [Header("TextMeshes para exibir as coordenadas")]
    public TextMeshProUGUI textX;
    public TextMeshProUGUI textY;
    public TextMeshProUGUI textZ;

    public Image cameraImage;
    public Transform cameraPos;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            textMeshPro.text = GenerateRandomString(characterCount);
            timer = 0f;
        }

        if (targetObject == null || textX == null || textY == null || textZ == null)
        {
            Debug.LogWarning("Certifique-se de que todos os campos estão preenchidos no inspetor.");
            return;
        }

        // Obtendo as coordenadas arredondadas
        Vector3 position = targetObject.position;
        int posX = Mathf.RoundToInt(position.x);
        int posY = Mathf.RoundToInt(position.y);
        int posZ = Mathf.RoundToInt(position.z);

        // Atualizando os textos
        textX.text = "X: " + posX;
        textY.text = "Y: " + posY;
        textZ.text = "Z: " + posZ;

        if (cameraImage != null && cameraPos != null) {
            cameraImage.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -cameraPos.eulerAngles.y + -90f));

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
