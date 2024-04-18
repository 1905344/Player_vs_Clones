using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonMovement : MonoBehaviour
{
    #region Variables

    [Header("Player Components")]
    [SerializeField] private CharacterController charController;
    [SerializeField] private GameObject characterBodyObject;
    private Transform characterBodyTransform;

    [Space(15)]

    [Header("Movement Variables")]
    [SerializeField] private Vector2 playerMovement;
    [SerializeField] private Vector3 characterMove;
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float sprintSpeed = 20f;
    [SerializeField] private float movementSpeed;
    private float directionX;
    private float directionZ;
    [SerializeField] private float playerGravity = -9.81f;
    
    [Space(10)]    

    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float timeToJumpApex;

    private Vector3 playerVelocity;

    [Space(15)]

    [Header("Cinemachine Virtual Camera Reference")]
    [SerializeField] private Transform cameraTransform;

    [Space(15)]

    [Header("Debugging")]
    [SerializeField] private bool disablePlayerMovement = false;
    [SerializeField] private bool disablePlayerJumping = false;
    [SerializeField] private bool onGround;
    [SerializeField] private bool canJump;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isSprinting;

    //Input manager
    private InputManager inputManager;

    #endregion

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        characterBodyTransform = characterBodyObject.transform;
    }

    private void Start()
    {
        inputManager = InputManager.Instance;
        GameManager.Instance.TrainingCourseStarted += EnablePlayerMovement;
        GameManager.Instance.TrainingCourseEnded += DisablePlayerMovement;

        cameraTransform = Camera.main.transform;
    }

    #region Enable and Disable Player Movement

    private void EnablePlayerMovement(int ID)
    {
        disablePlayerMovement = true;
        disablePlayerJumping = true;
    }

    private void DisablePlayerMovement(int ID)
    {
        disablePlayerMovement = false;
        disablePlayerJumping = false;
    }

    #endregion

    private void Update()
    {
        #region Disable Movement

        if (disablePlayerMovement)
        {
            inputManager.OnDisable();
        }
        else
        {
            inputManager.OnEnable();
        }

        #endregion

        #region Disable Jumping

        if (disablePlayerJumping)
        {
            jumpHeight = 0;
        }
        else
        {
            jumpHeight = 1f;
        }

        #endregion

        #region Ground Check

        onGround = charController.isGrounded;

        if (onGround && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            canJump = true;
            isJumping = false;
        }
        
        #endregion

        #region Player Movement and Sprinting

        playerMovement = inputManager.GetPlayerMovement();
        characterMove = new Vector3(playerMovement.x, 0f, playerMovement.y);

        characterMove = cameraTransform.forward * characterMove.z + cameraTransform.right * characterMove.x;
        characterMove.y = 0f;

        //Sprinting

        isSprinting = inputManager.isPlayerSprintingThisFrame;

        if (isSprinting)
        {
            movementSpeed = sprintSpeed;
            //Debug.Log("Character is sprinting!");
        }
        else
        {
            movementSpeed = moveSpeed;
            //Debug.Log("Character is not sprinting!");
        }

        charController.Move(characterMove * movementSpeed * Time.deltaTime);
        charController.Move(playerVelocity * Time.deltaTime);

        //Rotating the body game object when the player rotates the camera with the mouse
        characterBodyTransform.rotation = cameraTransform.rotation;

        #endregion

        #region Player Jumping

        if (inputManager.PlayerJumped() && onGround && canJump)
        {
            //Debug.Log("Jumping!");
            canJump = false;
            isJumping = true;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * playerGravity);
        }
        else if (onGround && playerVelocity.y <= 0.01f) 
        {
            isJumping = false;
            canJump = true;
        }

        playerVelocity.y += playerGravity * Time.deltaTime;

        #endregion

    }
}
