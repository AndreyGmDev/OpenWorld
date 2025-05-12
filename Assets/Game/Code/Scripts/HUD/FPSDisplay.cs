using System.Collections;
using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    [SerializeField] float updateFPS = 0.15f;
    private TextMeshProUGUI fpsText;

    private void Start()
    {
        if (GetComponent<TextMeshProUGUI>() != null)
        {
            fpsText = GetComponent<TextMeshProUGUI>();
        }
        
        if (fpsText != null)
        {
            StartCoroutine(nameof(ShowFPS));
        }
    }

    private IEnumerator ShowFPS()
    {
        while (true)
        {
            fpsText.text = Mathf.Floor(1f / Time.unscaledDeltaTime) + " FPS";

            yield return new WaitForSecondsRealtime(updateFPS);
        }
    }




}
