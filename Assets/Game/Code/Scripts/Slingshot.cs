using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject stone;

    [Header("Vectors")]
    [SerializeField] Transform spawnTransform;
    private Vector3 mouseDirection;

    [Header("GunInfos")]
    [SerializeField] int maxAmmo = 6;
    [SerializeField] int currentAmmo = 6;
    [SerializeField] float force = 3;

    [SerializeField] float delayShoots = 0.4f;

    private float countDelayShoots;

    [SerializeField] float holdFinalTime = 1;
    private float holdTime;

    [Header("SFX")]
    [SerializeField] AudioClip readySFX;
    [SerializeField] AudioClip shootSFX;
    private float slingShootVolume = 1f;
    private float slingReadyVolume = 1f;

    InputActionsManager input;

    private void Awake()
    {
        // Inicializando o NewInputSystem.
        input = InputActionsManager.Instance;
    }

    private void OnEnable()
    {
        countDelayShoots = delayShoots;
        holdTime = 0;
    }

    private void Update()
    {
        TryShoot();
    }

    private void Raycast()
    {
        LayerMask layer = LayerMask.GetMask("Default") | LayerMask.GetMask("Ground");
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer);
        if (hit.point != Vector3.zero)
        {
            mouseDirection = hit.point;
        }
    }

    private void PickUpItem()
    {
        if (currentAmmo == maxAmmo) return; // Retorna o c�digo se a muni��o estiver cheia.

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

    private void TryShoot()
    {
        if (countDelayShoots <= 0)
        {
            // Impede de atirar se n�o houver muni��o.
            if (currentAmmo < 1) return;

            // Enquanto segura, o tiro � carregado.
            if (input.inputActions.Game.Shoot.IsPressed())
            {
                if (holdTime < holdFinalTime)
                {
                    holdTime += Time.deltaTime;
                }
            }

            // Atira quando solta o bot�o de atirar.
            if (input.inputActions.Game.Shoot.WasReleasedThisFrame())
            {
                Shoot();
            }

            // Toca o som de carregar o slingshoot uma unica vez.
            if (input.inputActions.Game.Shoot.WasPressedThisFrame())
            {
                if (readySFX != null)
                {
                    AudioManager.Instance.PlaySoundFXClip(readySFX, transform, slingReadyVolume);
                }
            }
        }

        if (countDelayShoots > 0)
        {
            countDelayShoots -= Time.deltaTime;
        }

        if (input.inputActions.Game.PickUp.WasPressedThisFrame())
        {
            PickUpItem();
        }

        Raycast();
    }
    private void Shoot()
    {
        // Seta o delay para poder atirar novamente.
        countDelayShoots = delayShoots;

        float currentForce = force * holdTime;
        var spawnedStone = Instantiate(stone,spawnTransform.position, Quaternion.identity);

        spawnedStone.GetComponent<SlingshotProject>().directionShoot = (mouseDirection - spawnTransform.position).normalized * currentForce;

        // Tocar SFX
        AudioManager.Instance.InterruptSFX();
        if (shootSFX != null)
        {
            AudioManager.Instance.PlaySoundFXClip(shootSFX, transform, slingShootVolume);
        }

        // Diminui uma muni��o da arma.
        currentAmmo--; 
        holdTime = 0;
    }
}
