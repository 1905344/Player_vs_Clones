using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Space(10)]

    [Header("Payload Variables")]
    [SerializeField] private GameObject heistPayload;
    [SerializeField] private float distanceFromHeistPayload;
    
    [Space(10)]

    [SerializeField] private GameObject screenDistortionWithText;

    public int GetCurrentCharacter()
    {
        return currentIndexPos;
    }

    public GameObject GetCurrentlyActivePlayer()
    {
        return currentlyActiveCharacter;
    }

    public string GetCurrentIDString()
    {
        return currentIDString;
    }

    #endregion

    void Awake()
    {
        P2_GameManager.Instance.changePlayerCharacter += OnCharacterChanged;

        P2_GameManager.Instance.playerCharacterKilled += OnCharacterKilled;
        //P2_GameManager.Instance.playerCharacterKilled -= OnCharacterKilled;

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
        currentlyActiveGun = playerCharacters[currentIndexPos].GetComponentInChildren<P2_GunplayManager>().gameObject;

        //Enabling various different components attached to the character
        //and calling functions to update the GUI
        currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().isCharacterActive = true;
        currentIDString = currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().GetCharacterIDString();

        currentlyActiveCharacter.GetComponent<P2_PlayerCharacterBase>().UpdateHealth();
        currentlyActiveCharacter.GetComponent<P2_fpsMovement>().EnablePlayerMovement();
        currentlyActiveGun.GetComponent<P2_GunplayManager>().EnableGun();
    }

    private void OnCharacterKilled(Guid characterID)
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

        if (currentCharacterID == characterID)
        {
            RemoveCharacter(currentIndexPos);
            
            currentCharacterID = Guid.Empty;
            currentIDString = string.Empty;
            OnCharacterChanged();
        }
        else
        {
            for (int i = 0; i < playerCharacters.Count; i++)
            {
                Guid characterGuid = playerCharacters[i].GetComponent<P2_PlayerCharacterBase>().GetCharacterID();

                if (characterGuid == characterID)
                {
                    GameObject character = playerCharacters[i].gameObject;
                    playerCharacters.Remove(character);
                }
                else
                {
                    return;
                }
            }
        }
    }

    private void RemoveCharacter(int indexPosition)
    {
        GameObject character = playerCharacters[indexPosition];
        playerCharacters.Remove(character);
        Destroy(character);
    }

    private void Update()
    {
        float distance = Vector3.Distance(heistPayload.transform.position, currentlyActiveCharacter.transform.position);
        
        Debug.DrawLine(heistPayload.transform.position, currentlyActiveCharacter.transform.position, Color.magenta);

        float distanceIncreasing = distanceFromHeistPayload - distance;
        //Debug.Log($"distanceIncreasing = {distanceIncreasing}");

        float defaultPlayerMoveSpeed = currentlyActiveCharacter.GetComponent<P2_fpsMovement>().moveSpeed;
        float defaultPlayerSprintSpeed = currentlyActiveCharacter.GetComponent<P2_fpsMovement>().sprintSpeed;
        float slowPlayerMoveSpeed = currentlyActiveCharacter.GetComponent<P2_fpsMovement>().moveSpeed;

        if (distance < distanceFromHeistPayload)
        {
            screenDistortionWithText.gameObject.SetActive(false);
            currentlyActiveCharacter.GetComponent<P2_fpsMovement>().moveSpeed = defaultPlayerMoveSpeed;
            currentlyActiveCharacter.GetComponent<P2_fpsMovement>().sprintSpeed = defaultPlayerSprintSpeed;

            currentlyActiveCharacter.GetComponent<ConfigurableJoint>().xMotion = ConfigurableJointMotion.Free;
            currentlyActiveCharacter.GetComponent<ConfigurableJoint>().zMotion = ConfigurableJointMotion.Free;
        }
        else
        {
            if (distanceIncreasing > 0)
            {
                slowPlayerMoveSpeed -= Time.deltaTime;

                if (slowPlayerMoveSpeed < 0)
                {
                    slowPlayerMoveSpeed = 1f;
                }
            }

            screenDistortionWithText.gameObject.SetActive(true);
            currentlyActiveCharacter.GetComponent<P2_fpsMovement>().moveSpeed = slowPlayerMoveSpeed;
            currentlyActiveCharacter.GetComponent<P2_fpsMovement>().sprintSpeed = 0;

            currentlyActiveCharacter.GetComponent<ConfigurableJoint>().xMotion = ConfigurableJointMotion.Limited;
            currentlyActiveCharacter.GetComponent<ConfigurableJoint>().zMotion = ConfigurableJointMotion.Limited;
        }
    }
}
