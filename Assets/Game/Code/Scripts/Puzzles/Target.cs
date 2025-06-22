using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Target : MonoBehaviour
{
    private const string collisionTag = "Projectile";

    [SerializeField, Tooltip("Se ativado, No frame seguinte do alvo ser acertado, a colisão anterior será resetada, permitindo o alvo ser acertado mais de uma vez")]
    bool resetCollision;

    private bool wasCollided;

    private void LateUpdate()
    {
        if (resetCollision)
        {
            wasCollided = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(collisionTag))
        {
            wasCollided = true;
        }
    }

    public bool WasCollided()
    {
        return wasCollided;
    }
}
