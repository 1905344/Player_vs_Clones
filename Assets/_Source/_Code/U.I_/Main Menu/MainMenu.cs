using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;

public class MainMenu : MonoBehaviour
{
    #region Variables

    [Header("Main Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Space(10)]

    [Header("Title Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private string titleTextString;
    [SerializeField] private string prototypeSelectionTitleString;
    [SerializeField] private string creditsTitleString;

    [Space(15)]

    [Header("U.I. Pages")]
    [SerializeField] private GameObject selectPrototypePage;
    [SerializeField] private GameObject creditsPage;

    [Space(10)]

    [Header("Start Prototype Buttons")]
    [SerializeField] private Button startPrototype1Button;
    [SerializeField] private Button startPrototype2Button;
    [SerializeField] private Button startPrototype3Button;

    [Space(10)]

    [Header("Prototype Page Return Button")]
    [SerializeField] private Button returnFromPrototypeSelectionPageButton;

    [Space(10)]

    [Header("Credits Page Return Button")]
    [SerializeField] private Button returnFromCreditsPage;

    [Space(15)]

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonConfirmSFX;
    [SerializeField] private AudioClip buttonReturnSFX;

    [Space(10)]

    [Header("Fog Rendering Feature")]
    [SerializeField] UniversalRendererData fogData;

    #endregion

    private void Awake()
    {
        titleText.text = titleTextString;
        fogData.rendererFeatures[1].SetActive(false);
    }

    public void OnApplicationQuit()
    {
        SoundManager.instance.PlaySFX(buttonConfirmSFX);
        Debug.Log("Quit game from main menu.");
        Application.Quit();
    }

    #region OnButtons Clicked

    #region Selecting a Prototype to Play

    public void OnPrototype1ButtonClicked()
    {
        SoundManager.instance.PlaySFX(buttonConfirmSFX);
        this.DeactivateMenu();
        DisableReturnFromPrototypeSelectionButton();
        DisablePrototypeSelectionButtons();

        SceneManager.LoadSceneAsync("Prototype_1_Level");
    }

    public void OnPrototype2ButtonClicked()
    {
        SoundManager.instance.PlaySFX(buttonConfirmSFX);
        this.DeactivateMenu();
        DisableReturnFromPrototypeSelectionButton();
        DisablePrototypeSelectionButtons();

        SceneManager.LoadSceneAsync("Prototype_2_Level");
    }

    public void OnPrototype3ButtonClicked()
    {
        SoundManager.instance.PlaySFX(buttonConfirmSFX);
        this.DeactivateMenu();
        DisableReturnFromPrototypeSelectionButton();
        DisablePrototypeSelectionButtons();

        SceneManager.LoadSceneAsync("Prototype_3_Level");
    }

    #endregion

    public void OnNewGameClicked()
    {
        SoundManager.instance.PlaySFX(buttonConfirmSFX);
        this.DeactivateMenu();

        titleText.text = prototypeSelectionTitleString;
        selectPrototypePage.SetActive(true);
        EnablePrototypeSelectionButtons();
        EnableReturnFromPrototypeSelectionButton();
    }

    public void OnCreditsClicked()
    {
        SoundManager.instance.PlaySFX(buttonConfirmSFX);
        DisableMenuButtons();
        EnableCreditsPageButton();
        this.DeactivateMenu();

        titleText.text = creditsTitleString;
        creditsPage.SetActive(true);
    }

    public void OnReturnFromCreditsClicked()
    {
        SoundManager.instance.PlaySFX(buttonReturnSFX);

        this.ActivateMenu();
        DisableCreditsPageButton();
        EnableMenuButtons();
        creditsPage.SetActive(false);

        EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
    }

    public void OnReturnFromPrototypeSelectionScreenClicked()
    {
        SoundManager.instance.PlaySFX(buttonReturnSFX);

        this.ActivateMenu();
        DisableReturnFromPrototypeSelectionButton();
        DisablePrototypeSelectionButtons();
        EnableMenuButtons();
        creditsPage.SetActive(false);

        EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
    }

    #endregion

    #region Enable and Disable Main Menu Buttons

    private void DisableMenuButtons()
    {
        //Preventing the player from interacting with the buttons
        newGameButton.interactable = false;
        creditsButton.interactable = false;
        quitButton.interactable = false;
    }

    private void EnableMenuButtons()
    {
        //Allowing the player to interact with the buttons
        newGameButton.interactable = true;
        creditsButton.interactable = true;
        quitButton.interactable = true;
    }

    #endregion

    #region Active and Deactivate Main Menu

    public void ActivateMenu()
    {
        titleText.text = titleTextString;
        this.gameObject.SetActive(true);
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

    #endregion
    
    #region Enable and Disable Credits Page Return Button

    private void EnableCreditsPageButton()
    {
        returnFromCreditsPage.enabled = true;
        returnFromCreditsPage.interactable = true;
    }

    private void DisableCreditsPageButton()
    {
        returnFromCreditsPage.enabled = false;
        returnFromCreditsPage.interactable = false;
    }

    #endregion

    #region Enable and Disable Prototype Selection Page Buttons

    private void EnablePrototypeSelectionButtons()
    {
        startPrototype1Button.enabled = true;
        startPrototype1Button.interactable = true;

        startPrototype2Button.enabled = true;
        startPrototype2Button.interactable= true;

        startPrototype3Button.enabled = true;
        startPrototype3Button.interactable = true;
    }

    private void DisablePrototypeSelectionButtons()
    {
        startPrototype1Button.enabled = false;
        startPrototype1Button.interactable = false;

        startPrototype2Button.enabled = false;
        startPrototype2Button.interactable = false;

        startPrototype3Button.enabled = false;
        startPrototype3Button.interactable = false;
    }

    private void EnableReturnFromPrototypeSelectionButton()
    {
        returnFromPrototypeSelectionPageButton.enabled = true;
        returnFromPrototypeSelectionPageButton.interactable = true;
    }

    private void DisableReturnFromPrototypeSelectionButton()
    {
        returnFromPrototypeSelectionPageButton.enabled = false;
        returnFromPrototypeSelectionPageButton.interactable = false;
    }

    #endregion
}
