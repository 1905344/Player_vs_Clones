using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class projectileScript : MonoBehaviour
{
    #region Variables

    public int projectileID;
    public int damage { get; set; }
    
    [SerializeField] private float projectileLifespan = 10f;
    [SerializeField] private SphereCollider projectileCollider;
    private float timer;
    
    #endregion

    private void Awake()
    {
        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Projectile " + projectileID + " instantiated.");
        }

        projectileCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        #region Debug

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Projectile hit: " + other.gameObject.tag.ToString());
        }

        #endregion

        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag(""))
        {
            DestroyProjectile();
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            //Using events
            GameManager.Instance.OnPlayerHit(damage);

            DestroyProjectile();
            //Using colliders
            //collision.gameObject.GetComponent<playerGameCharacter>().OnPlayerHit(damage);
        }
    }

    private void DestroyProjectile()
    {
        #region Debug

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Destroyed projectile " + projectileID + " because it has reached the end of its lifespan.");
        }

        #endregion

        Destroy(gameObject);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= projectileLifespan)
        {
            timer = 0f;
            DestroyProjectile();
        }
    }
}
