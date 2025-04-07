using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P3_LighthouseManager : MonoBehaviour
{
    #region Variables

    [Header("U.I.")]
    [SerializeField] private GameObject progressScreen;
    [SerializeField] private TextMeshProUGUI progressScreenText;
    [SerializeField] private TextMeshProUGUI interactText;

    [Space(5)]

    [SerializeField] private Slider progressBar;
    [SerializeField] private Image progressBarFill;
    [SerializeField] private Gradient fillGradient;
    [SerializeField, Tooltip("How long does it take to hack a door")] private float hackingTime;
    private float hackingTimer;

    private P3_fpsMovement characterMoveScript;
    
    [Space(10)]

    [Header("Lighthouse Charge Variables")]
    [SerializeField] private bool needsCharge;
    [SerializeField] private bool isRepairing;
    [SerializeField] private float currentCharge;
    [SerializeField] private float defaultMaximumCharge;
    [SerializeField] private float cappedMaxCharge;

    #endregion

    private void Awake()
    {
        progressBar.maxValue = hackingTime;
    }

    private void Start()
    {
        #region Debug

        #endregion
    }

    private void OnInteraction()
    {
        characterMoveScript.DisablePlayerMovement();

        progressBar.value = hackingTimer;
        progressBarFill.color = fillGradient.Evaluate(progressBar.normalizedValue);
    }

    private void OnRecharge()
    {
        
    }

    private void OnRepair()
    {

    }

    private void Update()
    {

    }
}
