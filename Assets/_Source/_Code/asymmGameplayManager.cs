using Cinemachine.Examples;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class asymmGameplayManager : MonoBehaviour
{
    #region Variables

    private static asymmGameplayManager _instance;

    public static asymmGameplayManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField] private GameObject playerCharacter;
    private FirstPersonMovement playerMovementScript;
    [SerializeField] private Transform startingPosition;

    [SerializeField] private Transform enemyParentTransform;
    [SerializeField] private GameObject mainGameTutorialText;

    [Space(15)]

    [Header("Enemies")]
    [SerializeField] private GameObject aggressiveEnemyType;
    [SerializeField] private GameObject fastAggressiveEnemyType;
    [SerializeField] private GameObject slowAggressiveEnemyType;
    //[SerializeField] private GameObject aggressiveExplorativeEnemyType;
    [SerializeField] private GameObject defensiveEnemyType;
    [SerializeField] private GameObject fastDefensiveEnemyType;
    [SerializeField] private GameObject slowDefensiveEnemyType;
    //[SerializeField] private GameObject defensiveExplorativeEnemyType;
    //[SerializeField] private GameObject explorativeEnemyType;

    [Space(10)]

    [Header("Enemy Starting Positions")]
    [SerializeField] private Transform enemyOneStartingPosition;
    [SerializeField] private Transform enemyTwoStartingPosition;
    [SerializeField] private Transform enemyThreeStartingPosition;

    [Space(15)]

    [SerializeField] private bool setEnemyAggressive = false;
    [SerializeField] private bool setEnemyDefensive = false;
    [SerializeField] private bool setEnemyFast = false;
    [SerializeField] private bool setEnemySlow = false;
    //[SerializeField] private bool setEnemyExplorative = false;

    [Space(15)]

    [Header("U.I. Elements")]
    [SerializeField] private GameObject gameOverScreen;

    [Space(15)]

    [Header("Level Complete Trigger")]
    [SerializeField] private GameObject redDoor;

    [Space(10)]

    private bool createEnemies = false;

    private List<GameObject> enemyList = new List<GameObject>();

    private int enemyNumberID = 0;
 
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

        //GameManager.Instance.FinishedTraining += SetupAsymmetricalGameplay;
        ////GameManager.Instance.LevelFailed += OnGameOver;
        GameManager.Instance.SetAiBehaviour += SetupEnemyTypes;
        GameManager.Instance.OnStartGame += StartMainGameplay;
    }

    private void Start()
    {
        playerMovementScript = playerCharacter.GetComponent<FirstPersonMovement>();
    }

    public void SetupAsymmetricalGameplay()
    {
        InputManager.Instance.OnEnable();
        TrainingCourseManager.Instance.DisableTrainingCourseManager();

        playerCharacter.transform.position = startingPosition.position;

        enemyOneStartingPosition.gameObject.SetActive(true);
        enemyTwoStartingPosition.gameObject.SetActive(true);
        enemyThreeStartingPosition.gameObject.SetActive(true);

        GameManager.Instance.OnSetAiBehaviour();
    }

    public void StartMainGameplay()
    {
        if (TrainingCourseManager.Instance.isTrainingComplete)
        {
            mainGameTutorialText.SetActive(false);
            redDoor.SetActive(true);
        }
    }

    public void OnGameOver()
    {
        gameOverScreen.gameObject.SetActive(true);
        redDoor.SetActive(false);
    }

    #region Setting up and creating enemies

    public void SetupEnemyTypes()
    {
        Debug.Log("Creating enemies now!");

        if (enemyNumberID < 3)
        {
            createEnemies = true;
        }
        else if (enemyNumberID == 3)
        {
            createEnemies = false;
            Debug.Log("asymmGameplayManager: Created three enemies.");
        }
    }

    private void SetEnemyTypes()
    {
        #region Training Course 1

        if (enemyNumberID == 0)
        {
            if (TrainingCourseManager.Instance.trainingCourseOneParTimeAchieved && TrainingCourseManager.Instance.hasPlayerHitAllCourseOneTargets)
            {
                //Fast and aggressive enemy type
                setEnemyAggressive = true;
                setEnemyDefensive = false;
                setEnemyFast = true;
                setEnemySlow = false;
                CreateEnemy(fastAggressiveEnemyType);
            }
            else if (TrainingCourseManager.Instance.trainingCourseOneParTimeAchieved && !TrainingCourseManager.Instance.hasPlayerHitAllCourseOneTargets)
            {
                //Fast and defensive enemy type
                setEnemyAggressive = false;
                setEnemyDefensive = true;
                setEnemyFast = true;
                setEnemySlow = false;
                CreateEnemy(fastDefensiveEnemyType);
            }
            else if (!TrainingCourseManager.Instance.trainingCourseOneParTimeAchieved && TrainingCourseManager.Instance.hasPlayerHitAllCourseOneTargets)
            {
                //Slow and aggressive enemy type
                setEnemyAggressive = true;
                setEnemyDefensive = false;
                setEnemyFast = false;
                setEnemySlow = true;
                CreateEnemy(slowAggressiveEnemyType);
            }
            else if (!TrainingCourseManager.Instance.trainingCourseOneParTimeAchieved && !TrainingCourseManager.Instance.hasPlayerHitAllCourseOneTargets)
            {
                //Slow and defensive enemy type
                setEnemyAggressive = false;
                setEnemyDefensive = true;
                setEnemyFast = false;
                setEnemySlow = true;
                CreateEnemy(slowDefensiveEnemyType);
            }
        }

        #endregion

        #region Training Course 2

        else if (enemyNumberID == 1)
        {
            if (TrainingCourseManager.Instance.trainingCourseTwoParTimeAchieved && TrainingCourseManager.Instance.hasPlayerHitAllCourseTwoTargets)
            {
                //Fast and aggressive enemy type
                setEnemyAggressive = true;
                setEnemyDefensive = false;
                setEnemyFast = true;
                setEnemySlow = false;
                CreateEnemy(fastAggressiveEnemyType);
            }
            else if (TrainingCourseManager.Instance.trainingCourseTwoParTimeAchieved && !TrainingCourseManager.Instance.hasPlayerHitAllCourseTwoTargets)
            {
                //Fast and defensive enemy type
                setEnemyAggressive = false;
                setEnemyDefensive = true;
                setEnemyFast = true;
                setEnemySlow = false;
                CreateEnemy(fastDefensiveEnemyType);
            }
            else if (!TrainingCourseManager.Instance.trainingCourseTwoParTimeAchieved && TrainingCourseManager.Instance.hasPlayerHitAllCourseTwoTargets)
            {
                //Slow and aggressive enemy type
                setEnemyAggressive = true;
                setEnemyDefensive = false;
                setEnemyFast = false;
                setEnemySlow = true;
                CreateEnemy(slowAggressiveEnemyType);
            }
            else if (!TrainingCourseManager.Instance.trainingCourseTwoParTimeAchieved && !TrainingCourseManager.Instance.hasPlayerHitAllCourseTwoTargets)
            {
                //Slow and defensive enemy type
                setEnemyAggressive = false;
                setEnemyDefensive = true;
                setEnemyFast = false;
                setEnemySlow = true;
                CreateEnemy(slowDefensiveEnemyType);
            }
        }
        
        #endregion

        #region Training Course 3

        else if (enemyNumberID == 2)
        {
            if (TrainingCourseManager.Instance.trainingCourseThreeParTimeAchieved && TrainingCourseManager.Instance.hasPlayerHitAllCourseThreeTargets)
            {
                //Fast and aggressive enemy type
                setEnemyAggressive = true;
                setEnemyDefensive = false;
                setEnemyFast = true;
                setEnemySlow = false;
                CreateEnemy(fastAggressiveEnemyType);
            }
            else if (TrainingCourseManager.Instance.trainingCourseThreeParTimeAchieved && !TrainingCourseManager.Instance.hasPlayerHitAllCourseThreeTargets)
            {
                //Fast and defensive enemy type
                setEnemyAggressive = false;
                setEnemyDefensive = true;
                setEnemyFast = true;
                setEnemySlow = false;
                CreateEnemy(fastDefensiveEnemyType);
            }
            else if (!TrainingCourseManager.Instance.trainingCourseThreeParTimeAchieved && TrainingCourseManager.Instance.hasPlayerHitAllCourseThreeTargets)
            {
                //Slow and aggressive enemy type
                setEnemyAggressive = true;
                setEnemyDefensive = false;
                setEnemyFast = false;
                setEnemySlow = true;
                CreateEnemy(slowAggressiveEnemyType);
            }
            else if (!TrainingCourseManager.Instance.trainingCourseThreeParTimeAchieved && !TrainingCourseManager.Instance.hasPlayerHitAllCourseThreeTargets)
            {
                //Slow and defensive enemy type
                setEnemyAggressive = false;
                setEnemyDefensive = true;
                setEnemyFast = false;
                setEnemySlow = true;
                CreateEnemy(slowDefensiveEnemyType);
            }
        }

        #endregion

        #region Previous System 

        //if (TrainingCourseManager.Instance.hasPlayerHitAllTargets && TrainingCourseManager.Instance.didPlayerAchieveAboveAverageScore && !TrainingCourseManager.Instance.didPlayerAchieveBelowAverageScore && !TrainingCourseManager.Instance.hasPlayerAchievedParTimeForAllCourses)
        //{
        //    //Set enemy to aggressive and explorative
        //    setEnemyAggressive = true;
        //    setEnemyExplorative = true;
        //    CreateEnemy(aggressiveEnemyType);
        //}
        //else if (!TrainingCourseManager.Instance.hasPlayerHitAllTargets && !TrainingCourseManager.Instance.didPlayerAchieveAboveAverageScore && TrainingCourseManager.Instance.didPlayerAchieveBelowAverageScore && TrainingCourseManager.Instance.hasPlayerAchievedParTimeForAllCourses)
        //{
        //    //Set enemy to defensive and explorative
        //    setEnemyAggressive = false;
        //    setEnemyExplorative = true;
        //    setEnemyDefensive = true;
        //    CreateEnemy(defensiveExplorativeEnemyType);
        //}
        //else if (TrainingCourseManager.Instance.hasPlayerHitAllTargets && TrainingCourseManager.Instance.hasPlayerAchievedParTimeForAllCourses)
        //{
        //    //Set enemy to defensive and explorative
        //    setEnemyAggressive = false;
        //    setEnemyDefensive = true;
        //    setEnemyExplorative = true;
        //    CreateEnemy(aggressiveExplorativeEnemyType);
        //}
        //else if (!TrainingCourseManager.Instance.hasPlayerHitAllTargets && TrainingCourseManager.Instance.hasPlayerAchievedParTimeForAllCourses)
        //{
        //    //Set enemy to explorative
        //    setEnemyAggressive = false;
        //    setEnemyDefensive = false;
        //    setEnemyExplorative = true;
        //    CreateEnemy(explorativeEnemyType);
        //}
        //else if (!TrainingCourseManager.Instance.hasPlayerHitAllTargets && !TrainingCourseManager.Instance.hasPlayerAchievedParTimeForAllCourses)
        //{
        //    //Set enemy to explorative
        //    setEnemyAggressive = false;
        //    setEnemyDefensive = false;
        //    setEnemyExplorative = true;
        //    CreateEnemy(explorativeEnemyType);
        //}

        #endregion
    }

    private void CreateEnemy(GameObject enemy)
    {
        enemyNumberID++;

        if (enemyNumberID == 1)
        {
            Instantiate(enemy, enemyOneStartingPosition.position, Quaternion.identity, enemyParentTransform); 
            enemyList.Add(enemy);

            setEnemyAggressive = false;
            setEnemyDefensive = false;
            setEnemyFast= false;
            setEnemySlow= false;
            //setEnemyExplorative = false;

            Debug.Log("First enemy created.");

            enemyOneStartingPosition.gameObject.SetActive(false);
        }
        else if (enemyNumberID == 2)
        {
            Instantiate(enemy, enemyTwoStartingPosition.position, Quaternion.identity, enemyParentTransform); 
            enemyList.Add(enemy);
            setEnemyAggressive = false;
            setEnemyDefensive = false;
            setEnemyFast = false;
            //setEnemyExplorative = false;

            Debug.Log("Second enemy created.");

            enemyTwoStartingPosition.gameObject.SetActive(false);
        }
        else if (enemyNumberID == 3)
        {
            Instantiate(enemy, enemyThreeStartingPosition.position, Quaternion.identity, enemyParentTransform);
            enemyList.Add(enemy);
            setEnemyAggressive = false;
            setEnemyDefensive = false;
            setEnemySlow= false;
            //setEnemyExplorative = false;

            Debug.Log("Third enemy created.");

            enemyThreeStartingPosition.gameObject.SetActive(false);
        }
    }

    #endregion

    private void Update()
    {
        if (createEnemies && enemyNumberID != 3) 
        {
            SetEnemyTypes();
        }

        //if (InputManager.Instance.PlayerStartedMainGame())
        //{
        //    StartMainGameplay();
        //}
    }
}
