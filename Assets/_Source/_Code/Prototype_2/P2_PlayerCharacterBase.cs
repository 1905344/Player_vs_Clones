using Cinemachine;
using System;
using UnityEditor;
using UnityEngine;

public class P2_PlayerCharacterBase : MonoBehaviour
{
    #region Variables


    private Guid characterID;
    private CinemachineVirtualCamera characterCam;
    private P2_CameraID cameraID;

    private static Guid GenerateID()
    {
        return Guid.NewGuid();
    }

    public Guid GetCharacterID()
    {
        return characterID;
    }

    #endregion

    private void Awake()
    {
        characterID = GenerateID();
        characterCam = GetComponent<CinemachineVirtualCamera>();
        cameraID = characterCam.GetComponent<P2_CameraID>();
    }

    void Start()
    {
        cameraID.SetCameraID(characterID);

        P2_GameManager.Instance.changePlayerCharacter += ChangeCharacter;
        P2_GameManager.Instance.changePlayerCharacter -= ChangeCharacter;

        
    }

    private void ChangeCharacter(Guid getID)
    {
        if (getID != characterID || getID == null)
        {
            return;
        }

        this.gameObject.SetActive(true);
    }

    private void OnCharacterDeath(Guid getGuid)
    {
        if (getGuid != characterID || getGuid == null)
        {
            return;
        }

        P2_GameManager.Instance.OnPlayerCharacterKilled(getGuid);
        Destroy(gameObject);
    }

    void Update()
    {
        
    }
}
