using UnityEngine;

public class Slingshot : MonoBehaviour
{
    InputSystem_Actions inputActions;

    [Header("References")]
    [SerializeField] GameObject stone;

    [Header("Vectors")]
    [SerializeField] Transform spawnTransform;
    [SerializeField] Transform directionShoot;
    private Vector3 mouseDirection;

    [Header("GunInfos")]
    [SerializeField] int maxAmmo = 6;
    [SerializeField] int currentAmmo = 6;
    [SerializeField] float force = 3;

    [SerializeField] float delayShoots = 0.4f;

    private float countDelayShoots;
    [SerializeField] AudioClip readySFX;
    [SerializeField] AudioClip shootSFX;

    [SerializeField] float holdFinalTime = 1;
    private float holdTime;

    private void Awake()
    {
        // Inicializando o NewInputSystem.
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    private void OnEnable()
    {
        countDelayShoots = delayShoots;
        holdTime = 0;
    }

    private void Update()
    {
        if (countDelayShoots <= 0)
        {
            if (inputActions.Game.Shoot.IsPressed())
            {
                if (holdTime == 0f)
                {
                    SFXManager.instance.PlaySoundFXClip(readySFX, transform, 1f);
                }
                if (holdTime < holdFinalTime)
                {
                    holdTime += Time.deltaTime;
                }
            }
            if (inputActions.Game.Shoot.WasReleasedThisFrame())
            {
                Shoot();
            }
        }

        if (countDelayShoots > 0)
        {
            countDelayShoots -= Time.deltaTime;
        }

        if (inputActions.Game.PickUp.WasPressedThisFrame())
        {
            PickUpItem();
        }

        Raycast();
    }

    private void Raycast()
    {
        RaycastHit hit;
        Physics.Raycast(directionShoot.position, directionShoot.forward, out hit, Mathf.Infinity);
        if (hit.point != Vector3.zero)
        {
            mouseDirection = hit.point;
        }

        // Visual in UnityEditor.
        if (hit.point != null)
            Debug.DrawLine(directionShoot.position, hit.point, Color.red);
    }

    private void PickUpItem()
    {
        if (currentAmmo == maxAmmo) return; // Retorna o código se a munição estiver cheia.

        GameObject nearestItem = null;
        float nearestDistance = Mathf.Infinity;

        Collider[] itens = Physics.OverlapSphere(transform.position, 1.5f);
        foreach (var item in itens)
        {
            if (!item.CompareTag("Stone")) continue;

            float distance = Vector3.Distance(item.transform.position, transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestItem = item.gameObject;
            }
        }

        if (nearestItem != null)
        {
            Destroy(nearestItem);
            currentAmmo++;
        }
    }

    private void Shoot()
    {
        // Confere se ainda há munição na arma.
        if (currentAmmo < 1) return;

        // Seta o delay para poder atirar novamente.
        countDelayShoots = delayShoots;

        GameObject playerController = GameObject.Find("PlayerController");
        CharacterMovement characterMovement = playerController.transform.GetChild(0).GetComponent<CharacterMovement>();
        if (characterMovement != null)
        {
            characterMovement.SetUpdateRotation(directionShoot.rotation);
        }

        float currentForce = force * holdTime;
        var spawnedStone = Instantiate(stone,spawnTransform.position, Quaternion.identity);
        //spawnedStone.GetComponent<SlingshotProject>().directionShoot = directionShoot.forward * currentForce;
        spawnedStone.GetComponent<SlingshotProject>().directionShoot = (mouseDirection - spawnTransform.position).normalized * currentForce;

        // Tocar SFX
        SFXManager.instance.PlaySoundFXClip(shootSFX, transform, 1f);

        // Diminui uma munição da arma.
        currentAmmo--; 
        holdTime = 0;
    }
}
