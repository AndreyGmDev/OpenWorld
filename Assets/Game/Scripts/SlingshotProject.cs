using UnityEngine;

public class SlingshotProject : MonoBehaviour
{
    [HideInInspector] public Vector3 directionShoot;
    Rigidbody rdb;
    private void Start()
    {
        rdb = GetComponent<Rigidbody>();
        Vector3 force = directionShoot + Vector3.up * 0.3f;
        rdb.AddForce(directionShoot * 10, ForceMode.Impulse);
    }
}
