using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BelfryTwo_SecondRoom : MonoBehaviour
{
    [Header("Lever")]
    [SerializeField] GameObject lever;
    [SerializeField] GameObject platformLever;
    [SerializeField] Vector3 platformLeverFinalPosition;
    [SerializeField] float durationPlatformLever;
    private float delayLever;

    [Header("Target 1")]
    [SerializeField] GameObject target1;
    [SerializeField] GameObject platformTarget1;
    [SerializeField] Vector3 platformTarget1FinalPosition;
    [SerializeField] float durationPlatformTarget1;
    private float delayTarget1;

    [Header("Target 2")]
    [SerializeField] GameObject target2;
    [SerializeField] GameObject platformTarget2;
    [SerializeField] Vector3 platformTarget2FinalPosition;
    [SerializeField] float durationPlatformTarget2;
    private float delayTarget2;

    [Header("Time")]
    [SerializeField, Tooltip("Time until the platform reaches FinalPosition")] float time = 1;

    private PlayerInteraction playerInteraction;
    private InputActionsManager input;
    private void Start()
    {
        input = InputActionsManager.Instance;
        playerInteraction = FindFirstObjectByType<PlayerInteraction>();
    }
    private void Update()
    {   
        delayLever -= Time.deltaTime; // Delay para poder usar a lever novamente.

        // Se a alavanca for a interação mais próxima
        if (lever == playerInteraction.NextInteraction())
        {
            // Se o cooldown da alavanca tiver acabado
            if (delayLever < 0)
            {
                if (input.inputActions.Game.Interaction.WasPressedThisFrame())
                {
                    StartCoroutine(Platform(platformLever, platformLeverFinalPosition, durationPlatformLever)); // Levanta a plataforma.
                    delayLever = durationPlatformLever + (time * 2); // Calcula o cooldown.
                }
                else
                {
                    playerInteraction.HasInteracted(false);
                }
            }
            else
            {
                playerInteraction.HasInteracted(true); // Não permite o prompt aparecer enquanto estiver no cooldown da alavanca.
            }
        }

        if (target1.TryGetComponent<Target>(out var script))
        {
            delayTarget1 -= Time.deltaTime; // Delay para poder usar o alvo novamente.

            // Se o alvo for acertado e o cooldown estiver terminado.
            if (script.WasCollided() && delayTarget1 <= 0)
            {
                StartCoroutine(Platform(platformTarget1, platformTarget1FinalPosition, durationPlatformTarget1)); // Levanta a plataforma.
                delayTarget1 = durationPlatformTarget1 + (time * 2); // Calcula o cooldown.
            }
        }

        if (target2.TryGetComponent<Target>(out var script2))
        {
            delayTarget2 -= Time.deltaTime; // Delay para poder usar o alvo novamente.

            // Se o alvo for acertado e o cooldown estiver terminado.
            if (script2.WasCollided() && delayTarget2 <= 0)
            {
                StartCoroutine(Platform(platformTarget2, platformTarget2FinalPosition, durationPlatformTarget2)); // Levanta a plataforma.
                delayTarget2 = durationPlatformTarget2 + (time * 2); // Calcula o cooldown.
            }
        }
    }

    private IEnumerator Platform(GameObject platform, Vector3 finalPosition, float durationPlatform)
    {
        // Salva a posição inicial da plataforma.
        Vector3 startPosition = platform.transform.position;

        // Calcula distancia entre a posição inicial e final da plataform.
        float distance = Vector3.Distance(finalPosition, platform.transform.position);

        // Calcula a velocidade de cada plataforma para chegar ao ponto final de acordo com o valor da variável 'time'.
        float speed = distance / time;

        while (Vector3.Distance(finalPosition, platform.transform.position) > 0.1)
        {
            // Movimenta a plataforma para o 'FinalPosition'.
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, finalPosition, speed * Time.deltaTime);

            yield return new WaitForNextFrameUnit();
        }

        yield return new WaitForSeconds(durationPlatform);

        while (Vector3.Distance(startPosition, platform.transform.position) > 0.1)
        {
            // Movimenta a plataforma para o 'FinalPosition'.
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, startPosition, speed * Time.deltaTime);

            yield return new WaitForNextFrameUnit();
        }
    }
}
