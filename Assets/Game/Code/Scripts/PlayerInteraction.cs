using UnityEngine;

[RequireComponent (typeof(SphereCollider))]
public class PlayerInteraction : MonoBehaviour
{
    private LayerMask layerMask;
    private SphereCollider coll;
    private GameObject nextInteraction = null;

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
    }

    // Retorna a interação mais próxima para os outros scripts.
    public GameObject NextInteraction()
    {
        return nextInteraction;
    }
}
