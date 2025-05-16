using System.Collections;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] float speed;

    InputActionsManager input;
    private void Start()
    {
        input = InputActionsManager.Instance;
    }

    private void Update()
    {
        if (input.inputActions.Game.Shoot.WasPressedThisFrame())
        {
            if (TryShoot())
            {
                print("oi");

                StartCoroutine(nameof(ThrowHook));
                // Todo - hook funcionar.
            }
        }
    }

    // Confere se o Hook colide com algo, para poder ser usado.
    private bool TryShoot()
    {
        LayerMask layer = LayerMask.GetMask("UseHook");
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer);
        
        return hit.collider;
    }

    private IEnumerator ThrowHook()
    {
        input.inputActions.Game.Move.Disable();

        yield return new WaitForSeconds(2);

        input.EnableGameActions();
    }
}
