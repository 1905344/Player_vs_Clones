using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class playerGameCharacter : MonoBehaviour
{
    #region Variables

    [SerializeField] private int health;
    [SerializeField] private bool isAlive;
    //[SerializeField] private 

    [Space(10)]

    [SerializeField] private int enemyBulletDamageAmount;

    #endregion

    private void Awake()
    {
        isAlive = true;
    }

    private void Start()
    {
        GameManager.Instance.PlayerHit += OnPlayerHit;
    }

    public void OnPlayerHit(int damage)
    {
        if (!isAlive)
        {
            return;
        }

        health -= damage;

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Player has been hit for " + damage + " damage!");
            Debug.Log("Player has " + health + " health remaining.");
        }

        if (health <= 0)
        {
            OnPlayerDeath();
        }
    }

    private void OnPlayerDeath()
    {
        if (isAlive)
        {
            isAlive = false;
            asymmGameplayManager.Instance.OnGameOver();

            GameManager.Instance.OnPlayerKilled();

            if (GameManager.Instance.toggleDebug)
            {
                Debug.Log("Player has been killed.");
            }

            //Application.Quit();
        }
    }
}
