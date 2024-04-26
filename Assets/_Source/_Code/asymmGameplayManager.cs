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

    [Space(15)]

    [Header("Enemies")]
    [SerializeField] private GameObject aggressiveEnemyType;
    [SerializeField] private GameObject aggressiveExplorativeEnemyType;
    [SerializeField] private GameObject defensiveEnemyType;
    [SerializeField] private GameObject defensiveExplorativeEnemyType;
    [SerializeField] private GameObject explorativeEnemyType;

    [Space(10)]

    [Header("Enemy Starting Positions")]
    [SerializeField] private Transform enemyOneStartingPosition;
    [SerializeField] private Transform enemyTwoStartingPosition;
    [SerializeField] private Transform enemyThreeStartingPosition;

    [Space(15)]

    [SerializeField] private bool setEnemyAggressive = false;
    [SerializeField] private bool setEnemyDefensive = false;
    [SerializeField] private bool setEnemyExplorative = false;

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
    }

    private void Start()
    {
        GameManager.Instance.FinishedTraining += SetupAsymmetricalGameplay;
        GameManager.Instance.SetAiBehaviour += SetupEnemyTypes;
        playerMovementScript = playerCharacter.GetComponent<FirstPersonMovement>();
    }

    private void SetupAsymmetricalGameplay()
    {
        InputManager.Instance.OnEnable();
        TrainingCourseManager.Instance.DisableTrainingCourseManager();
    }

    private void SetupEnemyTypes()
    {
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
        if (TrainingCourseManager.Instance.hasPlayerHitAllTargets && TrainingCourseManager.Instance.didPlayerAchieveAboveAverageScore && !TrainingCourseManager.Instance.didPlayerAchieveBelowAverageScore && !TrainingCourseManager.Instance.hasPlayerAchievedParTimeForAllCourses)
        {
            setEnemyAggressive = true;
            setEnemyExplorative = true;
            CreateEnemy(aggressiveEnemyType);
        }
        else if (!TrainingCourseManager.Instance.hasPlayerHitAllTargets && !TrainingCourseManager.Instance.didPlayerAchieveAboveAverageScore && TrainingCourseManager.Instance.didPlayerAchieveBelowAverageScore && TrainingCourseManager.Instance.hasPlayerAchievedParTimeForAllCourses)
        {
            setEnemyAggressive = false;
            setEnemyExplorative = true;
            setEnemyDefensive = true;
            CreateEnemy(defensiveExplorativeEnemyType);
        }
        else if (TrainingCourseManager.Instance.hasPlayerHitAllTargets && TrainingCourseManager.Instance.hasPlayerAchievedParTimeForAllCourses)
        {
            setEnemyAggressive = false;
            setEnemyDefensive = true;
            setEnemyExplorative = true;
            CreateEnemy(aggressiveExplorativeEnemyType);
        }
        else if (!TrainingCourseManager.Instance.hasPlayerHitAllTargets && TrainingCourseManager.Instance.hasPlayerAchievedParTimeForAllCourses)
        {
            setEnemyAggressive = false;
            setEnemyDefensive = false;
            setEnemyExplorative = true;
            CreateEnemy(aggressiveExplorativeEnemyType);
        }
        else if (!TrainingCourseManager.Instance.hasPlayerHitAllTargets && !TrainingCourseManager.Instance.hasPlayerAchievedParTimeForAllCourses)
        {
            setEnemyAggressive = false;
            setEnemyDefensive = false;
            setEnemyExplorative = true;
            CreateEnemy(explorativeEnemyType);
        }
    }

    private void CreateEnemy(GameObject enemy)
    {
        enemyNumberID++;

        if (enemyNumberID == 1)
        {
            Instantiate(enemy, enemyOneStartingPosition.position, Quaternion.identity); 
            enemyList.Add(enemy);

            setEnemyAggressive = false;
            setEnemyDefensive = false;
            setEnemyExplorative = false;
        }
        else if (enemyNumberID == 2)
        {
            Instantiate(enemy, enemyTwoStartingPosition.position, Quaternion.identity); 
            enemyList.Add(enemy);
            setEnemyAggressive = false;
            setEnemyDefensive = false;
            setEnemyExplorative = false;

        }
        else if (enemyNumberID == 3)
        {
            Instantiate(enemy, enemyThreeStartingPosition.position, Quaternion.identity);
            enemyList.Add(enemy);
            setEnemyAggressive = false;
            setEnemyDefensive = false;
            setEnemyExplorative = false;
        }
    }

    private void Update()
    {
        if (createEnemies && enemyNumberID != 3) 
        {
            SetEnemyTypes();
        }
    }
}
