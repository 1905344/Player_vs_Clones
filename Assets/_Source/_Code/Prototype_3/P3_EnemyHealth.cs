using System;
using UnityEngine;

public class P3_EnemyHealth : MonoBehaviour
{
    #region Variables

    [Header("Health Variables")]
    [SerializeField] public int health = 100;
    public int maxHealth;

    [Space(10)]

    [SerializeField] public Guid enemyID;

    [Space(5)]

    [SerializeField] private P3_EnemyBase baseScript;

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
            Invoke(nameof(DestroyThisEnemy), 0.5f);
        }
    }

    private void DestroyThisEnemy()
    {
        //SoundManager.instance.PlaySFX(enemyDeathSFX);

        if (P3_GameManager.Instance.enableDebug)
        {
            Debug.Log("Enemy " + enemyID + " destroyed.");
        }

        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
