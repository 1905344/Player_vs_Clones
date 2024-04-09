using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrainingCourseManager : MonoBehaviour
{
    #region Variables

    private static TrainingCourseManager _instance;

    public static TrainingCourseManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("TMP Text References")]
    [SerializeField] private TextMeshProUGUI countdownTimerText;
    [SerializeField] private TextMeshProUGUI targetHitText;
    [SerializeField] private TextMeshProUGUI currentScoreText;

    [Space(15)]

    [Header("Input Manager Reference")]
    private InputManager inputManager;

    [Space(15)]

    [Header("Countdown Timer")]
    [SerializeField, Tooltip("The amount of time in the countdown timer for the first training course.")] private float firstTrainingCourseTimer;
    [SerializeField, Tooltip("The amount of time in the countdown timer for the second training course.")] private float secondTrainingCourseTimer;
    [SerializeField, Tooltip("The amount of time in the countdown timer for the third training course.")] private float thirdTrainingCourseTimer;
    [SerializeField, Tooltip("The amount of time in the default countdown timer.")] private float countdownTimerAmount;
    [SerializeField] private bool countdownTimerActive = false;
    private float countdownTimer;

    [Space(15)]

    [SerializeField] private float totalTargetCount;
    [SerializeField] private float targetHitCount;
    [SerializeField] private float currentScore;

    [Space(15)]

    [SerializeField] private bool hasPlayerStartedTrainingCourse = false;
    [SerializeField] private bool hasPlayerCompletedTrainingCourse = false;

    [Space(15)]

    [Header("Training Course Booleans")]
    [SerializeField] private bool isTrainingCourseOneComplete = false;
    [SerializeField] private bool isTrainingCourseTwoComplete = false;
    [SerializeField] private bool isTrainingCourseThreeComplete = false;

    [Space(15)]

    [Header("Training Course Gun Reference")]
    [SerializeField] private GunplayManager trainingCourseGun;

    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void CountdownTimer()
    {
        if (countdownTimerActive)
        {
            if (countdownTimer > 0)
            {
                inputManager.OnEnable();
                countdownTimer -= Time.deltaTime;
                countdownTimerText.gameObject.SetActive(true);
                DisplayCountDownTimer(countdownTimer);
            }
            else
            {
                countdownTimer = 0;
                inputManager.OnDisable();
                hasPlayerStartedTrainingCourse = false;
                countdownTimerActive = false;
                DisplayCountDownTimer(countdownTimer);
                OnCountdownTimerFinished();
            }
        }
    }

    private void DisplayCountDownTimer(float countTime)
    {
        countTime += 0;
        float minutes = Mathf.FloorToInt(countTime / 60);
        float seconds = Mathf.FloorToInt(countTime % 60);
        float milliseconds = (countTime % 1) * 1000;

        countdownTimerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    private void OnCountdownTimerFinished()
    {
        if (targetHitCount == totalTargetCount)
        {
            hasPlayerCompletedTrainingCourse = true;
            NextTrainingCourse();
        }
        else
        {
            hasPlayerCompletedTrainingCourse = false;
            RestartTrainingCourse();
        }
    }

    #region Start, Restart and Next Training Course
    
    private void OnStartTrainingCourse()
    {
        if (inputManager.PlayerStartedTrainingCourse())
        {
            hasPlayerStartedTrainingCourse = true;
        }
    }

    private void RestartTrainingCourse()
    {
        //Reset the training course if the player fails to complete it in time
    }

    private void NextTrainingCourse()
    {
        //Load and setup the next training course
    }

    #endregion

    public void OnTargetHit(float updateScore)
    {
        currentScore += updateScore;
        currentScoreText.text = ("Score: " + currentScore);

        targetHitCount++;
        targetHitText.text = ("Targets hit: " + targetHitCount + " / " + totalTargetCount);
    }

    void Update()
    {
        if (hasPlayerStartedTrainingCourse)
        {
            trainingCourseGun.isPlayerInTrainingCourse = true;
            countdownTimerActive = true;
        }
    }
}
