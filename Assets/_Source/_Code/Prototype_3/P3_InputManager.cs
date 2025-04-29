using Cinemachine;
using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class P3_InputManager : MonoBehaviour
{
    #region Variables

    private static P3_InputManager _instance;

    public static P3_InputManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public static P3_PlayerControls playerInputActions;
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

    [SerializeField] TMP_Text cameraFOVText;
    [SerializeField] TMP_Text mouseXSensText;
    [SerializeField] TMP_Text mouseYSensText;
    private bool updateMouseXSensText = false;
    private bool updateMouseYSensText = false;

    [Space(15)]

    [Header("Cinemachine Virtual Camera Reference")]
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private CinemachineVirtualCamera player1_Camera;
    [SerializeField] private CinemachineVirtualCamera player2_Camera;

    [Space(10)]

    [Header("Camera Field of View")]
    [SerializeField, Range(60f, 180f)] private float _FOV = 90f;
    public bool updateFOV;

    [Space(5)]

    [Header("Toggle or Hold Shift to Sprint")]
    public bool holdToSprint = true;

    //Booleans for returning which type of button interaction for firing the gun
    public bool isTappingFireButton = false;
    public bool isPressingFireButton = false;
    public bool isHoldingFireButton = false;

    public bool levelComplete = false;

    [SerializeField] private bool isPlayerDead = false;
    [SerializeField] public bool canChangeCharacter = true;

    [Space(5)]

    [SerializeField] GameObject lighthouseScreen;

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
        return playerInputActions.Player.Jump.triggered;
    }

    public Vector2 GetPlayerMovement()
    {
        return playerInputActions.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return playerInputActions.Player.MouseLook.ReadValue<Vector2>();
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
        return playerInputActions.Player.Reload.triggered;
    }

    public bool PlayerStartedMainGame()
    {
        return playerInputActions.Player.StartGame.triggered;
    }

    public bool PlayerChangedCharacters()
    {
        return playerInputActions.Player.ChangeCharacter.triggered;
    }

    public bool PlayerPressedInteractButton()
    {
        return playerInputActions.Player.Interact.triggered;
    }

    public bool PlayerPressedUiCancelInteractButton()
    {
        return playerInputActions.UI.CancelInteract.triggered;
    }

    public bool isPlayerSprintingThisFrame { get; private set; }
    public bool IsPlayerHoldingTheFireButton { get; private set; }
    public bool IsPlayerTappingTheFireButton { get; private set; }
    public bool pauseGame = false;

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

        playerInputActions = new P3_PlayerControls();

        //Sprinting
        playerInputActions.Player.Sprint.performed += SprintThisFrame;
        playerInputActions.Player.Sprint.canceled += StopSprintingThisFrame;

        //Shooting/Pressing the fire button
        playerInputActions.Player.Fire.started += FiringGunThisFrame;
        playerInputActions.Player.Fire.performed += StopFiringGunThisFrame;

        //Pause and Resume Game
        playerInputActions.Player.PauseGame.performed += OnPause;
        //playerInputActions.Player.PauseGame.performed -= OnResume;

        playerInputActions.UI.PauseGame.performed += OnResume;
        //playerInputActions.UI.PauseGame.performed -= OnResume;

        //Change characters
        playerInputActions.Player.ChangeCharacter.started += OnChangeCharacter;
        playerInputActions.Player.ChangeCharacter.canceled -= OnChangeCharacter;

        mouseXSensText.maxVisibleCharacters = 4;
        mouseYSensText.maxVisibleCharacters = 4;
    }

    private void Start()
    {
        //Event for when the player has been killed
        P3_GameManager.Instance.PlayerKilled += OnPlayerDeath;
        P3_GameManager.Instance.PlayerKilled -= OnPlayerDeath;

        //When the game starts
        P3_GameManager.Instance.OnStartGame += OnEnable;
        P3_GameManager.Instance.PlayerKilled += OnDisable;

        SetToggleStates();
        SetMouseSensSliders();
        SetCameraFOVSlider();

        ApplyMouseXSens();
        ApplyMouseYSens();

        #region Debug

        if (P3_GameManager.Instance.enableDebug)
        {
            Debug.Log("InputManager: The starting action map is: " + _currentControlScheme);
            Debug.Log("Camera FOV is: " + vCam.GetFocalLength());
        }

        #endregion
    }

    #region OnEnable and OnDisable

    public void OnEnable()
    {
        ToggleActionMap(playerInputActions.Player);
        playerInputActions.Player.Enable();
        updateFOV = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetCamera(mouseHorizontalSensitivity, mouseVerticalSensitivity, _FOV);
    }

    public void OnDisable()
    {
        ToggleActionMap(playerInputActions.UI);
        playerInputActions.Player.Disable();
        playerInputActions.UI.Enable();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void DisableUiInput()
    {
        playerInputActions.UI.Disable();
    }

    public void EnableGameInput()
    {
        #region Debug

        if (P3_GameManager.Instance.enableDebug)
        {
            Debug.Log("Input Manager: game input enabled.");
        }

        #endregion

        if (isPlayerDead || pauseGame)
        {
            return;
        }

        SetCamera(mouseHorizontalSensitivity, mouseVerticalSensitivity, _FOV);

        if (!lighthouseScreen.activeInHierarchy)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (vCam == player2_Camera && lighthouseScreen.activeInHierarchy)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        playerInputActions.UI.Disable();
        playerInputActions.Player.Enable();
        ToggleActionMap(playerInputActions.Player);
        
    }

    public void DisableGameInput()
    {
        #region Debug

        if (P3_GameManager.Instance.enableDebug)
        {
            Debug.Log("Input Manager: game input disabled.");
        }

        #endregion

        SetCamera(0, 0, _FOV);

        playerInputActions.Player.Disable();
        playerInputActions.UI.Enable();
        ToggleActionMap(playerInputActions.UI);

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

            if (!pauseGame && P3_GameManager.Instance.enableDebug)
            {
                Debug.Log("Player is tapping the fire gun input key.");
            }
        }
        else if (context.duration > 0.51f && !IsPlayerHoldingTheFireButton)
        {
            IsPlayerHoldingTheFireButton = true;
            IsPlayerTappingTheFireButton = false;

            if (!pauseGame && P3_GameManager.Instance.enableDebug)
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

    private void OnChangeCharacter(InputAction.CallbackContext context)
    {
        if (context.started && canChangeCharacter)
        {
            P3_GameManager.Instance.OnCharacterChanged();
            Debug.Log($"Player pressed change character input key.");
            UpdateCamera();
        }
        else
        {
            return;
        }
    }

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

        playerInputActions.Disable();
        changeActionMap?.Invoke(actionMap);
        actionMap.Enable();

        #region Debug

        //if (P3_GameManager.Instance.enableDebug)
        //{
        //    Debug.Log("InputManager: Changing action map to: " + actionMap.name.ToString());
        //}

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
            P3_GameManager.Instance.OnPause();
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

        SetMouseSensSliders();
        SetCameraFOVSlider();
        SetCamera(mouseHorizontalSensitivity, mouseVerticalSensitivity, _FOV);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        P3_GameManager.Instance.OnResume();
        pauseGame = false;
        EnableGameInput();
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

        SetMouseSensSliders();
        SetCameraFOVSlider();
        SetCamera(mouseHorizontalSensitivity, mouseVerticalSensitivity, _FOV);

        pauseGame = false;
        EnableGameInput();
    }

    #endregion

    #region Camera

    private void UpdateCamera()
    {
        if (vCam == player1_Camera)
        {
            vCam = player2_Camera;
        }
        else if (vCam == player2_Camera)
        {
            vCam = player1_Camera;
        }

        SetCamera(mouseHorizontalSensitivity, mouseVerticalSensitivity, _FOV);
    }

    private void SetCamera(float mouseX, float mouseY, float setFOV)
    {
        vCam.SetCameraPOV(mouseX, mouseY, mouseAcceleration, invertMouseY);
        vCam.SetFocalLength(setFOV);
    }

    #endregion

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

        if (P3_GameManager.Instance.enableDebug)
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

        if (P3_GameManager.Instance.enableDebug)
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
        invertMouseYToggle.isOn = invertMouseY;
        mouseAccelerationToggle.isOn = mouseAcceleration;

        #region Debug

        if (P3_GameManager.Instance.enableDebug)
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

    private void OnPlayerDeath()
    {
        isPlayerDead = true;
        ToggleActionMap(playerInputActions.UI);
        DisableGameInput();
    }
    
    public void OnShowLighthouseUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SetCamera(0, 0, _FOV);
        canChangeCharacter = false;
    }

    public void OnHideLighthouseUI()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SetCamera(mouseHorizontalSensitivity, mouseVerticalSensitivity, _FOV);
        canChangeCharacter = true;
    }

    private void Update()
    {
        #region Update the first person camera (Cinemachine virtual camera) FOV (Field Of View)

        if (updateFOV)
        {
            cameraFOVText.text = _FOV.ToString();
            vCam.SetFocalLength(_FOV);
            //SetCamera(mouseHorizontalSensitivity, mouseVerticalSensitivity, _FOV);

            updateFOV = false;

            #region Debug

            Debug.Log("Camera FOV is: " + vCam.GetFocalLength());

            #endregion
        }

        #endregion

        #region Enable and Disable Interact Input Button

        if (vCam == player1_Camera)
        {
            playerInputActions.Player.Interact.Disable();
        }
        else if (vCam == player2_Camera)
        {
            playerInputActions.Player.Interact.Enable();
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

        //if (P3_GameManager.Instance.enableDebug)
        //{
        //    Debug.Log("Input Manager: isPlayerSprintingThisFrame boolean is: " + isPlayerSprintingThisFrame);
        //}

        #endregion
    }
}
