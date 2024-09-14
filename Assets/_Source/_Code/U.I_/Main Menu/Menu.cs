using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    //This script defines a menu class type
    //This is currently used by the Main Menu, SaveSlotsMenu and ConfirmationPopupMenu scripts.

    //Best for controller inputs and to emphasise the priority of this button to the player
    [Header("First Selected Button")]
    [SerializeField] private Button firstSelected;

    protected virtual void OnEnable()
    {
        SetFirstSelected(firstSelected);
    }

    public void SetFirstSelected(Button firstSelectedButton)
    {
        firstSelectedButton.Select();
    }
}
