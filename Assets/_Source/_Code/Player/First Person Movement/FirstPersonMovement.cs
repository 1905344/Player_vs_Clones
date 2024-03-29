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
    private float directionX;
    private float directionZ;
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

        if (isGrounded)
        {
            playerVelocity.y = 0f;
        }

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
}
