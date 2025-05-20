using KinematicCharacterController;
using UnityEngine;

// Inputs para movimenta��o do player.
public struct CharacterMovementInput
{
    // PlayerController
    public Vector2 MoveInput;
    public bool WantsToJump;
    public bool WantsToCrouch;

    // Camera.
    public Quaternion LookRotation;
    public bool IsAiming;
    public CameraController.Orientation NormalOrientation;
    public CameraController.Orientation AimingOrientation;
}

// A��o de usar o Hook.
public struct UsingHook
{
    public bool IsUsingHook; // Est� usando o hook?
    public Vector3 WhereIsGoing; // Para onde o player deve ir com o hook.
    public float HookMaxSpeed; // Velocidade enquanto usa o hook.
}


[RequireComponent(typeof(KinematicCharacterMotor))]
public class CharacterMovement : MonoBehaviour, ICharacterController
{
    public KinematicCharacterMotor motor;

    [Header("Normal Movement")]
    [SerializeField, Tooltip("Velocidade m�xima do player na terra")] float maxSpeed = 5;
    [SerializeField, Tooltip("Acelera��o do player na terra")] float acceleration = 50;
    [SerializeField, Tooltip("Velocidade de rota��o do player")] float rotationSpeed = 15;
    [SerializeField, Tooltip("For�a da gravidade")] float gravity = 30;
    [SerializeField, Tooltip("Altura do pulo do player")] float jumpHeight = 1.5f;
    [Range(0.01f, 0.3f), Tooltip("Atraso de tempo para o pulo ainda ser considerado")] public float jumpRequestDuration = 0.1f;

    [Header("Air Movement")]
    [SerializeField, Tooltip("Velocidade m�xima do player no ar")] float airMaxSpeed = 3;
    [SerializeField, Tooltip("Acelera��o do player na ar")] float airAcceleration;
    [SerializeField, Tooltip("Atrito do no ar")] float drag;

    [Header("Crouch Movement")]
    [SerializeField, Tooltip("Velocidade m�xima do player agachado")] float crouchMaxSpeed = 3;
    [SerializeField, Tooltip("Acelera��o do player agachado")] float crouchAcceleration = 20;

    [Header("Animations")]
    [SerializeField] Animator animator;

    // PlayerController.
    // Informa��es sobre o Walk.
    private Vector3 moveInput; // Valor dados pelos inputs de movimenta��o.

    // Informa��es sobre o Jump.
    private float jumpSpeed => Mathf.Sqrt(2 * gravity * jumpHeight); // Velocidade do pulo.
    private float jumpRequestExpireTime; //Atraso de tempo para o pulo ainda ser considerado

    // Informa��es sobre o Crouch.
    private bool wantsToCrouch; // Apertando o input de crouch.
    [HideInInspector] public bool isCrouching; // Est� no crouch?
    private Collider[] probedColliders = new Collider[8]; // Colis�o que identifica se h� algum objeto sobre o player enquanto ele est� no crouch, se sim, player fica no crouch, se n�o, player levanta.

    // Informa��es sobre a Camera.
    private Quaternion lookRotation; // Rota��o da c�mera.
    private bool isAiming; // Player est� mirando?
    private CameraController.Orientation normalOrientation; // Modo de orienta��o do player normal.
    private CameraController.Orientation aimingOrientation; // Modo de orienta��o do player enquanto est� mirando.

    // Informa��es sobre o UsingHook.
    private bool isUsingHook; // Est� usando o hook?
    private Vector3 whereIsGoing; // Para onde o player deve ir com o hook.
    private Vector3 target;


    private void Awake()
    {
        motor.CharacterController = this;
    }

    public void SetInput(in CharacterMovementInput input)
    {
        moveInput = Vector3.zero;
        if (input.MoveInput != Vector2.zero)
        {
            moveInput = new Vector3(input.MoveInput.x, 0, input.MoveInput.y);
            moveInput = input.LookRotation * moveInput;
            moveInput.y = 0;
            moveInput.Normalize();
        }
        
        if (input.WantsToJump)
        {
            jumpRequestExpireTime = Time.time + jumpRequestDuration;
        }

        wantsToCrouch = input.WantsToCrouch;

        isAiming = input.IsAiming;
        lookRotation = input.LookRotation;
        normalOrientation = input.NormalOrientation;
        aimingOrientation = input.AimingOrientation;
    }

    public void HookActions(in UsingHook infos)
    {
        whereIsGoing = infos.WhereIsGoing;
        isUsingHook = infos.IsUsingHook;
        target = (whereIsGoing - transform.position);
        /*target.Normalize();
        print(target);
        target *= infos.HookMaxSpeed;
        print(target);*/
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (isAiming)
        {
            // Rota��o quando o player estiver mirando.
            switch (aimingOrientation)
            {
                // Rota��o dependendo dos inputs do teclado.
                case CameraController.Orientation.towardMovement:

                    if (moveInput != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(moveInput);
                        currentRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * deltaTime);
                    }
                    break;

                //Rota��o do player em rela��o � c�mera.
                case CameraController.Orientation.towardsCamera:

                    currentRotation = Quaternion.RotateTowards(currentRotation, lookRotation, 300);
                    currentRotation.x = 0;
                    currentRotation.z = 0;
                    break;
            }
        }
        else
        {
            // Rota��o quando o player n�o estiver mirando.
            switch (normalOrientation)
            {
                // Rota��o dependendo dos inputs do teclado.
                case CameraController.Orientation.towardMovement:

                    if (moveInput != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(moveInput);
                        currentRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * deltaTime);
                    }
                    break;

                //Rota��o do player em rela��o � c�mera.
                case CameraController.Orientation.towardsCamera:

                    currentRotation = Quaternion.RotateTowards(currentRotation, lookRotation, 300);
                    currentRotation.x = 0;
                    currentRotation.z = 0;
                    break;
            }
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (isUsingHook)
        {
            // Isso � uma a��o chamada somente quando o Hook � usado.
            UsingHook(ref currentVelocity, deltaTime);   
        }
        else
        {
            Movement(ref currentVelocity, deltaTime);
        }

        // Atualiza a anima��o.
        UpdateAnimation();
    }

    private void Movement(ref Vector3 currentVelocity, float deltaTime)
    {
        // Confere se o player est� no ch�o.
        if (motor.GroundingStatus.IsStableOnGround)
        {
            // Calcula velocidade do player andando.
            if (!isCrouching)
            {
                Vector3 targetVelocity = moveInput * maxSpeed;
                currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * deltaTime);
            }
            else // Calcula velocidade do player agachado.
            {
                Vector3 targetVelocity = moveInput * crouchMaxSpeed;
                currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, crouchAcceleration);
            }

            // Faz o personagem agachar.
            if (wantsToCrouch)
            {
                motor.SetCapsuleDimensions(motor.Capsule.radius, 0.85f, 0.85f * 0.5f);
                isCrouching = true;
            }
            else // Faz o personagem ficar de p�.
            {
                // Confere se h� obst�culos que impedem o player de ficar em p�.
                motor.SetCapsuleDimensions(0.2f, 1.35f, 0.675f);
                if (motor.CharacterOverlap(
                    motor.TransientPosition,
                    motor.TransientRotation,
                    probedColliders,
                    motor.CollidableLayers,
                    QueryTriggerInteraction.Ignore) > 0)
                {
                    // Se tiver obst�culos do caminho, o player continua agachado.
                    motor.SetCapsuleDimensions(0.2f, 0.85f, 0.85f * 0.5f);
                }
                else // Se n�o houver obst�culos, o player puder ficar em p�
                {
                    isCrouching = false;
                }
            }

            // Faz o personagem pular, se ele n�o estiver agachado.
            if (Time.time < jumpRequestExpireTime && !isCrouching)
            {
                currentVelocity.y = jumpSpeed;
                jumpRequestExpireTime = 0;
                motor.ForceUnground();
            }
        }
        else // Se o player estiver no ar.
        {
            Vector2 targetVelocityXZ = new Vector2(moveInput.x, moveInput.z) * airMaxSpeed;
            Vector2 currentVelocityXZ = new Vector2(currentVelocity.x, currentVelocity.z);

            currentVelocityXZ = Vector2.MoveTowards(currentVelocityXZ, targetVelocityXZ, airAcceleration * deltaTime);

            currentVelocity.x = ApplyDrag(currentVelocityXZ.x, drag, deltaTime);
            currentVelocity.z = ApplyDrag(currentVelocityXZ.y, drag, deltaTime);
            currentVelocity.y -= gravity * deltaTime;
        }
    }

    private void UsingHook(ref Vector3 currentVelocity, float deltaTime)
    {
        motor.ForceUnground();

        float distance = Vector3.Distance(whereIsGoing, transform.position);
        if (distance > 1.2f)
        {
            currentVelocity = target;
        }
        else
        {
            currentVelocity = Vector3.zero;
            isUsingHook = false;
            InputActionsManager.Instance.EnableGameActions();
        }
    }

    private static float ApplyDrag(float velocity, float drag, float deltaTime)
    {
        return velocity * (1 / (1 + drag * deltaTime));
    }

    // Fun��o respons�vel por calcular as anima��es.
    private void UpdateAnimation()
    {
        if (animator == null) return;

        if (motor.GroundingStatus.IsStableOnGround)
        {
            if (moveInput.magnitude != 0)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }

            if (wantsToCrouch)
            {
                animator.SetBool("isCrouching", true);
            }
            else if (!isCrouching)
            {
                animator.SetBool("isCrouching", false);
            }
        }

        // Troca de estados.
        /*if (isUsingHook)
        {
            animator.SetLayerWeight(0, 0);
            animator.SetLayerWeight(1, 1);
        }
        else
        {
            animator.SetLayerWeight(0, 1);
            animator.SetLayerWeight(1, 0);
        }*/
    }

    #region not implemented

    public void AfterCharacterUpdate(float deltaTime)
    {
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }
    #endregion
}
