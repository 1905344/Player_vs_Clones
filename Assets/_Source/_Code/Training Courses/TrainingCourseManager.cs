using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;

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
    [SerializeField] private GameObject trainingCourseGun;
    private GunplayManager trainingCourseGunManager;

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
    //private GameObject _target;
    [SerializeField] private int totalTargetCount;
    [SerializeField] private int targetHitCount;
    [SerializeField] private int currentScore;
    [SerializeField] public List<GameObject> currentTargetList;
    [SerializeField] public List<GameObject> courseOnetargetList;
    [SerializeField] public List<GameObject> courseTwotargetList;
    [SerializeField] public List<GameObject> courseThreetargetList;
    private List<TargetTrigger> targetTriggerScriptList;
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

    private void Start()
    {
        GameManager.Instance.OnTargetHit += TargetHit;
        //GameManager.Instance.OnTrainingCourseStart += OnStartTrainingCourse;
        //GameManager.Instance.OnTrainingCourseEnd += NextTrainingCourse();
        trainingCourseGunManager = trainingCourseGun.GetComponent<GunplayManager>();

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
        NextTrainingCourse();
        isTrainingCourseComplete = true;

        //This if statement needs to be changed to check if the 
        //player has hit all of the targets and then to trigger
        //some other check
        //if (targetHitCount == totalTargetCount)
        //{
        //    isTrainingCourseComplete = true;
        //    NextTrainingCourse();
        //}
        //else
        //{
        //    isTrainingCourseComplete = false;
        //    RestartTrainingCourse();
        //}
    }

    #endregion

    #region Start, Restart and Next Training Course

    private void OnStartTrainingCourse()
    {
        if (inputManager.PlayerStartedTrainingCourse())
        {
            hasPlayerStartedTrainingCourse = true;
            isTrainingCourseComplete = false;

            countdownTimer = countdownTimerAmount;

            if (currentTargetList.Count > 0)
            {
                //If the target list is somehow not empty
                Debug.LogError("TrainingCourseManager: Training course cannot setup because the target game object list is not clear!");
                Debug.Break();
            }

            currentTargetList = new List<GameObject>();
            targetTriggerScriptList = new List<TargetTrigger>();
            targetGuidList = new List<Guid>();

            //Populating the list for each of the scripts from the targets
            foreach (GameObject target in currentTargetList)
            {
                targetTriggerScriptList.Add(target.GetComponentInChildren<TargetTrigger>());
                target.GetComponent<Target>().isPlayerTraining = true;
            }

            //Populating the list 
            foreach(TargetTrigger targetTriggerScript in targetTriggerScriptList)
            {
                for (int i = 0; i < currentTargetList.Count; i++)
                {
                    targetTriggerScript.SetTargetID(i);
                    targetTriggerScript.ReportTarget();
                    targetGuidList.Add(targetTriggerScript.GetThisTargetsGuid());
                }

                if ((targetTriggerScriptList.Count != currentTargetList.Count))
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
            //currentTargetList = new List<GameObject>();
        }
    }

    private void OnTrainingCourseFinished()
    {
        hasPlayerStartedTrainingCourse = false;
        isTrainingCourseAllSetup = false;

        foreach (GameObject target in currentTargetList)
        {
            target.GetComponent<Target>().isPlayerTraining = false;
        }
    }


    private void RestartTrainingCourse()
    {
        //Reset the training course if the player fails to complete it in time
        hasPlayerStartedTrainingCourse = false;
        isTrainingCourseAllSetup = false;
        isTrainingCourseComplete = false;

        foreach (GameObject target in currentTargetList)
        {
            target.GetComponent<Target>().isPlayerTraining = false;
        }

        if (!isTrainingCourseOneComplete)
        {
            playerCharacter.transform.position = trainingCourseOneStartingPosition.position;
            OnStartTrainingCourse();
        }
        else if (isTrainingCourseOneComplete && !isTrainingCourseTwoComplete)
        {
            playerCharacter.transform.position = trainingCourseTwoStartingPosition.position;
            OnStartTrainingCourse();
        }
        else if (isTrainingCourseOneComplete && isTrainingCourseTwoComplete && !isTrainingCourseThreeComplete)
        {
            playerCharacter.transform.position = trainingCourseThreeStartingPosition.position;
            OnStartTrainingCourse();
        }
    }

    private void NextTrainingCourse()
    {
        currentTargetList.Clear();
        targetTriggerScriptList.Clear();
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

    public void TargetHit(Guid guid, int damage)
    {
        targetHitCount++;
        targetHitText.text = ("Targets hit: " + targetHitCount + " / " + totalTargetCount);

        targetGuidList.Remove(guid);

        foreach (GameObject target in currentTargetList)
        {
            Guid getTargetGuid = target.GetComponent<Target>().targetGuid;

            if (getTargetGuid == guid)
            {
                Destroy(target.transform);
            }
        }

        foreach (TargetTrigger targetTrigger in targetTriggerScriptList)
        {
            Guid getTargetGuid = targetTrigger.targetGuid;

            if (getTargetGuid == guid)
            {
                targetTriggerScriptList.Remove(targetTrigger);
            }
        }
    }

    public void UpdateScore(int updateScore)
    {
        if (hasPlayerStartedTrainingCourse && (countdownTimer != 0))
        {
            currentScore += updateScore;
            currentScoreText.text = ("Score: " + currentScore);
        }
    }

    #endregion

    void Update()
    {
        if (hasPlayerStartedTrainingCourse)
        {
            trainingCourseGunManager.isPlayerInTrainingCourse = true;
            countdownTimerActive = true;
        }
    }
}
