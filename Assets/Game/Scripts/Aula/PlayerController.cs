using KinematicCharacterController;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterMovement characterMovement;
    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool wantsToJump = Input.GetKeyDown(KeyCode.Space);

        characterMovement.SetInput(new CharacterMovementInput()
        {
            MoveInput = new Vector2(h, v),
            WantsToJump = wantsToJump
        });
    }
}
