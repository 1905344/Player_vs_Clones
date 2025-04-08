using JetBrains.Annotations;
using System;
using Unity.VisualScripting;
using UnityEngine;

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
    [SerializeField] public int health = 100;
    public int maxHealth;

    [Space(10)]

    [SerializeField] public P3_Enemy_Types enemyType;
    [SerializeField] float damageAmount;

    [SerializeField, Range(0.1f,1f)] private float waitTimer = 0.3f;
    private float timer = 0f;
    private bool startDeathTimer = false;

    [Space(10)]

    [Header("Enemy Type Booleans")]
    [SerializeField] private bool isRed = false;
    [SerializeField] private bool isYellow = false;
    [SerializeField] private bool isBlue = false;
    [SerializeField] private bool isGreen = false;

    private static Guid GenerateGuid()
    {
        return Guid.NewGuid();
    }

    #endregion

    private void Awake()
    {
        enemyID = GenerateGuid();
        maxHealth = health;
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

        health -= damage;

        if (P3_GameManager.Instance.enableDebug)
        {
            Debug.Log("Enemy " + enemyID + " has been hit." + health + " health remaining.");
        }

        if (health <= 0)
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
                DestroyThisEnemy();
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
                break;
            }
            case P3_Enemy_Types.Blue:
            {
                P3_GameManager.Instance.OnBlueEnemyKilled();
                break;
            }
            case P3_Enemy_Types.Yellow: 
            {
                P3_GameManager.Instance.OnYellowEnemyKilled();
                break;
            }
        }

        startDeathTimer = true;
    }

    private void DestroyThisEnemy()
    {
        //SoundManager.instance.PlaySFX(enemyDeathSFX);

        if (P3_GameManager.Instance.enableDebug)
        {
            Debug.Log("Enemy " + enemyID + " destroyed.");
        }

        timer = 0f;
        startDeathTimer = false;

        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    void Update()
    {
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