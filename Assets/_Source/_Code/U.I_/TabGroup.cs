using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<UiTabButton> tabButtons;
    //For sprite swapping
    //public Sprite tabIdle;
    //public Sprite tabHover;
    //public Sprite tabActive;

    public UiTabButton selectedTab;
    //Swapping between the different pages
    public List<GameObject> objectsToSwap;

    //Currently using colors as placeholders instead
    public Color32 tabIdle;
    public Color32 tabHover;
    public Color32 tabActive;

    public void Subscribe(UiTabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<UiTabButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(UiTabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            //If sprite swapping
            //button.background.sprite = tabHover;

            //Placeholder for now
            button.background.color = tabHover;
        }
    }

    public void OnTabExit(UiTabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(UiTabButton button)
    {
        if(selectedTab != null)
        {
            selectedTab.Deselect();
        }
        
        selectedTab = button;

        selectedTab.Select();

        ResetTabs();
        //If sprite swapping
        //button.background.sprite = tabActive;

        //Placeholder for now
        button.background.color = tabActive;

        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach(UiTabButton button in tabButtons)
        {
            if(selectedTab != null && button == selectedTab) { continue; }

            //If sprite swapping
            //button.background.sprite = tabIdle;

            //This is currently just a placeholder
            button.background.color = tabIdle;
        }
    }
}
