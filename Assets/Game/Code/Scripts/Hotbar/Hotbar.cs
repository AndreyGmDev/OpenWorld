using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public GameObject[] itens = new GameObject[3];

    //HUD

    public RectTransform[] slotTransforms = new RectTransform[4]; 
    public Image[] slotImages = new Image[4];
    public GameObject[] slotSelection = new GameObject[4];
    public Color selectedColor = Color.yellow; 
    public Color defaultColor = Color.white;
    public float ogSize = 0.5f;

    private InputActionsManager input;
    private float slotAnt;
    private float slot = 1;
    [HideInInspector] public float saveSlot = 1;

    private void Start()
    {
        input = InputActionsManager.Instance;

        //Load();
        slot = saveSlot;
        ChangeSlot();
        UpdateHUD();
    }

    private void Update()
    {
        slot = input.inputActions.Game.Slots.ReadValue<float>(); // Captura o slot de acordo com o input pressionado.

        if (slot > 0)
        {
            slot = itens[Mathf.RoundToInt(slot - 1)] != null ? slot : slotAnt; // Se houver algum item mantem no novo slot, se n�o houver, volta para o slot anterior.
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

        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (slotSelection.Any(x => x == null)) return;

        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i == Mathf.RoundToInt(slot - 1))
            {
                // Slot selecionado

                slotSelection[i].SetActive(true);
                slotImages[i].color = selectedColor;
                //slotImages[i].transform.localScale = new Vector3(ogSize, ogSize, ogSize) * 1.0f; // Aumenta o tamanho
            }
            else
            {
                // Slots não selecionados
                slotSelection[i].SetActive(false);
                slotImages[i].color = defaultColor;
                //slotImages[i].transform.localScale = new Vector3(ogSize, ogSize, ogSize); // Reseta o tamanho
            }
        }

    }

    // Carrega as informa��es do SaveGame.
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
