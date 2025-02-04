using System;
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
    //[SerializeField] Transform quitPromptScreen;
    [SerializeField] Transform gameOverScreen;

    [Space(5)]

    [Header("Buttons")]
    [SerializeField] Button tutorialStartGame;
    [SerializeField] Button tutorialQuitGame;
    [SerializeField] Button resumeFromPauseMenuButton;
    [SerializeField] Button restartFromPauseMenuButton;
    [SerializeField] Button quitFromPauseMenuButton;

    [Space(5)]

    //[SerializeField] Button returnToPauseScreen;
    //[SerializeField] Button quitToMainMenuButton;
    [SerializeField] Button quitGameFromGameOverScreenButton;

    [Space(10)]

    [Header("Game States")]
    public bool isInFPS = false;

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
            isInFPS = false;
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
            isInFPS = false;
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

        restartFromPauseMenuButton.enabled = true;
        restartFromPauseMenuButton.interactable = true;

        quitFromPauseMenuButton.enabled = true;
        quitFromPauseMenuButton.interactable = true;
    }

    private void DisablePauseButtons()
    {
        resumeFromPauseMenuButton.enabled = false;
        resumeFromPauseMenuButton.interactable = false;

        restartFromPauseMenuButton.enabled = false;
        restartFromPauseMenuButton.interactable = false;

        quitGameFromGameOverScreenButton.enabled = false;
        quitGameFromGameOverScreenButton.interactable = false;
    }

    private void EnableQuitScreenButtons()
    {
        //quitToMainMenuButton.enabled = true;
        //quitToMainMenuButton.interactable = true;

        quitGameFromGameOverScreenButton.enabled = true;
        quitGameFromGameOverScreenButton.interactable = true;
    }

    private void DisableQuitScreenButtons()
    {
        //quitToMainMenuButton.enabled = false;
        //quitToMainMenuButton.interactable = false;

        quitGameFromGameOverScreenButton.enabled = false;
        quitGameFromGameOverScreenButton.interactable = false;
    }

    #endregion

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
        //quitPromptScreen.gameObject.SetActive(false);

        DisablePauseButtons();
        DisableQuitScreenButtons();
    }

    public void OnQuitButtonPressed()
    {
        //quitPromptScreen.gameObject.SetActive(true);
        DisablePauseButtons();
        EnableQuitScreenButtons();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnReturnToPauseScreenPressed()
    {
        //quitPromptScreen.gameObject.SetActive(false);
        EnablePauseButtons();
        DisableQuitScreenButtons();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnRestartButtonPressed()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadSceneAsync(currentScene.buildIndex);
    }

    public void OnQuitToMainMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void OnApplicationQuit()
    {
        Debug.Log("Game quit");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Application.Quit();
    }

    #endregion

    #region Game Over Functions

    public void OnGameOverReturnToMainMenu()
    {
        isInFPS = false;

        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void RestartLevelFromGameOverScreen()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    private void Update()
    {
        if (isInFPS)
        { 
            InputManager.Instance.OnEnable();
        }
        else
        {
            InputManager.Instance.OnDisable();
        }

    }
}
