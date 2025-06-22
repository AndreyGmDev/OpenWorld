using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    [Header("Itens")]
    public List<HotbarBase> itensBase = new List<HotbarBase>(); // Todos os itens que o player pode possuir.v
    public GameObject[] itens; // Itens que o player possui.

    private float slotAnt;
    private float slot = 1;
    [HideInInspector] public float saveSlot = 1;

    [Header("HUD")]
    public Image[] slotImages = new Image[4];
    public GameObject[] slotSelection = new GameObject[4];
    public Color selectedColor = Color.yellow; 
    public Color defaultColor = Color.white;
    public float ogSize = 0.5f;

    [Header("Visor Sound Effects")]
    public AudioClip visorOnSFX;
    public AudioClip visorOffSFX;

    private InputActionsManager input;
    private SaveGame saveGame;

    private bool isVisorEnabled;
    private bool wasVisorEnabledLastFrame; // Variável para rastrear o estado anterior do visor
    [SerializeField] GameObject visorUI;

    private void Start()
    {
        itens = new GameObject[itens.Length];

        input = InputActionsManager.Instance;
        saveGame = SaveGame.Instance;

        Load();

        slot = saveSlot;

        foreach (var i in slotSelection)
        {
            i.SetActive(false);
        }

        ChangeSlot();
        UpdateHUD();
    }

    private void Update()
    {
        isVisorEnabled = GameObject.Find("Visor");

        // Verifica se o estado do visor mudou desde o último frame
        if (isVisorEnabled != wasVisorEnabledLastFrame)
        {
            if (isVisorEnabled)
            {
                // Visor foi ativado
                if (visorUI != null)
                {
                    visorUI.SetActive(true);
                }
                
                // Toca o som de ativar o visor
                if (visorOnSFX != null)
                {
                    AudioManager.Instance.InterruptSFX();
                    AudioManager.Instance.PlaySoundFXClip(visorOnSFX, transform, 1f, false);
                }
            }
            else 
            {
                // Visor foi desativado
                if (visorUI != null)
                {
                    visorUI.SetActive(false);
                }
                
                // Toca o som de desativar o visor
                if (visorOffSFX != null)
                {
                    AudioManager.Instance.InterruptSFX();
                    AudioManager.Instance.PlaySoundFXClip(visorOffSFX, transform, 1f, false);
                }
            }
            
            // Atualiza o estado anterior
            wasVisorEnabledLastFrame = isVisorEnabled;
        }

        // Reseta o slot (Evita que a mudança de slot seja chamada a cada frame).
        slot = 0; 

        // Somente troca o slot se clicar no Input.
        if (input.inputActions.Game.Slots.WasPerformedThisFrame())
        {
            slot = input.inputActions.Game.Slots.ReadValue<float>(); // Captura o slot de acordo com o input pressionado.
        }

        // Confere se há um item no slot pressionado.
        if (slot > 0)
        {
            slot = itens[Mathf.RoundToInt(slot - 1)] != null ? slot : slotAnt; // Se houver algum item mantem no novo slot, se não houver, volta para o slot anterior.
        }
        
        if (slot != 0)
        {
            ChangeSlot();
        }

        Save(); // Passa as informações para o SaveGame.
    }

    private void ChangeSlot()
    {
        if (itens == null) return;

        saveSlot = slot;

        // Desativa todos os itens primeiro.
        for (int i = 0; i < itens.Length; i++)
        {
            if (itens[i] != null)
                itens[i].SetActive(false);
        }

        // Ativa o item selecionado.
        if (slot != slotAnt)
        {
            itens[Mathf.RoundToInt(slot - 1)].SetActive(true);
        }
        else
        {
            slot = 0;
        }

        slotAnt = slot;

        UpdateHUD(); // Atualiza a HUD.
    }

    public void UpdateHUD()
    {
        for (int i = 0; i < itens.Length; i++)
        {
            if (itens[i] == null)
            {
                slotImages[i].GetComponent<Image>().enabled = false;
            }
            else
            {
                slotImages[i].GetComponent<Image>().enabled = true;
            }

        }

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

    // Passa as informações para o SaveGame.
    private void Save()
    {
        string[] itensID = new string[itens.Length];

        // Carrega todos os itens salvos pelo player.
        for (int i = 0; i < itens.Length; i++)
        {
            if (itens[i] != null)
            {
                HotbarBase hotbarBase = GetIDByItem(itens[i]);

                if (hotbarBase.itemID != null)
                {
                    itensID[i] = hotbarBase.itemID;
                }
            }
        }

        // Faz o save.
        saveGame.SaveHotbarData(new SaveGameInfos
        {
            Slot = saveSlot,
            ItensID = itensID,
        });
    }

    // Carrega as informações do SaveGame.
    private void Load()
    {
        if (saveGame != null)
        {
            SaveGameInfos save = saveGame.LoadData();
            saveSlot = save.Slot; // Carrega o slot que o player está usando.

            if (save.ItensID != null)
            {
                // Carrega todos os itens salvos pelo player.
                for (int i = 0; i < itens.Length; i++)
                {
                    if (!string.IsNullOrEmpty(save.ItensID[i]))
                    {
                        HotbarBase hotbarBase = GetItemByID(save.ItensID[i]);

                        if (hotbarBase != null)
                        {
                            itens[i] = hotbarBase.prefab;
                        }
                    }
                }
            }
        }
    }

    public HotbarBase GetItemByID(string ID)
    {
        return itensBase.Find(itens => itens.itemID == ID);
    }

    public HotbarBase GetIDByItem(GameObject prefab)
    {
        return itensBase.Find(itens => itens.prefab == prefab);
    }
}

[System.Serializable]
public class HotbarBase
{
    public GameObject prefab;
    public string itemID;
}