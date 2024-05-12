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
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float movementSpeed;
    private float directionX;
    private float directionZ;
    [SerializeField] private float playerGravity;
    
    [Space(10)]    

    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float timeToJumpApex;

    private Vector3 playerVelocity;

    [Space(15)]

    [Header("Cinemachine Virtual Camera Reference")]
    [SerializeField] private Transform cameraTransform;

    [Space(15)]

    [Header("Debugging")]
    [SerializeField] private bool disablePlayerMovement = true;
    [SerializeField] private bool disablePlayerJumping = true;
    [SerializeField] private bool onGround;
    [SerializeField] private bool canJump;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isSprinting;

    #endregion

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        characterBodyTransform = characterBodyObject.transform;
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        GameManager.Instance.SetAiBehaviour += EnablePlayerMovement;
    }

    #region Enable and Disable Player Movement

    public void EnablePlayerMovement()
    {
        disablePlayerMovement = false;
        disablePlayerJumping = false;
    }

    public void DisablePlayerMovement()
    {
        disablePlayerMovement = true;
        disablePlayerJumping = true;
    }

    #endregion

    private void Update()
    {
        #region Disable Jumping

        if (disablePlayerJumping)
        {
            jumpHeight = 0;
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

        playerMovement = InputManager.Instance.GetPlayerMovement();
        characterMove = new Vector3(playerMovement.x, 0f, playerMovement.y);

        characterMove = cameraTransform.forward * characterMove.z + cameraTransform.right * characterMove.x;
        characterMove.y = 0f;

        //Sprinting

        isSprinting = InputManager.Instance.isPlayerSprintingThisFrame;

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

        if (InputManager.Instance.PlayerJumped() && onGround && canJump)
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

        if (InputManager.Instance.PlayerStartedTrainingCourse() )
        {
            EnablePlayerMovement();
        }

    }
}
