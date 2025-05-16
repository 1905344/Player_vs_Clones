using System;
using UnityEngine;
using UnityEngine.AI;

public class P3_EnemyBase : MonoBehaviour
{
    #region Variables

    [SerializeField] public Guid enemyID;
    public string GetEnemyID()
    {
        return enemyID.ToString();
    }

    [Space(5)]

    [Header("Health Variables")]
    [SerializeField] public int defaultHealth = 100;
    public int maxHealth;

    [Space(10)]

    [SerializeField] public P3_Enemy_Types enemyType;
    [SerializeField, Tooltip("How much damage to deal to the lighthouse")] float damageAmount;

    [Space(5)]

    [Header("Timers")]
    [SerializeField, Range(0.1f,1f)] private float waitTimer = 0.3f;
    private float timer = 0f;
    private bool startDeathTimer = false;

    [Space(10)]

    [Header("Visual Feedback References")]
    [SerializeField] private GameObject explosionPrefab;

    [Header("External References")]
    [SerializeField] NavMeshAgent meshAgent;
    [SerializeField] private CapsuleCollider enemyCollider;
    public GameObject playerRef { get; set; }
    public Transform lighthouseRef { get; set; }
    private Transform destinationPosition;
    private Vector3 destinationVector;

    private GameObject explosion;

    [Space(10)]

    [Header("Sound Effects")]
    [SerializeField] private AudioClip enemyInjuredSFX;
    [SerializeField] private AudioClip enemyDeathSFX;
    [SerializeField] private AudioClip explosionSFX;

    [Space(10)]

    [Header("Debug")]
    [SerializeField] private bool startMoving = false;

    private static Guid GenerateGuid()
    {
        return Guid.NewGuid();
    }

    #endregion

    private void Awake()
    {
        enemyID = GenerateGuid();
        maxHealth = defaultHealth;
    }

    void Start()
    {
        P3_GameManager.Instance.EnemyHit += TakeDamage;

        GetDestination();
        startMoving = true;
        EnemyMovement();
    }

    public void TakeDamage(Guid id, int damage)
    {
        if (id != enemyID)
        {
            return;
        }

        SoundManager.instance.PlaySFX(enemyInjuredSFX);

        defaultHealth -= damage;

        if (P3_GameManager.Instance.enableDebug)
        {
            Debug.Log("Enemy " + enemyID + " has been hit." + defaultHealth + " health remaining.");
        }

        if (defaultHealth <= 0)
        {
            CheckType();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemyType == P3_Enemy_Types.Red)
        {
            if (other.CompareTag("Lighthouse"))
            {
                ExplodeEnemy();
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                P3_GameManager.Instance.OnPlayerKilled();
            }
        }
    }

    private void CheckType()
    {
        switch (enemyType)
        {
            case P3_Enemy_Types.Green: 
            {
                P3_GameManager.Instance.OnGreenEnemyKilled();
                DestroyThisEnemy();
                    break;
            }
            case P3_Enemy_Types.Blue:
            {
                P3_GameManager.Instance.OnBlueEnemyKilled();
                DestroyThisEnemy();
                break;
            }
            case P3_Enemy_Types.Yellow: 
            {
                P3_GameManager.Instance.OnYellowEnemyKilled();
                DestroyThisEnemy();
                break;
            }
        }

        startDeathTimer = true;
    }

    #region Set Enemy Destination and Enemy Movement

    private void GetDestination()
    {
        if (enemyType == P3_Enemy_Types.Red)
        {
            if (lighthouseRef.transform != null)
            {
                destinationPosition = lighthouseRef.transform;
            }
            else
            {
                Transform getLighthouseTransform = GameObject.FindGameObjectWithTag("Lighthouse").transform;
                destinationPosition = getLighthouseTransform;
            }
            
            //Debug.Log($"P3_EnemyBase: Enemy type is: {enemyType} and lighthouse destination is: {destinationPosition.position}");
        }
        else
        {
            destinationPosition = playerRef.transform;
        }
    }

    private void EnemyMovement()
    {
        meshAgent.SetDestination(destinationVector);
    }

    #endregion

    #region Enemy Death Functions

    private void DestroyThisEnemy()
    {
        if (enemyType != P3_Enemy_Types.Red)
        {
            SoundManager.instance.PlaySFX(enemyDeathSFX);
        }

        #region Debug

        if (P3_GameManager.Instance.enableDebug)
        {
            Debug.Log("Enemy " + enemyID + " destroyed.");
        }

        #endregion

        timer = 0f;
        startDeathTimer = false;

        //P3_GameManager.Instance.Instance.RemoveEnemy(this.gameObject);
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    private void ExplodeEnemy()
    {
        if (enemyType != P3_Enemy_Types.Red)
        {
            return;
        }

        SoundManager.instance.PlaySFX(explosionSFX);

        P3_GameManager.Instance.OnLighthouseHit(damageAmount);
        explosion = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        startDeathTimer = true;
    }

    #endregion

    void Update()
    {
        if (startMoving && !startDeathTimer)
        {
            destinationVector = destinationPosition.position;
            EnemyMovement();
        }

        if (startDeathTimer)
        {
            timer += Time.deltaTime;

            if (timer > waitTimer)
            {
                Destroy(explosion);
                DestroyThisEnemy();
            }
        }
    }
}