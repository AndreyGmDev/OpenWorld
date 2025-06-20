using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(JoinExitBelfry))] // Essa quest sempre manda o player para uma cena.
public class Hat_Quest : MonoBehaviour
{
    [SerializeField, Tooltip("Script que será ativado quando o chapéu for coletado.")] JoinExitBelfry scriptToActivate;

    private SaveGame saveGame;
    private PlayerInteraction playerInteraction;
    private InputActionsManager input;
    private DialogueManager dialogue;

    private static int hatsCollected = 0;

    private void Start()
    {
        saveGame = SaveGame.Instance;
        playerInteraction = FindFirstObjectByType<PlayerInteraction>();
        input = InputActionsManager.Instance;
        dialogue = DialogueManager.Instance;

        Load();
    }

    private void Update()
    {
        // Confere se o player está interagindo com o chapéu.
        if (playerInteraction.NextInteraction() == gameObject)
        {
            if (input.inputActions.Game.Interaction.WasPressedThisFrame())
            {
                UpdateQuest();
            }
        }
    }

    private IEnumerator UpdateQuest()
    {
        // Atualiza o número de chapéus coletados.
        hatsCollected++;

        // Salva a parte da quest que foi concluida.
        Save();

        // Impede o código de continuar enquanto o player estiver em dialogo.
        while (dialogue.IsInDialogue())
        {
            yield return new WaitForNextFrameUnit();
        }

        // Confere se a quest foi completada.
        if (hatsCollected == 4)
        {
            FinishQuest(); // Finaliza a quest.
        }

        // Ativa o script de trocar de cena.
        scriptToActivate.enabled = true;

        // Destói o objeto depois de coletá-lo.
        Destroy(gameObject);
    }

    private void Load()
    {
        SaveGameInfos save = saveGame.LoadData();
        hatsCollected = save.HatsCollected;
    }

    private void Save()
    {
        saveGame.SaveHatQuest(new SaveGameInfos
        {
            HatsCollected = hatsCollected
        });
    }

    private void FinishQuest()
    {

    }
    
}
