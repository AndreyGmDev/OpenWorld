using KinematicCharacterController;
using UnityEngine;

public struct CharacterMovementInput
{
    public Vector2 MoveInput;
    public bool WantsToJump;
    public bool WantsToCrouch;

    public Quaternion LookRotation;
    public bool IsAiming;
    public CameraController.Orientation NormalOrientation;
    public CameraController.Orientation AimingOrientation;
}

[RequireComponent(typeof(KinematicCharacterMotor))]
public class CharacterMovement : MonoBehaviour, ICharacterController
{
    public KinematicCharacterMotor motor;

    [Header("Normal Movement")]
    public float maxSpeed = 5;
    public float acceleration = 50;
    public float rotationSpeed = 15;
    public float gravity = 30;
    public float jumpHeight = 1.5f;
    [Range(0.01f, 0.3f)] public float jumpRequestDuration = 0.1f;

    [Header("Air Movement")]
    public float airMaxSpeed = 3;
    public float airAcceleration;
    public float drag;

    [Header("Crouch Movement")]
    public float crouchMaxSpeed = 3;
    public float crouchAcceleration = 20;

    [Header("Animations")]
    public Animator animator;

    private Vector3 moveInput; // Valor dados pelos inputs de movimentação.

    private float jumpRequestExpireTime;

    private bool wantsToCrouch; // Apertando o input de crouch.
    [HideInInspector] public bool isCrouching; // Está no crouch?
    private Collider[] probedColliders = new Collider[8];

    private bool isAiming;
    private Quaternion lookRotation;
    private CameraController.Orientation normalOrientation;
    private CameraController.Orientation aimingOrientation;

    public float jumpSpeed => Mathf.Sqrt(2 * gravity * jumpHeight);

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

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (isAiming)
        {
            // Rotação quando o player estiver mirando.
            switch (aimingOrientation)
            {
                // Rotação dependendo dos inputs do teclado.
                case CameraController.Orientation.towardMovement:

                    if (moveInput != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(moveInput);
                        currentRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * deltaTime);
                    }
                    break;

                //Rotação do player em relação à câmera.
                case CameraController.Orientation.towardsCamera:

                    currentRotation = Quaternion.RotateTowards(currentRotation, lookRotation, 300);
                    currentRotation.x = 0;
                    currentRotation.z = 0;
                    break;
            }

        }
        else
        {
            // Rotação quando o player não estiver mirando.
            switch (normalOrientation)
            {
                // Rotação dependendo dos inputs do teclado.
                case CameraController.Orientation.towardMovement:

                    if (moveInput != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(moveInput);
                        currentRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * deltaTime);
                    }
                    break;

                //Rotação do player em relação à câmera.
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
        // Confere se o player está no chão.
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
            else // Faz o personagem ficar de pé.
            {
                // Confere se há obstáculos que impedem o player de ficar em pé.
                motor.SetCapsuleDimensions(0.2f, 1.35f, 0.675f);
                if (motor.CharacterOverlap(
                    motor.TransientPosition,
                    motor.TransientRotation,
                    probedColliders,
                    motor.CollidableLayers,
                    QueryTriggerInteraction.Ignore) > 0)
                {
                    // Se tiver obstáculos do caminho, o player continua agachado.
                    motor.SetCapsuleDimensions(0.2f, 0.85f, 0.85f * 0.5f);
                }
                else // Se não houver obstáculos, o player puder ficar em pé
                {
                    isCrouching = false;
                }
            }

            // Faz o personagem pular, se ele não estiver agachado.
            if (Time.time < jumpRequestExpireTime && !wantsToCrouch)
            {
                currentVelocity.y = jumpSpeed;
                jumpRequestExpireTime = 0;
                motor.ForceUnground();
            }
        }
        else // Se o player não estiver no chão.
        {
            Vector2 targetVelocityXZ = new Vector2(moveInput.x,moveInput.z) * airMaxSpeed;
            Vector2 currentVelocityXZ = new Vector2(currentVelocity.x,currentVelocity.z);

            currentVelocityXZ = Vector2.MoveTowards(currentVelocityXZ, targetVelocityXZ, airAcceleration * deltaTime);

            currentVelocity.x = ApplyDrag(currentVelocityXZ.x, drag, deltaTime);
            currentVelocity.z = ApplyDrag(currentVelocityXZ.y, drag, deltaTime);
            currentVelocity.y -= gravity * deltaTime;
        }

        UpdateAnimation();
    }

    private static float ApplyDrag(float velocity, float drag, float deltaTime)
    {
        return velocity * (1 / (1 + drag * deltaTime));
    }

    // Função responsável por calcular as animações.
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
