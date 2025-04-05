using UnityEngine;

public class P2_RangeSensor : MonoBehaviour
{
    #region Variables

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private float detectionRangeHeight = 1.15f;
    private SphereCollider _sphereCollider;

    [Space(10)]

    [Header("Debug: Show Gizmos")]
    [SerializeField] private bool showTransformDetectionRadius = false;
    [SerializeField] private bool showGameObjectDetectionRadius = false;
    [SerializeField] private bool showLineOfSight = false;

    [Space(3)]

    [SerializeField] private float gizmosRadius = 1f;
    [SerializeField] private Color32 transformRadiusColour;
    [SerializeField] private Color32 gameObjectRadiusColour;
    [SerializeField] private Color32 gameObjectDetectedColour;
    [SerializeField] private Color32 gameObjectUndetectedColour;

    public GameObject DetectedTarget { get; set; }

    [SerializeField] private bool detectionDebug = false;

    #endregion

    private void Start()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _sphereCollider.isTrigger = true;
        _sphereCollider.radius = detectionRadius;
        gizmosRadius = _sphereCollider.radius;
    }

    public GameObject UpdateSensor()
    {
        //Sphere collider check
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionMask);

        DetectedTarget = null;

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                DetectedTarget = collider.gameObject;
                break;
            }
        }

        //if (colliders.Length > 0)
        //{
        //    DetectedTarget = colliders[0].gameObject;
        //}
        //else
        //{
        //    DetectedTarget = null;
        //}

        return DetectedTarget;
    }

    public GameObject OnDetectionPerformed(GameObject target)
    {
        Vector3 originPoint = transform.position + Vector3.up * detectionRangeHeight;

        RaycastHit hit;
        Vector3 directionToTarget = target.transform.position - originPoint;
        float maxDistance = Vector3.Distance(target.transform.position, originPoint);
        Physics.Raycast(originPoint, directionToTarget.normalized, out hit, maxDistance * 2, detectionMask);

        //Debug.Log("RangeSensor: " + target.transform.position);
        //Debug.DrawLine(originPoint, target.transform.position);

        //Debug.Log("RangeSensor: " + hit.collider);
        //Debug.Log("RangeSensor: " + target);

        if (hit.collider != null && P2_GameManager.Instance.enableDebug && detectionDebug)
        {
            Debug.Log($"Raycast hit. {hit.transform.tag}");
        }

        if (hit.collider != null && hit.collider.gameObject == target && hit.collider.gameObject.CompareTag("Player"))
        {
            #region Debug

            if (P2_GameManager.Instance.enableDebug)
            {
                if (showLineOfSight && this.enabled)
                {
                    Debug.DrawLine(transform.position + Vector3.up * detectionRangeHeight, target.transform.position, gameObjectDetectedColour);
                }
            }

            #endregion

            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    #region Debug: Show Gizmos

    private void OnDrawGizmosSelected()
    {
        //Visualising the radius of the sphere collider

        if (showTransformDetectionRadius)
        {
            Gizmos.color = transformRadiusColour;
            Gizmos.DrawWireSphere(transform.position, gizmosRadius);
        }

        if (showGameObjectDetectionRadius)
        {
            Gizmos.color = DetectedTarget ? gameObjectDetectedColour : gameObjectUndetectedColour;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }

        if (showLineOfSight)
        {
            Gizmos.color = gameObjectRadiusColour;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * detectionRangeHeight, 0.3f);
        }
    }

    #endregion

    private void Update()
    {
        #region Debug

        if (P2_GameManager.Instance.enableDebug)
        {
            _sphereCollider.radius = detectionRadius;
        }

        #endregion
    }
}
