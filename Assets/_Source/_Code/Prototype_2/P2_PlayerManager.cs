using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class P2_PlayerManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private List<GameObject> playerCharacters = new List<GameObject>();
    private List<string> playerCharacterIDs = new List<string>();

    #endregion

    void Start()
    {
        P2_GameManager.Instance.changePlayerCharacter += OnCharacterChanged;
        P2_GameManager.Instance.changePlayerCharacter -= OnCharacterChanged;

        P2_GameManager.Instance.playerCharacterKilled += OnCharacterKilled;
        P2_GameManager.Instance.playerCharacterKilled -= OnCharacterKilled;

        //Event for when a character is killed
        //P2_GameManager.Instance.characterKilled += OnCharacterKilled;

        foreach (GameObject character in playerCharacters)
        {
            //Get the guid of each character and add it to a list

            //playerCharacterIDs.Add(character.GetGuid());
        }
    }

    private void OnCharacterChanged(Guid characterID)
    {
        for (int i = 0; i < playerCharacters.Count; i++)
        {
            foreach (GameObject character in playerCharacters)
            {
                var characterGuid = character.GetComponent<P2_PlayerCharacterBase>();

                if (characterGuid.GetCharacterID() == characterID)
                {
                    playerCharacters[i].SetActive(true);

                    if (characterGuid.GetCharacterID() != characterID)
                    {
                        character.SetActive(false);
                    }
                }
                else
                {
                    return;
                }
            }
        }

       
    }

    private void OnCharacterKilled(Guid characterID)
    {
        //Check if the last character has been killed
        if (playerCharacters.Count == 0)
        {
            GameManager.Instance.OnPlayerKilled();
            return;
        }

        foreach (GameObject character in playerCharacters)
        {
            //if (character.GetGuid == characterID)
            //{
            //    //Remove character from the array
                playerCharacters.Remove(character);
            //}
            //else
            //{
            //    return;
            //}
        }
    }

    void Update()
    {
        if (P2_InputManager.Instance.PlayerChangedCharacters())
        {
            
        }
    }
}
