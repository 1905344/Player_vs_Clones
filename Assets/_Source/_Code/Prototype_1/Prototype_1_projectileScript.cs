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
        if (Prototype_1_GameManager.Instance.enableDebug)
        {
            Debug.Log("Projectile " + projectileID + " instantiated.");
        }

        projectileCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        #region Debug

        if (Prototype_1_GameManager.Instance.enableDebug)
        {
            Debug.Log("Projectile hit: " + other.gameObject.tag.ToString());
        }

        #endregion

        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Environment"))
        {
            DestroyProjectile();
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            //Using events
            Prototype_1_GameManager.Instance.OnPlayerHit(damage);

            DestroyProjectile();
            //Using colliders
            //collision.gameObject.GetComponent<playerGameCharacter>().OnPlayerHit(damage);
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