using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueData dialogueData;
    public Animator npcAnimator;
    public int flag; // Flag para escolher o grupo de diálogos

    private InputActionsManager input; // InputManager do jogo.

    private void Start()
    {
        input = InputActionsManager.Instance;
    }

    private void StartDialogue()
    {
        if (DialogueManager.Instance != null && dialogueData != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueData, npcAnimator, flag);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        PlayerInteraction playerInteraction = FindAnyObjectByType<PlayerInteraction>();
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
            GetComponent<Outline>().enabled = false;
        }
    }



}

