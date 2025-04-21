using UnityEngine;

public class Slingshot : MonoBehaviour
{
    InputSystem_Actions inputActions;

    [SerializeField] Transform spawnTransform;
    [SerializeField] GameObject stone;

    [Header("Vectors")]
    [SerializeField] Transform directionShoot;

    [Header("GunInfos")]
    [SerializeField] int maxAmmo = 6;
    [SerializeField] int currentAmmo = 6;
    [SerializeField] float force = 3;

    [SerializeField] float reloadTime = 2;
    [SerializeField] float delayShoots = 0.4f;

    private float countDelayShoots;

    [SerializeField] float holdInitialTime = 0.1f;
    [SerializeField] float holdFinalTime = 1;
    private float holdTime;

    private void Awake()
    {
        // Inicializando o NewInputSystem.
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    private void Start()
    {
        countDelayShoots = delayShoots;
    }

    private void Update()
    {
        if (countDelayShoots <= 0)
        {
            if (inputActions.Game.Shoot.IsPressed())
            {
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
        spawnedStone.GetComponent<SlingshotProject>().directionShoot = directionShoot.forward * currentForce;

        // Diminui uma munição da arma.
        currentAmmo--; 
        holdTime = 0;
    }
}
