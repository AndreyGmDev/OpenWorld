using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class Hittable : MonoBehaviour
{
    Rigidbody rb;
    float power = 5;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void HitAddForce(Vector3 direction)
    {
        Vector3 force = direction * power;
        rb.AddForce(force, ForceMode.Impulse);
        print(force);
    }
}
