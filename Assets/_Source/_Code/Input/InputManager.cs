using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
    
    //Only for testing the two cameras at once
    [SerializeField] CinemachineVirtualCamera playerTwoVCam;

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

    //Training course booleans
    public bool isPlayerInTrainingCourse = false;
    public bool isPlayerFinishedTraining = false;
    private int getTrainingCourseID = 1;

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

    public bool PlayerStartedTrainingCourse()
    {
        return playerActions.Training.StartTrainingCourse.triggered;
    }

    public bool PlayerPressedPauseDuringTraining()
    {
        return playerActions.Training.PauseGame.triggered;
    }

    public bool PlayerPressedPauseOutsideTraining()
    {
        return playerActions.Player.PauseGame.triggered;
    }

    public bool PlayerStartedMainGame()
    {
        return playerActions.Player.StartGame.triggered;
    }

    public bool isPlayerSprintingThisFrame { get; private set; }

    public bool IsPlayerHoldingTheFireButton { get; private set; }
    public bool IsPlayerTappingTheFireButton { get; private set; }

    public bool pauseGame = false;

    [Space(10)]

    [SerializeField] private bool isTestingTwoCharactersAtOnce = false;

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

        //Starting the main game
        playerActions.Player.StartGame.performed += StartMainGame;

        //Game manager event for when the player has completed all the training courses
        GameManager.Instance.FinishedTraining += OnFinishedTraining;

        //Pause and Resume Game
        playerActions.Training.PauseGame.performed += OnPause;
        playerActions.Player.PauseGame.performed += OnPause;

        playerActions.Player.PauseGame.performed -= OnResume;
        playerActions.Training.PauseGame.performed -= OnResume;
        
        playerActions.UI.PauseGame.performed += OnResume;
        playerActions.UI.PauseGame.performed -= OnResume;

        //Level completed
        //GameManager.Instance.LevelCompleted += DisableGameInput;
        //GameManager.Instance.LevelFailed += DisableGameInput;

        //GameManager.Instance.OnStartGame += OnEnable;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerActions.Training.StartTrainingCourse.Enable();
        ToggleActionMap(playerActions.Training);

        //Event for when the player has been killed
        GameManager.Instance.PlayerKilled += OnPlayerDeath; 
    }

    private void Start()
    {
        if (GameManager.Instance.isInTraining && !GameManager.Instance.isInFPS)
        {
            ToggleActionMap(playerActions.UI);
        }
        else if (GameManager.Instance.isInFPS && !GameManager.Instance.isInTraining)
        {
            ToggleActionMap(playerActions.Player);
        }

        //if (GameManager.Instance.toggleDebug)
        //{
        //    Debug.Log("InputManager: The starting action map is: ");
        //}

        vCam.SetFocalLength(_FOV);
        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Camera FOV is: " + vCam.GetFocalLength());
        }

        vCam.SetCameraPOV(mouseHorizontalSensitivity, mouseVerticalSensitivity, mouseAcceleration, invertMouseY);

        if (isTestingTwoCharactersAtOnce)
        {
            //Testing Two Characters At Once
            playerTwoVCam.SetFocalLength(_FOV);

            if (GameManager.Instance.toggleDebug)
            {
                Debug.Log("Camera FOV is: " + playerTwoVCam.GetFocalLength());
            }

            playerTwoVCam.SetCameraPOV(mouseHorizontalSensitivity, mouseVerticalSensitivity, mouseAcceleration, invertMouseY);
        }
    }

    #region OnEnable and OnDisable

    public void OnEnable()
    {
        if (GameManager.Instance.isInFPS && !GameManager.Instance.isInTraining)
        {
            ToggleActionMap(playerActions.Player);
            playerActions.Training.Disable();
            playerActions.Player.Enable();
        }
        else
        {
            ToggleActionMap(playerActions.Training);
            playerActions.Training.Enable();
            playerActions.Player.Disable();
        }
    }

    public void OnDisable()
    {
        if (GameManager.Instance.isInFPS && !GameManager.Instance.isInTraining)
        {
            ToggleActionMap(playerActions.Player);
            playerActions.Training.Disable();
            playerActions.Player.Enable();
        }
        else
        {
            ToggleActionMap(playerActions.Training);
            playerActions.Training.Enable();
            playerActions.Player.Disable();
        }
    }

    public void EnableGameInput()
    {
        if (isPlayerDead || pauseGame)
        {
            return;
        }

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Input Manager: game input enabled.");
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerActions.UI.Disable();

        if (isPlayerInTrainingCourse && GameManager.Instance.isInTraining && !GameManager.Instance.isInFPS)
        {
            ToggleActionMap(playerActions.Training);
            playerActions.Player.Disable();
        }
        else if (GameManager.Instance.isInFPS && !isPlayerInTrainingCourse && !GameManager.Instance.isInTraining)
        {
            ToggleActionMap(playerActions.Player);
            playerActions.Training.Disable();
        }
    }

    public void DisableGameInput()
    {
        //if (!levelComplete)
        //{
        //    return;
        //}

        if (isPlayerDead)
        {
            return;
        }
        
        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Input Manager: game input disabled.");
        }

        ToggleActionMap(playerActions.UI);
        playerActions.Training.Disable();
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

    #region Starting the Training Course and Disabling the Start Training Course Button

    private void StartTraining(InputAction.CallbackContext context)
    {
        OnEnable();
        getTrainingCourseID = TrainingCourseManager.Instance.currentTrainingCourse;

        if (isPlayerInTrainingCourse && GameManager.Instance.isInTraining)
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
        GameManager.Instance.isInTraining = false;
        isPlayerFinishedTraining = true;
        playerActions.Training.Disable();
        playerActions.Player.Fire.Disable();
    }

    #endregion

    #region Starting the Main Game

    private void StartMainGame(InputAction.CallbackContext context)
    {
        if (!isPlayerFinishedTraining)
        {
            return;
        }

        OnEnable();
        getTrainingCourseID = TrainingCourseManager.Instance.currentTrainingCourse;
        
        Debug.Log("InputManager: Starting the main game");

        //Trigger the start training course event
        GameManager.Instance.OnStartMainGame();
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

        if (GameManager.Instance.toggleDebug)
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

    private void Update()
    {
        #region Update the first person camera (Cinemachine virtual camera) FOV (Field Of View)

        if (updateFOV)
        {
            vCam.SetFocalLength(_FOV);
            Debug.Log("Camera FOV is: " + vCam.GetFocalLength());

            if (isTestingTwoCharactersAtOnce)
            {
                playerTwoVCam.SetFocalLength(_FOV);
                Debug.Log("Camera FOV is: " + playerTwoVCam.GetFocalLength());
            }

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

        //if (isPlayerInTrainingCourse)
        //{
        //    ToggleActionMap(playerActions.Player);
        //}
        //else if (!isPlayerInTrainingCourse && !isPlayerFinishedTraining && !GameManager.Instance.isInTraining && !GameManager.Instance.isInFPS)
        //{
        //    ToggleActionMap(playerActions.Training);
        //}
        //else if (!isPlayerInTrainingCourse && isPlayerFinishedTraining && !GameManager.Instance.isInTraining && GameManager.Instance.isInFPS)
        //{
        //    ToggleActionMap(playerActions.Player);
        //}
        //else if (!isPlayerInTrainingCourse && !isPlayerFinishedTraining && !GameManager.Instance.isInTraining && GameManager.Instance.isInFPS)
        //{
        //    ToggleActionMap(playerActions.Player);
        //}
    }
}
