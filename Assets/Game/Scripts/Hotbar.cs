using UnityEngine;

public class Hotbar : MonoBehaviour
{
    [SerializeField] GameObject[] itens;

    private InputSystem_Actions inputActions;
    private float slotAnt;
    private float slot = 1;
    public float saveSlot = 1;
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    private void Start()
    {
        slot = saveSlot;
        ChangeSlot();
    }

    private void Update()
    {
        slot = inputActions.Game.Slots.ReadValue<float>();

        if (slot != slotAnt)
        {
            ChangeSlot();
        }

        slotAnt = slot;
    }

    private void ChangeSlot()
    {
        if (itens == null || slot == 0) return;

        saveSlot = slot;

        // Desativa todos os itens primeiro.
        for (int i = 0; i < itens.Length; i++)
        {
            itens[i].SetActive(false);
        }

        // Ativa o item selecionado.
        for (int i = 0; i < itens.Length; i++)
        {
            if (slot == i + 1)
            {
                itens[i].SetActive(true);
            }
        }
    }
}
