using System.Collections;
using System.Collections.Generic;
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

    [Space(5)]

    [SerializeField] private float groundDistance;

    [Space(15)]

    [Header("Movement Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float playerGravity;
    [SerializeField] private float jumpHeight;

    [Space(10)]

    [SerializeField] private Vector3 playerVelocity;
    [SerializeField] private bool isGrounded;

    //Input Actions
    private InputAction playerMovement;

    #endregion

    #region OnEnable and OnDisable

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    #endregion

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded)
        {
            playerVelocity.y = 0f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        charController.Move(move * moveSpeed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * playerGravity);
        }

        if (!isGrounded)
        {
            playerVelocity.y += playerGravity * Time.deltaTime;
        }

        charController.Move(playerVelocity * Time.deltaTime);

    }
}
