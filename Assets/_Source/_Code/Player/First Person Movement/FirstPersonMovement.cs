using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonMovement : MonoBehaviour
{
    #region Variables

    [Header("Player Components")]
    [SerializeField] private CharacterController charController;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Rigidbody playerBody;

    [Space(5)]

    [SerializeField] private float groundDistance;

    [Space(15)]

    [Header("Movement Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float directionX;
    [SerializeField] private float directionZ;
    [SerializeField] private float playerGravity;
    
    [Space(10)]    

    [SerializeField] private float jumpHeight;
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

    //Input Actions
    private InputAction playerMove;
    private InputAction playerJump;

    #endregion

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody>();
        charController = GetComponent<CharacterController>();
    }

    #region OnEnable and OnDisable

    private void OnEnable()
    {
        InputManager.inputActions.Player.Move.performed += OnPlayerMove;
        InputManager.inputActions.Player.Jump.performed += OnPlayerJump;
    }

    private void OnDisable()
    {
        InputManager.inputActions.Player.Move.performed -= OnPlayerMove;
        InputManager.inputActions.Player.Jump.performed -= OnPlayerJump;
    }

    #endregion

    #region Input

    public void OnPlayerMove(InputAction.CallbackContext context)
    {
        if (!disablePlayerMovement)
        {
            directionX = context.ReadValue<float>();
            directionZ = context.ReadValue<float>();
        }
    }
   
    public void OnPlayerJump(InputAction.CallbackContext context)
    {
        if (!disablePlayerMovement && !disablePlayerJumping)
        {
            canJump = true;
        }
        
        if (context.canceled && !isGrounded)
        {
            canJump = false;
        }
    }

    #endregion

    #region Jump Physics

    private void JumpPhysics()
    {
        Vector3 newGravity = new Vector3(0, (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex));
        playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * playerGravity);
    }

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

        if (directionX != 0)
        {
            transform.localScale = new Vector3(directionX > 0 ? 1 : -1, 1, 1);
        }

        if (directionZ != 0)
        {
            transform.localScale = new Vector3(directionZ > 0 ? 1 : -1, 1, 1);
        }

        Vector3 move = transform.right * directionX + transform.forward * directionZ;

        #endregion

        #region Player Jumping

        if (canJump && isGrounded)
        {
            OnJump();
            playerBody.velocity = playerVelocity;
            return;
        }

        #endregion
    }

    void FixedUpdate()
    {
        playerVelocity = playerBody.velocity;

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
