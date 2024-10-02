using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class projectileScript : MonoBehaviour
{
    #region Variables

    public int projectileID;
    public int damage { get; set; }
    
    [SerializeField] private float projectileLifespan = 10f;
    private float timer;
    private bool timerFinished = false;

    #endregion

    private void Awake()
    {
        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Projectile " + projectileID + " instantiated.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Projectile hit: " + collision.collider.tag);
        }

        if (collision.collider.CompareTag("Player"))
        {
            //Using events
            GameManager.Instance.OnPlayerHit(damage);

            DestroyProjectile();
            //Using colliders
            //collision.gameObject.GetComponent<playerGameCharacter>().OnPlayerHit(damage);
        }
        else if (!collision.collider.CompareTag("Player"))
        {
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        if (GameManager.Instance.toggleDebug && timerFinished)
        {
            Debug.Log("Destroyed projectile " + projectileID + " because it has reached the end of its lifespan.");
        }

        Destroy(this.gameObject);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= projectileLifespan && !timerFinished)
        {
            timer = 0f;
            timerFinished = true;
            DestroyProjectile();
        }
    }
}
