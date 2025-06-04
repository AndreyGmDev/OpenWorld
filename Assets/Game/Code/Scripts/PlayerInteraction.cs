using UnityEngine;

[RequireComponent (typeof(SphereCollider))]
public class PlayerInteraction : MonoBehaviour
{
    public GameObject interactPrompt;

    private LayerMask layerMask;
    private SphereCollider coll;
    private GameObject nextInteraction = null;
    private bool hasInteracted = false;

    private void Start()
    {
        layerMask = LayerMask.GetMask("InteractableObjects");
        coll = gameObject.GetComponent<SphereCollider>();
    }

    private void Update()
    {
        float nextDistance = Mathf.Infinity; // Distancia mais próxima
        GameObject _nextInteraction = null; // Objeto mais próximo para interagir.

        // Pega todos os colliders que estão interagindo com o player.
        Collider[] allInteractions = Physics.OverlapSphere(transform.position + coll.center, coll.radius, layerMask);
        foreach (var interaction in allInteractions)
        {
            // Calcula a distancia entre cada collider e o player.
            float distance = Vector3.Distance(interaction.transform.position, transform.position);

            // Confere qual é a menor distancia entre cada collider e o player.
            if (distance < nextDistance)
            {
                nextDistance = distance;
                _nextInteraction = interaction.gameObject;
            }
        }
        
        // Se Trocar o objeto de interação.
        if (_nextInteraction != null && _nextInteraction != nextInteraction)
        {
            hasInteracted = false; // Nenhuma interação foi realizada ainda.
        }

        // Seta o objeto que está mais próximo do player.
        nextInteraction = _nextInteraction;

        // Encontrou um objeto para interagir.
        if (nextInteraction != null)
        {
            // Se ainda não interagiu.
            if (hasInteracted == false)
            {
                // Ativa a tela de interação.
                if (interactPrompt != null)
                {
                    interactPrompt.SetActive(true);
                }

                // Ativa a outline do objeto que está sendo interagido.
                if (nextInteraction.TryGetComponent<Outline>(out var outline))
                {
                    outline.enabled = true;
                }
            }
            // Depois de interagir.
            else
            {
                // Deativa a tela de interação.
                if (interactPrompt != null)
                {
                    interactPrompt.SetActive(false);
                }

                // Deativa a outline do objeto interagido.
                if (nextInteraction.TryGetComponent<Outline>(out var outline))
                {
                    outline.enabled = false;
                }
            }

            // Se tiver um objeto para interagir e apertar 'F' a interação foi realizada.
            InputActionsManager input = InputActionsManager.Instance;
            if (input.inputActions.Game.Interaction.WasPressedThisFrame())
            {
                hasInteracted = true;
            }
        }
        // Sem objeto para interagir.
        else
        {
            // Deativa a tela de interação.
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }

            // Nenhuma interação foi realizada ainda.
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
