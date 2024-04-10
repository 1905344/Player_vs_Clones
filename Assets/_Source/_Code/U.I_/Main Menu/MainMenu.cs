using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    [Space(5)]

    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Space(15)]

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonPressed;
    [SerializeField] private AudioClip buttonSelected;
    [SerializeField] private AudioClip backButtonPressed;
    [SerializeField] private AudioClip quitGame;

    void Start()
    {
        DisableButtonsDependingOnData();
    }

    //Disable this if you're not using the Save & Load System
    //Disables the menu buttons if no save data is found
    private void DisableButtonsDependingOnData()
    {
        if (!DataPersistenceManager.instance.HasGameData())
        {
            continueGameButton.interactable = false;
            loadGameButton.interactable = false;
        }
    }

    public void OnNewGameClicked()
    {
        //Requires the Sound Manager scripts
        SoundManager.instance.PlaySFX(buttonPressed);

        //Enable this if using multiple save slots in the Save & Load System
        saveSlotsMenu.ActivateMenu(false);
        this.DeactivateMenu();
    }

    public void OnContinueClicked()
    {
        DisableMenuButtons();

        //Requires the Sound Manager scripts
        //Play the button pressed sound effect
        SoundManager.instance.PlaySFX(buttonPressed);

        //Disable this if you're not using the Save & Load System
        //Save the game anytime before loading a new scene
        DataPersistenceManager.instance.SaveGame();

        //Disable this if you're not using the Save & Load System
        //Load the next scene which will in turn load the game because of
        //OnSceneLoaded() in the DataPersistenceManager
        SceneManager.LoadSceneAsync(DataPersistenceManager.instance.GetSavedSceneName());
    }

    public void OnLoadGameClicked()
    {
        //Requires the Sound Manager scripts
        SoundManager.instance.PlaySFX(buttonPressed);

        //Enable this if using multiple save slots in the Save & Load System
        saveSlotsMenu.ActivateMenu(true);
        this.DeactivateMenu();
    }

    public void OnSettingsClicked()
    {
        //Requires the Sound Manager scripts
        SoundManager.instance.PlaySFX(buttonPressed);

        this.DeactivateMenu();
    }

    public void OnExtrasClicked()
    {
        //Requires the Sound Manager scripts
        SoundManager.instance.PlaySFX(buttonPressed);

        this.DeactivateMenu();
    }

    public void OnApplicationQuit()
    {
        //Requires the Sound Manager scripts
        SoundManager.instance.PlaySFX(buttonPressed);

        Application.Quit();
    }

    private void DisableMenuButtons()
    {
        //Preventing the player from interacting with the buttons
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
        settingsButton.interactable = false;
        creditsButton.interactable = false;
        quitButton.interactable = false;
    }

    public void ActivateMenu()
    {
        this.gameObject.SetActive(true);

        //Disable this if you're not using the Save & Load System
        DisableButtonsDependingOnData();
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

}
