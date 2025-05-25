using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueData dialogueData;
    public Animator npcAnimator;
    public int flag; // Flag para escolher o grupo de diálogos

    public void StartDialogue()
    {
        if (DialogueManager.Instance != null && dialogueData != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueData, npcAnimator, flag);
        }
    }
}

