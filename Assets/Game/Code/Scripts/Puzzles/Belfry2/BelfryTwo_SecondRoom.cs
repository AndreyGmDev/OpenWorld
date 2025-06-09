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

    [Header("Target 1")]
    [SerializeField] GameObject target1;
    [SerializeField] GameObject platformTarget1;
    [SerializeField] Vector3 platformTarget1FinalPosition;
    [SerializeField] float durationTarget1Lever;

    [Header("Target 2")]
    [SerializeField] GameObject target2;
    [SerializeField] GameObject platformTarget2;
    [SerializeField] Vector3 platformTarget2FinalPosition;
    [SerializeField] float durationTarget2Lever;

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
        if (lever == playerInteraction.NextInteraction() && input.inputActions.Game.Interaction.WasPressedThisFrame())
        {
            StartCoroutine(Platform(platformLever, platformLeverFinalPosition, 5));
        }

        if (target1.TryGetComponent<Target>(out var script))
        {
            if (script.WasCollided())
            {
                StartCoroutine(Platform(platformTarget1, platformTarget1FinalPosition, 5));
            }
        }

        if (target2.TryGetComponent<Target>(out var script2))
        {
            if (script2.WasCollided())
            {
                StartCoroutine(Platform(platformTarget2, platformTarget2FinalPosition, 5));
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

        while (Vector3.Distance(finalPosition, platform.transform.position) > 0.1)
        {
            // Movimenta a plataforma para o 'FinalPosition'.
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, startPosition, speed * Time.deltaTime);

            yield return new WaitForNextFrameUnit();
        }
    }
}
