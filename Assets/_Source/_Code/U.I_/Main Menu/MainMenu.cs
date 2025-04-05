using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    //[Header("Menu Navigation")]
    //[SerializeField] private SaveSlotsMenu saveSlotsMenu;

    //[Space(5)]

    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    //[SerializeField] private Button continueGameButton;
    //[SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Space(15)]

    [Header("U.I. Panels")]
    [SerializeField] private GameObject controlsPage;
    [SerializeField] private GameObject creditsPage;


    //[Space(15)]

    //[Header("Sound Effects")]
    //[SerializeField] private AudioClip buttonPressed;
    //[SerializeField] private AudioClip buttonSelected;
    //[SerializeField] private AudioClip backButtonPressed;
    //[SerializeField] private AudioClip quitGame;

    void Start()
    {
        //DisableButtonsDependingOnData();
    }

    //Disable this if you're not using the Save & Load System
    //Disables the menu buttons if no save data is found
    //private void DisableButtonsDependingOnData()
    //{
    //    if (!DataPersistenceManager.instance.HasGameData())
    //    {
    //        continueGameButton.interactable = false;
    //        loadGameButton.interactable = false;
    //    }
    //}

    #region OnButtons Clicked

    public void OnNewGameClicked()
    {
        //saveSlotsMenu.ActivateMenu(false);
        //this.DeactivateMenu();
        SceneManager.LoadSceneAsync("DemoLevel");
    }

    //public void OnContinueClicked()
    //{
    //    DisableMenuButtons();
    //    DataPersistenceManager.instance.SaveGame();
    //    SceneManager.LoadSceneAsync(DataPersistenceManager.instance.GetSavedSceneName());
    //}

    public void OnLoadGameClicked()
    {
        //saveSlotsMenu.ActivateMenu(true);
        this.DeactivateMenu();
    }

    public void OnControlsClicked()
    {
        DisableMenuButtons();
        this.DeactivateMenu();
        controlsPage.SetActive(true);
    }

    public void OnReturnFromControlsPage()
    {
        this.ActivateMenu();
        EnableMenuButtons();
        controlsPage.SetActive(false);

        EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
    }

    //public void OnTutorialClicked()
    //{
    //    DisableMenuButtons();
    //    SceneManager.LoadSceneAsync("Tutorial");
    //}

    public void OnCreditsClicked()
    {
        DisableMenuButtons();
        this.DeactivateMenu();
        creditsPage.SetActive(true);
    }

    public void OnReturnFromCreditsClicked()
    {
        this.ActivateMenu();
        EnableMenuButtons();
        creditsPage.SetActive(false);

        EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
    }

    #endregion

    public void OnApplicationQuit()
    {
        //Requires the Sound Manager scripts
        //SoundManager.instance.PlaySFX(buttonPressed);

        Debug.Log("Quit game from main menu.");
        Application.Quit();
    }

    #region Enable and Disable Main Menu Buttons
    private void DisableMenuButtons()
    {
        //Preventing the player from interacting with the buttons
        newGameButton.interactable = false;
        //continueGameButton.interactable = false;
        controlsButton.interactable = false;
        creditsButton.interactable = false;
        quitButton.interactable = false;
    }

    private void EnableMenuButtons()
    {
        //Allowing the player to interact with the buttons
        newGameButton.interactable = true;
        //continueGameButton.interactable = true;
        controlsButton.interactable = true;
        creditsButton.interactable = true;
        quitButton.interactable = true;
    }

    #endregion

    #region Active and Deactivate Main Menu

    public void ActivateMenu()
    {
        this.gameObject.SetActive(true);

        //Disable this if you're not using the Save & Load System
        //DisableButtonsDependingOnData();
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

    #endregion
}
