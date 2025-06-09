using UnityEngine;

public class BackPlayerWhenDie : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform  point;
    [SerializeField] BoxCollider water;

    private void Update()
    {
        Collider[] colls = Physics.OverlapBox(water.transform.position + water.center, water.size, Quaternion.identity, LayerMask.GetMask("Player"));

        foreach (Collider coll in colls)
        {
            GameObject player = coll.gameObject;   
            player.GetComponent<CharacterMovement>().motor.SetPosition(point.position);
            print(player);
        }
    }
}
