using UnityEngine;

public class P2_enemyAttack : MonoBehaviour
{
    #region Variables

    private Transform playerCharacter;

    [Header("Attack Variables")]
    [SerializeField] public float timeBetweenAttacks;
    [SerializeField] private bool hasAttackedAlready = false;
    [SerializeField] private float attackTimer = 0f;
    [SerializeField] private bool stopShooting = false;

    [Space(5)]

    [SerializeField] public GameObject projectile;
    [SerializeField] private int bulletsFired = 0;
    [SerializeField] private int bulletDamage = 10;

    [Space(10)]

    [Header("SFX")]
    [SerializeField] private AudioClip attackSFX;
    [SerializeField] private AudioClip lostSightOfPlayerSFX;
    [SerializeField] private AudioClip enemyInjuredSFX;
    [SerializeField] private AudioClip enemyDeathSFX;

    public bool isAttacking { get; set; } = false;

    public bool GetAtackStatus()
    {
        return isAttacking;
    }

    #endregion

    private void Awake()
    {
        playerCharacter = P2_PlayerManager.Instance.GetCurrentlyActivePlayer().transform;
    }

    void Start()
    {
        P2_GameManager.Instance.PlayerKilled += StopAttacking;
    }

    private void Attacking()
    {
        //Prevent enemy from moving
        //meshAgent.SetDestination(transform.position);

        transform.LookAt(playerCharacter);

        Debug.Log($"hasAttackedAlready {hasAttackedAlready}");

        if (attackTimer > timeBetweenAttacks)
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
            attackTimer = 0f;
        }

        attackTimer += Time.deltaTime;
    }

    public void StopAttacking()
    {
        stopShooting = true;
        hasAttackedAlready = true;
        //timeBetweenAttacks = 9999f;

        isAttacking = false;
    }


    void Update()
    {
        Debug.Log($"Is Attacking{isAttacking}");
        Debug.Log($"Stop Shooting{stopShooting}");

        if (isAttacking)
        {
            stopShooting = false;
            Attacking();
        }
        else
        {
            StopAttacking();
        }
    }
}
