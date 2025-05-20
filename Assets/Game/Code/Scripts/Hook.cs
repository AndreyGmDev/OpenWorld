using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField, Tooltip("Velocidade do player usando o Hook")] float speed;
    [SerializeField, Tooltip("Direção para onde o player vai eixo y em relação ao ponto final. PosiçãoFinal.y + directionDown ")] float directionDown;

    InputActionsManager input;
    private void Start()
    {
        input = InputActionsManager.Instance;
    }

    private void Update()
    {
        if (input.inputActions.Game.Shoot.WasPressedThisFrame())
        {
            GameObject platform = TryShoot();
            if (platform)
            {
                StartCoroutine(ThrowHook(platform.transform.position));
            }
        }
    }

    // Confere se o Hook colide com algo, para poder ser usado.
    private GameObject TryShoot()
    {
        LayerMask layer = LayerMask.GetMask("UseHook");
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer);
        
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    private IEnumerator ThrowHook(Vector3 isGoingTo)
    {
        // Desativa os inputs de movimentação do player.
        input.DisableGameActions();
        input.inputActions.Game.Look.Enable();

        isGoingTo.y += directionDown;
        PlayerController player = FindFirstObjectByType<PlayerController>();

        // Calcula a rotação que o player deve seguir.
        Vector3 rotationEulerAngles = isGoingTo - player.characterMovement.transform.position;
        rotationEulerAngles.y = 0;
        rotationEulerAngles.Normalize();
        player.characterMovement.motor.RotateCharacter(Quaternion.LookRotation(rotationEulerAngles));

        // Passa as informações para o CharacterMovement que é responsável pela movimentação do player.
        player.characterMovement.HookActions(new UsingHook
        {
            IsUsingHook = true,
            WhereIsGoing = isGoingTo,
            HookMaxSpeed = speed,
        });

        yield return new WaitForSeconds(2);

        input.EnableGameActions();
    }
}
