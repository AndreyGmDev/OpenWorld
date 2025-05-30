using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public GameObject[] itens = new GameObject[4];

    private InputActionsManager input;
    private float slotAnt;
    private float slot = 1;
    [HideInInspector] public float saveSlot = 1;

    private void Start()
    {
        input = InputActionsManager.Instance;

        Load();
        slot = saveSlot;
        ChangeSlot();
    }

    private void Update()
    {
        slot = input.inputActions.Game.Slots.ReadValue<float>(); // Captura o slot de acordo com o input pressionado.

        if (slot > 0)
        {
            slot = itens[Mathf.RoundToInt(slot - 1)] != null ? slot : slotAnt; // Se houver algum item mantem no novo slot, se não houver, volta para o slot anterior.
        }

        if (slot != slotAnt)
        {
            ChangeSlot();
            slotAnt = slot;
        }
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
        itens[Mathf.RoundToInt(slot - 1)].SetActive(true);
    }

    // Carrega as informações do SaveGame.
    private void Load()
    {
        SaveGame saveGame = SaveGame.Instance;

        if (saveGame != null)
        {
            SaveGameInfos save = saveGame.LoadData();

            saveSlot = save.Slot;
            itens = save.Itens;
        }
    }
}
