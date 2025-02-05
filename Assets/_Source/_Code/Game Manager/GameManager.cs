using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //This script will check the relevant player behaviours to then 
    //set the states for the finite state machine / A.I.

    #region Variables

    private static GameManager instance;

    public static GameManager Instance
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
    public event Action gunRecoil;

    //Event to start the main game
    public event Action OnStartGame;

    //Events for game over states
    //public event Action LevelFailed;
    public event Action LevelCompleted;


    [Space(20)]

    [Header("U.I. Elements")]
    [SerializeField] Transform tutorialScreen;
    [SerializeField] Transform pauseScreen;
    [SerializeField] Transform settingsScreen;
    //[SerializeField] Transform quitPromptScreen;
    [SerializeField] Transform gameOverScreen;

    [Space(5)]

    [SerializeField] TextMeshProUGUI pauseTitleText;

    [Space(5)]

    [Header("Buttons")]
    [SerializeField] Button tutorialStartGame;
    [SerializeField] Button tutorialQuitGame;

    [Space(5)]

    [SerializeField] Button resumeFromPauseMenuButton;
    [SerializeField] Button restartFromPauseMenuButton;
    [SerializeField] Button settingsPauseMenuButton;
    [SerializeField] Button quitFromPauseMenuButton;

    [Space(5)]

    [SerializeField] Button returnFromSettingsPageButton;
    [SerializeField] Slider mouseXSensitivitySlider;
    [SerializeField] Slider mouseYSensitivitySlider;
    //[SerializeField] TMP_InputField mouseXSensitivtyTextInput;
    //[SerializeField] TMP_InputField mouseYSensitivtyTextInput;
    [SerializeField] Toggle invertMouseY;
    [SerializeField] Toggle mouseAcceleration;

    [Space(5)]

    //[SerializeField] Button returnToPauseScreen;
    //[SerializeField] Button quitToMainMenuButton;
    [SerializeField] Button quitGameFromGameOverScreenButton;

    [Space(10)]

    [SerializeField] public bool toggleDebug = false;

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
            OnGameOverReturnToMainMenu();
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
        if (gunRecoil != null)
        {
            OnGunFired();
        }
    }

    #endregion

    #region Tutorial Screen Function

    public void OnStartGameFromTutorial()
    {
        #region Disable buttons

        tutorialStartGame.enabled = false;
        tutorialStartGame.interactable = false;
        tutorialQuitGame.enabled = false;
        tutorialQuitGame.interactable = false;

        #endregion

        tutorialScreen.gameObject.SetActive(false);
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

        settingsPauseMenuButton.enabled = true;
        settingsPauseMenuButton.interactable = true;
        settingsPauseMenuButton.gameObject.SetActive(true);

        quitFromPauseMenuButton.enabled = true;
        quitFromPauseMenuButton.interactable = true;
        quitFromPauseMenuButton.gameObject.SetActive(true);

        pauseTitleText.gameObject.SetActive(true);
    }

    private void DisablePauseButtons()
    {
        resumeFromPauseMenuButton.enabled = false;
        resumeFromPauseMenuButton.interactable = false;
        resumeFromPauseMenuButton.gameObject.SetActive(false);

        restartFromPauseMenuButton.enabled = false;
        restartFromPauseMenuButton.interactable = false;
        restartFromPauseMenuButton.gameObject.SetActive(false);

        settingsPauseMenuButton.enabled = false;
        settingsPauseMenuButton.interactable = false;
        settingsPauseMenuButton.gameObject.SetActive(false);

        quitFromPauseMenuButton.enabled = false;
        quitFromPauseMenuButton.interactable = false;
        quitFromPauseMenuButton.gameObject.SetActive(false);

        pauseTitleText.gameObject.SetActive(false);
    }

    private void EnableSettingsUi()
    {
        returnFromSettingsPageButton.enabled = true;
        returnFromSettingsPageButton.interactable = true;
        returnFromSettingsPageButton.gameObject.SetActive(true);

        mouseXSensitivitySlider.enabled = true;
        mouseXSensitivitySlider.interactable = true;
        mouseXSensitivitySlider.gameObject.SetActive(true);
        
        mouseYSensitivitySlider.enabled = true;
        mouseYSensitivitySlider.interactable = true;
        mouseYSensitivitySlider.gameObject.SetActive(true);

        //mouseXSensitivtyTextInput.enabled = true;
        //mouseXSensitivtyTextInput.interactable = true;
        //mouseXSensitivtyTextInput.gameObject.SetActive(true);
        
        //mouseYSensitivtyTextInput.enabled = true;
        //mouseYSensitivtyTextInput.interactable = true;
        //mouseYSensitivtyTextInput.gameObject.SetActive(true);

        invertMouseY.enabled = true;
        invertMouseY.interactable = true;
        invertMouseY.gameObject.SetActive(true);

        mouseAcceleration.enabled = true;
        mouseAcceleration.interactable = true;
        mouseAcceleration.gameObject.SetActive(true);
    }

    private void DisableSettingsUi()
    {
        returnFromSettingsPageButton.enabled = false;
        returnFromSettingsPageButton.interactable = false;
        returnFromSettingsPageButton.gameObject.SetActive(false);
        
        mouseXSensitivitySlider.enabled = false;
        mouseXSensitivitySlider.interactable = false;
        mouseXSensitivitySlider.gameObject.SetActive(false);

        mouseYSensitivitySlider.enabled = false;
        mouseYSensitivitySlider.interactable = false;
        mouseYSensitivitySlider.gameObject.SetActive(false);

        //mouseXSensitivtyTextInput.enabled = false;
        //mouseXSensitivtyTextInput.interactable = false;
        //mouseXSensitivtyTextInput.gameObject.SetActive(false);

        //mouseYSensitivtyTextInput.enabled = false;
        //mouseYSensitivtyTextInput.interactable = false;
        //mouseYSensitivtyTextInput.gameObject.SetActive(false);

        invertMouseY.enabled = false;
        invertMouseY.interactable = false;
        invertMouseY.gameObject.SetActive(false);

        mouseAcceleration.enabled = false;
        mouseAcceleration.interactable = false;
        mouseAcceleration.gameObject.SetActive(false);
    }

    //private void EnableQuitScreenButtons()
    //{
    //    //quitToMainMenuButton.enabled = true;
    //    //quitToMainMenuButton.interactable = true;

    //    quitGameFromGameOverScreenButton.enabled = true;
    //    quitGameFromGameOverScreenButton.interactable = true;
    //}

    //private void DisableQuitScreenButtons()
    //{
    //    //quitToMainMenuButton.enabled = false;
    //    //quitToMainMenuButton.interactable = false;

    //    quitGameFromGameOverScreenButton.enabled = false;
    //    quitGameFromGameOverScreenButton.interactable = false;
    //}

    #endregion

    #region Functions for Pause Screen Buttons

    public void OnPause()
    {
        Time.timeScale = 0f;
        pauseScreen.gameObject.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        EnablePauseButtons();
    }

    public void OnResume()
    {
        if (!InputManager.Instance.pauseGame)
        {
            return;
        }
        else
        {
            if (toggleDebug)
            {
                Debug.Log("Resuming the game.");
            }

            Time.timeScale = 1.0f;
            DisablePauseUI();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; 
            InputManager.Instance.OnResumeUIButtonPressed();
        }
    }

    public void DisablePauseUI()
    {
        //In case the player presses the escape to resume instead
        //of the U.I. button to resume the game
        pauseScreen.gameObject.SetActive(false);
        settingsScreen.gameObject.SetActive(false);
        //quitPromptScreen.gameObject.SetActive(false);

        DisablePauseButtons();
        //DisableQuitScreenButtons();
    }

    //public void OnQuitButtonPressed()
    //{
    //    quitPromptScreen.gameObject.SetActive(true);
    //    DisablePauseButtons();
    //    EnableQuitScreenButtons();

    //    Cursor.visible = true;
    //    Cursor.lockState = CursorLockMode.None;
    //}

    public void OnReturnToPauseScreenPressed()
    {
        //quitPromptScreen.gameObject.SetActive(false);
        EnablePauseButtons();
        //DisableQuitScreenButtons();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnRestartButtonPressed()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadSceneAsync(currentScene.buildIndex);
    }

    public void OnSettingsButtonPressed()
    {
        DisablePauseButtons();
        EnableSettingsPage();
    }

    public void OnReturnFromSettingsButtonPressed()
    {
        EnablePauseButtons();
        DisableSettingsPage();
    }

    //public void OnQuitToMainMenu()
    //{
    //    Cursor.visible = true;
    //    Cursor.lockState = CursorLockMode.None;
    //    SceneManager.LoadSceneAsync("MainMenu");
    //}

    public void OnApplicationQuit()
    {
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

    #endregion

    #region Game Over Functions

    public void OnGameOverReturnToMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void RestartLevelFromGameOverScreen()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

}
