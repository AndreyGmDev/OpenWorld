using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField, Tooltip("Velocidade do player usando o Hook")] float speed;
    [SerializeField, Tooltip("Direção para onde o player vai eixo y em relação ao ponto final. PosiçãoFinal.y + directionDown ")] float directionDown;
    
    private LineRenderer lineRenderer;

    private List<GameObject> hookTargets = new List<GameObject>();

    InputActionsManager input;
    private void Start()
    {
        input = InputActionsManager.Instance;

        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        // Pega o HookTarget que está sendo mirado.
        GameObject currentHookTarget = TryShoot();

        // Confere um a um HookTarget.
        for (int i = 0; i < hookTargets.Count; i++)
        {
            // Se não houver um HookTarget atual, desativa todos os que estão na Lista.
            if (currentHookTarget == null) // Pega todos os HookTargets.
            {
                hookTargets[i].GetComponent<MeshRenderer>().enabled = false; // Desativa a border dos HookTargets antigos.
                hookTargets.RemoveAt(i); // Remove da lista todos os HookTargets antigos.
            }
            else if (hookTargets[i] != currentHookTarget) // Se houver um HookTarget atual, desativa todos os que estão na Lista, exceto o atual.
            {
                hookTargets[i].GetComponent<MeshRenderer>().enabled = false; // Desativa a border dos HookTargets antigos.
                hookTargets.RemoveAt(i); // Remove da lista todos os HookTargets antigos.
            }
        }

        // Confere se o HookTarget atual já está na lista para não repetir os processos.
        if (!hookTargets.Contains(currentHookTarget))
        {
            if (currentHookTarget != null)
            {
                currentHookTarget.GetComponent<MeshRenderer>().enabled = true; // Ativa a border do HookTarget atual.
                hookTargets.Add(currentHookTarget); // Adiciona na lista os HookTargets que estão ativados para poder desativa-los no futuro.
            }
        }

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

        if (lineRenderer != null)
        {
            lineRenderer.enabled = true; // Ativa a linha.
            lineRenderer.SetPosition(1, new Vector3(isGoingTo.x, isGoingTo.y - directionDown, isGoingTo.z));
        }
        
        while (Vector3.Distance(isGoingTo, player.characterMovement.transform.position) > 1.2f)
        {
            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, transform.position + Vector3.up * 0.9f);
            }
            
            yield return null;
        }

        // Desativa a linha quando acaba de usar o Hook.
        lineRenderer.enabled = false;
    }
}
