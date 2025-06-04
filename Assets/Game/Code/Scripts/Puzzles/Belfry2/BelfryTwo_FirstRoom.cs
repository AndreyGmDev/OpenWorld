using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BelfryTwo_FirstRoom : MonoBehaviour
{
    [SerializeField] GameObject[] targets = new GameObject[2];

    [SerializeField, Tooltip("Plataforma que será deslocada")] GameObject platform;
    [SerializeField, Tooltip("Posição final para onde a plataforma será deslocada")] Vector3 finalPosition;
    [Min(0.2f),SerializeField, Tooltip("Time in seconds until the platform reaches the final position")] float time;
    private Vector3 initialPosition;
    private float speed;
    private bool stopPuzzle;

    private void Start()
    {
        initialPosition = platform.transform.position;
        speed = Vector3.Distance(finalPosition, initialPosition) / time ;
    }

    private void Update()
    {
        if (!stopPuzzle)
        {
            bool bothCollided = true;

            foreach (var target in targets)
            {
                if (target.TryGetComponent<Target>(out var script))
                {
                    bothCollided &= script.WasCollided();
                }
            }

            if (bothCollided)
            {
                stopPuzzle = true;
                StartCoroutine(nameof(MakePlatform));
            }
        }
    }

    private IEnumerator MakePlatform()
    {
        while (Vector3.Distance(platform.transform.position, finalPosition) > 0.1f)
        {
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, finalPosition, speed * Time.deltaTime);
            yield return new WaitForNextFrameUnit();
        }
        
    }
}
