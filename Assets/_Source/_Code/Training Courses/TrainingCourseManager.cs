using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;
using Cinemachine.Examples;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Linq;

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
    private FirstPersonMovement playerMovementScript;

    [Space(15)]

    [Header("Training Course Starting Position Transforms")]
    [SerializeField] private GameObject trainingCourseOneStartingPosition;
    [SerializeField] private GameObject trainingCourseTwoStartingPosition;
    [SerializeField] private GameObject trainingCourseThreeStartingPosition;


    [Space(10)]

    [Header("Training Course")]
    [SerializeField] private GameObject trainingCourseOneFinishingPosition;
    [SerializeField] private GameObject trainingCourseTwoFinishingPosition;
    [SerializeField] private GameObject trainingCourseThreeFinishingPosition;

    [Space(15)]

    [Header("Starting Player Position After Completing Training")]
    [SerializeField] private Transform playerStartingPositionAfterTraining;

    [Space(15)]

    [Header("TMP Text References")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private TextMeshProUGUI timeTitleText;
    [SerializeField] private TextMeshProUGUI countupTimerText;
    [SerializeField] private TextMeshProUGUI previousTimeText;
    [SerializeField] private TextMeshProUGUI targetHitText;
    [SerializeField] private TextMeshProUGUI currentScoreText;

    private InputManager inputManager;

    [Space(15)]

    [Header("Countdown Timer")]
    [SerializeField, Tooltip("The par time for the first training course.")] private float firstTrainingCourseParTime;
    [SerializeField, Tooltip("The par time for the second training course.")] private float secondTrainingCourseParTime;
    [SerializeField, Tooltip("The par time for the third training course.")] private float thirdTrainingCourseParTime;
    [SerializeField] private bool countupTimerActive = false;
    private float countupTimer;

    [Space(5)]

    [SerializeField, Tooltip("The previous time set for the first training course.")] private float firstTrainingCoursePreviousTime;
    [SerializeField, Tooltip("The previous time set for the second training course.")] private float secondTrainingCoursePreviousTime;
    [SerializeField, Tooltip("The previous time set for the third training course.")] private float thirdTrainingCoursePreviousTime;

    [Space(5)]

    [SerializeField] public bool achievedParTimeForFirstCourse = false;
    [SerializeField] public bool achievedParTimeForSecondCourse = false;
    [SerializeField] public bool achievedParTimeForThirdCourse = false;
    [SerializeField] public bool achievedParTimeForAllCourses = false;

    [Space(15)]

    [Header("Targets")]
    [SerializeField] private int totalTargetCount = 0;
    [SerializeField] private int targetHitCount = 0;
    [SerializeField] private int currentScore;
    [SerializeField] public List<GameObject> currentTargetList;
    [SerializeField] public List<GameObject> courseOnetargetList;
    [SerializeField] public List<GameObject> courseTwotargetList;
    [SerializeField] public List<GameObject> courseThreetargetList;

    [SerializeField] private List<Target> targetScriptList;
    [SerializeField] private List<Guid> targetGuidList;
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

    private bool updateOnScreenText = false;

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

        if (!isTrainingCourseOneComplete)
        {
            currentTrainingCourse = 1;
        }

        currentTargetList = new List<GameObject>();
        targetScriptList = new List<Target>();
        targetGuidList = new List<Guid>();
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
        playerMovementScript = playerCharacter.GetComponent<FirstPersonMovement>();
        trainingCourseGunManager = trainingCourseGun.GetComponent<GunplayManager>();

        FirstTimeTutorialText();
    }

    #region Countdown Timer Functions

    public void CountUpTimer()
    {
        if (countupTimerActive)
        {
            countupTimer += Time.deltaTime;
            countupTimerText.gameObject.SetActive(true);
            DisplayCountUpTimer();
        }
    }

    private void DisplayCountUpTimer()
    {
        TimeSpan time = TimeSpan.FromSeconds(countupTimer);
        countupTimer += 0;

        countupTimerText.text = time.Minutes.ToString() + ":" + time.Seconds.ToString() + ":" + time.Milliseconds.ToString();

        if (time.Minutes >= 59)
        {
            countupTimerText.text = time.Hours.ToString() + ":" + time.Minutes.ToString() + ":" + time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
        }
    }

    private void StopCountUpTimer()
    {
        countupTimerActive = false;

        if (hasPlayerCrossedFinishingLine)
        {
            CheckForParTime(countupTimer);
            isTrainingCourseComplete = true;
            NextTrainingCourse(currentTrainingCourse);
        }
    }

    #endregion

    #region Training Course Functions

    #region Start, Finish and Restart Training Course

    private void OnStartTrainingCourse(int trainingCourseID)
    {
        tutorialText.gameObject.SetActive(false);
        timeTitleText.gameObject.SetActive(true);

        isTrainingCourseComplete = false;
        hasPlayerCrossedFinishingLine = false;

        trainingCourseGunManager.gameObject.SetActive(true);
        trainingCourseGunManager.EnableGun(trainingCourseID);
        trainingCourseGunManager.isPlayerInTrainingCourse = true;

        playerMovementScript.EnablePlayerMovement();

        countupTimerActive = true;

        SetTargetLists(trainingCourseID);

        if (countupTimer > 0)
        {
            countupTimer = 0f;
        }

        updateOnScreenText = true;
        isTrainingCourseAllSetup = true;

        //If instantiating the targets
        //currentTargetList = new List<GameObject>();
    }

    private void OnTrainingCourseFinished(int trainingCourseID)
    {
        StopCountUpTimer();
        inputManager.OnDisable();
        hasPlayerStartedTrainingCourse = false;

        isTrainingCourseComplete = true;
        hasPlayerCrossedFinishingLine = true;

        playerMovementScript.DisablePlayerMovement();
        trainingCourseGunManager.DisableGun();
        trainingCourseGunManager.gameObject.SetActive(false);

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
        countupTimerActive = false;
        hasPlayerStartedTrainingCourse = false;

        inputManager.isPlayerInTrainingCourse = false;
        trainingCourseGunManager.isPlayerInTrainingCourse = false;

        NextTrainingCourse(currentTrainingCourse);
    }

    public void RestartTrainingCourse()
    {
        //Reset the training course
        hasPlayerStartedTrainingCourse = false;
        isTrainingCourseAllSetup = false;
        isTrainingCourseComplete = false;
        countupTimerActive = false;
        updateOnScreenText = true;

        countupTimer = 0f;

        trainingCourseGunManager.isPlayerInTrainingCourse = false;
        inputManager.isPlayerInTrainingCourse = false;

        foreach (GameObject target in currentTargetList)
        {
            target.GetComponent<Target>().isPlayerTraining = false;
        }

        LoadTrainingCourse(currentTrainingCourse);
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
        ClearTargetLists();

        if (!isTrainingCourseOneComplete && (courseID == 1))
        {
            currentTrainingCourse = courseID;
            EnableStartingPoints(courseID);
            playerCharacter.transform.position = trainingCourseOneStartingPosition.transform.position;
            EnableFinishingPoints(courseID);
            OnStartTrainingCourse(courseID);
        }
        else if (isTrainingCourseOneComplete && (courseID == 2))
        {
            currentTrainingCourse = courseID;
            playerCharacter.transform.position = trainingCourseTwoStartingPosition.transform.position;
            EnableStartingPoints(courseID);
            EnableFinishingPoints(courseID);
            OnStartTrainingCourse(courseID);
        }
        else if (isTrainingCourseOneComplete && isTrainingCourseTwoComplete && (courseID == 3))
        {
            currentTrainingCourse = courseID;
            playerCharacter.transform.position = trainingCourseThreeStartingPosition.transform.position;
            EnableStartingPoints(courseID);
            EnableFinishingPoints(courseID);
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

    #region Enabling and Disabling Starting Points and Finishing Points For Each Training Course

    #region Starting Points
    private void EnableStartingPoints(int trainingCourseID)
    {
        if (!hasPlayerStartedTrainingCourse && !isTrainingCourseAllSetup && !hasPlayerCrossedFinishingLine)
        {
            if (trainingCourseID == 1)
            {
                trainingCourseOneStartingPosition.SetActive(true);
            }
            else if (trainingCourseID == 2)
            {
                trainingCourseTwoStartingPosition.SetActive(true);
            }
            else if (trainingCourseID == 3)
            {
                trainingCourseThreeStartingPosition.SetActive(true);
            }

        }
    }

    private void DisableStartingPoints(int courseID)
    {
        if (hasPlayerStartedTrainingCourse && isTrainingCourseAllSetup && !hasPlayerCrossedFinishingLine)
        {
            if (courseID == 1)
            {
                trainingCourseOneStartingPosition.SetActive(false);
            }
            else if (courseID == 2)
            {
                trainingCourseTwoStartingPosition.SetActive(false);
            }
            else if (courseID == 3)
            {
                trainingCourseThreeStartingPosition.SetActive(false);
            }

        }

        isTrainingCourseAllSetup = false;
    }

    #endregion

    #region Finishing Points

    private void EnableFinishingPoints(int trainingCourseID)
    {
        if (hasPlayerStartedTrainingCourse && !isTrainingCourseAllSetup && !hasPlayerCrossedFinishingLine)
        {
            if (trainingCourseID == 1)
            {
                trainingCourseOneFinishingPosition.SetActive(true);
            }
            else if (trainingCourseID == 2)
            {
                trainingCourseTwoFinishingPosition.SetActive(true);
            }
            else if (trainingCourseID == 3)
            {
                trainingCourseThreeFinishingPosition.SetActive(true);
            }
        }
    }

    private void DisableFinishingPoints(int trainingCourseID)
    {
        if (hasPlayerStartedTrainingCourse && hasPlayerCrossedFinishingLine)
        {
            if (trainingCourseID == 1)
            {
                trainingCourseOneFinishingPosition.SetActive(false);
            }
            else if (trainingCourseID == 2)
            {
                trainingCourseTwoFinishingPosition.SetActive(false);
            }
            else if (trainingCourseID == 3)
            {
                trainingCourseThreeFinishingPosition.SetActive(false);
            }

            hasPlayerCrossedFinishingLine = false;
        }
    }

    #endregion

    #endregion

    #region Check for Par Time

    private void CheckForParTime(float compareTime)
    {
        if (currentTrainingCourse == 1)
        {
            if (compareTime <= firstTrainingCourseParTime)
            {
                achievedParTimeForFirstCourse = true;
            }

            firstTrainingCoursePreviousTime = compareTime;
        }
        else if (currentTrainingCourse == 2)
        {
            if (compareTime <= secondTrainingCourseParTime)
            {
                achievedParTimeForSecondCourse = true;
            }

            secondTrainingCoursePreviousTime = compareTime;
        }
        else if (currentTrainingCourse == 3)
        {
            if (compareTime <= thirdTrainingCourseParTime)
            {
                achievedParTimeForThirdCourse = true;
            }

            thirdTrainingCoursePreviousTime = compareTime;
        }

        if (achievedParTimeForFirstCourse && achievedParTimeForSecondCourse && achievedParTimeForThirdCourse)
        {
            achievedParTimeForAllCourses = true;
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

        targetGuidList.Remove(guid);

        GameObject currentTarget = null;
        foreach(var target in currentTargetList)
        {
            if(target.GetComponent<Target>().targetGuid == guid)
            {
                currentTarget = target.gameObject;
                break;
            }
        }
        if(currentTarget != null)
        {
            currentTargetList.Remove(currentTarget);
            Destroy(currentTarget);
        }


        Target targetToRemove = null;
        foreach (Target targetScript in targetScriptList)
        {
            if(targetScript.targetGuid == guid)
            {
                targetToRemove = targetScript;
            }
        }
        if(targetToRemove != null)
        {
            targetScriptList.Remove(targetToRemove);
        }
    }

    public void UpdateScore(int updateScore)
    {
        if (hasPlayerStartedTrainingCourse && (countupTimer != 0))
        {
            currentScore += updateScore;
            updateOnScreenText = true;
        }
    }

    private void ClearTargetLists()
    {
        //Clear the lists for targets
        if (currentTargetList.Count > 0)
        {
            currentTargetList.Clear();
        }

        if (targetScriptList.Count > 0)
        {
            targetScriptList.Clear();
        }

        if (targetGuidList.Count > 0)
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

        if (courseID == 1)
        {
            currentTargetList = courseOnetargetList;

            foreach (GameObject target in courseOnetargetList)
            {
                targetScriptList.Add(target.GetComponent<Target>());
            }

            if (currentTargetList.Count != courseOnetargetList.Count)
            {
                Debug.LogError("TrainingCourseManager: Incorrect number of targets for the first training course!");
            }
        }
        else if (courseID == 2)
        {
            currentTargetList = courseTwotargetList;

            foreach (GameObject target in courseTwotargetList)
            {
                targetScriptList.Add(target.GetComponent<Target>());
            }
        }
        else if (courseID == 3)
        {
            currentTargetList = courseThreetargetList;

            foreach (GameObject target in courseThreetargetList)
            {
                targetScriptList.Add(target.GetComponent<Target>());
            }
        }

        totalTargetCount = currentTargetList.Count;

        //Populating the list for each of the scripts from the targets
        foreach (GameObject target in currentTargetList)
        {
            target.gameObject.SetActive(true);

            if (targetScriptList.Count != currentTargetList.Count)
            {
                Debug.LogError("Too many target scripts in the target script list!");
            }
        }

        isTrainingCourseAllSetup = true;

        //Setting and Checking Target IDs in the Target Scripts
        for(int i = 0; i < currentTargetList.Count; i++)
        {
            var targetScript = targetScriptList[i];
            targetScript.SetTargetID(i + 1);

            targetScript.isPlayerTraining = true;
            targetScript.targetGuid = Guid.NewGuid();
            targetGuidList.Add(targetScript.targetGuid);
            targetScript.ReportTarget();

            if ((targetScriptList.Count != currentTargetList.Count))
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

    #region Setting and Updating U.I. Elements

    private void SetUiText()
    {
        currentScoreText.text = "Score: " + currentScore.ToString();
        targetHitText.text = ("Targets hit: " + targetHitCount.ToString() + " / " + totalTargetCount.ToString());

        if (currentTrainingCourse == 1 && firstTrainingCoursePreviousTime > 0f)
        {
            previousTimeText.text = "Previous time: " + firstTrainingCoursePreviousTime;
        }
        else if (currentTrainingCourse == 2 && secondTrainingCoursePreviousTime > 0f)
        {
            previousTimeText.text = "Previous time: " + secondTrainingCoursePreviousTime;
        }
        else if (currentTrainingCourse == 3 && thirdTrainingCoursePreviousTime > 0f)
        {
            previousTimeText.text = "Previous time: " + thirdTrainingCoursePreviousTime;
        }

        updateOnScreenText = false;
    }

    private void FirstTimeTutorialText()
    {
        if (currentTrainingCourse == 1)
        {
            tutorialText.gameObject.SetActive(true);
            timeTitleText.gameObject.SetActive(false);
        }
        else
        {
            tutorialText.gameObject.SetActive(false);
        }
    }

    #endregion

    void Update()
    {
        if (inputManager.PlayerStartedTrainingCourse())
        {
            hasPlayerStartedTrainingCourse = true;
        }

        if (hasPlayerStartedTrainingCourse && countupTimerActive && !hasPlayerCrossedFinishingLine)
        {
            CountUpTimer();

            if (targetHitCount != 0)
            {
                if (targetHitCount == totalTargetCount)
                {
                    Debug.Log("All targets hit!");
                    hasPlayerHitAllTargets = true;
                }
            }
        }

        if (updateOnScreenText)
        {
            SetUiText();
        }

        if (isTrainingCourseAllSetup)
        {
            DisableStartingPoints(currentTrainingCourse);
        }

        if (hasPlayerCrossedFinishingLine)
        {
            DisableFinishingPoints(currentTrainingCourse);
            StopCountUpTimer();

        }
    }
}
