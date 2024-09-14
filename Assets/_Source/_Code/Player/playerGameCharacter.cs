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

    #endregion

    private void Awake()
    {
        isAlive = true;
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.tag)
    //    {
            
    //    }
    //    OnPlayerDeath();
    //    //OnPlayerHit();
    //}

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
            Debug.Log("Player has been killed!");
            Application.Quit();
        }
    }
}
