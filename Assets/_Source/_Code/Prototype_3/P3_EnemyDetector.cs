using System.Collections.Generic;
using UnityEngine;

public class P3_EnemyDetector : MonoBehaviour
{
    #region Variables 

    private static P3_EnemyDetector instance;

    public static P3_EnemyDetector Instance
    {
        get
        {
            return instance;
        }
    }

    [Header("Collider Reference")]
    [SerializeField] private SphereCollider detectorCollider;

    [Space(10)]

    [Header("Lights and Warning Colours")]
    [SerializeField] private Light warningLight;
    [SerializeField] private Color32 detectedColour;
    [SerializeField] private Color32 undetectedColour;

    private bool updateLightColour = false;

    [Space(10)]

    [Header("Enemies In/Out of Range Lists")]
    [SerializeField] private List<GameObject> enemiesInRange = new();

    [Space(10)]

    [Header("Debug Variables")]
    [SerializeField] private bool showRadius = false;
    [SerializeField] private bool enemyDetected = false;
    [SerializeField] private float gizmosDrawSphereRadius;

    #endregion

    //public void RemoveEnemy(GameObject getEnemy)
    //{
    //    enemiesInRange.Remove(getEnemy);
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!enemiesInRange.Contains(other.gameObject))
            {
                enemiesInRange.Add(other.gameObject);
            }

            if (enemiesInRange.Count == 0)
            {
                updateLightColour = true;
                enemyDetected = false;
            }
            else
            {
                updateLightColour = true;
                enemyDetected = true;
            }
        }
    }

    #region Debug: Show Gizmos

    private void OnDrawGizmosSelected()
    {
        //Visualising the radius of the sphere collider
        if (showRadius)
        {
            Gizmos.color = enemyDetected ? undetectedColour : detectedColour;
            Gizmos.DrawWireSphere(transform.position, gizmosDrawSphereRadius);
        }
    }

    #endregion

    void Update()
    {
        #region Update Light Colour

        if (updateLightColour)
        {
            if (enemyDetected)
            {
                warningLight.color = detectedColour;
            }
            else
            {
                warningLight.color = undetectedColour;
            }
        }

        #endregion

        #region Debug

        if (P3_GameManager.Instance.enableDebug && showRadius)
        {
            gizmosDrawSphereRadius = detectorCollider.radius;
        }

        #endregion
    }
}
