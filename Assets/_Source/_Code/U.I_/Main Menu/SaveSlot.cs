using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class SaveSlot : MonoBehaviour
{
    //This script manages each individual save slot in the Save Slots menu

    [Header("Profile")]
    [SerializeField] private string profileID = "";

    [Header("Content")]
    [SerializeField] [Tooltip("Drag the noDataContent child object of this save slot here")] private GameObject noDataContent;
    [SerializeField] [Tooltip("Drag the hasDataContent child object of this save slot here")] private GameObject hasDataContent;
    [SerializeField] [Tooltip("Drag the text child object of the hasDataContent here")] private TextMeshProUGUI saveSlotText;

    //This is an example to show how to connect to a text element which shows the player how far they have progressed
    //[SerializeField] private TextMeshProUGUI percentageCompleteText;

    [Header("Clear Data Button")]
    [SerializeField] private Button clearButton;

    public bool hasData { get; private set; } = false;

    private Button saveSlotButton;

    public string GetProfileID()
    {
        return this.profileID;
    }

    public void SetInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
        clearButton.interactable = interactable;
    }

    private void Awake()
    {
        saveSlotButton = this.GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        //If there is no data for this profileID
        if (data == null)
        {
            hasData = false;
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            clearButton.gameObject.SetActive(false);

            saveSlotText.text = "Save Slot:" + profileID;
        }

        //If there is data for this profileID
        else
        {
            hasData = true;
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            clearButton.gameObject.SetActive(true);

            //Example: This calls a function from the gamedata.
            //That function would keep track of the progress of the game
            //and would return a percentage for the function below
            //percentageCompleteText.text = data.GetPercentageComplete() + "% COMPLETE";

            saveSlotText.text = "Save Slot:" + profileID;
        }
    }
}
