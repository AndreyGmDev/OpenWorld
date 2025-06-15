using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject stone;

    [Header("Vectors")]
    [SerializeField] Transform spawnTransform;
    private Vector3 mouseDirection;

    [Header("GunInfos")]
    [SerializeField] float force = 3; // Força do tiro máxima.
    [SerializeField] float holdFinalTime = 1; // Aumenta a força do tiro com relação ao tempo do input segurado.
    [SerializeField] float delayShoots = 0.4f; // Delay entre tiros.
    private float holdTime; // Contaliza o tempo que o tiro está sendo carregado. Isso influencia na força do tiro.
    private float countDelayShoots; // Contabiliza esse delay.

    /*[SerializeField] int maxAmmo = 6; // Munição maxima.
    [SerializeField] int currentAmmo = 6; // Munição atual.*/

    [Header("SFX")]
    [SerializeField] AudioClip readySFX;
    [SerializeField] AudioClip shootSFX;
    private float slingShootVolume = 1f;
    private float slingReadyVolume = 1f;

    private InputActionsManager input;

    private void Awake()
    {
        // Inicializando o NewInputSystem.
        input = InputActionsManager.Instance;
    }

    // Se trocar o slot para o estilingue.
    private void OnEnable()
    {
        countDelayShoots = delayShoots;
        holdTime = 0;
    }
    
    private void Update()
    {
        // Tenta atirar.
        TryShoot();
    }

    // Calcula direção do tiro usando o raycast.
    private void Raycast()
    {
        LayerMask layer = LayerMask.GetMask("Ground") | LayerMask.GetMask("Default");
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer);
        if (hit.collider)
        {
            mouseDirection = hit.point;
        }
    }

    // Tenta atirar.
    private void TryShoot()
    {
        // Confere se já pode atirar.
        if (countDelayShoots > 0)
        {
            // Se não puder.
            countDelayShoots -= Time.deltaTime; // Diminui o contabilizador do delay.
            return; // Impede o seguimento do script.
        }

        // Impede de atirar se nao houver muniçao.
        // if (currentAmmo < 1) return;

        // Enquanto segura, o tiro esta carregado.
        if (input.inputActions.Game.Shoot.IsPressed())
        {
            Raycast(); // Calcula a direção do tiro enquanto o tiro está sendo carregado.

            // Carrega o tiro.
            if (holdTime < holdFinalTime)
            {
                holdTime += Time.deltaTime;
            }
        }

        // Atira quando solta o botao de atirar.
        if (input.inputActions.Game.Shoot.WasReleasedThisFrame())
        {
            AudioManager.Instance.InterruptSFX(); // Interrompe o som de carregar o tiro.

            // Somente atira se o player carregar o estilingue por mais de 0.2 segundos.
            if (holdTime > 0.2f)
            {
                Shoot();
            }
        }

        // Toca o som de carregar o slingshoot uma unica vez.
        if (input.inputActions.Game.Shoot.WasPressedThisFrame())
        {
            if (readySFX != null)
            {
                AudioManager.Instance.PlaySoundFXClip(readySFX, transform, slingReadyVolume, false);
            }
        }

        // Recarregar - Não necessita mais de recarregar.
        /*if (input.inputActions.Game.Interaction.WasPressedThisFrame())
        {
            PickUpItem();
        }*/
    }

    // Função de atirar.
    private void Shoot()
    {
        // Seta o delay para poder atirar novamente.
        countDelayShoots = delayShoots;

        // Calcula a força atual dependendo do tempo que o tiro foi carregado.
        float currentForce = force * holdTime;

        // Instancia o tiro.
        var spawnedStone = Instantiate(stone,spawnTransform.position, Quaternion.identity);

        // Passa o direção para onde o tiro deve seguir.
        spawnedStone.GetComponent<SlingshotProject>().directionShoot = (mouseDirection - spawnTransform.position).normalized * currentForce;

        // Tocar SFX de tiro.
        if (shootSFX != null)
        {
            AudioManager.Instance.PlaySoundFXClip(shootSFX, transform, slingShootVolume, false);
        }

        // Diminui uma muni��o da arma.
        // currentAmmo--; 

        holdTime = 0; // Reseta o tempo de segurar o tiro.
    }



    /*private void PickUpItem()
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
    }*/
}
