using UnityEngine;

public class P3_NearbyEnemyIndicator : MonoBehaviour
{
    #region Variables

    [Header("Direction Indicator")]
    [SerializeField] private GameObject indicator;
    [SerializeField] private MeshRenderer indicatorMesh;

    [Space(10)]

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask detectionMask;

    [Space(3)]

    [SerializeField] private Color32 radiusColour;
    [SerializeField] private Color32 detectedColour;
    [SerializeField] private Color32 undetectedColour;

    private GameObject DetectedTarget { get; set; }

    [Space(5)]

    [SerializeField] private Transform indicatorTransform;

    [Space(10)]

    [Header("Debug: Show Gizmos")]
    [SerializeField] private bool showDetectionRadius = false;

    #endregion

    private GameObject GetNearestEnemy()
    {
        //Physics Overlap sphere check
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionMask);

        if (colliders.Length > 0)
        {
            DetectedTarget = colliders[0].gameObject;
        }
        else
        {
            DetectedTarget = null;
        }

        return DetectedTarget;
    }

    #region Debug: Show Gizmos

    private void OnDrawGizmosSelected()
    {
        //Visualising the radius of the physics sphere
        if (showDetectionRadius)
        {
            Gizmos.color = radiusColour;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }

    #endregion

    void Update()
    {
        indicator.transform.position = indicatorTransform.position;

        if (GetNearestEnemy() != null)
        {
            indicatorMesh.enabled = true;
            indicator.transform.LookAt(GetNearestEnemy().transform);
        }
        else
        {
            indicatorMesh.enabled = false;
        }
    }
}
