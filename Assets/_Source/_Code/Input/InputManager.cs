using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Cinemachine;
using UnityEngine.InputSystem.Interactions;

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

    public bool isGamepad = false;
    public bool isKeyboard = true;

    //Input Action References
    public InputActionReference fireGunButton;
    public InputActionReference sprintButton;

    [SerializeField] public string _currentControlScheme;

    [Space(15)]

    [Header("Mouse Settings")]
    public bool mouseAcceleration = false;
    public bool invertMouseY = false;

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
    [SerializeField, Range(90f, 180f)] private float _FOV = 90f;

    public bool updateFOV;

    [Header("Toggle or Hold Shift to Sprint")]
    public bool holdToSprint = true;

    //Booleans for returning which type of button interaction for firing the gun
    public bool isTappingFireButton = false;
    public bool isPressingFireButton = false;
    public bool isHoldingFireButton = false;

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

    public bool PlayerPressedReload()
    {
        return playerActions.Player.Reload.triggered;
    }

    public bool PlayerStartedTrainingCourse()
    {
        return playerActions.Player.StartTrainingCourse.triggered;
    }

    public bool PlayerPressedSprintThisFrame()
    {
        if (holdToSprint)
        {
            if (playerActions.Player.Sprint.WasPressedThisFrame())
            {
                Debug.Log("Player is holding sprint!");
                return playerActions.Player.Sprint.triggered;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (playerActions.Player.Sprint.IsPressed())
            {
                Debug.Log("Player tapped sprint!");
                return playerActions.Player.Sprint.triggered;
            }
            else
            {
                return false;
            }
        }
    }

    public bool IsPlayerHoldingTheFireButton { get; private set; }
    public bool IsPlayerTappingTheFireButton { get; private set; }

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
        if (!(fireGunButton.action.interactions.Contains("Tap") && fireGunButton.action.interactions.Contains("Press") && fireGunButton.action.interactions.Contains("Hold")))
        {
            return;
        }

        if (!(sprintButton.action.interactions.Contains("Tap") && sprintButton.action.interactions.Contains("Hold")))
        {
            return;
        }

        #region Gun action interaction context

        fireGunButton.action.started += context =>
        {
            if (context.interaction is TapInteraction)
            {
                IsPlayerHoldingTheFireButton = false;
                IsPlayerTappingTheFireButton = true;

                Debug.Log("Player is tapping the fire button.");
            }
            else if (context.interaction is HoldInteraction)
            {
                IsPlayerHoldingTheFireButton = true;
                IsPlayerTappingTheFireButton = false;
                Debug.Log("Player is holding the fire button.");
            }
        };

        #endregion

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

    #region Firing Gun

    //public void IsPlayerHoldingFireButton()
    //{
    //    isHoldingFireButton = true;
    //    isTappingFireButton = false;
    //}

    //public void IsPlayerTappingFireButton()
    //{
    //    isTappingFireButton = true;
    //    isHoldingFireButton = false;
    //}

    #endregion

    private void Update()
    {
        #region Update the first person camera (Cinemachine virtual camera) FOV (Field Of View)

        if (updateFOV)
        {
            vCam.SetFocalLength(_FOV);
            Debug.Log("Camera FOV is: " + vCam.GetFocalLength());

            updateFOV = false;
        }

        #endregion

        #region Get The Current Input Device

        if (isGamepad)
        {
            //Debug.Log("Input Manager: Gamepad connected.");
            _currentControlScheme = "Gamepad";
        }
        else if (isKeyboard)
        {
            //Debug.Log("Input Manager: Keyboard connected.");
            _currentControlScheme = "Keyboard";
        }

        #endregion

        #region Getting Interaction Type for the Shoot/Fire Button

        //Action started
        fireGunButton.action.started += context =>
        {
            isTappingFireButton = false;
            isPressingFireButton = false;
            isHoldingFireButton = false;

            if (context.interaction is TapInteraction)
            {
                isTappingFireButton = true;
            }
            else if (context.interaction is PressInteraction)
            {
                isPressingFireButton = true;
            }
            else if (context.interaction is HoldInteraction)
            {
                isHoldingFireButton = true;
            }
        };

        //Action performed
        fireGunButton.action.performed += context =>
        {
            if (context.interaction is TapInteraction)
            {
                isTappingFireButton = false;
            }
            else if (context.interaction is PressInteraction)
            {
                isPressingFireButton = false;
            }
            else if (context.interaction is HoldInteraction)
            {
                isHoldingFireButton = false;
            }
        };

        //Action cancelled
        fireGunButton.action.canceled += context =>
        {
            if (context.interaction is TapInteraction)
            {
                isTappingFireButton = false;
            }
            else if (context.interaction is PressInteraction)
            {
                isPressingFireButton = false;
            }
            else if (context.interaction is HoldInteraction)
            {
                isHoldingFireButton = false;
            }
        };

        #endregion

    }
}
