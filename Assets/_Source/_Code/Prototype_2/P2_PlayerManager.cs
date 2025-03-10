using System;
using System.Collections.Generic;
using UnityEngine;

public class P2_PlayerManager : MonoBehaviour
{
    #region Variables

    private static P2_PlayerManager instance;

    public static P2_PlayerManager Instance
    {
        get
        {
            return instance;
        }
    }

    [Header("Active Players and Guns")]
    [SerializeField] private List<GameObject> playerCharacters = new List<GameObject>();
    [SerializeField] private GameObject currentlyActiveCharacter;
    [SerializeField] private GameObject currentlyActiveGun;
    private Guid currentCharacterID;

    [Space(5)]

    [SerializeField] private int currentIndexPos = 0;
    [SerializeField] private string currentIDString = string.Empty;

    public int GetCurrentCharacter()
    {
        return currentIndexPos;
    }

    public GameObject GetCurrentlyActivePlayer()
    {
        return playerCharacters[currentIndexPos].gameObject;
    }

    public string GetCurrentIDString()
    {
        return currentIDString;
    }

    #endregion

    void Awake()
    {
        P2_GameManager.Instance.OnStartGame += GameStarted;
        P2_GameManager.Instance.changePlayerCharacter += OnCharacterChanged;
        P2_GameManager.Instance.playerCharacterKilled += OnCharacterKilled;
        
        currentIndexPos = 0;
    }

    private void Start()
    {
        currentlyActiveCharacter = playerCharacters[0].gameObject;
        currentCharacterID = currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().GetCharacterID();
        currentIDString = currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().GetCharacterIDString();
        currentlyActiveGun = playerCharacters[0].GetComponentInChildren<P2_GunplayManager>().gameObject;

        currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().isCharacterActive = true;
        currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().UpdateHealth();
        currentlyActiveCharacter.GetComponent<P2_fpsMovement>().EnablePlayerMovement();
        currentlyActiveGun.GetComponent<P2_GunplayManager>().EnableGun();
    }

    //This function could be removed.
    private void GameStarted()
    {
        currentlyActiveCharacter = playerCharacters[0].gameObject;
        currentCharacterID = currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().GetCharacterID();
        currentIDString = currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().GetCharacterIDString();
        currentlyActiveGun = playerCharacters[0].GetComponentInChildren<P2_GunplayManager>().gameObject;

        currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().isCharacterActive = true;
        currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().UpdateHealth();
        currentlyActiveCharacter.GetComponent<P2_fpsMovement>().EnablePlayerMovement();
        currentlyActiveGun.GetComponent<P2_GunplayManager>().EnableGun();
    }

    #region Character Functions

    private void OnCharacterChanged()
    {
        //Disable the active character
        currentlyActiveCharacter.GetComponent<P2_fpsMovement>().DisablePlayerMovement();

        //currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().otherHealthBar.gameObject.SetActive(true);
        currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().isCharacterActive = false;

        //Disable the gun attached to the active character
        currentlyActiveGun.GetComponent<P2_GunplayManager>().DisableGun();

        //Increment the integer for the current index position.
        currentIndexPos++;

        //Check that the integer for the current index position is not greater
        //than the number of items within the list.
        currentIndexPos %= playerCharacters.Count;

        //Set the currentlyActiveCharacter game object variable to the next
        //character game object in the list.
        currentlyActiveCharacter = playerCharacters[currentIndexPos];

        if (playerCharacters[currentIndexPos].GetComponent<P2_PlayerCharacterBase>().CharacterGunStatus())
        {
            currentlyActiveGun = playerCharacters[currentIndexPos].GetComponentInChildren<P2_GunplayManager>().gameObject;
            currentlyActiveGun.GetComponent<P2_GunplayManager>().EnableGun();
        }

        //Enabling various different components attached to the character
        //and calling functions to update the GUI
        currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().isCharacterActive = true;
        currentIDString = currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().GetCharacterIDString();

        currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().UpdateHealth();
        //currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().otherHealthBar.gameObject.SetActive(false);
        currentlyActiveCharacter.GetComponent<P2_fpsMovement>().EnablePlayerMovement();
    }

    private void OnCharacterKilled(string characterID)
    {
        //Check if the last character has been killed
        if (playerCharacters.Count == 0)
        {
            P2_GameManager.Instance.OnPlayerKilled();
            return;
        }

        if (characterID == null)
        {
            Debug.Log($"Character killed received no characterID: {characterID}");
            return;
        }

        if (currentCharacterID.ToString() == characterID)
        {
            //Disable the active character
            currentlyActiveCharacter.GetComponent<P2_fpsMovement>().DisablePlayerMovement();

            currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().isCharacterActive = false;
            
            //Disable the gun attached to the active character
            if (playerCharacters[currentIndexPos].GetComponent<P2_PlayerCharacterBase>().CharacterGunStatus())
            {
                //Disable the gun attached to the active character
                currentlyActiveGun.GetComponent<P2_GunplayManager>().DisableGun();
            }

            currentCharacterID = Guid.Empty;
            currentIDString = string.Empty;

            RemoveCharacter();
        }

        for (int i = 0; i < playerCharacters.Count; i++)
        {
            string characterGuid = playerCharacters[i].GetComponent<P2_PlayerCharacterBase>().GetCharacterIDString();

            if (characterGuid == characterID)
            {
                GameObject character = playerCharacters[i].gameObject;
                playerCharacters.Remove(character);
            }
        }
    }

    private void RemoveCharacter()
    {
        playerCharacters.RemoveAt(currentIndexPos);
    }

    private void Update()
    {
        for (int i = 0; i < playerCharacters.Count; i++)
        {
            if (i != currentIndexPos)
            {
                foreach (GameObject character in playerCharacters)
                {
                    character.transform.LookAt(playerCharacters[currentIndexPos].transform);
                }
            }
        }
    }

    #endregion
}
