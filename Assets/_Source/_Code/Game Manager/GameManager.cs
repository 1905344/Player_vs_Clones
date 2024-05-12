
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
using UnityEditor.ShaderKeywordFilter;

public class GameManager : MonoBehaviour
{
    //This script will check the relevant player behaviours to then 
    //set the states for the finite state machine / A.I.

    #region Variables

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    //Events for when a target is hit
    public event Action<Guid, int> OnTargetHit;

    //Ending training courses
    public event Action<int> TrainingCourseStarted;
    public event Action<int> TrainingCourseEnded;

    //Changing to the player gameplay
    public event Action FinishedTraining;

    //Events for A.I. Behaviours
    public event Action SetAiBehaviour;

    [Space(20)]

    [Header("U.I. Elements")]
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject quitPromptScreen;

    [Space(5)]

    [SerializeField] Button resumeButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;

    [Space(5)]

    [SerializeField] Button returnToPauseScreen;
    [SerializeField] Button quitToMainMenuButton;
    [SerializeField] Button quitGameButton;

    public bool pauseGame;

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
            TrainingCourseStarted(ID);
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

    #endregion

    #region Pause Game Functions

    #region Enable and Disable Buttons

    private void EnablePauseButtons()
    {
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

    public void OnApplicationPause()
    {
        if (pauseGame)
        {
            Pause();
        }
        else
        {
            OnApplicationResume();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseScreen.gameObject.SetActive(true);
        EnablePauseButtons();
    }

    public void OnApplicationResume()
    {
        Time.timeScale = 1.0f;
        pauseGame = false;

        pauseScreen.SetActive(false);
        quitPromptScreen.SetActive(false);
        DisablePauseButtons();
        DisableQuitScreenButtons();
    }

    public void OnQuitButtonPressed()
    {
        quitPromptScreen.SetActive(true);
        DisablePauseButtons();
        EnableQuitScreenButtons();
    }

    public void OnReturnToPauseScreenPressed()
    {
        quitPromptScreen.SetActive(false);
        EnablePauseButtons();
        DisableQuitScreenButtons();
    }

    public void OnQuitToMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void OnApplicationQuit()
    {
        Debug.Log("Game quit");
        Application.Quit();
    }

    #endregion

    private void Update()
    {
        if (InputManager.Instance.PlayerPressedPauseButtonThisFrame() && !pauseGame)
        {
            pauseGame = true;
        }
        else if (InputManager.Instance.PlayerPressedPauseButtonThisFrame() && pauseGame)
        {
            pauseGame = false;
        }
    }
}
