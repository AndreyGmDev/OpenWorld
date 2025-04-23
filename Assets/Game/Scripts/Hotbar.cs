using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public GameObject[] itens;
    public GameObject item;

    private InputSystem_Actions inputActions;
    private float slotAnt;
    private float slot = 1;
    [HideInInspector] public float saveSlot = 1;

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
        int n = 0;
        foreach (var item in itens)
        {
            if (item != null)
            {
                n++;
            }
        }

        slot = inputActions.Game.Slots.ReadValue<float>();
        slot = Mathf.Clamp(slot, 0, n);

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
            if (itens[i] != null)
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
