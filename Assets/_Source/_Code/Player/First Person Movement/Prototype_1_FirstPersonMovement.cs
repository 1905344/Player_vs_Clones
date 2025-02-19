using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Prototype_1_FirstPersonMovement : MonoBehaviour
{
    #region Variables

    [Header("Player Components")]
    [SerializeField] private CharacterController charController;
    [SerializeField] private GameObject characterBodyObject;
    [SerializeField] private Rigidbody characterRigidBody;
    [SerializeField] private CapsuleCollider characterCollider;
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

    [Space(10)]

    [Header("Gun Recoil Variables")]
    [SerializeField] private bool isGunRecoilActive = false;
    [SerializeField, Tooltip("How fast the player should move after applying recoil")] private float gunRecoilMoveSpeed;
    [SerializeField, Tooltip("How long the recoil should move the player for")] private float recoilMoveTimeInterval;
    [SerializeField, Tooltip("The force applied to the player - how far you want the player to move")] private float gunRecoilMoveAmount;
    [SerializeField, Tooltip("How much force to apply to smaller objects if the player collides with them")] private float collisionPushStrength;
    private float recoilMoveTimer;
    [SerializeField] private Vector3 forceBackwardMove;

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
        characterRigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        Prototype_1_GameManager.Instance.OnStartGame += EnablePlayerMovement;
        Prototype_1_GameManager.Instance.PlayerKilled += DisablePlayerMovement;

        Prototype_1_GameManager.Instance.GunRecoil += ApplyGunRecoil;
        Prototype_1_GameManager.Instance.GunRecoil -= StopGunRecoil;
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

    #region Gun Recoil Effecting Movement

    private void ApplyGunRecoil()
    {
        if (isGunRecoilActive)
        {
            return;
        }

        #region Debug

        if (Prototype_1_GameManager.Instance.toggleDebug)
        {
            Debug.Log("FirstPersonMovement: Gun's recoil is moving the character backwards");
        }

        #endregion

        forceBackwardMove = cameraTransform.TransformDirection(Vector3.back);
        isGunRecoilActive = true;
        recoilMoveTimer += Time.deltaTime;
    }

    private void StopGunRecoil()
    {
        #region Debug

        if (Prototype_1_GameManager.Instance.toggleDebug)
        {
            Debug.Log("FirstPersonMovement: Stopping gun recoil movement.");
        }

        #endregion

        if (!isGunRecoilActive)
        {
            return;
        }

        recoilMoveTimer = 0f;
        isGunRecoilActive = false;
    }

    #endregion

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //characterRigidBody = hit.collider.attachedRigidbody;

        if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Enemy"))
        {
            if (!onGround)
            {
                return;
            }

            StopGunRecoil();
        }
        
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        Vector3 pushDireciton = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        characterRigidBody.linearVelocity = pushDireciton * collisionPushStrength;
    }

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
        //characterMove = new Vector3(playerMovement.x, 0f, playerMovement.y);

        //Recoil moves the player
        //characterMove = new Vector3(gunRecoilMoveAmount.x, 0f, gunRecoilMoveAmount.y);

        if (isGunRecoilActive)
        {
            //Vector3 forceBackwardMove = cameraTransform.forward * -1;

            //float moveTimer = recoilMoveTimer / recoilMoveTimeInterval;

            if (recoilMoveTimer > 0)
            {
                recoilMoveTimer += Time.deltaTime;

                //characterMove = -cameraTransform.forward * characterMove.z + cameraTransform.right * characterMove.x;
                //characterMove.y = 0f;

                //Might need to change Time.deltaTime to a fixed time variable
                //charController.Move(characterMove * gunRecoilMoveSpeed * Time.deltaTime);
                //charController.Move(playerVelocity * Time.deltaTime);

                float setMoveSpeed = gunRecoilMoveSpeed * gunRecoilMoveAmount;
                charController.SimpleMove(forceBackwardMove * setMoveSpeed);
                
                if (recoilMoveTimer > recoilMoveTimeInterval || characterRigidBody.CompareTag("Wall") || characterRigidBody.CompareTag("Enemy"))
                {
                    #region Debug

                    if (Prototype_1_GameManager.Instance.toggleDebug)
                    {
                        if (characterRigidBody.CompareTag("Wall") || characterRigidBody.CompareTag("Enemy"))
                        {
                            Debug.Log("FirstPersonMovement: Stopping recoil because character has hit a wall.");
                        }
                    }

                    #endregion

                    Vector3 stopMove = new Vector3(0f, playerGravity, 0f);
                    charController.SimpleMove(stopMove * Time.deltaTime);

                    StopGunRecoil();
                }
            }
        }

        if (!onGround && !isGunRecoilActive)
        {
            Vector3 fallMove = new Vector3(0f, playerGravity, 0f);
            charController.SimpleMove(fallMove * Time.deltaTime);
        }

        //characterMove = cameraTransform.forward * characterMove.z + cameraTransform.right * characterMove.x;
        //characterMove.y = 0f;

        //Sprinting

        //isSprinting = InputManager.Instance.isPlayerSprintingThisFrame;

        //if (isSprinting)
        //{
        //    movementSpeed = sprintSpeed;
        //    //Debug.Log("Character is sprinting!");
        //}
        //else
        //{
        //    movementSpeed = moveSpeed;
        //    //Debug.Log("Character is not sprinting!");
        //}

        //charController.Move(characterMove * movementSpeed * Time.deltaTime);
        //charController.Move(playerVelocity * Time.deltaTime);

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
    }
}
