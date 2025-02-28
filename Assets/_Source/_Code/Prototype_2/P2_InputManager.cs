using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class P2_InputManager : MonoBehaviour
{
    #region Variables

    private static P2_InputManager _instance;

    public static P2_InputManager Instance
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
    [SerializeField, Tooltip("How sensitive the mouse is on the horizontal axis")]
    public float mouseHorizontalSensitivity = 0.3f;

    [SerializeField, Tooltip("How sensitive the mouse is on the vertical axis")]
    public float mouseVerticalSensitivity = 0.3f;

    [Space(10)]

    [Header("U.I. for Settings Page")]
    [SerializeField] Slider cameraFOVSlider;
    [SerializeField] Slider mouseXSensSlider;
    [SerializeField] Slider mouseYSensSlider;

    [Space(5)]

    [SerializeField] Toggle invertMouseYToggle;
    [SerializeField] Toggle mouseAccelerationToggle;

    [Space(5)]

    [SerializeField] TextMeshProUGUI cameraFOVText;
    [SerializeField] TextMeshProUGUI mouseXSensText;
    [SerializeField] TextMeshProUGUI mouseYSensText;
    private bool updateMouseXSensText = false;
    private bool updateMouseYSensText = false;

    [Space(15)]

    [Header("Cinemachine Virtual Camera Reference")]
    [SerializeField] CinemachineVirtualCamera vCam;

    [Space(10)]

    [Header("Camera Field of View")]
    [SerializeField, Range(60f, 180f)] private float _FOV = 90f;

    public bool updateFOV;

    [Header("Toggle or Hold Shift to Sprint")]
    public bool holdToSprint = true;

    //Booleans for returning which type of button interaction for firing the gun
    public bool isTappingFireButton = false;
    public bool isPressingFireButton = false;
    public bool isHoldingFireButton = false;

    public bool levelComplete = false;

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

    public bool PlayerChangedCharacters()
    {
        return playerActions.Player.ChangeCharacter.triggered;
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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Event for when the player has been killed
        GameManager.Instance.PlayerKilled += OnPlayerDeath;

        mouseXSensText.maxVisibleCharacters = 4;
        mouseYSensText.maxVisibleCharacters = 4;
    }

    private void Start()
    {
        SetToggleStates();
        SetMouseSensSliders();
        SetCameraFOVSlider();

        ApplyMouseXSens();
        ApplyMouseYSens();

        vCam.SetFocalLength(_FOV);
        vCam.SetCameraPOV(mouseHorizontalSensitivity, mouseVerticalSensitivity, mouseAcceleration, invertMouseY);

        #region Debug

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("InputManager: The starting action map is: " + _currentControlScheme);
            Debug.Log("Camera FOV is: " + vCam.GetFocalLength());
        }

        #endregion
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
        #region Debug

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Input Manager: game input enabled.");
        }

        #endregion

        if (isPlayerDead || pauseGame)
        {
            return;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        vCam.SetCameraPOV(mouseHorizontalSensitivity, mouseVerticalSensitivity, mouseAcceleration, invertMouseY);
        playerActions.UI.Disable();
        ToggleActionMap(playerActions.Player);
    }

    public void DisableGameInput()
    {
        #region Debug

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Input Manager: game input disabled.");
        }

        #endregion

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

            if (!pauseGame && GameManager.Instance.toggleDebug)
            {
                Debug.Log("Player is tapping the fire gun input key.");
            }
        }
        else if (context.duration > 0.51f && !IsPlayerHoldingTheFireButton)
        {
            IsPlayerHoldingTheFireButton = true;
            IsPlayerTappingTheFireButton = false;

            if (!pauseGame && GameManager.Instance.toggleDebug)
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

        playerActions.Disable();
        changeActionMap?.Invoke(actionMap);
        actionMap.Enable();

        #region Debug

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("InputManager: Changing action map to: " + actionMap.name.ToString());
        }

        #endregion
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
            GameManager.Instance.OnPause();
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
            //SetToggleStates();
            SetMouseSensSliders();
            SetCameraFOVSlider();

            GameManager.Instance.DisablePauseUI();
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
            //SetToggleStates();
            SetMouseSensSliders();
            SetCameraFOVSlider();

            pauseGame = false;
            EnableGameInput();
        }
    }

    #endregion

    private void OnPlayerDeath()
    {
        isPlayerDead = true;
        ToggleActionMap(playerActions.UI);
        DisableGameInput();
    }

    #region Settings Functions

    #region Field of View

    public void OnCameraFOVChange()
    {
        _FOV = cameraFOVSlider.value;
        updateFOV = true;
    }

    private void SetCameraFOVSlider()
    {
        cameraFOVSlider.value = _FOV;
        cameraFOVText.text = _FOV.ToString();
    }

    #endregion

    #region MouseSensitivity

    public void ApplyMouseXSens()
    {
        mouseHorizontalSensitivity = mouseXSensSlider.value;
        updateMouseXSensText = true;

        #region Debug

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Input Manager: Mouse horizontal sensitivity has been changed to: " + mouseHorizontalSensitivity.ToString());
        }

        #endregion
    }

    public void ApplyMouseYSens()
    {
        mouseVerticalSensitivity = mouseYSensSlider.value;
        updateMouseYSensText = true;

        #region Debug

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Input Manager: Mouse verical sensitivity has been changed to: " + mouseVerticalSensitivity.ToString());
        }

        #endregion
    }

    private void SetMouseSensSliders()
    {
        mouseXSensSlider.value = mouseHorizontalSensitivity;
        mouseYSensSlider.value = mouseVerticalSensitivity;
    }

    #endregion

    #region U.I. Toggle Functions

    private void SetToggleStates()
    {
        invertMouseYToggle.isOn = !invertMouseY;
        mouseAccelerationToggle.isOn = mouseAcceleration;

        #region Debug

        if (GameManager.Instance.toggleDebug)
        {
            if (invertMouseYToggle.isOn)
            {
                Debug.Log("Input Manager: Invert mouse Y has been toggled.");
            }

            if (mouseAccelerationToggle.isOn)
            {
                Debug.Log("Input Manager: Mouse acceleration has been toggled.");
            }
        }

        #endregion
    }

    #endregion

    #endregion

    private void Update()
    {
        #region Update the first person camera (Cinemachine virtual camera) FOV (Field Of View)

        if (updateFOV)
        {
            cameraFOVText.text = _FOV.ToString();
            vCam.SetFocalLength(_FOV);

            updateFOV = false;

            #region Debug

            Debug.Log("Camera FOV is: " + vCam.GetFocalLength());

            #endregion
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

        #region Update Mouse Sensitivity Text

        if (updateMouseXSensText)
        {
            mouseXSensText.text = mouseHorizontalSensitivity.ToString();
            updateMouseXSensText = false;
        }

        if (updateMouseYSensText)
        {
            mouseYSensText.text = mouseVerticalSensitivity.ToString();
            updateMouseYSensText = false;
        }

        #endregion

        #region Debugging

        //if (GameManager.Instance.toggleDebug)
        //{
        //    Debug.Log("Input Manager: isPlayerSprintingThisFrame boolean is: " + isPlayerSprintingThisFrame);
        //}

        #endregion
    }
}
