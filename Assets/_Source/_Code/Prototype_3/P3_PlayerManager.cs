using TMPro;
using UnityEditor;
using UnityEngine;

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

    [Header("Player Characters")]
    [SerializeField] private GameObject playerCharacter_1;
    [SerializeField] private GameObject playerCharacter_2;

    [Space(5)]

    [Header("HUD References")]
    [SerializeField] private GameObject hudAmmoBar;
    [SerializeField] private GameObject hudHealthBar;
    [SerializeField] private TMP_Text HealthText;
    [SerializeField] private TMP_Text AmmoText;
    [SerializeField] private TMP_Text lighthouseInteractText;

    #endregion

    private void Start()
    {
        P3_GameManager.Instance.changePlayerCharacter += OnCharacterChanged;

        playerCharacter_1.GetComponent<P3_fpsCharacterBase>().isCharacterActive = true;
        playerCharacter_2.GetComponent<P3_LighthouseCharacter>().isCharacterActive = false;

        playerCharacter_1.GetComponent<P3_fpsCharacterBase>().EnableFPSCharacter();
        playerCharacter_2.GetComponent<P3_LighthouseCharacter>().DisableLighthouseCharacter();
    }

    #region Character Functions

    private void OnCharacterChanged()
    {
        if (playerCharacter_1.GetComponent<P3_fpsCharacterBase>().isCharacterActive)
        {
            playerCharacter_1.GetComponentInChildren<P3_GunplayManager>().DisableGun();
            playerCharacter_1.GetComponent<P3_fpsCharacterBase>().DisableFPSCharacter();
            playerCharacter_1.GetComponent<P3_fpsMovement>().DisablePlayerMovement();
            playerCharacter_1.GetComponent<P3_fpsCharacterBase>().isCharacterActive  = false;

            playerCharacter_2.GetComponent<P3_LighthouseCharacter>().isCharacterActive = true;
            playerCharacter_2.GetComponent<P3_LighthouseCharacter>().EnableLighthouseCharacter();
            playerCharacter_2.GetComponent<P3_fpsMovement>().EnablePlayerMovement();

            hudAmmoBar.SetActive(false);
            hudHealthBar.SetActive(false);
            HealthText.gameObject.SetActive(false);
            AmmoText.gameObject.SetActive(false);
            lighthouseInteractText.gameObject.SetActive(true);
        }
        else if (playerCharacter_2.GetComponent<P3_LighthouseCharacter>().isCharacterActive)
        {
            playerCharacter_2.GetComponent<P3_LighthouseCharacter>().DisableLighthouseCharacter();
            playerCharacter_2.GetComponent<P3_fpsMovement>().DisablePlayerMovement();
            playerCharacter_2.GetComponent<P3_LighthouseCharacter>().isCharacterActive = false;

            playerCharacter_1.GetComponent<P3_fpsCharacterBase>().isCharacterActive = true;
            playerCharacter_1.GetComponent<P3_fpsCharacterBase>().EnableFPSCharacter();
            playerCharacter_1.GetComponent<P3_fpsMovement>().EnablePlayerMovement();
            playerCharacter_1.GetComponentInChildren<P3_GunplayManager>().EnableGun();

            hudAmmoBar.SetActive(true);
            hudHealthBar.SetActive(true);
            HealthText.gameObject.SetActive(true);
            AmmoText.gameObject.SetActive(true);
            lighthouseInteractText.gameObject.SetActive(false);
        }
    }

    #endregion
}
