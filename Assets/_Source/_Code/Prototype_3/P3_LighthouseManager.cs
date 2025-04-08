using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P3_LighthouseManager : MonoBehaviour
{
    #region Variables

    [Header("U.I.")]
    [SerializeField] private GameObject progressScreen;
    [SerializeField] private GameObject repairProgressScreen;
    [SerializeField] private TextMeshProUGUI progressScreenText;
    [SerializeField] private TextMeshProUGUI interactText;

    [Space(5)]

    [SerializeField] private Slider chargeProgressBar;
    [SerializeField] private Image chargeProgressBarFill;
    [SerializeField] private Gradient chargeFillGradient;

    [Space(5)]

    [SerializeField] private Slider repairProgressBar;
    [SerializeField] private Image repairProgressBarFill;
    [SerializeField] private Gradient repairFillGradient;

    [Space(5)]

    [SerializeField] private Button repairButton;
    [SerializeField] private Button chargeButton;

    private P3_fpsMovement characterMoveScript;

    [Space(10)]

    [Header("Charge and Repair Requirements")]
    [SerializeField] private int currentYellowEnemiesKilled;
    [SerializeField] private int maxYellowEnemiesNeeded;

    [Space(2)]

    [SerializeField] private int currentBlueEnemiesKilled;
    [SerializeField] private int maxBlueEnemiesNeeded;

    [Space(10)]

    [Header("Lighthouse Variables")]
    [SerializeField] private bool needsCharge;
    [SerializeField] private bool needsRepair;
    [SerializeField] private bool isRepairing;
    [SerializeField] private bool canCharge;
    [SerializeField] private bool canRepair;

    [Space(3)]

    [SerializeField] private float currentCharge;
    [SerializeField] private float increaseChargeAmount;

    [Space(3)]

    [SerializeField] private float restoreMaxChargeAmount;
    [SerializeField] private float currentMaxCharge;
    [SerializeField] private float defaultMaximumCharge;
    [SerializeField] private float cappedMaxCharge;

    [Space(10)]

    [Header("Timers")]
    [SerializeField, Tooltip("How fast the charge progress bar decreases.")] private float chargeCountdownTimer;
    private float chargeTimer = 0f;
    [SerializeField, Tooltip("How long it takes to repair")] private float repairTimeDuration;
    private float repairTimer = 0f;

    #endregion

    private void Awake()
    {
        chargeProgressBar.maxValue = defaultMaximumCharge;
        repairProgressBar.maxValue = defaultMaximumCharge;
    }

    private void Start()
    {
        P3_GameManager.Instance.LighthouseHit += OnTakeDamage;
        P3_GameManager.Instance.BlueEnemyKilled += UpdateRepairUI;
        P3_GameManager.Instance.YellowEnemyKilled += UpdateChargeUI;

    }

    public void OnInteraction()
    {
        characterMoveScript.DisablePlayerMovement();

        //progressBar.value = hackingTimer;
        //progressBarFill.color = fillGradient.Evaluate(progressBar.normalizedValue);
    }

    private void OnTakeDamage(float Amount)
    {
        needsCharge = true;
    }

    #region Charge Related Functions

    private void UpdateChargeUI()
    {
        currentYellowEnemiesKilled++;

        
    }

    public void OnRecharge()
    {
        if (!canCharge && currentCharge == currentMaxCharge)
        {
            return;
        }

        if (currentYellowEnemiesKilled > maxYellowEnemiesNeeded)
        {
            currentYellowEnemiesKilled -= maxYellowEnemiesNeeded;
        }

        

    }

    private void Recharging()
    {
        currentCharge += increaseChargeAmount;
        chargeProgressBar.value = currentCharge;
    }

    #endregion

    #region Repair Related Functions

    private void UpdateRepairUI()
    {
        currentBlueEnemiesKilled++;

        if (canRepair)
        {

        }
    }

    public void OnRepair()
    {
        if (!canRepair && currentMaxCharge == defaultMaximumCharge)
        {
            return;
        }

        if (currentBlueEnemiesKilled > maxBlueEnemiesNeeded)
        {
            currentBlueEnemiesKilled -= maxBlueEnemiesNeeded;
        }

        currentMaxCharge += restoreMaxChargeAmount;
    }

    private void Repairing()
    {
        isRepairing = true;
        repairProgressScreen.gameObject.SetActive(true);
    }

    private void FinishedRepairing()
    {
        currentMaxCharge += restoreMaxChargeAmount;

        repairProgressScreen.gameObject.SetActive(false);
        repairProgressBar.value = currentMaxCharge;
    }

    #endregion

    private void Update()
    {
        repairButton.enabled = canRepair;
        repairButton.interactable = canRepair;

        chargeButton.enabled = canCharge;
        chargeButton.interactable = canCharge;

        if (currentCharge < currentMaxCharge)
        {
            needsCharge = true;
        }

        if (currentMaxCharge < defaultMaximumCharge)
        {
            canRepair = true;
        }
    }
}
