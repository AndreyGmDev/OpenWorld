using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public GameObject[] itens;

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
        int n = 0;
        foreach (var item in itens)
        {
            if (item != null)
            {
                n++;
            }
        }

        slot = input.inputActions.Game.Slots.ReadValue<float>();
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

    // Carrega as informações do SaveGame.
    private void Load()
    {
        SaveGame saveGame = SaveGame.Instance;

        if (saveGame != null)
        {
            SaveGameInfos save = saveGame.LoadPlayerData();

            saveSlot = save.Slot;
        }
    }
}
