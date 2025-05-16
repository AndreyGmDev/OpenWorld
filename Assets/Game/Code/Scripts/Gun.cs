using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Vectors")]
    [SerializeField] Transform directionShoot;
    [SerializeField] Transform visualTest;

    [SerializeField] Mode mode;

    [Header("GunInfos")]
    [SerializeField] int maxAmmo = 6;
    [SerializeField] int currentAmmo = 6;
    [SerializeField] float reloadTime = 2;
    [SerializeField] float delayShoots = 0.4f;

    [SerializeField] AudioClip reloadSFX;
    [SerializeField] AudioClip shootSFX;

    private bool isReloading;
    private float countDelayShoots;

    InputActionsManager input;

    private enum Mode { pressToShoot, canHoldToShoot, needHoldToShoot }
    private void Awake()
    {
        // Inicializando o NewInputSystem.
        input = InputActionsManager.Instance;
    }

    private void Start()
    {
        countDelayShoots = delayShoots;
    }

    private void Update()
    {
        switch (mode)
        {
            case Mode.pressToShoot:
                if (input.inputActions.Game.Shoot.WasPressedThisFrame() && countDelayShoots <= 0 && !isReloading)
                {
                    Shoot();
                }
                break;
            case Mode.canHoldToShoot:
                if (input.inputActions.Game.Shoot.IsPressed() && countDelayShoots <= 0 && !isReloading)
                {
                    Shoot();
                }
                break;
            case Mode.needHoldToShoot:
                break;
        }

        if (countDelayShoots > 0)
        {
            countDelayShoots -= Time.deltaTime;
        }

        if (input.inputActions.Game.Reload.WasPressedThisFrame() && !isReloading)
        {
            StartCoroutine("Reload");
        }
    }

    private void Shoot()
    {
        // Confere se ainda h� muni��o na arma.
        if (currentAmmo < 1) return;

        // Seta o delay para poder atirar novamente.
        countDelayShoots = delayShoots;

        GameObject playerController = GameObject.Find("PlayerController");
        CharacterMovement characterMovement = playerController.transform.GetChild(0).GetComponent<CharacterMovement>();
        if (characterMovement != null)
        {
            characterMovement.SetUpdateRotation(directionShoot.rotation);
        }

        //SFX
        if (shootSFX != null)
            SFXManager.Instance.PlaySoundFXClip(shootSFX, transform, 1f);

        // Calcula a trajetoria do tiro.
        RaycastHit hit;
        Physics.Raycast(directionShoot.position, directionShoot.forward, out hit, Mathf.Infinity);
        if (hit.point != Vector3.zero)
        {
            if (visualTest != null)
            {
                visualTest.position = hit.point;
            }

            Hittable hittable = hit.transform.GetComponent<Hittable>();
            if (hittable != null)
            {
                hittable.HitAddForce(directionShoot.forward);
            }
        }

        // Diminui uma muni��o da arma.
        currentAmmo--;
    }

    private IEnumerator Reload()
    {
        if (currentAmmo == maxAmmo) yield break;

        isReloading = true;

        if (reloadSFX != null)
            SFXManager.Instance.PlaySoundFXClip(reloadSFX, transform, 1f);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;

        isReloading = false;
    }
}
