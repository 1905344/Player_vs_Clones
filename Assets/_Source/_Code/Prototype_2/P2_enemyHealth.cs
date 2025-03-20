using System;
using UnityEngine;

public class P2_enemyHealth : MonoBehaviour
{
    #region Variables

    [Header("Health Variables")]
    [SerializeField] public int health = 100;
    public int maxHealth;
    [SerializeField] private P2_Healthbar healthBar;

    [Space(10)]

    [SerializeField] public Guid enemyID;

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
        P2_GameManager.Instance.EnemyHit += TakeDamage;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(Guid id, int damage)
    {
        if (id != enemyID)
        {
            return;
        }

        //SoundManager.instance.PlaySFX(enemyInjuredSFX);

        health -= damage;

        if (P2_GameManager.Instance.enableDebug)
        {
            Debug.Log("Enemy " + enemyID + " has been hit." + health + " health remaining.");
        }

        if (health <= 0)
        {
            Invoke(nameof(DestroyThisEnemy), 0.5f);
        }

        healthBar.SetCurrentHealth(health);
    }

    private void DestroyThisEnemy()
    {
        //SoundManager.instance.PlaySFX(enemyDeathSFX);

        if (P2_GameManager.Instance.enableDebug)
        {
            Debug.Log("Enemy " + enemyID + " destroyed.");
        }

        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
