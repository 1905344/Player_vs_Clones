using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ConfirmationPopupMenu : Menu
{
    //This script manages a simple confirmation pop up when the 
    //player clicks on "Load" from the main menu

    [Header("Components")]
    [SerializeField] [Tooltip("The question text that asks the player for their confirmation")] private TextMeshProUGUI displayText;
    [SerializeField] [Tooltip("The confirmation button")] private Button confirmButton;
    [SerializeField] [Tooltip("The cancellation button")] private Button cancelButton;

    public void ActivateMenu(string displayText, UnityAction confirmAction, UnityAction cancelAction)
    {
        this.gameObject.SetActive(true);

        //Set the display text
        this.displayText.text = displayText;

        //Remove any existing listeners, just to make sure there aren't any previous listeners hanging around
        //Note - this only removes listeners 
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        //Assign the onClick listeners
        confirmButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            confirmAction();
        });
        cancelButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            cancelAction();
        });
    }

    //Deactivate the menu
    private void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }
}
