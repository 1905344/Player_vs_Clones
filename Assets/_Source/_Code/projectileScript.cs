using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class projectileScript : MonoBehaviour
{
    #region Variables

    [SerializeField] private bool enableDebug = false;

    public int projectileID { get; set; }
    public int damage { get; set; }

    #endregion

    private void Awake()
    {
        if (enableDebug)
        {
            Debug.Log("Projectile " + projectileID + " instantiated.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableDebug)
        {
            Debug.Log("Projectile hit: " + collision.collider.tag);
        }

        if (collision.collider.CompareTag("player"))
        {
            GameManager.Instance.OnPlayerHit(damage);
        }
        else if (!collision.collider.CompareTag("player"))
        {
            Destroy(this);
        }
    }
}
