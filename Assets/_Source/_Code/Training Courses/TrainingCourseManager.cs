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
    private FirstPersonMovement playerMovementScript;
    private Transform playerCharacterTransform;

    private bool updatePlayerTransformPosition = false;

    [Space(15)]

    [Header("Training Course Starting Position Transforms")]
    [SerializeField] private GameObject trainingCourseOneStartingPosition;
    [SerializeField] private GameObject trainingCourseTwoStartingPosition;
    [SerializeField] private GameObject trainingCourseThreeStartingPosition;

    private Transform trainingCourseOneStartPositionTransform;
    private Transform trainingCourseTwoStartPositionTransform;
    private Transform trainingCourseThreeStartPositionTransform;

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

    [Header("Training Course Par Times")]
    [SerializeField] private float trainingCourseOneParTime;
    [SerializeField] private float trainingCourseTwoParTime;
    [SerializeField] private float trainingCourseThreeParTime;

    [Space(5)]

    [SerializeField] public bool trainingCourseOneParTimeAchieved = false;
    [SerializeField] public bool trainingCourseTwoParTimeAchieved = false;
    [SerializeField] public bool trainingCourseThreeParTimeAchieved = false;
    [SerializeField] public bool hasPlayerAchievedParTimeForAllCourses = false;

    [Space(15)]

    [Header("Targets")]
    [SerializeField] private int totalTargetCount = 0;
    [SerializeField] private int targetHitCount = 0;
    [SerializeField] public int totalTargetHitCount = 0;
    [SerializeField] private int currentScore;
    [SerializeField] private int totalScore = 0;
    [SerializeField] public int finalScore = 0;

    [Space(10)]

    [SerializeField] public List<GameObject> currentTargetList;
    [SerializeField] public List<GameObject> courseOnetargetList;
    [SerializeField] public List<GameObject> courseTwotargetList;
    [SerializeField] public List<GameObject> courseThreetargetList;

    [Space(5)]

    [SerializeField] private List<Target> targetScriptList;
    [SerializeField] private List<Guid> targetGuidList;

    [Space(15)]

    [SerializeField] private bool hasPlayerStartedTrainingCourse = false;
    [SerializeField] private bool hasPlayerCrossedFinishingLine = false;
    [SerializeField] public bool hasPlayerHitAllTargets = false;

    [Space(10)]

    [SerializeField] public bool hasPlayerHitAllCourseOneTargets = false;
    [SerializeField] public bool hasPlayerHitAllCourseTwoTargets = false;
    [SerializeField] public bool hasPlayerHitAllCourseThreeTargets = false;

    [Space(10)]

    [SerializeField] public bool didPlayerAchieveHighestScore = false;
    [SerializeField] public bool didPlayerAchieveAboveAverageScore = false;
    [SerializeField] public bool didPlayerAchieveBelowAverageScore = false;

    [Space(10)]

    [SerializeField] private bool isTrainingCourseAllSetup = false;
    [SerializeField] private bool isTrainingCourseComplete = false;

    [Space(15)]

    [Header("Training Course Booleans")]
    [SerializeField] private bool isTrainingCourseOneComplete = false;
    [SerializeField] private bool isTrainingCourseTwoComplete = false;
    [SerializeField] private bool isTrainingCourseThreeComplete = false;
    [SerializeField] public bool isTrainingComplete = false;

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
        GameManager.Instance.TrainingCourseRestarted += RestartTrainingCourse;
        GameManager.Instance.TrainingCourseEnded += OnTrainingCourseFinished;
        GameManager.Instance.FinishedTraining += OnTrainingComplete;

        trainingCourseGunManager = trainingCourseGun.GetComponent<GunplayManager>();
        trainingCourseGunManager = trainingCourseGun.GetComponent<GunplayManager>();

        playerMovementScript = playerCharacter.GetComponent<FirstPersonMovement>();
        playerCharacterTransform = playerCharacter.GetComponent<Transform>();

        trainingCourseOneStartPositionTransform = trainingCourseOneStartingPosition.GetComponent<Transform>();
        trainingCourseTwoStartPositionTransform = trainingCourseTwoStartingPosition.GetComponent<Transform>();
        trainingCourseThreeStartPositionTransform = trainingCourseThreeStartingPosition.GetComponent<Transform>();
    }

    public void DisableTrainingCourseManager()
    {
        gameObject.SetActive(false);
    }

    #region Countdown Timer Functions

    public void CountdownTimer()
    {
        if (countdownTimerActive)
        {
            if (countdownTimer > 0)
            {
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

        //if (hasPlayerCrossedFinishingLine)
        //{
        //    isTrainingCourseComplete = true;
        //    NextTrainingCourse(currentTrainingCourse);
        //}
        //else
        //{
        //    isTrainingCourseComplete = false;
        //    RestartTrainingCourse(currentTrainingCourse);
        //}
    }

    private void ResetCountdownTimer(int courseID)
    {
        countdownTimer = 0f;

        if (courseID == 1)
        {
            countdownTimer = firstTrainingCourseTimer;
        }
        else if (courseID == 2)
        {
            countdownTimer = secondTrainingCourseTimer;
        }
        else if (courseID == 3)
        {
            countdownTimer = thirdTrainingCourseTimer;
        }
    }

    #endregion

    #region Training Course Functions

    #region Start, Finish and Restart Training Course

    private void OnStartTrainingCourse(int trainingCourseID)
    {
        isTrainingCourseComplete = false;
        hasPlayerCrossedFinishingLine = false;
        updatePlayerTransformPosition = false;

        trainingCourseGunManager.gameObject.SetActive(true);
        trainingCourseGunManager.EnableGun(trainingCourseID);
        trainingCourseGunManager.isPlayerInTrainingCourse = true;

        //playerMovementScript.EnablePlayerMovement();

        countdownTimerActive = true;

        ClearTargetLists();
        SetTargetLists(trainingCourseID);

        targetHitCount = 0;
        totalTargetCount = currentTargetList.Count;

        EnableGuiText();

        if (countdownTimer == 0)
        {
            countdownTimer = countdownTimerAmount;
        }

        updateOnScreenText = true;
        isTrainingCourseAllSetup = true;

        Debug.Log("TrainingCourseManager: Number of targets: " + currentTargetList.Count + " ." + "Total score is currently: " + totalScore + " .");

        //If instantiating the targets
        //currentTargetList = new List<GameObject>();
    }

    private void OnTrainingCourseFinished(int trainingCourseID)
    {
        trainingCourseID = currentTrainingCourse;

        DisableFinishingPoints(trainingCourseID);
        
        isTrainingCourseComplete = true;
        hasPlayerCrossedFinishingLine = true;

        playerMovementScript.DisablePlayerMovement();

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
            target.gameObject.SetActive(false);
        }

        hasPlayerHitAllTargets = false;
        hasPlayerStartedTrainingCourse = false;
        countdownTimerActive = false;
        hasPlayerStartedTrainingCourse = false;

        inputManager.isPlayerInTrainingCourse = false;

        totalScore += currentScore;
        currentScore = 0;
        totalTargetHitCount += targetHitCount;

        CheckForParTime();

        currentTrainingCourse++;
        NextTrainingCourse(currentTrainingCourse);
    }

    private void RestartTrainingCourse(int courseID)
    {
        //Need a trigger for the restart training course event in order for 
        //this to be called correctly

        //Reset the training course if the player fails to complete it in time
        hasPlayerStartedTrainingCourse = false;
        isTrainingCourseAllSetup = false;
        isTrainingCourseComplete = false;
        countdownTimerActive = false;
        updateOnScreenText = true;

        currentScore = 0;
        targetHitCount = 0;

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
            hasPlayerCrossedFinishingLine = false;
            DisableGuiText();

            EnableStartingPoints(courseID);
            updatePlayerTransformPosition = true;

            ResetCountdownTimer(courseID);
        }
    }

    private void LoadTrainingCourse(int courseID)
    {
        courseID = currentTrainingCourse;

        if (!isTrainingCourseOneComplete && (courseID == 1))
        {
            OnStartTrainingCourse(courseID);
            EnableFinishingPoints(courseID);
        }
        else if (isTrainingCourseOneComplete && (courseID == 2))
        {
            OnStartTrainingCourse(courseID);
            EnableFinishingPoints(courseID);
        }
        else if (isTrainingCourseOneComplete && isTrainingCourseTwoComplete && (courseID == 3))
        {
            OnStartTrainingCourse(courseID);
            EnableFinishingPoints(courseID);
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
            if (hasPlayerHitAllCourseOneTargets && hasPlayerHitAllCourseTwoTargets && hasPlayerHitAllCourseThreeTargets)
            {
                hasPlayerHitAllTargets = true;
                Debug.Log("TrainingCourseManager: Player has hit all the targets!");
            }

            float checkScore = (finalScore / totalScore) * 100f;

            if (finalScore == totalScore)
            {
                didPlayerAchieveHighestScore = true;
            }
            else if (checkScore > 48.5f)
            {
                didPlayerAchieveAboveAverageScore = true;
                didPlayerAchieveBelowAverageScore = false;
            }
            else if (checkScore <= 48.5f)
            {
                didPlayerAchieveAboveAverageScore = false;
                didPlayerAchieveBelowAverageScore = true;
            }

            if (trainingCourseOneParTimeAchieved && trainingCourseTwoParTimeAchieved && trainingCourseThreeParTimeAchieved)
            {
                hasPlayerAchievedParTimeForAllCourses = true;
            }
            else
            {
                hasPlayerAchievedParTimeForAllCourses = false;
            }
            
            updatePlayerTransformPosition = true;
            GameManager.Instance.OnSetAiBehaviour();
        }
        else
        {
            Debug.LogError("TrainingCourseManager: isTrainingComplete is somehow set to false at the end of training course 3");
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
        if (!hasPlayerStartedTrainingCourse && isTrainingCourseAllSetup && !hasPlayerCrossedFinishingLine)
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
        if (!hasPlayerStartedTrainingCourse && isTrainingCourseAllSetup && !hasPlayerCrossedFinishingLine)
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
        if (hasPlayerStartedTrainingCourse && (countdownTimer != 0))
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
            ////If the target list is somehow not empty
            //Debug.LogError("TrainingCourseManager: Training course cannot setup because the target game object list is not clear!");
            //Debug.Break();
        }

        if (courseID == 1)
        {
            currentTargetList = courseOnetargetList;

            foreach (GameObject target in courseOnetargetList)
            {
                targetScriptList.Add(target.GetComponent<Target>());
            }

            //if (currentTargetList.Count != courseOnetargetList.Count)
            //{
            //    Debug.LogError("TrainingCourseManager: Incorrect number of targets for the first training course!");
            //}
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

            totalScore += targetScript.targetScoreValue;

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

    #region Updating, Enabling and Disabling the U.I. Text Elements

    private void SetUiText()
    {
        currentScoreText.text = "Score: " + currentScore.ToString();
        targetHitText.text = ("Targets hit: " + targetHitCount.ToString() + " / " + totalTargetCount.ToString());

        updateOnScreenText = false;
    }

    private void EnableGuiText()
    {
        currentScoreText.gameObject.SetActive(true);
        targetHitText.gameObject.SetActive(true);
        countdownTimerText.gameObject.SetActive(true);
    }

    private void DisableGuiText()
    {
        currentScoreText.gameObject.SetActive(false);
        targetHitText.gameObject.SetActive(false);
        countdownTimerText.gameObject.SetActive(false);
    }

    #endregion

    private void OnMovePlayerTransform(int courseID)
    {
        if (!isTrainingCourseOneComplete && (courseID == 1))
        {
            playerCharacterTransform.position = trainingCourseOneStartPositionTransform.position;
            playerCharacterTransform.rotation = Quaternion.identity;
        }
        else if (isTrainingCourseOneComplete && (courseID == 2))
        {
            playerCharacterTransform.position = trainingCourseTwoStartPositionTransform.position;
            playerCharacterTransform.rotation = Quaternion.identity;
        }
        else if (isTrainingCourseOneComplete && isTrainingCourseTwoComplete && (courseID == 3))
        {
            playerCharacterTransform.position = trainingCourseThreeStartPositionTransform.position;
            playerCharacterTransform.rotation = Quaternion.identity;
        }
        else if (isTrainingComplete)
        {
            playerCharacter.transform.position = playerStartingPositionAfterTraining.position;
        }
    }

    private void OnAllTargetsHit()
    {
        if (hasPlayerHitAllTargets)
        {
            if (currentTrainingCourse == 1)
            {
                hasPlayerHitAllCourseOneTargets = true;
                hasPlayerHitAllTargets = false;
            }
            else if (currentTrainingCourse == 2)
            {
                hasPlayerHitAllCourseOneTargets = true;
                hasPlayerHitAllTargets = false;
            }
            else if (currentTrainingCourse == 3)
            {
                hasPlayerHitAllCourseOneTargets = true;
                hasPlayerHitAllTargets = false;
            }
        }
    }

    private void CheckForParTime()
    {
        if (currentTrainingCourse == 1)
        {
            if (countdownTimer >= trainingCourseOneParTime)
            {
                trainingCourseOneParTimeAchieved = true;
                Debug.Log("TrainingCourseManager: Par time achieved for the first training course!");
            }
            else
            {
                trainingCourseOneParTimeAchieved = false;
            }
        }
        else if (currentTrainingCourse == 2)
        {
            if (countdownTimer >= trainingCourseTwoParTime)
            {
                trainingCourseTwoParTimeAchieved = true;
                Debug.Log("TrainingCourseManager: Par time achieved for the second training course!");
            }
            else
            {
                trainingCourseTwoParTimeAchieved = false;
            }
        }
        else if (currentTrainingCourse == 3)
        {
            if (countdownTimer >= trainingCourseThreeParTime)
            {
                trainingCourseThreeParTimeAchieved = true;
                Debug.Log("TrainingCourseManager: Par time achieved for the third training course!");
            }
            else
            {
                trainingCourseThreeParTimeAchieved = false;
            }
        }
    }

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
                    OnAllTargetsHit();
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

        if (updatePlayerTransformPosition)
        {
            OnMovePlayerTransform(currentTrainingCourse);
        }
    }
}
