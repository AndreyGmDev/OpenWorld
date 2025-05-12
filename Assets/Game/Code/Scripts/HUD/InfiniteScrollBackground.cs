using UnityEngine;
using UnityEngine.UI;

public class InfiniteScrollBackground : MonoBehaviour
{
    [SerializeField] private RawImage backgroundImage; // Referência ao RawImage do background
    [SerializeField] private float scrollSpeed = 0.1f; // Velocidade de scroll

    private Vector2 offset = Vector2.zero;

    void Update()
    {
        // Atualiza o offset baseado no tempo e na velocidade
        offset.x += scrollSpeed * Time.deltaTime;

        // Aplica o offset no material do RawImage
        backgroundImage.uvRect = new Rect(offset.x, offset.y, backgroundImage.uvRect.width, backgroundImage.uvRect.height);
    }
}
