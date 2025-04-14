using System;
using UnityEngine;
using UnityEngine.AI;
public class Prototype_1_EnemyAiController1 : MonoBehaviour
{
    #region Variables

    private NavMeshAgent meshAgent;
    private Transform playerCharacter;
    [SerializeField] public LayerMask groundLayerMask;
    [SerializeField] public LayerMask playerLayerMask;
    [SerializeField] private float enemyHealth = 30f;

    //For Patrolling State
    [SerializeField] public Transform walkPointTransform;
    [SerializeField] private Vector3 walkPoint;
    [SerializeField] private bool isWalkPointSet;
    [SerializeField] public float walkPointRange;

    //For Attacking State
    [SerializeField] public float timeBetweenAttacks;
    [SerializeField] private bool hasAttackedAlready;

    //States
    [SerializeField] public float aiSightRange;
    [SerializeField] public float aiAttackRange;
    [SerializeField] public bool isPlayerInSightRange;
    [SerializeField] public bool isPlayerInAttackRange;

    [SerializeField] private AudioClip attackSFX;
    [SerializeField] private AudioClip playerDiscoveredSFX;
    [SerializeField] private AudioClip lostSightOfPlayerSFX;
    [SerializeField] private AudioClip enemyInjuredSFX;
    [SerializeField] private AudioClip enemyDeathSFX;

    //Placeholder shooting
    [SerializeField] public GameObject projectile;
    [SerializeField] private int bulletsFired = 0;
    [SerializeField] private int bulletDamage = 10;

    [Space(10)]

    [Header("Debug")]
    [SerializeField] private bool showSightRange;
    [SerializeField] private bool showAttackRange;
    [SerializeField] private bool stopShooting = false;

    [SerializeField] public Guid enemyID;

    private static Guid GenerateGuid()
    {
        return Guid.NewGuid();
    }

    #endregion

    private void Awake()
    {
        playerCharacter = GameObject.Find("Player").transform;
        meshAgent = GetComponent<NavMeshAgent>();

        walkPoint = new Vector3(walkPointTransform.position.x, walkPointTransform.position.y, walkPointTransform.position.z);

        enemyID = GenerateGuid();
    }

    private void Start()
    {
        //Prototype_1_GameManager.Instance.LevelCompleted += DestroyThisEnemy;
        //Prototype_1_GameManager.Instance.LevelFailed += DestroyThisEnemy;

        Prototype_1_GameManager.Instance.EnemyHit += TakeDamage;
        Prototype_1_GameManager.Instance.PlayerKilled += StopAttacking;

        #region Debugging

        if (Prototype_1_GameManager.Instance.enableDebug)
        {
            Debug.Log("Enemy " + enemyID + " active.");
        }

        #endregion
    }

    #region States

    private void Patrolling()
    {
        if (!isWalkPointSet)
        {
            SearchForWalkPoint();
        }
        else
        {
            meshAgent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            isWalkPointSet = false;
        }
    }

    private void Chasing()
    {
        //SoundManager.instance.PlaySFX(playerDiscoveredSFX);

        meshAgent.SetDestination(playerCharacter.position);
    }

    private void Attacking()
    {
        //Prevent enemy from moving

        meshAgent.SetDestination(transform.position);

        transform.LookAt(playerCharacter);

        if (!hasAttackedAlready)
        {
            bulletsFired++;

            //Attacking code - currently this is a placeholder
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();

            rb.gameObject.GetComponent<projectileScript>().projectileID = bulletsFired;
            rb.gameObject.GetComponent<projectileScript>().damage = bulletDamage;

            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 2f, ForceMode.Impulse);

            //Debug.DrawLine(transform.forward, projectile.transform.position, Color.red, 5f);

            hasAttackedAlready = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void StopAttacking()
    {
        stopShooting = true;
        hasAttackedAlready = true;
        timeBetweenAttacks = 9999f;
    }

    #endregion

    #region Functions for states

    private void SearchForWalkPoint()
    {
        //SoundManager.instance.PlaySFX(lostSightOfPlayerSFX);

        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayerMask))
        {
            isWalkPointSet = true;
        }
    }

    private void ResetAttack()
    {
        hasAttackedAlready = false;
    }

    #endregion

    #region Take Damage and Destroy Enemy

    public void TakeDamage(Guid id, int damage)
    {
        if (id != enemyID)
        {
            return;
        }

        //SoundManager.instance.PlaySFX(enemyInjuredSFX);

        enemyHealth -= damage;

        if (Prototype_1_GameManager.Instance.enableDebug)
        {
            Debug.Log("Enemy " + enemyID + " has been hit." + enemyHealth + " health remaining.");
        }

        if (enemyHealth <= 0)
        {
            Invoke(nameof(DestroyThisEnemy), 0.5f);
        }
    }

    private void DestroyThisEnemy()
    {
        //SoundManager.instance.PlaySFX(enemyDeathSFX);

        if (Prototype_1_GameManager.Instance.enableDebug)
        {
            Debug.Log("Enemy " + enemyID + " destroyed.");
        }

        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    #endregion

    #region Debugging: Showing Gizmos

    private void OnDrawGizmosSelected()
    {
        //Visualing the attack and sight range of the enemy A.I.

        if (showAttackRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aiAttackRange);

            showAttackRange = false;
        }
        else if (showSightRange)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, aiSightRange);

            showSightRange = false;
        }
        else if (showAttackRange && showSightRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aiAttackRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, aiSightRange);

            showAttackRange = false;
            showSightRange = false;
        }
    }

    #endregion

    private void Update()
    {
        isPlayerInSightRange = Physics.CheckSphere(transform.position, aiSightRange, playerLayerMask);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, aiAttackRange, playerLayerMask);

        if (!stopShooting)
        {
            if (!isPlayerInSightRange && !isPlayerInAttackRange)
            {
                Patrolling();
            }

            if (isPlayerInSightRange && !isPlayerInAttackRange)
            {
                Chasing();
            }

            if (isPlayerInAttackRange && isPlayerInSightRange)
            {
                Attacking();
            }
        }
        else
        {
            return;
        }

        #region Debugging

        if (Prototype_1_GameManager.Instance.enableDebug && (showSightRange || showAttackRange))
        {
            OnDrawGizmosSelected();
        }

        #endregion
    }
}
