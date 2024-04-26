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

    public bool isPlayerInTrainingCourse = false;
    public bool isPlayerFinishedTraining = false;
    private int getTrainingCourseID = 1;

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
        return playerActions.Training.StartTrainingCourse.triggered;
    }

    public bool isPlayerSprintingThisFrame { get; private set; }

    public bool IsPlayerHoldingTheFireButton { get; private set; }
    public bool IsPlayerTappingTheFireButton { get; private set; }

    #endregion

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

        //Sprinting
        playerActions.Player.Sprint.performed += SprintThisFrame;
        playerActions.Player.Sprint.canceled += StopSprintingThisFrame;

        //Shooting/Pressing the fire button
        playerActions.Player.Fire.started += FiringGunThisFrame;
        playerActions.Player.Fire.performed += StopFiringGunThisFrame;

        //Starting the training course
        playerActions.Training.StartTrainingCourse.performed += StartTraining;

        //Game manager event for when the player has completed all the training courses
        GameManager.Instance.FinishedTraining += OnFinishedTraining;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerActions.Training.StartTrainingCourse.Enable();
        ToggleActionMap(playerActions.Training);
    }

    private void Start()
    {
        ToggleActionMap(playerActions.UI);

        vCam.SetFocalLength(_FOV);
        Debug.Log("Camera FOV is: " + vCam.GetFocalLength());

        vCam.SetCameraPOV(mouseHorizontalSensitivity, mouseVerticalSensitivity, mouseAcceleration, invertMouseY);
    }

    #region OnEnable and OnDisable

    public void OnEnable()
    {
        ToggleActionMap(playerActions.Player);
        playerActions.Training.Disable();
        playerActions.Player.Enable();
    }

    public void OnDisable()
    {
        ToggleActionMap(playerActions.Training);
        playerActions.Training.Enable();
        playerActions.Player.Disable();
    }

    #endregion

    #region Input

    #region Sprint

    private void SprintThisFrame(InputAction.CallbackContext context)
    {
        if (holdToSprint)
        {
            if (context.duration > 0.51f && !isPlayerSprintingThisFrame)
            {
                isPlayerSprintingThisFrame = true;
            }
        }
        else
        {
            if (!isPlayerSprintingThisFrame)
            {
                if (context.duration < 0.5f)
                {
                    isPlayerSprintingThisFrame = true;
                }
            }
            else if (context.duration < 0.5f && isPlayerSprintingThisFrame)
            {
                isPlayerSprintingThisFrame = false;
            }
        }
    }

    private void StopSprintingThisFrame(InputAction.CallbackContext context)
    {
        isPlayerSprintingThisFrame = false;
    }

    #endregion

    #region Firing the Gun

    private void FiringGunThisFrame(InputAction.CallbackContext context)
    {
        if (context.duration < 0.5f && !IsPlayerTappingTheFireButton)
        {
            IsPlayerTappingTheFireButton = true;
            IsPlayerHoldingTheFireButton = false;
            Debug.Log("Player is tapping the fire gun input key.");
        }
        else if (context.duration > 0.51f && !IsPlayerHoldingTheFireButton)
        {
            IsPlayerHoldingTheFireButton = true;
            IsPlayerTappingTheFireButton = false;

            Debug.Log("Player is holding the fire gun input key.");
        }
    }

    private void StopFiringGunThisFrame(InputAction.CallbackContext context)
    {
        IsPlayerHoldingTheFireButton = false;
        IsPlayerTappingTheFireButton = false;
    }

    #endregion

    #region Starting the Training Course and Disabling the Start Training Course Button

    private void StartTraining(InputAction.CallbackContext context)
    {
        OnEnable();
        getTrainingCourseID = TrainingCourseManager.Instance.currentTrainingCourse;

        if (isPlayerInTrainingCourse)
        {
            return;
        }
        else
        {
            isPlayerInTrainingCourse = true;
            getTrainingCourseID = TrainingCourseManager.Instance.currentTrainingCourse;
            Debug.Log("InputManager: The current training course is: " + getTrainingCourseID);
            //Trigger the start training course event
            GameManager.Instance.OnTrainingCourseStart(getTrainingCourseID);
        }
    }

    public void OnFinishedTraining()
    {
        OnEnable();
        isPlayerFinishedTraining = true;
        playerActions.Training.Disable();
        playerActions.Player.Fire.Disable();
    }

    #endregion

    #endregion

    #region Input Device Changed

    public void OnControlsChanged(PlayerInput input)
    {
        isGamepad = input.currentControlScheme.Equals("Gamepad");
        isKeyboard = input.currentControlScheme.Equals("Keyboard");
    }

    #endregion

    #region Toggle Action Maps

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

        #region Checking if the player has pressed the start training course button

        if (isPlayerInTrainingCourse)
        {
            playerActions.Training.StartTrainingCourse.Disable();
        }

        #endregion

        //Debug.Log("Input Manager: isPlayerSprintingThisFrame boolean is: " + isPlayerSprintingThisFrame);

        if (isPlayerInTrainingCourse)
        {
            ToggleActionMap(playerActions.Player);
        }
        else if (!isPlayerInTrainingCourse && !isPlayerFinishedTraining)
        {
            ToggleActionMap(playerActions.Training);
        }
        else if (!isPlayerInTrainingCourse && isPlayerFinishedTraining)
        {
            ToggleActionMap(playerActions.Player);
        }

    }
}
