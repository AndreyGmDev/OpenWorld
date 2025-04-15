using Unity.VisualScripting;
using UnityEngine;

public class Gun : MonoBehaviour
{
    InputSystem_Actions inputActions;

    [SerializeField] Transform initialPositionShoot;
    [SerializeField] Transform directionShoot;

    [SerializeField] Vector3 incressPos;
    [SerializeField] Vector3 incressRot;

    [SerializeField] Mode mode;
    private enum Mode { pressToShoot, HoldToShoot}
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
                break;
            case Mode.HoldToShoot:
                break;
        }

        if (inputActions.Game.Shoot.WasPressedThisFrame())
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(initialPositionShoot.position, directionShoot.forward, out hit, Mathf.Infinity))
        {

        }
        if (hit.point != null)
            Debug.DrawLine(initialPositionShoot.position, hit.point, Color.red, 1f);
    }
}
