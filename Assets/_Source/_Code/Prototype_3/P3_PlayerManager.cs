using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class P3_PlayerManager : MonoBehaviour
{
    #region Variables

    private static P3_PlayerManager instance;

    public static P3_PlayerManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] private Camera mainCamera;

    [Space(5)]

    [Header("Active Players and Guns")]
    [SerializeField] private List<GameObject> playerCharacters = new List<GameObject>(2);
    [SerializeField] private GameObject currentlyActiveCharacter;
    [SerializeField] private GameObject currentlyActiveGun;
    private Guid currentCharacterID;

    [Space(5)]

    [SerializeField] private int currentIndexPos = 0;
    [SerializeField] private string currentIDString = string.Empty;

    //[Space(5)]

    //[SerializeField] private TextMeshProUGUI currentPlayerCharacterText;

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
        //P3_GameManager.Instance.OnStartGame += GameStarted;
        P3_GameManager.Instance.changePlayerCharacter += OnCharacterChanged;
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

    #region Character Functions

    private void OnCharacterChanged()
    {
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

        //Enabling various different components attached to the character
        //and calling functions to update the GUI
        currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().isCharacterActive = true;
        currentIDString = currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().GetCharacterIDString();

        currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().UpdateHealth();
        currentlyActiveCharacter.GetComponent<P2_fpsMovement>().EnablePlayerMovement();
    }

    #endregion

    //private void Update()
    //{
    //    currentPlayerCharacterText.text = $"Current character: \n {currentlyActiveCharacter.GetComponentInChildren<P2_PlayerCharacterBase>().characterName}";
    //}
}
