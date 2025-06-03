using UnityEngine;

[RequireComponent (typeof(SphereCollider))]
public class PlayerInteraction : MonoBehaviour
{
    private LayerMask layerMask;
    private SphereCollider coll;
    private GameObject nextInteraction = null;

    public GameObject interactPrompt;
    public bool hasInteracted = false;

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
        
        if (_nextInteraction != null && _nextInteraction != nextInteraction)
        {
            hasInteracted = false;
        }

        nextInteraction = _nextInteraction;

        if (nextInteraction != null)
        {
            if (hasInteracted == false)
            {
                if (interactPrompt != null)
                {
                    interactPrompt.SetActive(true);
                }

                if (nextInteraction.GetComponent<Outline>())
                {
                    nextInteraction.GetComponent<Outline>().enabled = true;
                }
            }
            else if (hasInteracted == true)
            {

                if (interactPrompt != null)
                {
                    interactPrompt.SetActive(false);
                }

                if (nextInteraction.GetComponent<Outline>())
                {
                    nextInteraction.GetComponent<Outline>().enabled = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                hasInteracted = true;
            }
        }
        else
        {
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }

            hasInteracted = false;
        }
    }

    // Retorna a interação mais próxima para os outros scripts.
    public GameObject NextInteraction()
    {
        return nextInteraction;
    }

    public void HasInteracted(bool isInteracted) 
    {
        hasInteracted = isInteracted;
    }

}
