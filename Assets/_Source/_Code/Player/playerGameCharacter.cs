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

    [SerializeField] private CapsuleCollider bodyCollider;
    [SerializeField] private BoxCollider headCollider;

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
