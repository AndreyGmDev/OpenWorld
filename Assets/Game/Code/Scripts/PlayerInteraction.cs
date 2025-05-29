using UnityEngine;

[RequireComponent (typeof(SphereCollider))]
public class PlayerInteraction : MonoBehaviour
{
    public LayerMask layerMask;
    public SphereCollider coll;
    public GameObject nextInteraction = null;
    public GameObject lastInteraction = null;
    public GameObject interactPrompt;
    public bool hasInteracted = false;
    private DialogueTrigger currentTrigger;

    private void Start()
    {
        layerMask = LayerMask.GetMask("InteractableObjects");
        coll = gameObject.GetComponent<SphereCollider>();
    }

    private void Update()
    {
        float nextDistance = Mathf.Infinity;
        GameObject _nextInteraction = null;

        Collider[] allInteractions = Physics.OverlapSphere(transform.position + coll.center, coll.radius ,layerMask);
        foreach (var interaction in allInteractions)
        {
            float distance = Vector3.Distance(interaction.transform.position, transform.position);

            if (distance < nextDistance)
            {
                nextDistance = distance;
                _nextInteraction = interaction.gameObject;
            }
        }

        nextInteraction = _nextInteraction;

        if (nextInteraction != null)
        {
            DialogueTrigger trigger = nextInteraction.GetComponent<DialogueTrigger>();
            if (trigger != null)
            {
                currentTrigger = trigger; // Armazena a referência ao trigger
            }

            

            if (hasInteracted == false)
            {
                interactPrompt.SetActive(true);
                nextInteraction.GetComponent<Outline>().enabled = true;
            }
            else if (hasInteracted == true){
                interactPrompt.SetActive(false);
                //nextInteraction.GetComponent<Outline>().enabled = false;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                NextInteraction();
            }

            lastInteraction = nextInteraction;
            
        }
        else {
            lastInteraction.GetComponent<Outline>().enabled = false;
            interactPrompt.SetActive(false);
            hasInteracted = false;

            if (lastInteraction.GetComponent<DialogueTrigger>() == currentTrigger)
            {
                currentTrigger = null;
            }

            
        }
    }

    // Retorna a interação mais próxima para os outros scripts.
    public GameObject NextInteraction()
    {
        
        if (currentTrigger != null)
        {
            hasInteracted = true;
            interactPrompt.SetActive(true);
            //lastInteraction.GetComponent<Outline>().enabled = false;
            currentTrigger.StartDialogue(); // Chama o diálogo da placa
        }
        return nextInteraction;
    }
}
