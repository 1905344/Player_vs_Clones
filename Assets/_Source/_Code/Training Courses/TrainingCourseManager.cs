using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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

    [Header("Training Course Gun Reference")]
    [SerializeField] private GunplayManager trainingCourseGun;

    [Space(15)]

    [Header("Player Character Reference")]
    [SerializeField] private GameObject playerCharacter;

    [Space(15)]

    [Header("Training Course Starting Position Transforms")]
    [SerializeField] private Transform trainingCourseOneStartingPosition;
    [SerializeField] private Transform trainingCourseTwoStartingPosition;
    [SerializeField] private Transform trainingCourseThreeStartingPosition;

    [Space(15)]

    [Header("TMP Text References")]
    [SerializeField] private TextMeshProUGUI countdownTimerText;
    [SerializeField] private TextMeshProUGUI targetHitText;
    [SerializeField] private TextMeshProUGUI currentScoreText;

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

    [Header("Targets")]
    [SerializeField] private GameObject _target;
    [SerializeField] private float totalTargetCount;
    [SerializeField] private float targetHitCount;
    [SerializeField] private float currentScore;
    [SerializeField] public List<GameObject> targetList;
    private List<Target> targetScriptList;
    private List<Guid> targetGuidList;
    private bool checkForAllTargets = false;

    [Space(15)]

    [SerializeField] private bool hasPlayerStartedTrainingCourse = false;
    [SerializeField] private bool isTrainingCourseAllSetup = false;
    [SerializeField] private bool isTrainingCourseComplete = false;

    [Space(15)]

    [Header("Training Course Booleans")]
    [SerializeField] private bool isTrainingCourseOneComplete = false;
    [SerializeField] private bool isTrainingCourseTwoComplete = false;
    [SerializeField] private bool isTrainingCourseThreeComplete = false;

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

    #region Countdown Timer Functions

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
            isTrainingCourseComplete = true;
            NextTrainingCourse();
        }
        else
        {
            isTrainingCourseComplete = false;
            RestartTrainingCourse();
        }
    }

    #endregion

    #region Start, Restart and Next Training Course

    private void OnStartTrainingCourse()
    {
        if (inputManager.PlayerStartedTrainingCourse())
        {
            hasPlayerStartedTrainingCourse = true;
            isTrainingCourseComplete = false;

            if (targetList.Count > 0)
            {
                //If the target list is somehow not empty
                Debug.LogError("TrainingCourseManager: Training course cannot setup because the target game object list is not clear!");
                Debug.Break();
            }

            targetList = new List<GameObject>();
            targetScriptList = new List<Target>();
            targetGuidList = new List<Guid>();

            //Populating the list for each of the scripts from the targets
            foreach (GameObject target in targetList)
            {
                targetScriptList.Add(target.GetComponent<Target>());
            }

            //Populating the list 
            foreach(Target targetScript in targetScriptList)
            {
                for (int i = 0; i < targetList.Count; i++)
                {
                    targetScript.SetTargetID(i);
                    targetScript.ReportTarget();
                    targetGuidList.Add(targetScript.GetThisTargetsGuid());
                }

                if ((targetScriptList.Count != targetList.Count))
                {
                    Debug.LogError("TrainingCourseManager: Target script list count is not equal to the target game object list count!");
                    break;
                }
                else
                {
                    isTrainingCourseAllSetup = true;
                    continue;
                }
            }

            //If instantiating the targets
            //targetList = new List<GameObject>();
        }
    }

    private void RestartTrainingCourse()
    {
        //Reset the training course if the player fails to complete it in time
    }

    private void NextTrainingCourse()
    {
        targetList.Clear();
        targetScriptList.Clear();
        targetGuidList.Clear();

        //Load and setup the next training course
        if (isTrainingCourseOneComplete)
        {
            LoadTrainingCourseTwo();
        }
        else if (isTrainingCourseTwoComplete)
        {
            LoadTrainingCourseThree();
        }
        else if (isTrainingCourseThreeComplete)
        {
            //Call the function to switch the player to the actual gameplay
            //Call the relevant finite state machine function(s) to create the A.I.
        }
    }

    #endregion

    #region Training Course Functions

    private void LoadTrainingCourseOne()
    {

    }

    private void LoadTrainingCourseTwo()
    {
        if (isTrainingCourseOneComplete)
        {
            playerCharacter.transform.position = trainingCourseTwoStartingPosition.position;
        }
    }

    private void LoadTrainingCourseThree()
    {
        if (isTrainingCourseTwoComplete)
        {
            playerCharacter.transform.position = trainingCourseThreeStartingPosition.position;
        }
    }

    #endregion

    #region Target Functions

    //private void SetupTargets()
    //{
    //This function is only needed if instantiating the targets
    //Then instantiate and set the numerical ID of each target

    //Instantiate(_target);
    //}

    public void OnTargetHit(float updateScore)
    {
        currentScore += updateScore;
        currentScoreText.text = ("Score: " + currentScore);

        targetHitCount++;
        targetHitText.text = ("Targets hit: " + targetHitCount + " / " + totalTargetCount);
    }

    #endregion

    void Update()
    {
        if (hasPlayerStartedTrainingCourse)
        {
            trainingCourseGun.isPlayerInTrainingCourse = true;
            countdownTimerActive = true;
        }
    }
}
