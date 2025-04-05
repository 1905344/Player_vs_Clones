using UnityEngine;

[RequireComponent (typeof(SphereCollider))]
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
        #region Debug

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Projectile " + projectileID + " instantiated.");
        }

        #endregion

        projectileCollider = GetComponent<SphereCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.collider.gameObject;

        if (collidedObject.CompareTag("Wall") || collidedObject.CompareTag("Ground") || collidedObject.CompareTag("Environment"))
        {
            DestroyProjectile();

            #region Debug

            if (GameManager.Instance.toggleDebug)
            {
                Debug.Log("Projectile hit: " + collidedObject.tag.ToString());
            }

            #endregion
        }
        else if (collidedObject.CompareTag("Player"))
        {
            #region Debug

            if (GameManager.Instance.toggleDebug)
            {
                Debug.Log("Projectile hit: " + collidedObject.tag.ToString());
            }

            #endregion

            //Using events
            GameManager.Instance.OnPlayerHit(damage);

            DestroyProjectile();
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
