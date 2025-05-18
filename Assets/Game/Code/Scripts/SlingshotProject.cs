using UnityEngine;

public class SlingshotProject : MonoBehaviour
{
    [HideInInspector] public Vector3 directionShoot;
    Rigidbody rdb;
    
    [Header("SFX")]
    [SerializeField] AudioClip hitSFX;
    private float hitVolume = 0.6f;
    private bool hasHit = false;

    private void Start()
    {
        rdb = GetComponent<Rigidbody>();
        rdb.AddForce(directionShoot * 10, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasHit)
        {
            hasHit = true;
            if (hitSFX != null)
            {
                AudioManager.Instance.PlaySoundFXClip(hitSFX, transform, hitVolume);
            }
        }
    }
}
