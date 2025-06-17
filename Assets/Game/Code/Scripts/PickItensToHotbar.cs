using UnityEngine;

public class PickItensToHotbar : MonoBehaviour
{
    [SerializeField] GameObject obj;
    [SerializeField] int elementArray;

    private InputActionsManager input;
    private PlayerInteraction playerInteraction;
    private Hotbar hotbar;

    private void Start()
    {
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
                }
            }
        }
    }
}
