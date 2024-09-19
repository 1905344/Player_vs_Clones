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

    [SerializeField] private Collider playerCollider;

    [Space(10)]

    [SerializeField] private int enemyBulletDamageAmount;

    #endregion

    private void Awake()
    {
        isAlive = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "bullet")
        {
            OnPlayerHit(enemyBulletDamageAmount);
        }
    }

    private void OnPlayerHit(int damage)
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
            Debug.Log("Player has been killed!");
            //Application.Quit();
        }
    }
}
