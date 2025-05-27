using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Elements")]
    public GameObject dialogueUI;
    public TextMeshProUGUI characterNameText;
    public Image characterImage;
    public TextMeshProUGUI dialogueText;
    public Button nextButton;

    [Header("Animation Settings")]
    public Animator dialogueAnimator;

    private List<DialogueData.DialogueLine> currentDialogue;
    private int currentIndex;
    private bool isDialogueActive;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogue(DialogueData dialogueData, Animator npcAnimator, int flag)
    {
        Cursor.lockState = CursorLockMode.None;
        if (isDialogueActive)
        {
            Debug.LogWarning("Um diálogo já está em andamento.");
            return;
        }

        // Busca o grupo de diálogo correspondente à flag
        var dialogueGroup = dialogueData.dialogueGroups.Find(group => group.flag == flag);

        if (dialogueGroup == null || dialogueGroup.dialogueLines.Count == 0)
        {
            Debug.LogWarning($"Nenhum diálogo encontrado para a flag {flag}");
            return;
        }

        currentDialogue = dialogueGroup.dialogueLines;
        currentIndex = 0;

        isDialogueActive = true;

        // Ativa a animação inicial do NPC, se houver
        npcAnimator?.SetTrigger("StartDialogue");

        // Ativa a UI de diálogo
        dialogueUI.SetActive(true);

        // Inicia o diálogo
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentIndex < currentDialogue.Count)
        {
            var line = currentDialogue[currentIndex];
            characterNameText.text = line.characterName;
            characterImage.sprite = line.characterImage;
            StartCoroutine(FadeInDialogueText(line.dialogueText));
            currentIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator FadeInDialogueText(string text)
    {
        // Configura o texto instantaneamente e prepara o fade-in
        dialogueText.text = text;
        dialogueText.alpha = 0;

        float fadeDuration = 0.5f; // Duração do fade-in
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            dialogueText.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dialogueText.alpha = 1; // Garante que o texto atinja opacidade total no final
    }

    public void EndDialogue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isDialogueActive = false;

        if (dialogueAnimator != null)
        {
            dialogueAnimator.SetTrigger("EndDialogue");

            // Inicia a coroutine para aguardar a animação terminar
            StartCoroutine(WaitForCloseAnimation());
        }
        else
        {
            // Se não houver animação, desativa a UI diretamente
            dialogueUI.SetActive(false);
        }
    }

    private IEnumerator WaitForCloseAnimation()
    {
        // Obtém o nome do estado que será tocado (nome da animação de fechamento)
        string closingStateName = "Dialogue End"; // Substitua "Close" pelo nome real do estado de animação

        // Aguarda até que o Animator entre no estado de fechamento
        while (!dialogueAnimator.GetCurrentAnimatorStateInfo(0).IsName(closingStateName))
        {
            yield return null;
        }

        // Aguarda a animação terminar
        while (dialogueAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        // Desativa a UI após a animação terminar
        dialogueUI.SetActive(false);
    }

    private void Update()
    {
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            DisplayNextLine();
        }
    }
}
