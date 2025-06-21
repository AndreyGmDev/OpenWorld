using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueData dialogueData;
    public Animator npcAnimator;
    public int flag; // Flag para escolher o grupo de diálogos

    private InputActionsManager input; // InputManager do jogo.
    private PlayerInteraction playerInteraction;

    private void Start()
    {
        input = InputActionsManager.Instance;
        playerInteraction = FindAnyObjectByType<PlayerInteraction>();
    }

    public void StartDialogue()
    {
        if (DialogueManager.Instance != null && dialogueData != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueData, npcAnimator, flag);
        }
    }

    public void Update()
    {
        GameObject nextInteraction = playerInteraction.NextInteraction();

        if (nextInteraction == gameObject)
        {
            if (input.inputActions.Game.Interaction.WasPressedThisFrame()) // Se apertar a letra 'F' Gabriel!
            {
                StartDialogue();
            }
        }
        else
        {
            if (TryGetComponent<Outline>(out var outline))
            {
                outline.enabled = false;
            }
        }
    }
}

