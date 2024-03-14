    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Events;

//Informing Unity that the player character must have these items
//Change, comment out or remove any of these if they aren't needed
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))] //Change this if you're using a different collider type
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]

public class platformerController : MonoBehaviour, IDataPersistence
{
    /*Code sourced from:
        - Tarodev's Ultimate 2D Controller: https://github.com/Matthew-J-Spencer/Ultimate-2D-Controller/tree/main
        - Game Maker's Toolkit Platformer Toolkit: https://gmtk.itch.io/platformer-toolkit
        - Trevor Mock's CharacterController2D: https://github.com/trevermock/save-load-system/blob/5-bug-fixes-and-polish/Assets/Scripts/Player/CharacterController2D.cs
    */

    #region Character Components
    //Components attached to the player character
    [Header("Character Components")]
    private Rigidbody2D playerBody;
    private BoxCollider2D playerColl;
    private Animator playerAnimator;
    private SpriteRenderer playerSprite;
    public AudioSource playerAudio;

    [Space(10)]

    //Components not attached to the player characer
    [Header("External Components")]
    [SerializeField] Vector3 respawnPoint;

    [Space(10)]

    [Header("Sound Effects")]
    [SerializeField] private AudioSource injuredSFX; 

    [Space(10)]

    //Parameters for the player collider(s)
    [Header("Collider Parameters")]
    [SerializeField] [Tooltip("Length of the ground-checking collider")] private float groundCollSize = 0.95f;
    [SerializeField] [Tooltip("Distance between the ground checking colliders")] private Vector3 collOffset;

    //Parameters for the player layer mask(s)
    [Space(10)]
    [Header("Layer Masks")]
    [SerializeField] [Tooltip("The layers that are read as ground layers")] private LayerMask groundLayer;

    #endregion

    [Space(20)]

    #region Movement Variables
    //Input parameters for the player character's left/right (X axis) movement 
    [Header("Movement Parameters")]
    [SerializeField, Range(0f, 20f)][Tooltip("The maximum movement speed")] private float maxRunSpeed = 16f;
    [SerializeField, Range(0f, 100f)] [Tooltip("The time it takes to reach the maximum speed")] private float maxAcceleration = 52f;
    [SerializeField, Range(0f, 100f)] [Tooltip("The time it takes to completely stop after releasing the movement key")] private float maxDeceleration = 52f;
    [SerializeField, Range(0f, 100f)] [Tooltip("The time it takes to stop when changing direction")] private float maxTurningSpeed = 80f;
    [Space(5)]
    [SerializeField, Range(0f, 100f)] [Tooltip("The time it takes to reach the maximum mid-air speed")] private float maxAirAcceleration;
    [SerializeField, Range(0f, 100f)] [Tooltip("The time it takes to stop mid-air when no movement key is pressed")] private float maxAirDeceleration;
    [SerializeField, Range(0f, 100f)] [Tooltip("The time it takes to stop mid-air when no movement key is pressed")] private float maxAirTurningSpeed = 8f;
    [Space(5)]
    [SerializeField] [Tooltip("The time it takes to stop mid-air when no movement key is pressed")] private float characterFriction;

    [Space(15)]

    //Specific settings for the movement of the player character
    [Header("Movement Variables")]
    [SerializeField] [Tooltip("Enable or disable acceleration. Disabling allows the character to instantly move and come to a stop")] private bool useAcceleration;

    [Space(15)]

    //Calculations for the making the player character move
    [Header("Movement Calculations")]
    public float directionX;    //left or right direction
    private Vector2 desiredVelocity;
    public Vector2 velocity;
    private float maxSpeedChange;
    private float acceleration;
    private float deceleration;
    private float turnSpeed;

    [Space(15)]

    //Feedback variables to get the current state of the player character for movement
    [Header("Current Movement State")]
    [SerializeField] private bool onGround;
    [SerializeField] private bool pressingMovementKey;
    [SerializeField] public bool disableMovement = false;
    [SerializeField] bool disableJumping = false;

    private bool checkingGround;
    //Public ground detection boolean for other scripts to refer to
    public bool checkOnGround()
    {
        return checkingGround;
    }

    #endregion

    [Space(15)]

    #region Jump Variables
    //Input parameters for the player character's jump (Y axis)
    [Header("Jumping Parameters")]
    [SerializeField, Range(2f, 5.5f)] [Tooltip("The maximum jump height")] private float jumpHeight = 7.3f;
    [SerializeField, Range(0.2f, 1.25f)] [Tooltip("Time to reach the peak height of the jump before falling back down")] private float timeToJumpApex;
    [SerializeField, Range(0f, 5f)] [Tooltip("Gravity strength multiplier to apply just after jumping")] private float gravityUpMultiplier = 1f;
    [SerializeField, Range(1f, 10f)] [Tooltip("Gravity strength multiplier to apply when falling after jumping")] private float gravityDownMultiplier = 6.17f;
    [SerializeField, Range(0 , 3)] [Tooltip("The number of times the player can jump in the air")] private int maxExtraJumps = 0;
    
    [Space(15)]

    //Specific settings for the player character's jump
    [Header("Jump Variables")]
    [SerializeField] [Tooltip("The number of times the player can jump in the air")] private bool adjustableJumpHeight;
    [SerializeField, Range(1f, 10f)] [Tooltip("Gravity strength multiplier to apply when releasing the jump key")] private float jumpStopped;
    [SerializeField] [Tooltip("The terminal velocity speed (the maximum speed at which the character can fall at")] private float speedlimit;
    [SerializeField, Range(0f, 0.3f)] [Tooltip("How long should the 'grace' period for jumping last after the character has left the platform")] private float coyoteTime = 0.15f;
    [SerializeField, Range(0f, 0.3f)] [Tooltip("How far from the ground should the jump be cached")] private float jumpBufferTime = 0.15f;

    [Space(15)]

    //Calculations for the making the player character jump
    [Header("Jumping Calculations")]
    [SerializeField] [Tooltip("How quickly the character jumps")] private float jumpSpeed = 1;
    [SerializeField] [Tooltip("The default gravity multiplier")] private float gravityMultiplier;
    private float gravityScale;     //The default gravity scale

    [Space(15)]

    //Feedback variables to get the current state of the player character for jumping
    [Header("Current Jump State")]
    private bool canJumpAgain = false;
    private bool desiredJump;
    private float jumpBufferTimer;
    private float coyoteTimeCounter = 0;
    private bool jumpKeyPressed = false;
    private bool currentlyJumping;

#endregion

    [Space(15)]

    #region Health Variables
    //Settings for when the player character is injured
    [Header("Collision and Injury Parameters")]
    [SerializeField] float respawnTimer;
    [SerializeField] private float hurtAnimDuration;
    private Coroutine deathFlashRoutine;

    [Space(15)]

    [Header("Player Character Health Status")]
    private bool waiting = false;
    private bool injured = false;
    #endregion

    [Space(15)]

    [Header("Events")]
    [SerializeField] public UnityEvent onInjury = new UnityEvent();

    private void Awake()
    {
        //Get all of the player components
        playerBody = GetComponent<Rigidbody2D>();
        playerColl = GetComponent<BoxCollider2D>();     //Change this variable if using a different collider type
        playerSprite = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();

        gravityScale = 1f;
    }

    #region Reading Input Values
    public void OnMovement(InputAction.CallbackContext context)
    {
        //Reading the input values when moving left or right,
        //ranging from -1 (left) to 1 (right) on the X-axis

        if (!disableMovement)
        {
            directionX = context.ReadValue<float>();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //Reading the input value when a jump button is pressed,

        if (!disableMovement && !disableJumping)
        {
            //Using started and canceled contexts to read whether
            //the jump button is being held or not
            if (context.started)
            {
                desiredJump = true;
                jumpKeyPressed = true;
            }

            if (context.canceled)
            {
                jumpKeyPressed = false;
            }
        }
    }

    #endregion


    #region Checkpoints, Collision & Respawning
    public void Checkpoint(Vector3 checkpointPos)
    {
        //When a checkpoint is activated, get its position
        respawnPoint = checkpointPos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
      //Start the injured coroutine if the player character collides with
      //something that can hurt them (layer 7)
      //something that can kill them (layer 8)
      if (collision.gameObject.layer == 7 || collision.gameObject.layer == 8)
        {
            if (injured == false)
            {
                //If injured, stop the player character moving
                if (collision.gameObject.layer == 8)
                {
                    playerBody.velocity = Vector2.zero;
                }

                injured = true;
                //injuredRoutine();
            }
        }


    }

    public void injuredRoutine()
    {
        disableMovement = true;

        //Screenshake event
        onInjury.Invoke();

        SoundManager.instance.PlaySFX(injuredSFX.clip);

        

    }

    private void respawnRoutine()
    {
        transform.position = respawnPoint;
        disableMovement = false;
        injured = false;
    }


    #endregion

    #region Animations
    
    //These two stop functions and the Wait enumerator 
    //are used to create a 'hit-stop' (momentary pause) effect on death
    public void Stop(float duration)
    {
        Stop(duration, 0.0f);
    }

    public void Stop(float duration, float timeScale)
    {
        if (waiting)
        {
            return;
        }
        Time.timeScale = timeScale;
        StartCoroutine(Wait(duration));
    }

    IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        waiting = false;
    }

    //These two functions create a white flashing effect 
    //when the player character dies
    public void Flashing()
    {
        if (deathFlashRoutine != null)
        {
            StopCoroutine(deathFlashRoutine);
        }

        deathFlashRoutine = StartCoroutine(FlashingRoutine());
    }

    private IEnumerator FlashingRoutine()
    {
        //Enable the flash
        playerSprite.enabled = true;

        //Pause for a predefined number of seconds
        yield return new WaitForSeconds(hurtAnimDuration);

        //Hide the flashing animation
        playerSprite.enabled = false;

        //Set routine to null when finished
        deathFlashRoutine = null;
    }


    #endregion


    void Update()
    {
        #region Preventing movement
        //Useful if playing animations, e.g. death animation
        if (disableMovement)
        {
            directionX = 0;
        }
        #endregion

        #region Preventing jumping
        //Useful if playing animations, e.g. death animation
        if (disableJumping)
        {
            jumpSpeed = 0;
        }
        #endregion

        #region Checking if the player is currently on the ground
        //Using a pair of raycasts to determine whether the player is standing on a game object assigned with the ground layer
        checkingGround = Physics2D.Raycast(transform.position + collOffset, Vector2.down, groundCollSize, groundLayer) || Physics2D.Raycast(transform.position - collOffset, Vector2.down, groundCollSize, groundLayer);
        #endregion

        #region When the player character is moving

        //Checking if an movement key is pressed and in which direction
        //Will also flip the sprite when the direction is changed

        if (directionX != 0)
        {
            transform.localScale = new Vector3(directionX > 0 ? 1 : -1, 1, 1);
            pressingMovementKey = true;
        }
        else
        {
            pressingMovementKey = false;
        }

        //Calculating the desired velocity: direction the player character is facing * player character's maximum running speed 
        //Comment this line out if using friction
        desiredVelocity = new Vector2(directionX, 0f) * Mathf.Max(maxRunSpeed - characterFriction, 0f);

        #endregion

        #region When the player character jumps
        setJumpPhysics();

        //Check if the player character is on the ground
        onGround = checkOnGround();

        //Jump buffer allows a jump to be queued, which plays the next time the 
        //player character touches the ground
        if (jumpBufferTime > 0)
        {
            //Counting up instead of disabling the desireJump variable
            //the nowJump function will be repeatedly activated
            if (desiredJump)
            {
                jumpBufferTimer += Time.deltaTime;

                if (jumpBufferTimer > jumpBufferTime)
                {
                    //If the timer exceeds the jump buffer variable, disable the desireJump variable
                    desiredJump = false;
                    jumpBufferTimer = 0;
                }
            }
        }

        //Coyote time countdown
        //Checking whether the player character is on the ground and is currently jumping
        if (!currentlyJumping && !onGround)
        {
            coyoteTimeCounter += Time.deltaTime;
        }
        else
        {
            //Reset the timer when the player character touches the ground or jumps
            coyoteTimeCounter = 0;
        }

        #endregion
    }

    #region Physics calculations for jumping
    private void setJumpPhysics()
    {
        //Determine the player character's gravity scale, then multiply it by the gravity multiplier
        Vector2 newGravity = new Vector2(0, (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex));
        playerBody.gravityScale = (newGravity.y / Physics2D.gravity.y) * gravityMultiplier;
    }
    #endregion

    private void FixedUpdate()
    {
        #region Movement
        //Check the player character is standing on something in the ground layer
        onGround = checkOnGround();

        //Getting the current velocity of the player character's rigidbody
        velocity = playerBody.velocity;

        //Calculating the movement, depending on whether "Instant Movement" has been enabled
        if(useAcceleration)
        {
            enableAcceleration();
        }
        else
        {
            if (onGround)
            {
                disableAcceleration();
            }
            else
            {
                enableAcceleration();
            }
        }
        #endregion

        #region Jumping with Frame Accuracy
        if (desiredJump)
        {
            nowJump();
            playerBody.velocity = velocity;

            //Skipping gravity calculations this frame to make sure that the 
            //currentlyJumping boolean isn't set to false
            //This prevents a bug where you can double jump with coyote time
            return;
        }

        calculateGravity();

        #endregion
    }

    #region Visualising Colliders for Debugging
    private void OnDrawGizmos()
    {
        //Drawing ground colliders
        if (onGround) 
        { 
            Gizmos.color = Color.yellow;  
        } 
        else 
        { 
            Gizmos.color = Color.red; 
        }

        Gizmos.DrawLine(transform.position + collOffset, transform.position + collOffset + Vector3.down * groundCollSize);
        Gizmos.DrawLine(transform.position - collOffset, transform.position - collOffset + Vector3.down * groundCollSize);
    }

    #endregion

    #region Movement with and without acceleration & deceleration

    private void enableAcceleration()
    {
        //Setting the acceleration, deceleration and turn speed variables, 
        //depending whether the player character is in the air or not

        acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        deceleration = onGround ? maxDeceleration : maxAirDeceleration;
        turnSpeed = onGround ? maxTurningSpeed : maxAirTurningSpeed;


        if(pressingMovementKey)
        {
            //Checking if the x-axis of the input direction is positive or negative, 
            //and if it is opposite to the current movement, then the player character is turning around

            if (Mathf.Sign(directionX) != Mathf.Sign(velocity.x))
            {
                
                maxSpeedChange = turnSpeed * Time.deltaTime;
            }
            else
            {
                //If the x-axis of the input direction matches the current movement
                //then start accelerating the player character
                maxSpeedChange = acceleration * Time.deltaTime;
            }
        }
        else
        {
            //If no movement key is pressed, start decelerating the player character
            maxSpeedChange = deceleration * Time.deltaTime;
        }

        //Moving the velocity until it reaches the desired velocity, 
        //at the rate of the calculated maximum speed change 
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        //Update the player character's rigidbody with the new velocity
        playerBody.velocity = velocity;

    }

    private void disableAcceleration()
    {
        //If acceleration and deceleration is disabled, apply the desired velocity to the player character's rigidbody
        velocity.x = desiredVelocity.x;

        playerBody.velocity = velocity;
    }

    #endregion

    #region Calculating gravity for jumping

    private void calculateGravity()
    {
        //Changing the player character's gravity depending on whether the character
        //is moving closer to or further from zero in the Y-Axis

        //If the player character is jumping, i.e. moving away from zero on the Y-Axis
        if (playerBody.velocity.y > 0.01f)
        {
            if (onGround)
            {
                //Preventing the gravity from changing if the player character
                //is standing on something, e.g. a moving platform
                gravityMultiplier = gravityScale;
            }
            else
            {
                //If adjustable jump height is enabled
                if (adjustableJumpHeight)
                {
                    //Apply the upward multiplier if the jump key is held
                    if (jumpKeyPressed && currentlyJumping)
                    {
                        gravityMultiplier = gravityUpMultiplier;
                    }
                    //Apply a special downward multiplier when the player lets go of the jump key
                    else
                    {
                        gravityMultiplier = jumpStopped;
                    }
                }
                else
                {
                    gravityMultiplier = gravityUpMultiplier;
                }
            }
        }

        //Else if the player character is falling from the jump, i.e. moving toward zero on the Y-Axis
        else if (playerBody.velocity.y < -0.01f)
        {
            if (onGround)
            {
                //Preventing the gravity from changing if the player character
                //is standing on something, e.g. a moving platform 
                gravityMultiplier = gravityScale;
            }
            else
            {
                //Otherwise, apply the downward gravity multiplier 
                gravityMultiplier = gravityDownMultiplier;
            }
        }
        //Else, if there is no vertical movement
        else
        {
            if (onGround)
            {
                currentlyJumping = false;
            }

            gravityMultiplier = gravityScale;
        }

        //Set the velocity of the player character's rigidbody, but
        //clamp the Y variable within the bounds of the speed limit, for the terminal velocity assist option
        playerBody.velocity = new Vector3(velocity.x, Mathf.Clamp(velocity.y, -speedlimit, 100));
    }

    #endregion

    #region Jumping when requested
    private void nowJump()
    {
        //Create the jump if the player character is standing on the ground, or is in coyote time or has another jump available
        if (onGround || (coyoteTimeCounter > 0.03f && coyoteTimeCounter < coyoteTime) || canJumpAgain)
        {
            desiredJump = false;
            jumpBufferTimer = 0;
            coyoteTimeCounter = 0;

            //If more than one jump is allowed, allow the player character to jump again but only once (double jump)
            //Change the maxExtraJumps variable to allow for more than one extra jump 
            canJumpAgain = (maxExtraJumps == 1 && canJumpAgain == false);

            //Calculating the strength of the jump using the gravity and current statistics
            jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * playerBody.gravityScale * jumpHeight);

            //Calculating the jumpSpeed variable depending on whether the player character is
            //jumping or falling from the jump (e.g. when double jumping)
            //This ensures each jump has the exact same strength, regardless of the velocity
            if (velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
            }
            else if (velocity.y < 0f)
            {
                jumpSpeed += Mathf.Abs(playerBody.velocity.y);
            }

            //Apply the updated jumpSpeed variable to the velocity
            velocity.y += jumpSpeed;
            currentlyJumping = true;
        }

        if (jumpBufferTime == 0)
        {
            //If there is no jump buffer, disable desiredJump immediately after the jump key is pressed
            desiredJump = false;
        }
    }
    #endregion

    #region Jump/Bounce Pad/Object
    public void bounceUp(float bounceAmount)
    {
        //If the player touches a jump pad or object that boosts them into the air
        playerBody.AddForce(Vector2.up * bounceAmount, ForceMode2D.Impulse);
    }
    #endregion

    #region Saving & Loading
    public void SaveData(GameData data) 
    {

    }
    public void LoadData(GameData data)
    {

    }

    #endregion

}
