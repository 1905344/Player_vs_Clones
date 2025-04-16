using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Prototype_1_GameManager : MonoBehaviour
{
    //This script will check the relevant player behaviours to then 
    //set the states for the finite state machine / A.I.

    #region Variables

    private static Prototype_1_GameManager instance;

    public static Prototype_1_GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    //Events for A.I. Behaviours
    public event Action SetAiBehaviour;

    //Gameplay events
    public event Action<int> PlayerHit;
    public event Action PlayerKilled;
    public event Action<Guid, int> EnemyHit;

    //Event for gun recoil
    public event Action GunRecoil;

    //Event to start the main game
    public event Action OnStartGame;

    //Events for game over states
    //public event Action LevelFailed;
    public event Action LevelCompleted;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip confirmSFX;
    [SerializeField] private AudioClip returnSFX;

    [Space(5)]

    [Header("Restart Scene")]
    [SerializeField] private string sceneName;

    [Space(5)]

    [Header("Level Objective")]
    [SerializeField] private string levelObjective;

    [Header("Reload Prompt Variables")]
    private bool startReloadPromptTimer = false;
    [SerializeField, Tooltip("This turns on the reload prompt text")] public bool enableReloadPromptTextAsTimer { get; set; } = false;

    [Space(5)]

    [SerializeField, Tooltip("How long the prompt should stay on screen for")] private float reloadPromptLength;
    [SerializeField, Range(0, 100), Tooltip("The alpha value for the reload prompt text colour to tween")] private int reloadPromptTextAlpha;
    [SerializeField, Tooltip("The frequency of the tweening of the alpha value for the reload prompt text colour")] private float reloadPromptTextAlphaTime;
    private float promptTimer;
    private bool toggleReloadPromptText = false;

    [Space(10)]

    [Header("U.I. Elements")]
    [SerializeField] TMP_Text reloadPromptText;
    [SerializeField] TMP_Text pauseTitleText;

    [Space(5)]

    [Header("Screen References")]
    [SerializeField] Transform tutorialScreen;
    [SerializeField] Transform pauseScreen;
    [SerializeField] Transform controlsScreen;
    [SerializeField] Transform settingsScreen;
        [SerializeField] Transform quitPromptScreen;
    [SerializeField] Transform gameOverScreen;

    [Space(5)]

    [Header("Tutorial Screen Buttons")]
    [SerializeField] Button tutorialStartGame;
    [SerializeField] Button tutorialMainMenu;
    [SerializeField] Button tutorialQuitGame;
    [SerializeField] Button tutorialScreenReturnButton;

    [Space(5)]

    [Header("Pause Menu Buttons")]
    [SerializeField] Button resumeFromPauseMenuButton;
    [SerializeField] Button restartFromPauseMenuButton;
    [SerializeField] Button tutorialPauseMenuButton;
    [SerializeField] Button controlsPauseMenuButton;
    [SerializeField] Button settingsPauseMenuButton;
    [SerializeField] Button quitFromPauseMenuButton;

    [Space(5)]

    [Header("Quit Screen Buttons")]
    [SerializeField] Button returnToPauseScreenFromQuitPrompt;
    [SerializeField] Button quitPromptQuitMainMenu;
    [SerializeField] Button quitPromptQuitGame;

    [Space(5)]

    [Header("Settings Menu U.I. Elements")]
    [SerializeField] Button returnFromSettingsPageButton;

    [Space(3)]

    [SerializeField] Slider fovSlider;
    [SerializeField] Slider mouseXSensitivitySlider;
    [SerializeField] Slider mouseYSensitivitySlider;

    [Space(5)]

    [SerializeField] TMP_Text fovText;
    [SerializeField] TMP_Text mouseXSensitivityText;
    [SerializeField] TMP_Text mouseYSensitivityText;
    [SerializeField] TMP_Text objectiveText;

    [Space(5)]

    [SerializeField] Toggle invertMouseY;
    [SerializeField] Toggle mouseAcceleration;
    [SerializeField] Toggle promptForReloadToggle;

    [Space(5)]

    [Header("Controls Screen")]
    [SerializeField] Button returnFromControlsPageButton;

    [Space(5)]

    [Header("Game Over Screen")]
    [SerializeField] Button quitToMainMenuButton;
    [SerializeField] Button quitGameFromGameOverScreenButton;
    [SerializeField] TMP_Text gameOverText;
    [SerializeField] TMP_Text levelCompletedText;

    [Space(10)]

    [SerializeField] public bool enableDebug = false;
    [SerializeField] public bool skipTutorial = false;

    #endregion

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        Prototype_1_InputManager.Instance.DisableGameInput();

        SetReloadPromptToggle();

        if (!skipTutorial)
        {
            ShowTutorial();
            DisableReturnButtonTutorialScreen();
            Prototype_1_InputManager.Instance.DisableGameInput();
        }
        else
        {
            //Time.timeScale = 0f;

            HideTutorial();
            Prototype_1_InputManager.Instance.EnableGameInput();
        }

        UpdateObjectiveText();
    }

    #region Event Functions

    public void OnSetAiBehaviour()
    {
        if (SetAiBehaviour != null)
        {
            SetAiBehaviour();
        }
    }

    //public void OnLevelFailed()
    //{
    //    if (LevelFailed != null)
    //    {
    //        LevelFailed();
    //    }
    //}

    public void OnLevelCompleted()
    {
        if (LevelCompleted != null)
        {
            LevelCompleted();
            OnGameCompleted();
        }
    }

    public void OnPlayerHit(int damage)
    {
        if (PlayerHit != null)
        {
            PlayerHit(damage);
        }
    }

    public void OnPlayerKilled()
    {
        if (PlayerKilled != null)
        {
            //OnGameOverReturnToMainMenu();
            PlayerKilled();
            OnGameOver();
        }
    }

    public void OnEnemyHit(Guid enemyID, int damage)
    {
        if (EnemyHit != null)
        {
            EnemyHit(enemyID, damage);
        }
    }

    public void OnStartMainGame()
    {
        if (OnStartGame != null)
        {
            OnStartGame();
        }
    }

    public void OnGunFired()
    {
        if (GunRecoil != null)
        {
            GunRecoil();
        }
    }

    #endregion

    #region Tutorial Screen Functions

    public void OnStartGameFromTutorial()
    {
        #region Disable buttons

        tutorialStartGame.gameObject.SetActive(false);
        tutorialStartGame.enabled = false;
        tutorialStartGame.interactable = false;

        tutorialQuitGame.gameObject.SetActive(false);
        tutorialQuitGame.enabled = false;
        tutorialQuitGame.interactable = false;

        #endregion

        tutorialScreen.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        OnStartMainGame();
    }

    private void ShowTutorial()
    {
        tutorialScreen.gameObject.SetActive(true);

        tutorialStartGame.gameObject.SetActive(true);
        tutorialStartGame.enabled = true;
        tutorialStartGame.interactable = true;

        tutorialMainMenu.gameObject.SetActive(true);
        tutorialMainMenu.enabled = true;
        tutorialMainMenu.interactable = true;

        tutorialQuitGame.gameObject.SetActive(true);
        tutorialQuitGame.enabled = true;
        tutorialQuitGame.interactable = true;

        DisableReturnButtonTutorialScreen();
    }

    private void HideTutorial()
    {
        tutorialScreen.gameObject.SetActive(false);

        tutorialStartGame.gameObject.SetActive(false);
        tutorialStartGame.enabled = false;
        tutorialStartGame.interactable = false;

        tutorialMainMenu.gameObject.SetActive(false);
        tutorialMainMenu.enabled = false;
        tutorialMainMenu.interactable = false;

        tutorialQuitGame.gameObject.SetActive(false);
        tutorialQuitGame.enabled = false;
        tutorialQuitGame.interactable = false;
    }

    private void ShowTutorialScreenFromPauseMenu()
    {
        tutorialScreen.gameObject.SetActive(true);
        tutorialScreenReturnButton.gameObject.SetActive(true);

        tutorialScreenReturnButton.enabled = true;
        tutorialScreenReturnButton.interactable = true;
        tutorialScreenReturnButton.gameObject.SetActive(true);
    }

    private void HideTutorialScreenFromPauseMenu()
    {
        tutorialScreen.gameObject.SetActive(false);

        DisableReturnButtonTutorialScreen();
    }

    public void OnQuitToMainMenuFromTutorialScreen()
    {
        HideTutorial();
        DisableReturnButtonTutorialScreen();
        LoadMainMenu();
    }

    #endregion

    #region Pause Game Functions

    #region Enable and Disable Buttons

    private void EnablePauseButtons()
    {
        EventSystem.current.SetSelectedGameObject(resumeFromPauseMenuButton.gameObject);

        resumeFromPauseMenuButton.enabled = true;
        resumeFromPauseMenuButton.interactable = true;
        resumeFromPauseMenuButton.gameObject.SetActive(true);

        restartFromPauseMenuButton.enabled = true;
        restartFromPauseMenuButton.interactable = true;
        restartFromPauseMenuButton.gameObject.SetActive(true);

        tutorialPauseMenuButton.enabled = true;
        tutorialPauseMenuButton.interactable = true;
        tutorialPauseMenuButton.gameObject.SetActive(true);

        controlsPauseMenuButton.enabled = true;
        controlsPauseMenuButton.interactable = true;
        controlsPauseMenuButton.gameObject.SetActive(true);

        settingsPauseMenuButton.enabled = true;
        settingsPauseMenuButton.interactable = true;
        settingsPauseMenuButton.gameObject.SetActive(true);

        quitFromPauseMenuButton.enabled = true;
        quitFromPauseMenuButton.interactable = true;
        quitFromPauseMenuButton.gameObject.SetActive(true);

        pauseTitleText.gameObject.SetActive(true);
        objectiveText.gameObject.SetActive(true);
    }

    private void DisablePauseButtons()
    {
        resumeFromPauseMenuButton.enabled = false;
        resumeFromPauseMenuButton.interactable = false;
        resumeFromPauseMenuButton.gameObject.SetActive(false);

        restartFromPauseMenuButton.enabled = false;
        restartFromPauseMenuButton.interactable = false;
        restartFromPauseMenuButton.gameObject.SetActive(false);

        tutorialPauseMenuButton.enabled = false;
        tutorialPauseMenuButton.interactable = false;
        tutorialPauseMenuButton.gameObject.SetActive(false);

        controlsPauseMenuButton.enabled = false;
        controlsPauseMenuButton.interactable = false;
        controlsPauseMenuButton.gameObject.SetActive(false);

        settingsPauseMenuButton.enabled = false;
        settingsPauseMenuButton.interactable = false;
        settingsPauseMenuButton.gameObject.SetActive(false);

        quitFromPauseMenuButton.enabled = false;
        quitFromPauseMenuButton.interactable = false;
        quitFromPauseMenuButton.gameObject.SetActive(false);

        pauseTitleText.gameObject.SetActive(false);
        objectiveText.gameObject.SetActive(false);
    }

    private void EnableSettingsUi()
    {
        returnFromSettingsPageButton.enabled = true;
        returnFromSettingsPageButton.interactable = true;
        returnFromSettingsPageButton.gameObject.SetActive(true);

        fovSlider.enabled = true;
        fovSlider.interactable = true;
        fovSlider.gameObject.SetActive(true);

        mouseXSensitivitySlider.enabled = true;
        mouseXSensitivitySlider.interactable = true;
        mouseXSensitivitySlider.gameObject.SetActive(true);

        mouseYSensitivitySlider.enabled = true;
        mouseYSensitivitySlider.interactable = true;
        mouseYSensitivitySlider.gameObject.SetActive(true);

        fovText.gameObject.SetActive(true);
        mouseXSensitivityText.gameObject.SetActive(true);
        mouseYSensitivityText.gameObject.SetActive(true);

        invertMouseY.enabled = true;
        invertMouseY.interactable = true;
        invertMouseY.gameObject.SetActive(true);

        mouseAcceleration.enabled = true;
        mouseAcceleration.interactable = true;
        mouseAcceleration.gameObject.SetActive(true);

        promptForReloadToggle.enabled = true;
        promptForReloadToggle.interactable = true;
        promptForReloadToggle.gameObject.SetActive(true);
    }

    private void DisableSettingsUi()
    {
        returnFromSettingsPageButton.enabled = false;
        returnFromSettingsPageButton.interactable = false;
        returnFromSettingsPageButton.gameObject.SetActive(false);

        fovSlider.enabled = false;
        fovSlider.interactable = false;
        fovSlider.gameObject.SetActive(false);

        mouseXSensitivitySlider.enabled = false;
        mouseXSensitivitySlider.interactable = false;
        mouseXSensitivitySlider.gameObject.SetActive(false);

        mouseYSensitivitySlider.enabled = false;
        mouseYSensitivitySlider.interactable = false;
        mouseYSensitivitySlider.gameObject.SetActive(false);

        fovText.gameObject.SetActive(false);
        mouseXSensitivityText.gameObject.SetActive(false);
        mouseYSensitivityText.gameObject.SetActive(false);

        invertMouseY.enabled = false;
        invertMouseY.interactable = false;
        invertMouseY.gameObject.SetActive(false);

        mouseAcceleration.enabled = false;
        mouseAcceleration.interactable = false;
        mouseAcceleration.gameObject.SetActive(false);

        promptForReloadToggle.enabled = false;
        promptForReloadToggle.interactable = false;
        promptForReloadToggle.gameObject.SetActive(false);
    }

    private void EnableQuitScreenButtons()
    {
        quitToMainMenuButton.enabled = true;
        quitToMainMenuButton.interactable = true;

        quitGameFromGameOverScreenButton.enabled = true;
        quitGameFromGameOverScreenButton.interactable = true;

        returnToPauseScreenFromQuitPrompt.enabled = true;
        returnToPauseScreenFromQuitPrompt.interactable = true;
    }

    private void DisableQuitScreenButtons()
    {
        quitToMainMenuButton.enabled = false;
        quitToMainMenuButton.interactable = false;

        quitGameFromGameOverScreenButton.enabled = false;
        quitGameFromGameOverScreenButton.interactable = false;

        returnToPauseScreenFromQuitPrompt.enabled = false;
        returnToPauseScreenFromQuitPrompt.interactable = false;
    }

    #endregion

    #region Functions for Pause Screen Buttons

    public void OnPause()
    {
        Time.timeScale = 0f;
        pauseScreen.gameObject.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        EnablePauseButtons();
        SoundManager.instance.PlaySFX(confirmSFX);
    }

    public void OnResume()
    {
        if (!P3_InputManager.Instance.pauseGame)
        {
            return;
        }
        else
        {
            #region Debug

            if (enableDebug)
            {
                Debug.Log("Resuming the game.");
            }

            #endregion

            Time.timeScale = 1.0f;
            SoundManager.instance.PlaySFX(confirmSFX);
            DisableControlsPage();
            DisableSettingsPage();
            HideTutorialScreenFromPauseMenu();
            DisableReturnButtonTutorialScreen();
            DisablePauseUI();

            P3_InputManager.Instance.OnResumeUIButtonPressed();
        }
    }

    public void DisablePauseUI()
    {
        //In case the player presses the escape to resume instead
        //of the U.I. button to resume the game
        pauseScreen.gameObject.SetActive(false);
        settingsScreen.gameObject.SetActive(false);
        quitPromptScreen.gameObject.SetActive(false);

        DisablePauseButtons();
        DisableQuitScreenButtons();
    }

    public void OnReturnToPauseScreenPressed()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        quitPromptScreen.gameObject.SetActive(false);
        EnablePauseButtons();
        DisableQuitScreenButtons();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnRestartButtonPressed()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        RestartScene(sceneName);
    }

    public void OnSettingsButtonPressed()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        DisablePauseButtons();
        EnableSettingsPage();
    }

    public void OnReturnFromSettingsButtonPressed()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        EnablePauseButtons();
        DisableSettingsPage();
    }

    public void OnTutorialButtonPressed()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        DisablePauseButtons();
        ShowTutorialScreenFromPauseMenu();
    }

    public void OnReturnFromPausedTutorialScreen()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        EnablePauseButtons();
        HideTutorialScreenFromPauseMenu();
    }

    private void EnableReturnButtonTutorialScreen()
    {
        tutorialScreenReturnButton.enabled = true;
        tutorialScreenReturnButton.interactable = true;
        tutorialScreenReturnButton.gameObject.SetActive(true);
    }

    private void DisableReturnButtonTutorialScreen()
    {
        tutorialScreenReturnButton.enabled = false;
        tutorialScreenReturnButton.interactable = false;
        tutorialScreenReturnButton.gameObject.SetActive(false);
    }

    public void OnQuitButtonPressed()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        quitPromptScreen.gameObject.SetActive(true);
        DisablePauseButtons();
        EnableQuitScreenButtons();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnReturnToPauseScreenFromQuitScreenPressed()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        quitPromptScreen.gameObject.SetActive(false);
        EnablePauseButtons();
        DisableQuitScreenButtons();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnQuitToMainMenu()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        LoadMainMenu();
    }

    public void OnApplicationQuit()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        Debug.Log("Game quit");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Application.Quit();
    }

    #endregion

    #region Enable and Disable Settings Page

    private void EnableSettingsPage()
    {
        settingsScreen.gameObject.SetActive(true);
        EnableSettingsUi();
    }

    private void DisableSettingsPage()
    {
        settingsScreen.gameObject.SetActive(false);
        DisableSettingsUi();
    }

    #endregion

    #region Settings Page: Reload Prompt Toggle

    private void SetReloadPromptToggle()
    {
        promptForReloadToggle.isOn = enableReloadPromptTextAsTimer;
    }

    #endregion

    #region Controls Page Functions

    private void EnableControlsPage()
    {
        returnFromControlsPageButton.interactable = true;
        returnFromControlsPageButton.enabled = true;
        returnFromControlsPageButton.gameObject.SetActive(true);

        controlsScreen.gameObject.SetActive(true);
    }

    private void DisableControlsPage()
    {
        returnFromControlsPageButton.interactable = false;
        returnFromControlsPageButton.enabled = false;
        returnFromControlsPageButton.gameObject.SetActive(false);

        controlsScreen.gameObject.SetActive(false);
    }

    public void OnControlsButtonPressed()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        EnableControlsPage();
        DisablePauseButtons();
    }

    public void OnControlsPageReturnButtonPressed()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        DisableControlsPage();
        EnablePauseButtons();
    }

    #endregion

    #endregion

    #region Game Over Functions

    public void OnGameOverReturnToMainMenu()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        LoadMainMenu();
    }

    public void RestartLevelFromGameOverScreen()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        RestartScene("Prototype_1_Level");
    }

    #endregion

    #region Game Functions

    private void OnGameOver()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        gameOverScreen.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
    }

    private void OnGameCompleted()
    {
        SoundManager.instance.PlaySFX(confirmSFX);
        gameOverScreen.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        levelCompletedText.gameObject.SetActive(true);
    }

    private void RestartScene(string name)
    {
        SceneManager.LoadSceneAsync(name);
        Time.timeScale = 1.0f;
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void ShowReloadPrompt()
    {
        if (!enableReloadPromptTextAsTimer)
        {
            return;
        }

        SoundManager.instance.PlaySFX(confirmSFX);
        startReloadPromptTimer = true;
        toggleReloadPromptText = true;
        reloadPromptText.gameObject.SetActive(true);
    }

    public void HideReloadPrompt()
    {
        if (!enableReloadPromptTextAsTimer)
        {
            return;

        }

        SoundManager.instance.PlaySFX(confirmSFX);
        promptTimer = 0f;
        startReloadPromptTimer = false;
        toggleReloadPromptText = false;

        reloadPromptText.gameObject.SetActive(false);
        reloadPromptText.alpha = 0.5f;
    }

    private void UpdateObjectiveText()
    {
        objectiveText.text = "Objective:" + "\n" + levelObjective;
    }

    #endregion

    private void Update()
    {
        if (startReloadPromptTimer)
        {
            promptTimer += Time.deltaTime;

            reloadPromptText.CrossFadeAlpha(reloadPromptTextAlpha, reloadPromptTextAlphaTime * Time.deltaTime, false);

            if (promptTimer > reloadPromptLength)
            {
                HideReloadPrompt();
            }
        }

        if (toggleReloadPromptText)
        {
            reloadPromptText.CrossFadeAlpha(reloadPromptTextAlpha, reloadPromptTextAlphaTime * Time.deltaTime, false);
        }
    }
}