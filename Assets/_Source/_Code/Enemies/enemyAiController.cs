using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class enemyAiController : MonoBehaviour
{
    #region Variables

    private NavMeshAgent meshAgent;
    private Transform playerCharacter;
    [SerializeField] public LayerMask groundLayerMask;
    [SerializeField] public LayerMask playerLayerMask;
    [SerializeField] private float enemyHealth;

    //For Patrolling State
    [SerializeField] public Vector3 walkPoint;
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
    [SerializeField] private List<GameObject> projectilesList;
    [SerializeField] private int projectileLimit = 2;
    [SerializeField] private int bulletsFired;
    [SerializeField] private int bulletDamage = 10;

    #endregion

    private void Awake()
    {
        playerCharacter = GameObject.Find("Player").transform;
        meshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        //GameManager.Instance.LevelCompleted += DestroyThisEnemy;
        //GameManager.Instance.LevelFailed += DestroyThisEnemy;

        GameManager.Instance.EnemyHit += TakeDamage;
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
        SoundManager.instance.PlaySFX(playerDiscoveredSFX);

        meshAgent.SetDestination(playerCharacter.position);
    }

    private void Attacking()
    {
        //Prevent enemy from moving

        meshAgent.SetDestination(transform.position);

        transform.LookAt(playerCharacter);

        if (!hasAttackedAlready)
        {
            //Attacking code - currently this is a placeholder
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();

            //Attempting to remove the first projectiles
            if (projectilesList.Count < projectileLimit)
            {
                projectilesList.Add(rb.gameObject);
            }
            else
            {
                Destroy(projectilesList.First());
                projectilesList.Add(rb.gameObject);
            }

            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 2f, ForceMode.Impulse);

            //Debug.DrawLine(transform.forward, projectile.transform.position, Color.red, 5f);

            hasAttackedAlready = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    #endregion

    #region Functions for states

    private void SearchForWalkPoint()
    {
        SoundManager.instance.PlaySFX(lostSightOfPlayerSFX);

        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

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

    private void TakeDamage(int damage)
    {
        SoundManager.instance.PlaySFX(enemyInjuredSFX);

        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            Invoke(nameof(DestroyThisEnemy), 0.5f);
        }
    }

    private void DestroyThisEnemy()
    {
        SoundManager.instance.PlaySFX(enemyDeathSFX);

        Destroy(gameObject);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        //Visualing the attack and sight range of the enemy A.I.

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aiAttackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aiSightRange);
    }

    private void Update()
    {
        isPlayerInSightRange = Physics.CheckSphere(transform.position, aiSightRange, playerLayerMask);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, aiAttackRange, playerLayerMask);

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
}
