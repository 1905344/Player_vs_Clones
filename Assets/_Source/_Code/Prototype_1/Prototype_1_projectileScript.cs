using UnityEngine;

public class Prototype_1_projectileScript : MonoBehaviour
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

        if (Prototype_1_GameManager.Instance.enableDebug)
        {
            Debug.Log("Projectile " + projectileID + " instantiated.");
        }

        #endregion

        projectileCollider = GetComponent<SphereCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Environment"))
        {
            DestroyProjectile();
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            //Using events
            Prototype_1_GameManager.Instance.OnPlayerHit(damage);

            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        #region Debug

        if (Prototype_1_GameManager.Instance.enableDebug)
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