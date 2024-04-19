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
    [SerializeField] private Transform playerStartingPositionAfterTraining;

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
    [SerializeField] private bool hasPlayerCrossedFinishingLine = false;
    [SerializeField] private bool hasPlayerHitAllTargets = false;
    [SerializeField] private bool isTrainingCourseAllSetup = false;
    [SerializeField] private bool isTrainingCourseComplete = false;

    [Space(15)]

    [Header("Training Course Booleans")]
    [SerializeField] private bool isTrainingCourseOneComplete = false;
    [SerializeField] private bool isTrainingCourseTwoComplete = false;
    [SerializeField] private bool isTrainingCourseThreeComplete = false;
    [SerializeField] private bool isTrainingComplete = false;

    public int currentTrainingCourse = 0;

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
        inputManager = InputManager.Instance;
        GameManager.Instance.OnTargetHit += TargetHit;

        //Associating the functions for the events in the Game Manager
        GameManager.Instance.TrainingCourseStarted += LoadTrainingCourse;
        GameManager.Instance.TrainingCourseEnded += OnTrainingCourseFinished;
        GameManager.Instance.FinishedTraining += OnTrainingComplete;

        trainingCourseGunManager = trainingCourseGun.GetComponent<GunplayManager>();

        if (!isTrainingCourseOneComplete)
        {
            currentTrainingCourse = 1;
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
        countdownTimerText.text = string.Format("{0:00}:{1:00}:{2:000}", 0, 0, 0);
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

    #region Training Course Functions

    #region Start, Finish and Restart Training Course

    private void OnStartTrainingCourse(int trainingCourseID)
    {
        isTrainingCourseComplete = false;
        hasPlayerCrossedFinishingLine = false;

        trainingCourseGunManager.isPlayerInTrainingCourse = true;

        countdownTimerActive = true;

        if (currentTargetList.Count > 0)
        {
            //If the target list is somehow not empty
            Debug.LogError("TrainingCourseManager: Training course cannot setup because the target game object list is not clear!");
            Debug.Break();
        }
        else
        {
            SetTargetLists(trainingCourseID);
        }
        
        if (countdownTimer == 0)
        {
            countdownTimer = countdownTimerAmount;
        }

        //If instantiating the targets
        //currentTargetList = new List<GameObject>();
    }

    private void OnTrainingCourseFinished(int trainingCourseID)
    {
        isTrainingCourseComplete = true;
        hasPlayerCrossedFinishingLine = true;

        if (isTrainingCourseComplete)
        {
            //Clear the lists for targets
            ClearTargetLists();

            if (trainingCourseID == 1)
            {
                isTrainingCourseOneComplete = true;
            }
            else if (trainingCourseID == 2)
            {
                isTrainingCourseTwoComplete = true;
            }
            else if (trainingCourseID == 3)
            {
                isTrainingCourseThreeComplete = true;
            }

            foreach (GameObject target in currentTargetList)
            {
                target.GetComponent<Target>().isPlayerTraining = false;
            }

            hasPlayerStartedTrainingCourse = false;
            isTrainingCourseAllSetup = false;
            countdownTimerActive = false;
            hasPlayerStartedTrainingCourse = false;
            inputManager.isPlayerInTrainingCourse = false;

            trainingCourseGunManager.isPlayerInTrainingCourse = false;
            NextTrainingCourse(currentTrainingCourse);
        }
    }

    private void RestartTrainingCourse(int courseID)
    {
        //Reset the training course if the player fails to complete it in time
        hasPlayerStartedTrainingCourse = false;
        isTrainingCourseAllSetup = false;
        isTrainingCourseComplete = false;
        countdownTimerActive = false;

        trainingCourseGunManager.isPlayerInTrainingCourse = false;
        inputManager.isPlayerInTrainingCourse = false;

        foreach (GameObject target in currentTargetList)
        {
            target.GetComponent<Target>().isPlayerTraining = false;
        }

        LoadTrainingCourse(courseID);
    }


    #endregion

    #region Next and Load Training Courses and All Training Courses Completed

    private void NextTrainingCourse(int courseID)
    {
        if (!isTrainingCourseComplete)
        {
            return;
        }
        else
        {
            LoadTrainingCourse(courseID);
        }
    }

    private void LoadTrainingCourse(int courseID)
    {
        if (!isTrainingCourseOneComplete && courseID == 1)
        {
            currentTrainingCourse = courseID;
            playerCharacter.transform.position = trainingCourseOneStartingPosition.position;
            OnStartTrainingCourse(courseID);
        }
        else if (isTrainingCourseOneComplete && courseID == 2)
        {
            currentTrainingCourse = courseID;
            playerCharacter.transform.position = trainingCourseTwoStartingPosition.position;
            OnStartTrainingCourse(courseID);
        }
        else if (isTrainingCourseOneComplete && isTrainingCourseTwoComplete)
        {
            currentTrainingCourse = courseID;
            playerCharacter.transform.position = trainingCourseThreeStartingPosition.position;
            OnStartTrainingCourse(courseID);
        }
        else if (isTrainingCourseThreeComplete && courseID == 3)
        {
            OnTrainingComplete();
            isTrainingComplete = true;
        }
    }

    private void OnTrainingComplete()
    {
        if (isTrainingComplete)
        {
            playerCharacter.transform.position = playerStartingPositionAfterTraining.position; 
            GameManager.Instance.OnPlayerFinishedTraining();
            GameManager.Instance.OnSetAiBehaviour();
        }
        else
        {
            return;
        }
    }
    #endregion

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
            targetTriggerScriptList.Add(targetTrigger);

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

    private void ClearTargetLists()
    {
        //Clear the lists for targets
        if (currentTargetList.Count > 0)
        {
            currentTargetList.Clear();
        }

        if (targetTriggerScriptList != null && targetTriggerScriptList.Count > 0)
        {
            targetTriggerScriptList.Clear();
        }

        if (targetGuidList != null && targetGuidList.Count > 0)
        {
            targetGuidList.Clear();
        }
    }

    private void SetTargetLists(int courseID)
    {
        if (currentTargetList.Count > 0)
        {
            //If the target list is somehow not empty
            Debug.LogError("TrainingCourseManager: Training course cannot setup because the target game object list is not clear!");
            Debug.Break();
        }

        currentTargetList = new List<GameObject>();
        targetTriggerScriptList = new List<TargetTrigger>();
        targetGuidList = new List<Guid>();

        if (courseID == 1)
        {
            currentTargetList = courseOnetargetList;
        }
        else if (courseID == 2)
        {
            currentTargetList = courseTwotargetList;
        }
        else if (courseID == 3)
        {
            currentTargetList = courseThreetargetList;
        }

        //Populating the list for each of the scripts from the targets
        foreach (GameObject target in currentTargetList)
        {
            targetTriggerScriptList.Add(target.GetComponentInChildren<TargetTrigger>());
            target.GetComponent<Target>().isPlayerTraining = true;
        }

        //Setting and Checking Target IDs in the Target Trigger Scripts
        foreach (TargetTrigger targetTriggerScript in targetTriggerScriptList)
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
    }

    #endregion

    void Update()
    {
        if (inputManager.PlayerStartedTrainingCourse())
        {
            hasPlayerStartedTrainingCourse = true;
        }

        if (hasPlayerStartedTrainingCourse && countdownTimerActive && countdownTimer > 0f && !hasPlayerCrossedFinishingLine)
        {
            CountdownTimer();

            if (targetHitCount != 0)
            {
                if (targetHitCount == totalTargetCount)
                {
                    Debug.Log("All targets hit!");
                    hasPlayerHitAllTargets = true;
                    //countdownTimerActive = false;
                    //NextTrainingCourse(currentTrainingCourse);
                }
            }
        }
        //else if (hasPlayerStartedTrainingCourse && !countdownTimerActive && countdownTimer <= 0f)
        //{
        //    if (targetHitCount != totalTargetCount)
        //    {
        //        Debug.Log("Player missed some targets!");
        //        isTrainingCourseComplete = false;
        //        countdownTimerActive = false;
        //        RestartTrainingCourse(currentTrainingCourse);
        //    }
        //}
    }
}
