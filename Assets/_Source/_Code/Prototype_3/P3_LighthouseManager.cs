using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P3_LighthouseManager : MonoBehaviour
{
    #region Variables

    private static P3_LighthouseManager instance;

    public static P3_LighthouseManager Instance
    {
        get
        {
            return instance;
        }
    }

    [Header("Rotating Lights")]
    [SerializeField] private GameObject rotatingObject;
    [SerializeField] private float rotationSpeed;
    private Vector3 lightRotation;

    [Header("U.I.")]
    [SerializeField] private GameObject lighthouseProgressScreen;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text rechargingText;
    [SerializeField] private TMP_Text repairingText;

    [Space(5)]

    [Header("Primary Progress Bar")]
    [SerializeField] private Slider lighthouseProgressBar;
    [SerializeField] private Image lighthouseProgressBarFill;
    [SerializeField] private Image lighthouseProgressBarFillMask;
    [SerializeField] private Gradient lighthouseProgressBarFillGradient;

    [Space(5)]

    [Header("Recharging Progress Bar")]
    [SerializeField] private Slider rechargeProgressBar;
    [SerializeField] private Image rechargeProgressBarFill;
    [SerializeField] private Gradient rechargeFillGradient;

    [Space(5)]

    [Header("Repairing Progress Bar")]
    [SerializeField] private Slider repairProgressBar;
    [SerializeField] private Image repairProgressBarFill;
    [SerializeField] private Gradient repairFillGradient;

    [Space(5)]

    [Header("Buttons")]
    [SerializeField] private Button repairButton;
    [SerializeField] private Button rechargeButton;
    [SerializeField] private Button closeWindowButton;

    [Space(10)]

    [Header("Recharge Requirements")]
    [SerializeField] private int currentYellowEnemiesKilled;
    [SerializeField] private int requiredYellowEnemiesNeeded;
    [SerializeField] private TMP_Text currentYellowEnemiesText;

    [Space(5)]

    [Header("Repair Requirements")]
    [SerializeField] private int currentBlueEnemiesKilled;
    [SerializeField] private int requiredBlueEnemiesNeeded;
    [SerializeField] private TMP_Text currentBlueEnemiesText;

    [Space(10)]

    [Header("Lighthouse Variables")]
    [SerializeField] private bool startDischarging = false;
    [SerializeField] private bool startRotating = false;
    [SerializeField, Tooltip("Increase the difficulty of the game by increasing the discharge speed"), Range(0.1f, 10f)] private float increaseDischargeRate = 0.5f;

    [Space(3)]

    [SerializeField] private bool needsCharge;
    [SerializeField] private bool canCharge;
    [SerializeField] private bool isRecharging;

    [Space(3)]

    [SerializeField] private bool needsRepair;
    [SerializeField] private bool isRepairing;
    [SerializeField] private bool canRepair;

    [Space(3)]

    [SerializeField] private float currentCharge;
    [SerializeField] private float increaseChargeAmount;

    [Space(3)]

    [SerializeField] private float restoreMaxChargeAmount;
    [SerializeField] private float currentMaxCharge;
    [SerializeField] private float mininumMaxCharge;
    [SerializeField] private float defaultMaximumCharge;
    [SerializeField] private float cappedMaxCharge;

    [Space(10)]

    [Header("Timers")]
    [SerializeField, Tooltip("How fast the charge progress bar decreases")] private float rechargeCountdownTimer;
    private float rechargeTimer = 0f;
    [SerializeField, Tooltip("How long it takes to recharge")] private float rechargeTimeDuration;
    [SerializeField, Tooltip("How long it takes to repair")] private float repairTimeDuration;
    private float repairTimer = 0f;

    [Space(10)]

    [Header("Player Reference")]
    [SerializeField] private GameObject lighthousePlayerCharacter;

    public bool isLighthouseScreenActive;

    public GameObject GetLighthouseObject()
    {
        return this.gameObject;
    }

    #endregion

    private void Awake()
    {
        rechargeProgressBar.maxValue = requiredYellowEnemiesNeeded;
        repairProgressBar.maxValue = requiredBlueEnemiesNeeded;

        currentCharge = defaultMaximumCharge;
        currentMaxCharge = defaultMaximumCharge;
        cappedMaxCharge = defaultMaximumCharge;

        lighthouseProgressBar.maxValue = defaultMaximumCharge;
        lighthouseProgressBar.value = currentCharge;
    }

    private void Start()
    {
        P3_GameManager.Instance.LighthouseHit += OnTakeDamage;
        P3_GameManager.Instance.BlueEnemyKilled += UpdateRepairUI;
        P3_GameManager.Instance.YellowEnemyKilled += UpdateChargeUI;
        P3_GameManager.Instance.OnStartGame += StartDischargeAndRotation;

        #region Debug

        if (P3_GameManager.Instance.enableDebug && P3_GameManager.Instance.skipTutorial)
        {
            StartDischargeAndRotation();
        }

        #endregion
    }

    private void StartDischargeAndRotation()
    {
        startDischarging = true;
        startRotating = true;
    }

    private void UpdatePrimaryProgressBar()
    {
        currentCharge -= Time.deltaTime * increaseDischargeRate;
        lighthouseProgressBar.value = currentCharge;
    }

    public void OnInteraction()
    {
        lighthousePlayerCharacter.GetComponent<P3_fpsMovement>().DisablePlayerMovement();
        EnableLighthouseUI();
        P3_InputManager.Instance.OnShowLighthouseUI();
    }

    public void OnCancelInteraction()
    {
        lighthousePlayerCharacter.GetComponent<P3_fpsMovement>().EnablePlayerMovement();
        DisableLighthouseUI();
        P3_InputManager.Instance.OnHideLighthouseUI();
    }

    private void OnTakeDamage(float Amount)
    {
        needsCharge = true;

        cappedMaxCharge += Amount;

        if (cappedMaxCharge >= defaultMaximumCharge)
        {
            cappedMaxCharge = mininumMaxCharge;
        }

        currentMaxCharge = defaultMaximumCharge - cappedMaxCharge;

        //Testing: masking the progress bar
        //lighthouseProgressBarFillMask.fillAmount = currentMaxCharge;
    }

    #region Charge Related Functions

    private void UpdateChargeUI()
    {
        currentYellowEnemiesKilled++;
        rechargeProgressBar.value = currentYellowEnemiesKilled;
    }

    public void OnRecharge()
    {
        if (!canCharge && !isRecharging && currentCharge == currentMaxCharge)
        {
            return;
        }

        if (currentYellowEnemiesKilled > requiredYellowEnemiesNeeded)
        {
            currentYellowEnemiesKilled -= requiredYellowEnemiesNeeded;
        }

        Recharging();
    }

    private void Recharging()
    {
        canCharge = false;
        isRecharging = true;
        currentCharge += increaseChargeAmount;

        if (currentCharge >= currentMaxCharge)
        {
            currentCharge = currentMaxCharge;
        }

        //currentYellowEnemiesText.gameObject.SetActive(false);
    }

    private void FinishedRecharging()
    {
        rechargeProgressBar.value = currentYellowEnemiesKilled;
        
        //currentYellowEnemiesText.gameObject.SetActive(true);

        rechargeTimer = 0f;
        isRecharging = false;
    }

    #endregion

    #region Repair Related Functions

    private void UpdateRepairUI()
    {
        currentBlueEnemiesKilled++;
        repairProgressBar.value = currentBlueEnemiesKilled;
    }

    public void OnRepair()
    {
        if (!canRepair || !isRepairing || currentMaxCharge == defaultMaximumCharge)
        {
            return;
        }

        if (currentBlueEnemiesKilled > requiredBlueEnemiesNeeded)
        {
            currentBlueEnemiesKilled -= requiredBlueEnemiesNeeded;
        }
        
        Repairing();
    }

    private void Repairing()
    {
        canRepair = false;
        isRepairing = true;

        //currentBlueEnemiesText.gameObject.SetActive(false);
    }

    private void FinishedRepairing()
    {
        currentMaxCharge += restoreMaxChargeAmount;

        if (currentMaxCharge >= defaultMaximumCharge)
        {
            currentMaxCharge = defaultMaximumCharge;
        }

        repairProgressBar.value = currentBlueEnemiesKilled;
        cappedMaxCharge -= restoreMaxChargeAmount;
        currentMaxCharge += restoreMaxChargeAmount;

        //currentBlueEnemiesText.gameObject.SetActive(true);

        repairTimer = 0f;
        isRepairing = false;
    }

    #endregion

    #region U.I.
    
    public void EnableLighthouseUI()
    {
        lighthouseProgressScreen.SetActive(true);
        isLighthouseScreenActive = true;

        closeWindowButton.enabled = true;
        closeWindowButton.interactable = true;

        //rechargeButton.enabled = true;
        rechargeButton.interactable = true;

        //repairButton.enabled = true;
        repairButton.interactable = true;
    }

    public void DisableLighthouseUI()
    {
        lighthouseProgressScreen.SetActive(false);
        isLighthouseScreenActive = false;

        closeWindowButton.enabled = false;
        closeWindowButton.interactable = false;

        //rechargeButton.enabled = false;
        rechargeButton.interactable = false;

        //repairButton.enabled = false;
        repairButton.interactable = false;
    }

    public void OnCloseUiScreenButtonPressed()
    {
        OnCancelInteraction();
    }

    #endregion

    private void Update()
    {
        #region Rotating Lights

        if (startRotating)
        {
            lightRotation = new Vector3(0f, rotationSpeed, 0f);
            rotatingObject.gameObject.transform.Rotate(lightRotation * Time.deltaTime);
        }

        #endregion

        #region Status Text

        if (!isRecharging && !isRepairing)
        {
            statusText.text = "Discharging";
        }
        else if (isRecharging && !isRepairing)
        {
            statusText.text = "Charging";
        }
        else if (isRepairing && !isRecharging)
        {
            statusText.text = "Discharging \n Repairing";
        }
        else if (isRecharging && isRepairing)
        {
            statusText.text = "Charging \n Repairing";
        }

        #endregion

        #region Interaction Input

        //Getting the input from the InputManager
        if (P3_InputManager.Instance.PlayerPressedInteractButton() && !isLighthouseScreenActive)
        {
            OnInteraction();
        }
        
        if (P3_InputManager.Instance.PlayerPressedUiCancelInteractButton() && lighthouseProgressScreen)
        {
            OnCancelInteraction();
        }

        #endregion

        #region Discharging

        if (startDischarging)
        {
            UpdatePrimaryProgressBar();
        }

        #endregion

        #region Enabling and Disabling Buttons

        repairButton.enabled = canRepair;
        repairButton.interactable = canRepair;

        rechargeButton.enabled = canCharge;
        rechargeButton.interactable = canCharge;

        #endregion

        #region Checking current charge

        if (currentCharge < currentMaxCharge && !isRecharging)
        {
            needsCharge = true;
        }
        else 
        {
            needsCharge = false;
        }

        #endregion

        #region Checking maximum charge (for repairing)

        if (currentMaxCharge < defaultMaximumCharge && !isRepairing)
        {
            canRepair = true;
        }
        else
        {
            canRepair = false;
        }

        #endregion

        #region Recharging 

        if (isRecharging)
        {
            rechargeTimer += Time.deltaTime;

            if (rechargeTimer >= rechargeTimeDuration)
            {
                FinishedRecharging();
            }
        }

        #endregion

        #region Repairing

        if (isRepairing)
        {
            repairTimer += Time.deltaTime;

            if (repairTimer >= repairTimeDuration)
            {
                FinishedRepairing();
            }
        }

        #endregion

        #region Updating U.I.

        lighthouseProgressBarFillMask.fillAmount = currentMaxCharge;

        rechargingText.gameObject.SetActive(isRecharging);
        rechargeProgressBar.gameObject.SetActive(isRecharging);
        
        currentYellowEnemiesText.gameObject.SetActive(!isRecharging);
        currentYellowEnemiesText.text = $"Yellow killed: {currentYellowEnemiesKilled} / {requiredYellowEnemiesNeeded}";

        repairingText.gameObject.SetActive(isRepairing);
        repairProgressBar.gameObject.SetActive(isRepairing);

        currentBlueEnemiesText.gameObject.SetActive(!isRepairing);
        currentBlueEnemiesText.text = $"Blue killed: {currentBlueEnemiesKilled} / {requiredBlueEnemiesNeeded}";

        #endregion

    }
}
