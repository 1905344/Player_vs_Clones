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
    private bool isCharacter1Active = true;
    private bool isCharacter2Active = false;

    [Space(5)]

    [Header("HUD References")]
    [SerializeField] private TMP_Text HealthText;
    [SerializeField] private TMP_Text AmmoText;
    [SerializeField] private TMP_Text lighthouseInteractText;

    #endregion

    private void Start()
    {
        P3_GameManager.Instance.changePlayerCharacter += OnCharacterChanged;

        isCharacter1Active = playerCharacter_1.GetComponent<P3_fpsCharacterBase>().isCharacterActive;
        isCharacter2Active = playerCharacter_2.GetComponent<P3_LighthouseCharacter>().isCharacterActive;

        isCharacter1Active = true;
        isCharacter2Active = false;

        playerCharacter_1.GetComponent<P3_fpsCharacterBase>().EnableFPSCharacter();
        playerCharacter_2.GetComponent<P3_LighthouseCharacter>().DisableLighthouseCharacter();

    }

    #region Character Functions

    private void OnCharacterChanged()
    {
        if (isCharacter1Active && !isCharacter2Active)
        {
            playerCharacter_1.GetComponent<P3_fpsCharacterBase>().DisableFPSCharacter();
            playerCharacter_1.GetComponent<P3_fpsMovement>().DisablePlayerMovement();

            playerCharacter_2.GetComponent<P3_LighthouseCharacter>().EnableLighthouseCharacter();
            playerCharacter_2.GetComponent<P3_fpsMovement>().EnablePlayerMovement();

            HealthText.gameObject.SetActive(false);
            AmmoText.gameObject.SetActive(false);
            lighthouseInteractText.gameObject.SetActive(true);
        }
        else if (isCharacter2Active && !isCharacter1Active)
        {
            playerCharacter_2.GetComponent<P3_LighthouseCharacter>().DisableLighthouseCharacter();
            playerCharacter_2.GetComponent<P3_fpsMovement>().DisablePlayerMovement();

            playerCharacter_1.GetComponent<P3_fpsCharacterBase>().EnableFPSCharacter();
            playerCharacter_1.GetComponent<P3_fpsMovement>().EnablePlayerMovement();

            HealthText.gameObject.SetActive(true);
            AmmoText.gameObject.SetActive(true);
            lighthouseInteractText.gameObject.SetActive(false);
        }
    }

    #endregion

    private void Update()
    {
        isCharacter1Active = !isCharacter2Active;
    }
}
