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
    [SerializeField] private Gradient lighthouseProgressBarFillGradient;

    [Space(5)]

    [Header("Maximum Charge Capacity Progress Bar")]
    [SerializeField] private Slider maxChargeProgressBar;
    [SerializeField] private Image maxChargeProgressBarFill;
    [SerializeField] private Gradient maxChargeProgressBarGradient;
    
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

    [SerializeField] private bool needsCharge = false;
    [SerializeField] private bool isRecharging = false;

    [Space(3)]

    [SerializeField] private bool needsRepair = false;
    [SerializeField] private bool isRepairing = false;

    [Space(3)]

    [SerializeField] private float currentChargeCapacity;
    [SerializeField] private float increaseCurrentChargeCapacity;

    [Space(3)]

    [SerializeField] private float restoreMaxChargeCapacity;
    [SerializeField] private float currentMaxChargeCapacity;
    [SerializeField] private float defaultMaximumChargeCapacity;

    [Space(2)]

    [SerializeField, Tooltip("Maximum value for the cap on the max charge capacity")] private float maxChargeCapCapacity;
    [SerializeField] private float cappedMaxChargeCapacity;

    [Space(10)]

    [Header("Timers")]
    private float rechargeTimer = 0f;
    [SerializeField, Tooltip("How long it takes to recharge")] private float rechargeTimeDuration;
    [SerializeField, Tooltip("How long it takes to repair")] private float repairTimeDuration;
    private float repairTimer = 0f;

    [Space(10)]

    [Header("Player Reference")]
    [SerializeField] private GameObject lighthousePlayerCharacter;

    [Space(10)]

    [Header("Sound Effects")]
    [SerializeField] private AudioClip openLighthouseUiSFX;
    [SerializeField] private AudioClip closeLighthouseUiSFX;
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip progressBarCompletedSFX;

    public bool isLighthouseScreenActive;

    #endregion

    private void Awake()
    {
        currentChargeCapacity = defaultMaximumChargeCapacity;
        currentMaxChargeCapacity = defaultMaximumChargeCapacity;

        //Setting the progress bars
        PrimaryProgressBarCurrentValue(defaultMaximumChargeCapacity);
        PrimaryProgressBarMaxValue(defaultMaximumChargeCapacity);

        MaxChargeProgressBarMaxValue(defaultMaximumChargeCapacity);
        MaxChargeProgressBarCurrentValue(defaultMaximumChargeCapacity);

        RechargeProgressBarMaxValue(requiredYellowEnemiesNeeded);
        RepairProgressBarMaxValue(requiredBlueEnemiesNeeded);
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

    private void OnDischarging()
    {
        currentChargeCapacity -= Time.deltaTime * increaseDischargeRate;

        if (currentChargeCapacity <= 0)
        {
            startRotating = false;
            startDischarging = false;
            currentChargeCapacity = 0;
        }

        lighthouseProgressBar.value = currentChargeCapacity;
        lighthouseProgressBarFill.color = lighthouseProgressBarFillGradient.Evaluate(1f);
    }

    private void OnTakeDamage(float Amount)
    {
        cappedMaxChargeCapacity += Amount;
        currentChargeCapacity -= Amount;

        if (currentChargeCapacity <= 0)
        {
            currentChargeCapacity = 0;
        }

        if (cappedMaxChargeCapacity >= maxChargeCapCapacity)
        {
            cappedMaxChargeCapacity = maxChargeCapCapacity;
        }

        currentMaxChargeCapacity = defaultMaximumChargeCapacity - cappedMaxChargeCapacity;

        PrimaryProgressBarCurrentValue(currentChargeCapacity);
        MaxChargeProgressBarCurrentValue(currentMaxChargeCapacity);

        #region Debug

        if (P3_GameManager.Instance.enableDebug)
        {
            Debug.Log($"P3_LighthouseManager: Lighthouse has been damaged by {Amount}");
        }

        #endregion
    }

    #region Progress Bar Functions

    #region Primary Progress Bar

    private void PrimaryProgressBarCurrentValue(float currentValue)
    {
        lighthouseProgressBar.value = currentValue;
        lighthouseProgressBarFill.color = lighthouseProgressBarFillGradient.Evaluate(lighthouseProgressBar.normalizedValue);
    }

    private void PrimaryProgressBarMaxValue(float maxValue)
    {
        lighthouseProgressBar.maxValue = maxValue;
    }

    #endregion

    #region Max Charge Progress Bar

    private void MaxChargeProgressBarCurrentValue(float currentValue)
    {
        maxChargeProgressBar.value = currentValue;
        maxChargeProgressBarFill.color = maxChargeProgressBarGradient.Evaluate(maxChargeProgressBar.normalizedValue);

        if (!needsRepair)
        {
            maxChargeProgressBar.handleRect.gameObject.SetActive(false);
        }
        else
        {
            maxChargeProgressBar.handleRect.gameObject.SetActive(true);
        }
    }

    private void MaxChargeProgressBarMaxValue(float maxValue)
    {
        maxChargeProgressBar.maxValue = maxValue;
        maxChargeProgressBar.handleRect.gameObject.SetActive(false);
    }

    #endregion

    #region Recharge Progress Bar

    private void RechargeProgressBarCurrentValue(float currentValue)
    {
        rechargeProgressBar.value = currentValue;
        rechargeProgressBarFill.color = rechargeFillGradient.Evaluate(rechargeProgressBar.normalizedValue);
    }

    private void RechargeProgressBarMaxValue(float maxValue)
    {
        rechargeProgressBar.maxValue = maxValue;
    }

    #endregion

    #region Repair Progress Bar

    private void RepairProgressBarCurrentValue(float currentValue)
    {
        repairProgressBar.value = currentValue;
        repairProgressBarFill.color = repairFillGradient.Evaluate(repairProgressBar.normalizedValue);
    }

    private void RepairProgressBarMaxValue(float maxValue)
    {
        repairProgressBar.maxValue = maxValue;
    }

    #endregion

    #endregion

    #region Recharge Functions

    private void CheckCharge()
    {
        if (currentChargeCapacity < currentMaxChargeCapacity && !isRecharging)
        {
            needsCharge = true;
        }
        else if (currentChargeCapacity == currentMaxChargeCapacity || isRecharging)
        {
            needsCharge = false;
        }
    }

    private void SetChargeButtonStatus()
    {
        if (needsCharge && !isRecharging && currentYellowEnemiesKilled >= requiredYellowEnemiesNeeded)
        {
            rechargeButton.enabled = true;
            rechargeButton.interactable = true;
        }
        else if (!needsCharge || isRecharging || currentYellowEnemiesKilled < requiredYellowEnemiesNeeded)
        {
            rechargeButton.enabled = false;
            rechargeButton.interactable = false;
        }
    }

    private void UpdateChargeUI()
    {
        currentYellowEnemiesKilled++;
        RechargeProgressBarCurrentValue(currentYellowEnemiesKilled);
    }

    public void OnRecharge()
    {
        //Function for the recharge U.I. button 

        if (!needsCharge)
        {
            return;
        }

        SoundManager.instance.PlaySFX(buttonClickSFX);

        if (currentYellowEnemiesKilled > requiredYellowEnemiesNeeded)
        {
            currentYellowEnemiesKilled -= requiredYellowEnemiesNeeded;
        }

        isRecharging = true;
        currentYellowEnemiesText.gameObject.SetActive(false);
        rechargingText.gameObject.SetActive(true);
    }

    private void Recharging()
    {
        startDischarging = false;
        needsCharge = false;

        currentChargeCapacity += increaseCurrentChargeCapacity;

        if (currentChargeCapacity >= currentMaxChargeCapacity)
        {
            currentChargeCapacity = currentMaxChargeCapacity;
        }
    }

    private void FinishedRecharging()
    {
        rechargingText.gameObject.SetActive(false);
        currentYellowEnemiesText.gameObject.SetActive(true);

        PrimaryProgressBarCurrentValue(currentChargeCapacity);
        RechargeProgressBarMaxValue(requiredYellowEnemiesNeeded);
        RechargeProgressBarCurrentValue(currentYellowEnemiesKilled);

        SoundManager.instance.PlaySFX(progressBarCompletedSFX);

        if (!startRotating)
        {
            startRotating = true;
        }

        startDischarging = true;
        rechargeTimer = 0f;
        isRecharging = false;
    }

    #endregion

    #region Repair Functions

    private void CheckRepair()
    {
        if (currentMaxChargeCapacity < defaultMaximumChargeCapacity && !isRepairing)
        {
            needsRepair = true;
        }
        else if (currentMaxChargeCapacity >= defaultMaximumChargeCapacity || isRepairing)
        {
            needsRepair = false;
        }
    }
    
    private void SetRepairButtonStatus()
    {
        if (needsRepair && !isRepairing && currentBlueEnemiesKilled >= requiredBlueEnemiesNeeded)
        {
            repairButton.enabled = true;
            repairButton.interactable = true;
        }
        else if (!needsRepair || isRepairing || currentBlueEnemiesKilled < requiredBlueEnemiesNeeded)
        {
            repairButton.enabled = false;
            repairButton.interactable = false;
        }
    }

    private void UpdateRepairUI()
    {
        currentBlueEnemiesKilled++;
        RepairProgressBarCurrentValue(currentBlueEnemiesKilled);
    }

    public void OnRepair()
    {
        //Function for the repair U.I. button 

        if (!needsRepair)
        {
            return;
        }

        SoundManager.instance.PlaySFX(buttonClickSFX);

        if (currentBlueEnemiesKilled > requiredBlueEnemiesNeeded)
        {
            currentBlueEnemiesKilled -= requiredBlueEnemiesNeeded;
        }

        isRepairing = true;
        repairingText.gameObject.SetActive(true);
        currentBlueEnemiesText.gameObject.SetActive(false);
        startRotating = false;
    }

    private void FinishedRepairing()
    {
        cappedMaxChargeCapacity -= restoreMaxChargeCapacity;
        currentMaxChargeCapacity += restoreMaxChargeCapacity;

        if (currentMaxChargeCapacity >= defaultMaximumChargeCapacity)
        {
            currentMaxChargeCapacity = defaultMaximumChargeCapacity;
        }

        repairingText.gameObject.SetActive(false);
        currentBlueEnemiesText.gameObject.SetActive(true);

        MaxChargeProgressBarCurrentValue(currentMaxChargeCapacity);
        RepairProgressBarMaxValue(requiredBlueEnemiesNeeded);
        RepairProgressBarCurrentValue(currentBlueEnemiesKilled);

        SoundManager.instance.PlaySFX(progressBarCompletedSFX);

        repairTimer = 0f;
        isRepairing = false;
        //startRotating = true;
    }

    #endregion

    #region Player Interaction with U.I.

    public void OnInteraction()
    {
        lighthousePlayerCharacter.GetComponent<P3_fpsMovement>().DisablePlayerMovement();
        EnableLighthouseUI();
        P3_InputManager.Instance.OnShowLighthouseUI();
        SoundManager.instance.PlaySFX(openLighthouseUiSFX);
    }

    public void OnCancelInteraction()
    {
        lighthousePlayerCharacter.GetComponent<P3_fpsMovement>().EnablePlayerMovement();
        DisableLighthouseUI();
        P3_InputManager.Instance.OnHideLighthouseUI();
        SoundManager.instance.PlaySFX(closeLighthouseUiSFX);
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
        //rechargeButton.interactable = true;

        //repairButton.enabled = true;
        //repairButton.interactable = true;
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
        else if (P3_InputManager.Instance.PlayerPressedInteractButton() && isLighthouseScreenActive)
        {
            OnCancelInteraction();
        }

        //if (P3_InputManager.Instance.PlayerPressedUiCancelInteractButton() && isLighthouseScreenActive)
        //{
        //    OnCancelInteraction();
        //}

        #endregion

        #region Discharging

        if (startDischarging)
        {
            OnDischarging();
        }

        #endregion

        #region Checking current charge

        CheckCharge();
        CheckRepair();

        #endregion

        #region Enabling and Disabling Buttons

        SetChargeButtonStatus();
        SetRepairButtonStatus();

        #endregion

        #region Recharging

        if (isRecharging)
        {
            rechargeTimer += Time.deltaTime;
            Recharging();
            RechargeProgressBarMaxValue(rechargeTimeDuration);
            RechargeProgressBarCurrentValue(rechargeTimer);

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
            RepairProgressBarMaxValue(repairTimeDuration);
            RepairProgressBarCurrentValue(repairTimer);

            if (repairTimer >= repairTimeDuration)
            {
                FinishedRepairing();
            }
        }

        #endregion

        #region Updating U.I.
        
        currentYellowEnemiesText.text = $"Yellow killed: {currentYellowEnemiesKilled} / {requiredYellowEnemiesNeeded}";
        currentBlueEnemiesText.text = $"Blue killed: {currentBlueEnemiesKilled} / {requiredBlueEnemiesNeeded}";

        #endregion
    }
}
