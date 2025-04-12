using KinematicCharacterController;
using Unity.VisualScripting;
using UnityEngine;

public struct CharacterMovementInput
{
    public Vector2 MoveInput;
    public bool WantsToJump;
}

[RequireComponent(typeof(KinematicCharacterMotor))]
public class CharacterMovement : MonoBehaviour, ICharacterController
{
    public KinematicCharacterMotor motor;

    [Header("Ground Movement")]
    public float maxSpeed = 5;
    public float acceleration = 50;
    //public float rotationSpeed = 15;
    public float gravity = 30;
    public float jumpHeight = 1.5f;
    [Range(0.01f, 0.3f)]
    public float jumpRequestDuration = 0.1f;

    [Header("Air Movement")]
    public float airMaxSpeed = 3;
    public float airAcceleration;
    public float drag;



    private Vector3 moveInput;
    private float jumpRequestExpireTime;


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
            moveInput = new Vector3(input.MoveInput.x, 0, input.MoveInput.y).normalized;
        }

        if (input.WantsToJump)
        {
            jumpRequestExpireTime = Time.time + jumpRequestDuration;
        }
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (motor.GroundingStatus.IsStableOnGround)
        {
            Vector3 targetVelocity = moveInput * maxSpeed;
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * deltaTime);
        
            if (Time.time < jumpRequestExpireTime)
            {
                currentVelocity.y = jumpSpeed;
                jumpRequestExpireTime = 0;
                motor.ForceUnground();
            }
        }
        else
        {
            Vector2 targetVelocityXZ = new Vector2(moveInput.x,moveInput.z) * airMaxSpeed;
            Vector2 currentVelocityXZ = new Vector2(currentVelocity.x,currentVelocity.z);

            currentVelocityXZ = Vector2.MoveTowards(currentVelocityXZ, targetVelocityXZ, airAcceleration * deltaTime);

            currentVelocity.x = ApplyDrag(currentVelocityXZ.x, drag, deltaTime);
            currentVelocity.z = ApplyDrag(currentVelocityXZ.y, drag, deltaTime);
            currentVelocity.y -= gravity * deltaTime;
        }
    }

    private static float ApplyDrag(float velocity, float drag, float deltaTime)
    {
        return velocity * (1 / (1 + drag * deltaTime));
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
