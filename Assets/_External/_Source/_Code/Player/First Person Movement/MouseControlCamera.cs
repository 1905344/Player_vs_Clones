using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseControlCamera : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform playerBody;

    [Space(10)]

    [SerializeField, Tooltip("How sensitive the mouse is")] private float mouseSens;
    private float xRotation;

    //Input Actions
    private InputAction mouseLook;

    #endregion

    #region OnEnable and OnDisable

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }

    #endregion

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}
