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
    //[SerializeField] private GameObject explosionParent;

    //[Space(10)]

    //[SerializeField] private int healthDropAmount;

    [Header("External References")]
    [SerializeField] NavMeshAgent meshAgent;
    public GameObject playerRef { get; set; }
    public GameObject lighthouseRef { get; set; }

    [Space(5)]

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

        startMoving = true;
    }

    void Start()
    {
        P3_GameManager.Instance.EnemyHit += TakeDamage;
    }

    public void TakeDamage(Guid id, int damage)
    {
        if (id != enemyID)
        {
            return;
        }

        //SoundManager.instance.PlaySFX(enemyInjuredSFX);

        defaultHealth -= damage;

        if (P3_GameManager.Instance.enableDebug)
        {
            Debug.Log("Enemy " + enemyID + " has been hit." + defaultHealth + " health remaining.");
        }

        if (defaultHealth <= 0)
        {
            //Invoke(nameof(DestroyThisEnemy), 0.5f);
            //startDeathTimer = true;
            CheckType();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enemyType == P3_Enemy_Types.Red)
        {
            if (collision.gameObject.CompareTag("Lighthouse"))
            {
                P3_GameManager.Instance.OnLighthouseHit(damageAmount);
                ExplodeEnemy();
            }
        }
        else
        {
            if (collision.gameObject.CompareTag("Player"))
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

    private void EnemyMovement()
    {
        if (enemyType == P3_Enemy_Types.Red)
        {
            meshAgent.SetDestination(lighthouseRef.transform.position);
        }
        else
        {
            meshAgent.SetDestination(playerRef.transform.position);
        }
    }

    #region Enemy Death Functions

    private void DestroyThisEnemy()
    {
        //SoundManager.instance.PlaySFX(enemyDeathSFX);

        #region Debug

        if (P3_GameManager.Instance.enableDebug)
        {
            Debug.Log("Enemy " + enemyID + " destroyed.");
        }

        #endregion

        timer = 0f;
        startDeathTimer = false;

        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    private void ExplodeEnemy()
    {
        if (enemyType != P3_Enemy_Types.Red)
        {
            return;
        }

        //Instantiate(explosionPrefab, explosionParent.transform, this.transform);
        Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        startDeathTimer = true;
    }

    #endregion

    void Update()
    {
        if (startMoving)
        {
            EnemyMovement();
        }

        if (startDeathTimer)
        {
            timer += Time.deltaTime;

            if (timer > waitTimer)
            {
                DestroyThisEnemy();
            }
        }
    }
}