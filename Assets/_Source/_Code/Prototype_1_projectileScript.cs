using UnityEngine;

<<<<<<<< HEAD:Assets/_Source/_Code/Enemies/projectileScript.cs
[RequireComponent (typeof(SphereCollider))]
public class projectileScript : MonoBehaviour
========
public class Prototype_1_projectileScript : MonoBehaviour
>>>>>>>> Merging-First-Prototype:Assets/_Source/_Code/Prototype_1_projectileScript.cs
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
<<<<<<<< HEAD:Assets/_Source/_Code/Enemies/projectileScript.cs
        #region Debug

        if (GameManager.Instance.toggleDebug)
========
        if (Prototype_1_GameManager.Instance.toggleDebug)
>>>>>>>> Merging-First-Prototype:Assets/_Source/_Code/Prototype_1_projectileScript.cs
        {
            Debug.Log("Projectile " + projectileID + " instantiated.");
        }

<<<<<<<< HEAD:Assets/_Source/_Code/Enemies/projectileScript.cs
        #endregion

========
>>>>>>>> Merging-First-Prototype:Assets/_Source/_Code/Prototype_1_projectileScript.cs
        projectileCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
<<<<<<<< HEAD:Assets/_Source/_Code/Enemies/projectileScript.cs
        GameObject collidedObject = collision.collider.gameObject;

        if (collidedObject.CompareTag("Wall") || collidedObject.CompareTag("Ground") || collidedObject.CompareTag("Environment"))
========
        #region Debug

        if (Prototype_1_GameManager.Instance.toggleDebug)
        {
            Debug.Log("Projectile hit: " + other.gameObject.tag.ToString());
        }

        #endregion

        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Environment"))
        {
            DestroyProjectile();
        }
        else if (other.gameObject.CompareTag("Player"))
>>>>>>>> Merging-First-Prototype:Assets/_Source/_Code/Prototype_1_projectileScript.cs
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
            Prototype_1_GameManager.Instance.OnPlayerHit(damage);

            DestroyProjectile();
<<<<<<<< HEAD:Assets/_Source/_Code/Enemies/projectileScript.cs
========
            //Using colliders
            //collision.gameObject.GetComponent<playerGameCharacter>().OnPlayerHit(damage);
>>>>>>>> Merging-First-Prototype:Assets/_Source/_Code/Prototype_1_projectileScript.cs
        }
    }

    private void DestroyProjectile()
    {
        #region Debug

<<<<<<<< HEAD:Assets/_Source/_Code/Enemies/projectileScript.cs
        if (GameManager.Instance.toggleDebug)
========
        if (Prototype_1_GameManager.Instance.toggleDebug)
>>>>>>>> Merging-First-Prototype:Assets/_Source/_Code/Prototype_1_projectileScript.cs
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