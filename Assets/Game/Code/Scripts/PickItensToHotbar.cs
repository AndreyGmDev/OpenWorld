using UnityEngine;

public class PickItensToHotbar : MonoBehaviour
{
    [SerializeField] GameObject obj;
    [SerializeField] int elementArray;

    private InputActionsManager input;
    private PlayerInteraction playerInteraction;
    private Hotbar hotbar;

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f; // Velocidade de rotação no eixo Y

    [Header("Oscillation Settings")]
    public float oscillationAmplitude = 0.5f; // Amplitude do movimento (distância vertical)
    public float oscillationSpeed = 2f; // Velocidade da oscilação

    private Vector3 initialPosition; // Posição inicial do objeto

    private void Start()
    {
        initialPosition = transform.position;
        input = FindAnyObjectByType<InputActionsManager>();
        playerInteraction = FindFirstObjectByType<PlayerInteraction>();
        hotbar = FindAnyObjectByType<PlayerController>().hotbar;
    }

    private void Update()
    {
        if (obj == null) return;

        if (playerInteraction.NextInteraction() == gameObject)
        {
            if (input.inputActions.Game.Interaction.WasPressedThisFrame())
            {
                HotbarBase hotbarBase = hotbar.GetIDByItem(obj);

                if (hotbarBase != null)
                {
                    hotbar.itens[elementArray] = hotbarBase.prefab;
                    hotbar.UpdateHUD();
                }

                Destroy(gameObject);
            }
        }

        // Rotação no eixo Y
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

        // Oscilação de cima para baixo
        float newY = initialPosition.y + Mathf.Sin(Time.time * oscillationSpeed) * oscillationAmplitude;
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}
