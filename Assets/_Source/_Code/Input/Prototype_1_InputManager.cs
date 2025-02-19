using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

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

    [Header("Mouse Settings")]
    public bool mouseAcceleration { get; set; } = false;
    public bool invertMouseY { get; set; } = true;

    [Space(10)]

    [Header("Mouse Sensitivity Settings")]
    [SerializeField, Tooltip("How sensitive the mouse is on the horizontal axis"), Range(0.01f, 5f)]
    public float mouseHorizontalSensitivity = 0.3f;

    [SerializeField, Tooltip("How sensitive the mouse is on the vertical axis"), Range(0.01f,5f)]
    public float mouseVerticalSensitivity = 0.3f;

    [Space(10)]

    [Header("U.I. for Settings Page")]
    [SerializeField] Slider mouseXSensSlider;
    [SerializeField] Slider mouseYSensSlider;
    [SerializeField] Toggle invertMouseYToggle;
    [SerializeField] Toggle mouseAccelerationToggle;
    [SerializeField, Tooltip("Drag the placeholder text child object of the text input object")] TMP_InputField mouseXSensTextInput;
    [SerializeField, Tooltip("Drag the placeholder text child object of the text input object")] TMP_InputField mouseYSensTextInput;

    [Space(15)]

    [Header("Cinemachine Virtual Camera Reference")]
    [SerializeField] CinemachineVirtualCamera vCam;
    
    //Only for testing the two cameras at once
    //[SerializeField] CinemachineVirtualCamera playerTwoVCam;

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

    public bool levelComplete;

    [SerializeField] private bool isPlayerDead = false;

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

    public bool PlayerStartedMainGame()
    {
        return playerActions.Player.StartGame.triggered;
    }

    public bool isPlayerSprintingThisFrame { get; private set; }

    public bool IsPlayerHoldingTheFireButton { get; private set; }
    public bool IsPlayerTappingTheFireButton { get; private set; }

    public bool pauseGame = false;

    //[Space(10)]

    //[SerializeField] private bool isTestingTwoCharactersAtOnce = false;

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

        //Pause and Resume Game
        playerActions.Player.PauseGame.performed += OnPause;

        playerActions.Player.PauseGame.performed -= OnResume;
        
        playerActions.UI.PauseGame.performed += OnResume;
        playerActions.UI.PauseGame.performed -= OnResume;

        //Level completed
        //Prototype_1_GameManager.Instance.LevelCompleted += DisableGameInput;
        //Prototype_1_GameManager.Instance.LevelFailed += DisableGameInput;

        //Prototype_1_GameManager.Instance.OnStartGame += OnEnable;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Event for when the player has been killed
        Prototype_1_GameManager.Instance.PlayerKilled += OnPlayerDeath;
    }

    private void Start()
    {
        if (Prototype_1_GameManager.Instance.toggleDebug)
        {
            Debug.Log("InputManager: The starting action map is: ");
        }

        //mouseXSensSlider.onValueChanged.AddListener(delegate { ApplyMouseXSens(); });
        //mouseYSensSlider.onValueChanged.AddListener(delegate { ApplyMouseYSens(); });

        SetToggleStates();
        SetMouseSensSliders();

        //invertMouseYToggle.onValueChanged.AddListener(delegate { InvertMouseYToggled(); });
        //mouseAccelerationToggle.onValueChanged.AddListener(delegate { MouseAccelerationToggled(); });

        ApplyMouseXSens();
        ApplyMouseYSens();

        vCam.SetFocalLength(_FOV);
        if (Prototype_1_GameManager.Instance.toggleDebug)
        {
            Debug.Log("Camera FOV is: " + vCam.GetFocalLength());
        }

        vCam.SetCameraPOV(mouseHorizontalSensitivity, mouseVerticalSensitivity, mouseAcceleration, invertMouseY);
    }

    #region OnEnable and OnDisable

    public void OnEnable()
    {
        ToggleActionMap(playerActions.Player);
        playerActions.Player.Enable();
    }

    public void OnDisable()
    {
        ToggleActionMap(playerActions.UI);
        playerActions.Player.Disable();
        playerActions.UI.Enable();
    }

    public void EnableGameInput()
    {
        if (isPlayerDead || pauseGame)
        {
            return;
        }

        if (Prototype_1_GameManager.Instance.toggleDebug)
        {
            Debug.Log("Input Manager: game input enabled.");
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        vCam.SetCameraPOV(mouseHorizontalSensitivity, mouseVerticalSensitivity, mouseAcceleration, invertMouseY);
        playerActions.UI.Disable();
        ToggleActionMap(playerActions.Player);
    }

    public void DisableGameInput()
    {
        //if (!levelComplete)
        //{
        //    return;
        //}

        //if (isPlayerDead)
        //{
        //    return;
        //}
        
        if (Prototype_1_GameManager.Instance.toggleDebug)
        {
            Debug.Log("Input Manager: game input disabled.");
        }

        vCam.SetCameraPOV(0, 0, mouseAcceleration, invertMouseY);
        playerActions.Player.Disable();
        ToggleActionMap(playerActions.UI);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
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

            if (!pauseGame && Prototype_1_GameManager.Instance.toggleDebug)
            {
                Debug.Log("Player is tapping the fire gun input key.");
            }
        }
        else if (context.duration > 0.51f && !IsPlayerHoldingTheFireButton)
        {
            IsPlayerHoldingTheFireButton = true;
            IsPlayerTappingTheFireButton = false;

            if (!pauseGame && Prototype_1_GameManager.Instance.toggleDebug)
            {
                Debug.Log("Player is holding the fire gun input key.");
            }
        }
    }

    private void StopFiringGunThisFrame(InputAction.CallbackContext context)
    {
        IsPlayerHoldingTheFireButton = false;
        IsPlayerTappingTheFireButton = false;
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

        if (Prototype_1_GameManager.Instance.toggleDebug)
        {
            Debug.Log("InputManager: Changing action map to: " + actionMap.name.ToString());
        }

        playerActions.Disable();
        changeActionMap?.Invoke(actionMap);
        actionMap.Enable();
    }

    #endregion

    #region OnPause and OnResume

    private void OnPause(InputAction.CallbackContext context)
    {
        if (isPlayerDead)
        {
            return;
        }

        if (pauseGame)
        {
            OnResume(context);
        }
        else
        {
            DisableGameInput();
            Prototype_1_GameManager.Instance.OnPause();
            pauseGame = true;
        }
    }

    private void OnResume(InputAction.CallbackContext context)
    {
        if (isPlayerDead)
        {
            return;
        }

        if (!pauseGame)
        {
            OnPause(context);
        }
        else
        {
            SetToggleStates();
            SetMouseSensSliders();

            Prototype_1_GameManager.Instance.DisablePauseUI();
            pauseGame = false;
            EnableGameInput();
        }
    }

    public void OnResumeUIButtonPressed()
    {
        if (isPlayerDead)
        {
            return;
        }

        if (!pauseGame)
        {
            return;
        }
        else
        {
            pauseGame = false;
            EnableGameInput();
        }
    }

    #endregion

    #region Settings Functions

    #region MouseSensitivity

    public void ApplyMouseXSens()
    {
        mouseHorizontalSensitivity = mouseXSensSlider.value;
        mouseXSensTextInput.text = mouseHorizontalSensitivity.ToString();

        if (Prototype_1_GameManager.Instance.toggleDebug)
        {
            Debug.Log("Input Manager: Mouse horizontal sensitivity has been changed to: " + mouseHorizontalSensitivity.ToString());
        }
    }

    public void ApplyMouseYSens()
    {
        mouseVerticalSensitivity = mouseYSensSlider.value;
        mouseYSensTextInput.text = mouseVerticalSensitivity.ToString();

        if (Prototype_1_GameManager.Instance.toggleDebug)
        {
            Debug.Log("Input Manager: Mouse verical sensitivity has been changed to: " + mouseVerticalSensitivity.ToString());
        }
    }

    private void SetMouseSensSliders()
    {
        mouseXSensSlider.value = mouseHorizontalSensitivity;
        mouseYSensSlider.value = mouseVerticalSensitivity;
    }

    #endregion

    #region TextInput Functions

    //public void GetMouseXSensTextInput(string inputText)
    //{
    //    mouseXSensTextInput.text = inputText;
    //    ApplyMouseXSens();

    //    if (Prototype_1_GameManager.Instance.toggleDebug)
    //    {
    //        Debug.Log("Input Manager: Horizontal mouse sensitivity is: " + mouseHorizontalSensitivity.ToString());
    //    }
    //}

    //public void GetMouseYSensTextInput(string inputText)
    //{
    //    inputText = mouseVerticalSensitivity.ToString();
    //    ApplyMouseYSens();

    //    if (Prototype_1_GameManager.Instance.toggleDebug)
    //    {
    //        Debug.Log("Input Manager: Vertical mouse sensitivity is: " + mouseVerticalSensitivity.ToString());
    //    }
    //}

    //public void OnMouseXSensTextInputSelected()
    //{
    //    mouseXSensTextInput.text = string.Empty;
    //}

    //public void OnMouseXSensTextInputDeselected(string inputText)
    //{
    //    mouseYSensTextInput.text = inputText;
    //}

    //public void OnMouseYSensTextInputSelected()
    //{
    //    mouseYSensTextInput.text = string.Empty;
    //}

    //public void OnMouseYSensTextInputDeselected(string inputText)
    //{
    //    mouseYSensTextInput.text = inputText;
    //}

    #endregion

    #region U.I. Toggle Functions

    private void SetToggleStates()
    {
        invertMouseYToggle.isOn = invertMouseY;
        mouseAccelerationToggle.isOn = mouseAcceleration;
    }

    private void InvertMouseYToggled()
    {
        if (invertMouseY)
        {
            invertMouseY = false;
        }
        else
        {
            invertMouseY = true;
        }

        if (Prototype_1_GameManager.Instance.toggleDebug)
        {
            Debug.Log("Input Manager: Invert mouse Y has been toggled.");
        }
    }

    private void MouseAccelerationToggled()
    {
        if (mouseAcceleration)
        {
            mouseAcceleration = false;
        }
        else
        {
            mouseAcceleration = true;
        }

        if (Prototype_1_GameManager.Instance.toggleDebug)
        {
            Debug.Log("Input Manager: Mouse acceleration has been toggled.");
        }
    }

    #endregion

    #endregion

    private void OnPlayerDeath()
    {
        isPlayerDead = true;
        ToggleActionMap(playerActions.UI);
        DisableGameInput();
    }

    private void Update()
    {
        #region Update the first person camera (Cinemachine virtual camera) FOV (Field Of View)

        if (updateFOV)
        {
            vCam.SetFocalLength(_FOV);
            Debug.Log("Camera FOV is: " + vCam.GetFocalLength());

            /*if (isTestingTwoCharactersAtOnce)
            {
                playerTwoVCam.SetFocalLength(_FOV);
                Debug.Log("Camera FOV is: " + playerTwoVCam.GetFocalLength());
            }*/

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

        #region Debugging
        
        //if (Prototype_1_GameManager.Instance.toggleDebug)
        //{
        //    Debug.Log("Input Manager: isPlayerSprintingThisFrame boolean is: " + isPlayerSprintingThisFrame);
        //}

        #endregion
    }
}
