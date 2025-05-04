using UnityEngine;

public class SlingshotProject : MonoBehaviour
{
    [HideInInspector] public Vector3 directionShoot;
    Rigidbody rdb;
    private void Start()
    {
        rdb = GetComponent<Rigidbody>();
        rdb.AddForce(directionShoot * 10, ForceMode.Impulse);
    }
}
