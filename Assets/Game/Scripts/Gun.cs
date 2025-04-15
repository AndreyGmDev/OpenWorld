using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : MonoBehaviour
{
    InputSystem_Actions inputActions;

    [Header("Vectors")]
    [SerializeField] Transform initialPositionShoot;
    [SerializeField] Transform directionShoot;

    [SerializeField] Mode mode;

    [Header("GunInfos")]
    [SerializeField] int maxAmmo = 6;
    [SerializeField] int currentAmmo = 6;
    [SerializeField] float reloadTime;

    private enum Mode { pressToShoot, canHoldToShoot, needHoldToShoot}
    private void Awake()
    {
        // Inicializando o NewInputSystem.
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    private void Update()
    {
        switch (mode)
        {
            case Mode.pressToShoot:
                if (inputActions.Game.Shoot.WasPressedThisFrame())
                {
                    Shoot();
                }
                break;
            case Mode.canHoldToShoot:
                if (inputActions.Game.Shoot.IsPressed())
                {
                    Shoot();
                }
                break;
            case Mode.needHoldToShoot:
                break;
        }

        if (inputActions.Game.Reload.WasPressedThisFrame())
        {
            StartCoroutine("Reload");
        }
    }

    private void Shoot()
    {
        if (currentAmmo < 1) return;

        // Calcula a trajetoria do tiro.
        RaycastHit hit;
        Physics.Raycast(initialPositionShoot.position, directionShoot.forward, out hit, Mathf.Infinity);
        if (hit.point != null)
        {

        }
        if (hit.point != null)
            Debug.DrawLine(initialPositionShoot.position, hit.point, Color.red, 1f);

        // Diminui uma munição da arma.
        currentAmmo--;
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
    }
}
