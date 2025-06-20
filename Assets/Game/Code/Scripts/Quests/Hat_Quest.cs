using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(JoinExitBelfry))] // Essa quest sempre manda o player para uma cena.
public class Hat_Quest : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Dialogo que será iniciado quando o chapéu for coletado. Não é obrigatório")] DialogueTrigger startDialogue;

    private SaveGame saveGame;
    private PlayerInteraction playerInteraction;
    private InputActionsManager input;
    private DialogueManager dialogue;
    private JoinExitBelfry scriptToActivate; // Script que será ativado quando o chapéu for coletado.

    private static int hatsCollected = 0;
    private bool interacted;

    private void Start()
    {
        saveGame = SaveGame.Instance;
        playerInteraction = FindFirstObjectByType<PlayerInteraction>();
        input = InputActionsManager.Instance;
        dialogue = DialogueManager.Instance;
        scriptToActivate = GetComponent<JoinExitBelfry>();
        scriptToActivate.enabled = false;

        Load();
    }

    private void Update()
    {
        // Confere se o player está interagindo com o chapéu.
        if (playerInteraction.NextInteraction() == gameObject)
        {
            if (input.inputActions.Game.Interaction.WasPressedThisFrame() && !interacted)
            {
                StartCoroutine(nameof(UpdateQuest));
                interacted = true;
            }

            if (interacted)
            {
                playerInteraction.HasInteracted(true);
            }
        }
    }

    private IEnumerator UpdateQuest()
    {
        // Atualiza o número de chapéus coletados.
        hatsCollected++;

        // Salva a parte da quest que foi concluida.
        Save();

        // Se for necessário, um dialogo é iniciado quando um chapéu é coletado.
        if (startDialogue != null)
        {
            startDialogue.StartDialogue();
        }

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
        //Destroy(gameObject);
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
