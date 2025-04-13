using Cinemachine;
using UnityEngine;
using TMPro;

public class P3_LighthouseCharacter : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera characterCam;
    [SerializeField] private P3_LighthouseManager lighthouseScript;

    [Space(10)]

    [Header("Debug Variables")]
    [SerializeField] public bool isCharacterActive = false;

    #endregion

    public void EnableLighthouseCharacter()
    {
        if (!isCharacterActive)
        {
            return;
        }

        isCharacterActive = true;
        lighthouseScript.EnableLighthouseUI();

        if (lighthouseScript.isLighthouseScreenActive)
        {
            lighthouseScript.EnableLighthouseScreen();
        }
    }

    public void DisableLighthouseCharacter()
    {
        if (isCharacterActive)
        {
            return;
        }

        isCharacterActive = false;
    }
}
