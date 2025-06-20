using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private const int VOL = 1; // Volume constant


    [Header("UI Elements")]
    public GameObject dialogueUI;
    public TextMeshProUGUI characterNameText;
    public Image characterImage;
    public TextMeshProUGUI dialogueText;
    public Button nextButton;

    [Header("Animation Settings")]
    public Animator dialogueAnimator;

    [Header("SFX")]
    [SerializeField] AudioClip blipSFX; // Som quando o diálogo aparece
    [SerializeField] AudioClip clickSFX; // Som quando o diálogo avança/termina

    private List<DialogueData.DialogueLine> currentDialogue;
    private int currentIndex;
    private bool isDialogueActive;

    private AudioManager audioManager;
    
    private static DialogueManager dialogueManager;

    public static DialogueManager Instance
    {
        get
        {
            if (dialogueManager == null)
            {
                dialogueManager = FindFirstObjectByType<DialogueManager>();

                if (dialogueManager == null)
                {
                    if (GameObject.Find("GameManager"))
                    {
                        GameObject obj = GameObject.Find("GameManager");
                        obj.AddComponent<AudioManager>();
                        Debug.Log("Adicionado DialogueManager ao GameManager");
                    }
                    else
                    {
                        GameObject obj = new GameObject("GameManager");
                        obj.AddComponent<AudioManager>();
                        Debug.Log("Criado GameManager e adicionado DialogueManager");
                    }
                }
            }
            return dialogueManager;
        }
    }

    private void Awake()
    {
        if (dialogueManager == null)
        {
            dialogueManager = this;
            //Cursor.lockState = CursorLockMode.None;
        }
        else if (dialogueManager != this)
        {
            Debug.Log("Encontradas múltiplas instâncias do DialogueManager. Destruindo a duplicata em: " + gameObject.name);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Inicializa a referência do AudioManager.
        audioManager = AudioManager.Instance;
    }

    public void StartDialogue(DialogueData dialogueData, Animator npcAnimator, int flag)
    {
        dialogueUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        InputActionsManager.Instance.DisableGameActions();

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
            
            // Toca o som quando uma linha aparece
            if (audioManager != null && blipSFX != null)
            {
                audioManager.PlaySoundFXClip(blipSFX, transform, VOL, false);
            }
            
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
        InputActionsManager.Instance.EnableGameActions();

        PlayerInteraction playerInteraction = FindAnyObjectByType<PlayerInteraction>();
        playerInteraction.HasInteracted(false);

        isDialogueActive = false;

        // Toca o som quando o diálogo termina
        if (audioManager != null && clickSFX != null)
        {
            audioManager.PlaySoundFXClip(clickSFX, transform, VOL, false);
        }

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
            // Toca o som quando o diálogo avança
            if (audioManager != null && clickSFX != null)
            {
                audioManager.PlaySoundFXClip(clickSFX, transform, VOL, false);
            }
            
            DisplayNextLine();
        }
    }

    public bool IsInDialogue()
    {
        return isDialogueActive;
    }
}
