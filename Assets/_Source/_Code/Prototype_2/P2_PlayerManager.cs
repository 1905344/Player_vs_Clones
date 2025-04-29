using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using System.Linq;

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

    [SerializeField] private Camera mainCamera;

    [Space(5)]

    [Header("Active Players and Guns")]
    [SerializeField] private List<GameObject> playerCharacters = new List<GameObject>();
    [SerializeField] private GameObject currentlyActiveCharacter;
    [SerializeField] private GameObject currentlyActiveGun;
    private Guid currentCharacterID;

    [Space(5)]

    [SerializeField] private int currentIndexPos = 0;
    [SerializeField] private string currentIDString = string.Empty;

    [Space(5)]

    [SerializeField] private TMP_Text currentPlayerCharacterText;
    [SerializeField] private GameObject ammoHudBar;
    [SerializeField] private TMP_Text ammoTitleText;

    [Space(10)]

    [SerializeField] private List<Sprite> playerCharacterSprites = new List<Sprite>();
    [SerializeField] private Sprite placeholderSprite;
    [SerializeField] private Image previousPlayerCharacterSpritePlaceholder;
    [SerializeField] private Image activePlayerCharacterSpritePlaceholder;
    [SerializeField] private Image nextPlayerCharacterSpritePlaceholder;
    [SerializeField] private Image changeCharacterInputSprite;

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
        //P2_GameManager.Instance.OnStartGame += GameStarted;
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

        SetCharacterSpritesOnStart();
    }

    #region Character Functions

    private void OnCharacterChanged()
    {
        if (playerCharacters.Count == 0)
        {
            return;
        }

        //Disable the active character
        currentlyActiveCharacter.GetComponent<P2_fpsMovement>().DisablePlayerMovement();
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

        if (currentIndexPos == 2)
        {
            ammoHudBar.SetActive(false);
            ammoTitleText.gameObject.SetActive(false);
        }
        else
        {
            ammoHudBar.SetActive(true);
            ammoTitleText.gameObject.SetActive(true);
        }

        //Enabling various different components attached to the character
        //and calling functions to update the GUI
        currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().isCharacterActive = true;
        currentIDString = currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().GetCharacterIDString();

        currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().UpdateHealth();
        currentlyActiveCharacter.GetComponent<P2_fpsMovement>().EnablePlayerMovement();

        UpdatePlayerCharacterSprites(currentIndexPos);
    }

    private void OnCharacterKilled(string characterID)
    {
        if (characterID == null)
        {
            #region Debug

            if (P2_GameManager.Instance.enableDebug)
            {
                Debug.Log($"Character killed received no characterID: {characterID}");
            }

            #endregion

            return;
        }

        //Check if the last character has been killed
        if (playerCharacters.Count == 0)
        {
            P2_GameManager.Instance.OnPlayerKilled();
            return;
        }

        for (int i = 0; i < playerCharacters.Count; i++)
        {
            string characterGuid = playerCharacters[i].GetComponent<P2_PlayerCharacterBase>().GetCharacterIDString();
            GameObject playerCharaterToRemove = playerCharacters[i].gameObject;

            if (characterGuid == characterID && characterGuid != currentIDString)
            {
                playerCharacters.Remove(playerCharaterToRemove);
                RemovePlayerCharacterSprite(i);
            }
            else if (characterGuid == currentIDString && characterID == currentIDString)
            {
                currentlyActiveCharacter.GetComponent<P2_fpsMovement>().DisablePlayerMovement();
                currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().isCharacterActive = false;

                if (playerCharacters[currentIndexPos].GetComponent<P2_PlayerCharacterBase>().CharacterGunStatus())
                {
                    currentlyActiveGun.GetComponent<P2_GunplayManager>().DisableGun();
                }

                currentCharacterID = Guid.Empty;
                currentIDString = string.Empty;

                playerCharacters.Remove(playerCharaterToRemove);
                RemovePlayerCharacterSprite(i);
                P2_GameManager.Instance.OnCharacterChanged();
            }
        }
    }

    #endregion

    #region Update Player Character Sprites

    private void SetCharacterSpritesOnStart()
    {
        previousPlayerCharacterSpritePlaceholder.sprite = playerCharacterSprites[2];
        activePlayerCharacterSpritePlaceholder.sprite = playerCharacterSprites[0];
        nextPlayerCharacterSpritePlaceholder.sprite = playerCharacterSprites[1];
    }

    private void UpdatePlayerCharacterSprites(int position)
    {
        if (playerCharacterSprites.Count == 0)
        {
            return;
        }

        if (playerCharacterSprites.Count == 2)
        {
            previousPlayerCharacterSpritePlaceholder.gameObject.SetActive(false);
        }
        else if (playerCharacterSprites.Count == 1)
        {
            previousPlayerCharacterSpritePlaceholder.gameObject.SetActive(false);
            nextPlayerCharacterSpritePlaceholder.gameObject.SetActive(false);
        }

        int prevSprite = position - 1;
        
        if (prevSprite < 0)
        {
            prevSprite = playerCharacters.Count - 1;
        }

        prevSprite %= playerCharacterSprites.Count;

        int nextSprite = position + 1;
        nextSprite %= playerCharacterSprites.Count;

        #region Debug

        if (P2_GameManager.Instance.enableDebug)
        {
            Debug.Log($"P2_PlayerManager: prevSprite = {prevSprite}");
            Debug.Log($"P2_PlayerManager: nextSprite = {nextSprite}");
        }

        #endregion

        previousPlayerCharacterSpritePlaceholder.sprite = playerCharacterSprites[prevSprite];
        activePlayerCharacterSpritePlaceholder.sprite = playerCharacterSprites[currentIndexPos];
        nextPlayerCharacterSpritePlaceholder.sprite = playerCharacterSprites[nextSprite];
    }

    private void RemovePlayerCharacterSprite(int index)
    {
        if (playerCharacters.Count == 1)
        {
            changeCharacterInputSprite.gameObject.SetActive(false);
        }

        Sprite removeSprite = playerCharacterSprites[index];

        if (previousPlayerCharacterSpritePlaceholder.sprite == removeSprite)
        {
            previousPlayerCharacterSpritePlaceholder.sprite = placeholderSprite;
        }

        if (nextPlayerCharacterSpritePlaceholder.sprite == removeSprite)
        {
            nextPlayerCharacterSpritePlaceholder.sprite = placeholderSprite;
        }

        playerCharacterSprites.Remove(removeSprite);
    }

    #endregion

    private void Update()
    {
        currentPlayerCharacterText.text = $"Active character: \n {currentlyActiveCharacter.GetComponentInChildren<P2_PlayerCharacterBase>().characterName}";
    }
}
