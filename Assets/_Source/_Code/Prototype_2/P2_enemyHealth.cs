using System;
using UnityEngine;

public class P2_enemyHealth : MonoBehaviour
{
    #region Variables

    [Header("Health Variables")]
    [SerializeField] private float health = 100f;


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
    }

    void Start()
    {
        P2_GameManager.Instance.EnemyHit += TakeDamage;
    }

    public void TakeDamage(Guid id, int damage)
    {
        if (id != enemyID)
        {
            return;
        }

        //SoundManager.instance.PlaySFX(enemyInjuredSFX);

        health -= damage;

        if (GameManager.Instance.toggleDebug)
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

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Enemy " + enemyID + " destroyed.");
        }

        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
