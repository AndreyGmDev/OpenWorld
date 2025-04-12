using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("References")]
    SaveGame saveGame;
    Rigidbody rb; 
    Animator anim;

    [Header("Moviment")]
    [SerializeField] float speed = 100;
    Vector3 moveDirection;
    bool move;
    
    [Header("Turn")]
    [SerializeField] Transform camera;
    [SerializeField] float mouseSensitivity = 100; // Sensibilidade do mouse.
    float rangeRotationX; // Limite de rotação no eixo X.

    [Header("Jump")]
    [SerializeField] float jumpPower = 20;
    bool isGrounded;
    bool jump;

    private void Awake()
    {
        saveGame = SaveGame.instance;
    }
    void Start()
    {
        // Pega as informações do SaveGame
        transform.position = saveGame.playerPosition;
        transform.eulerAngles = saveGame.playerRotation;

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Passa as informações para o SaveGame
        saveGame.playerPosition = transform.position;
        saveGame.playerRotation = transform.eulerAngles;

        Movement();
        Turn();

        TryJump();
    }

    private void FixedUpdate()
    {
        transform.Translate(moveDirection * Time.fixedDeltaTime * speed);
        //rb.linearVelocity = moveDirection * Time.fixedDeltaTime * speed;
        if (!move)
        {
            rb.linearVelocity = new Vector3(
                Mathf.Lerp(moveDirection.x, Vector3.zero.x, 0.6f),
                rb.linearVelocity.y,
                Mathf.Lerp(moveDirection.z, Vector3.zero.z, 0.6f));

        }

        //Jump();
    }

    private void Movement()
    {
        float moveHorizontal = (Input.GetAxisRaw("Horizontal") > 0 ? 1 : 0) + (Input.GetAxisRaw("Horizontal") < 0 ? -1 : 0);
        float moveVertical = (Input.GetAxisRaw("Vertical") > 0 ? 1 : 0) + (Input.GetAxisRaw("Vertical") < 0 ? -1 : 0);
        move = Input.GetButton("Horizontal") || Input.GetButton("Vertical");

        // Movimento quando o Player anda na Diagonal.
        Vector3 moveDiagonal = new Vector3((Mathf.Sqrt(2) / 2) * moveHorizontal, 0, (Mathf.Sqrt(2) / 2) * moveVertical);

        // Movimento quando o Player anda em só um Vetor.
        Vector3 moveNormal = new Vector3(moveHorizontal, 0, moveVertical);

        // Armazena o Movimento do Player.
        moveDirection = (moveHorizontal != 0 && moveVertical != 0) ? moveDiagonal : moveNormal;
        // Fala para o animator se o Player está em movimento.
        anim.SetBool("isWalking", move);
    }

    
    private void Turn()
    {
        Cursor.lockState = CursorLockMode.Locked;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rangeRotationX -= mouseY;
        rangeRotationX = Mathf.Clamp(rangeRotationX, -50, 30);
        
        camera.rotation = Quaternion.Euler(rangeRotationX, transform.eulerAngles.y, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Grounded"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Grounded"))
        {
            isGrounded = false;
        }
    }


    private void TryJump()
    {
        if (Input.GetButtonDown("Jump"))
            jump = true;
    }

    /*IEnumator void Jump()
    {
        
        if (isGrounded)
        {
            if (jump)
            {
                rb.AddRelativeForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
        }
        else if(rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += -Vector3.up * (jumpPower) * Time.fixedDeltaTime;
        }



    }*/

}
