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

    //Events for when a target is hit
    public event Action<Guid, int> OnTargetHit;

    //Ending training courses
    public event Action<int> TrainingCourseStarted;
    public event Action TrainingCourseRestarted;
    public event Action<int> TrainingCourseEnded;

    //Changing to the player gameplay
    public event Action FinishedTraining;

    //Events for A.I. Behaviours
    public event Action SetAiBehaviour;

    //Events for the asymmetrical gameplay
    public event Action<int> PlayerHit;
    public event Action PlayerKilled;
    public event Action<Guid, int> EnemyHit;

    //Event to start the main game
    public event Action OnStartGame;

    //Events for game over states
    //public event Action LevelFailed;
    public event Action LevelCompleted;

    [Space(20)]

    [Header("U.I. Elements")]
    [SerializeField] Transform pauseScreen;
    [SerializeField] Transform quitPromptScreen;
    [SerializeField] Transform gameOverScreen;

    [Space(5)]

    [SerializeField] Button resumeButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;

    [Space(5)]

    [SerializeField] Button returnToPauseScreen;
    [SerializeField] Button quitToMainMenuButton;
    [SerializeField] Button quitGameButton;

    [Space(10)]

    [Header("Game States")]
    public bool isInTraining = false;
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

    private void Start()
    {
        TrainingCourseRestarted += OnRestartButtonPressed;
    }

    #region Event Functions

    public void TargetHit(Guid guid, int damage)
    {
        if (OnTargetHit != null)
        {
            OnTargetHit(guid, damage);
        }
    }

    public void OnTrainingCourseStart(int ID)
    {
        if (TrainingCourseStarted != null)
        {
            isInTraining = true;
            TrainingCourseStarted(ID);
        }
    }

    public void OnTrainingCourseRestart()
    {
        if (TrainingCourseRestarted != null)
        {
            TrainingCourseRestarted();
        }
    }

    public void OnTrainingCourseEnd(int ID)
    {
        if (TrainingCourseEnded != null)
        {
            TrainingCourseEnded(ID);
        }
    }

    public void OnPlayerFinishedTraining()
    {
        if (FinishedTraining != null)
        {
            isInTraining = false;
            FinishedTraining();
        }
    }

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
            isInTraining = true;

            OnStartGame();
        }
    }

    #endregion

    #region Pause Game Functions

    #region Enable and Disable Buttons

    private void EnablePauseButtons()
    {
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);

        resumeButton.enabled = true;
        resumeButton.interactable = true;

        restartButton.enabled = true;
        restartButton.interactable = true;

        quitButton.enabled = true;
        quitButton.interactable = true;
    }

    private void DisablePauseButtons()
    {
        resumeButton.enabled = false;
        resumeButton.interactable = false;

        restartButton.enabled = false;
        restartButton.interactable = false;

        quitGameButton.enabled = false;
        quitGameButton.interactable = false;
    }

    private void EnableQuitScreenButtons()
    {
        quitToMainMenuButton.enabled = true;
        quitToMainMenuButton.interactable = true;

        quitGameButton.enabled = true;
        quitGameButton.interactable = true;
    }

    private void DisableQuitScreenButtons()
    {
        quitToMainMenuButton.enabled = false;
        quitToMainMenuButton.interactable = false;

        quitGameButton.enabled = false;
        quitGameButton.interactable = false;
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
        quitPromptScreen.gameObject.SetActive(false);

        DisablePauseButtons();
        DisableQuitScreenButtons();
    }

    public void OnQuitButtonPressed()
    {
        quitPromptScreen.gameObject.SetActive(true);
        DisablePauseButtons();
        EnableQuitScreenButtons();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnReturnToPauseScreenPressed()
    {
        quitPromptScreen.gameObject.SetActive(false);
        EnablePauseButtons();
        DisableQuitScreenButtons();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnRestartButtonPressed()
    {

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
        isInTraining = false;
        isInFPS = false;

        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void RestartLevelFromGameOverScreen()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}
