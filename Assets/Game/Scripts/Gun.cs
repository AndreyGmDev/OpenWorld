using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    InputSystem_Actions inputActions;

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

    private enum Mode { pressToShoot, canHoldToShoot, needHoldToShoot }
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
        switch (mode)
        {
            case Mode.pressToShoot:
                if (inputActions.Game.Shoot.WasPressedThisFrame() && countDelayShoots <= 0 && !isReloading)
                {
                    Shoot();
                }
                break;
            case Mode.canHoldToShoot:
                if (inputActions.Game.Shoot.IsPressed() && countDelayShoots <= 0 && !isReloading)
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

        if (inputActions.Game.Reload.WasPressedThisFrame() && !isReloading)
        {
            StartCoroutine("Reload");
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

        //SFX
        SFXManager.instance.PlaySoundFXClip(shootSFX, transform, 1f);

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

        // Visual in UnityEditor.
        if (hit.point != null)
            Debug.DrawLine(directionShoot.position, hit.point, Color.red, 1f);

        // Diminui uma munição da arma.
        currentAmmo--;
    }

    private IEnumerator Reload()
    {
        if (currentAmmo == maxAmmo) yield break;

        isReloading = true;

        SFXManager.instance.PlaySoundFXClip(reloadSFX, transform, 1f);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;

        isReloading = false;
    }
}
