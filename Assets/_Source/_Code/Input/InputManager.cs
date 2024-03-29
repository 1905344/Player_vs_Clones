using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Cinemachine;

public class InputManager : MonoBehaviour
{
    #region Variables

    private static InputManager _instance;

    public static InputManager Instance 
    {  
        get 
        { 
            return _instance; 
        } 
    }

    public static PlayerControls playerActions;
    public static event Action<InputActionMap> changeActionMap;

    public bool isGamepad;
    public bool isKeyboard;

    [SerializeField] public string _currentControlScheme;

    [Space(15)]

    [Header("Mouse Settings")]
    [SerializeField] private bool mouseAcceleration = false;
    [SerializeField] private bool invertMouseY = false;

    [Space(10)]

    [Header("Mouse Sensitivity Settings")]
    [SerializeField, Tooltip("How sensitive the mouse is on the horizontal axis")]
    public float mouseHorizontalSensitivity = 0.3f;

    [SerializeField, Tooltip("How sensitive the mouse is on the vertical axis")]
    public float mouseVerticalSensitivity = 0.3f;

    [Space(15)]

    [Header("Cinemachine Virtual Camera Reference")]
    [SerializeField] CinemachineVirtualCamera vCam;

    [Space(10)]

    [Header("Camera Field of View")]
    [SerializeField, Range(90f,180f)] private float _FOV = 90f;

    public bool updateFOV;

    public static bool HasDevice<T>(PlayerInput input) where T : InputDevice
    {
        for (int i = 0; i < input.devices.Count; i++)
        {
            if (input.devices[i] is T)
            {
                return true;
            }
        }

        return false;
    }

    public bool PlayerJumped()
    {
        return playerActions.Player.Jump.triggered;
    }

    public Vector2 GetPlayerMovement()
    {
        return playerActions.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return playerActions.Player.MouseLook.ReadValue<Vector2>();
    }

    public float GetMouseHorizontalSensitivity()
    {
        return mouseHorizontalSensitivity;
    }

    public float GetMouseVerticalSensitivity()
    {
        return mouseVerticalSensitivity;
    }
        
    public bool PlayerFiredGun()
    {
        return playerActions.Player.Fire.triggered;
    }

    public bool PlayerPressedReload()
    {
        return playerActions.Player.Reload.triggered;
    }

    #endregion

    public void OnControlsChanged(PlayerInput input)
    {
        isGamepad = input.currentControlScheme.Equals("Gamepad");
        isKeyboard = input.currentControlScheme.Equals("Keyboard");
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        playerActions = new PlayerControls();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        ToggleActionMap(playerActions.UI);

        vCam.SetFocalLength(_FOV);
        Debug.Log("Camera FOV is: " + vCam.GetFocalLength());

        vCam.SetCameraPOV(mouseHorizontalSensitivity, mouseVerticalSensitivity, mouseAcceleration, invertMouseY);
    }

    public void OnEnable()
    {
        playerActions.Enable();
    }

    public void OnDisable()
    {
        playerActions.Disable();
    }

    public static void ToggleActionMap(InputActionMap actionMap)
    {
        if (actionMap.enabled)
        {
            return;
        }

        playerActions.Disable();
        changeActionMap?.Invoke(actionMap);
        actionMap.Enable();
    }

    private void Update()
    {
        if (updateFOV)
        {
            vCam.SetFocalLength(_FOV);
            Debug.Log("Camera FOV is: " + vCam.GetFocalLength());

            updateFOV = false;
        }

        if (isGamepad)
        {
            Debug.Log("Input Manager: Gamepad connected.");
        }
        else if (isKeyboard)
        {
            Debug.Log("Input Manager: Keyboard connected.");
        }
    }
}
