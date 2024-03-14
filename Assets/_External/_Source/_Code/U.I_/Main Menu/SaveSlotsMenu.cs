using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : Menu
{
    //This script manages the Save Slots Menu that pops up when the 
    //player clicks "Load" from the Main Menu

    [Header("Menu Navigation")]
    [SerializeField] [Tooltip("Drag the Main Menu parent object here")] private MainMenu mainMenu;

    [Header("Menu Buttons")]
    [SerializeField] [Tooltip("Drag the back button here")] private Button backButton;

    [Header("Confirmation Popup")]
    [SerializeField] [Tooltip("Drag the Confirmation Popup Menu parent object here")] private ConfirmationPopupMenu confirmationPopupMenu;

    [Header("Sound Effect for Selecting a Menu Item")]
    [SerializeField] private AudioClip buttonPressed;

    private SaveSlot[] saveSlots;

    private bool isLoadingGame = false;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        SoundManager.instance.PlaySFX(buttonPressed);
        //Disable all the buttons
        DisableMenuButtons();

        //Case - loading game
        if (isLoadingGame)
        {
            //Create a new game, which will initialise the data to a clean state
            DataPersistenceManager.instance.ChangeSelectedProfileID(saveSlot.GetProfileID());
            //SaveGameAndLoadScene();
            SceneManager.LoadSceneAsync(DataPersistenceManager.instance.GetSavedSceneName());
        }
        //Case - if the new game is started, but the save slot has data
        else if (saveSlot.hasData)
        {
            confirmationPopupMenu.ActivateMenu(
                "Starting a new function with this save slot will overrirde the currently saved data. Are you sure?",
            //Function to execute if the player selects "Yes"
                () => {
                    DataPersistenceManager.instance.ChangeSelectedProfileID(saveSlot.GetProfileID());
                    DataPersistenceManager.instance.NewGame();
                    SaveGameAndLoadScene();
                },
                //Function to execute if we select the visual
                () => { 
                
                    this.ActivateMenu(isLoadingGame);
                }
            );
        }
        //Case - if there is no data for both the new game and the save slot
        else
        {
            DataPersistenceManager.instance.ChangeSelectedProfileID(saveSlot.GetProfileID());
            DataPersistenceManager.instance.NewGame();
            SaveGameAndLoadScene();
        }
    }

    private void SaveGameAndLoadScene()
    {
        //Save the game anytime prior to loading a new scene
        DataPersistenceManager.instance.SaveGame();

        //Load the scene
        SceneManager.LoadSceneAsync("CharacterSelection");
    }

    public void OnClearClicked(SaveSlot saveSlot)
    {
        SoundManager.instance.PlaySFX(buttonPressed);
        DisableMenuButtons();

        confirmationPopupMenu.ActivateMenu(
        "Are you sure you want to delete this saved data?",
        //Function to execute if the player selects 'yes'
         () => {
            DataPersistenceManager.instance.DeleteProfileData(saveSlot.GetProfileID());
            ActivateMenu(isLoadingGame);
        },
        //Function to execute if the player selects 'normal'
        () =>
        {
            ActivateMenu(isLoadingGame);
        }
        );
    }

    public void OnBackClicked()
    {
        SoundManager.instance.PlaySFX(buttonPressed);
        mainMenu.ActivateMenu();
        this.DeactivateMenu();
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        //Set this menu to be active
        this.gameObject.SetActive(true);

        //Set the mode
        this.isLoadingGame = isLoadingGame;

        //Load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        //Ensure that the back button is enabled when the save slot menu is activated
        backButton.interactable = true;
        
        //Loop through each save slot in the UI and set the content appropriately
        GameObject firstSelected = backButton.gameObject;
        foreach(SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileID(), out profileData);
            saveSlot.SetData(profileData);
            if (profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
                if (firstSelected.Equals(backButton.gameObject))
                {
                    firstSelected = saveSlot.gameObject;
                }
            }
        }

        //Set the first selected button
        Button firstSelectedButton = firstSelected.GetComponent<Button>();
        this.SetFirstSelected(firstSelectedButton);
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }

        backButton.interactable = false;
    }
}
