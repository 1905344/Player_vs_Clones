using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class FirstPersonMovement : MonoBehaviour
{
    #region Variables

    [Header("Player Components")]
    [SerializeField] private CharacterController charController;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    //[SerializeField] private Rigidbody playerBody;

    [Space(5)]

    [SerializeField] private float groundDistance;

    [Space(15)]

    [Header("Movement Variables")]
    [SerializeField] private Vector2 playerMovement;
    [SerializeField] private Vector3 characterMove;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float directionX;
    [SerializeField] private float directionZ;
    [SerializeField] private float playerGravity = -9.81f;
    
    [Space(10)]    

    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float timeToJumpApex;

    [Space(10)]

    [SerializeField] private Vector3 playerVelocity;

    [Space(15)]

    [Header("Debugging")]
    [SerializeField] private bool disablePlayerMovement = false;
    [SerializeField] private bool disablePlayerJumping = false;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool canJump;
    [SerializeField] private bool isJumping;

    //Input manager
    private InputManager inputManager;

    #endregion

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        inputManager = InputManager.Instance;
    }

    #region Jump Physics

    //private void JumpPhysics()
    //{
    //    Vector3 newGravity = new Vector3(0, (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex));
    //    playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * playerGravity);
    //}

    #endregion

    #region Jumping When Requested

    private void OnJump()
    {
        if (isGrounded)
        {
            canJump = true;

            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * playerGravity);

            jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * playerGravity * jumpHeight);

            if (playerVelocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - playerVelocity.y, 0f);
            }
            else if (playerVelocity.y < 0f)
            {
                jumpSpeed += Mathf.Abs(playerVelocity.y);
            }

            playerVelocity.y = jumpSpeed;
            isJumping = true;
        }
    }

    #endregion

    private void Update()
    {
        #region Disable Movement

        if (disablePlayerMovement)
        {
            directionX = 0;
            directionZ = 0;
        }

        #endregion

        #region Disable Jumping

        if (disablePlayerJumping)
        {
            jumpHeight = 0;
        }

        #endregion

        #region Ground Check

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        #endregion

        #region Player Movement

        playerMovement = inputManager.GetPlayerMovement();

        characterMove = new Vector3(playerMovement.x, 0f, playerMovement.y);

        charController.Move(characterMove * moveSpeed * Time.deltaTime);

        if (characterMove != Vector3.zero)
        {
            gameObject.transform.forward = characterMove;
        }

        charController.Move(playerVelocity * Time.deltaTime);

        #endregion

        #region Player Jumping

        if (inputManager.PlayerJumped() && isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * playerGravity);
        }

        #endregion
    }

    void FixedUpdate()
    {
        //playerVelocity = playerBody.velocity;

        if (isGrounded)
        {
            playerVelocity.y = 0f;
        }

        //float x = Input.GetAxis("Horizontal");
        //float z = Input.GetAxis("Vertical");

        //Vector3 move = transform.right * x + transform.forward * z;

        //charController.Move(move * moveSpeed * Time.deltaTime);

        //if (Input.GetButtonDown("Jump") && isGrounded)
        //{
        //    playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * playerGravity);
        //}

        //if (!isGrounded)
        //{
        //    playerVelocity.y += playerGravity * Time.deltaTime;
        //}

        //charController.Move(playerVelocity * Time.deltaTime);

    }
}
