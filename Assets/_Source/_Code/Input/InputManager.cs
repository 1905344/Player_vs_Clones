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
    [SerializeField] public bool mouseAcceleration = false;
    [SerializeField] public bool invertMouse = false;

    [Space(10)]

    [Header("Mouse Sensitivity Settings")]
    [SerializeField, Tooltip("How sensitive the mouse is on the horizontal axis")]
    public float mouseHorizontalSensitivity = 0.3f;

    [SerializeField, Tooltip("How sensitive the mouse is on the vertical axis")]
    public float mouseVerticalSensitivity = 0.3f;

    [Space(15)]

    [Header("Cinemachine Virtual Camera Reference")]
    [SerializeField] CinemachineVirtualCamera characterFollowCamera;
    
    [Space(15)]

    [Header("Camera FOV")]
    [SerializeField] private float _FOV = 90f;

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

        Debug.Log("Camera FOV is: " + characterFollowCamera.GetFocalLength());

        //characterFollowCamera.SetFocalLength(_FOV);
    }

    private void Start()
    {
        ToggleActionMap(playerActions.UI);
    }

    private void OnEnable()
    {
        playerActions.Enable();
    }

    private void OnDisable()
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
        characterFollowCamera.SetFocalLength(_FOV);

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
