using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class RangeSensor : MonoBehaviour
{
    #region Variables

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private List<string> targetTags = new();
    [SerializeField] private LayerMask detectionMask;
    private Vector3 detectionRadiusVector;

    [Space(5)]

    [SerializeField] private float detectionRangeLine = 10.0f;
    [SerializeField] private float detectionRangeHeight = 1.15f;

    readonly List<Transform> detectedObjects = new(10);
    private SphereCollider _sphereCollider;

    [Space(10)]

    [Header("Debug: Show Gizmos")]
    [SerializeField] private bool showDetectionRadius = false;
    [SerializeField] private bool showOtherDetectionRadius = false;
    [SerializeField] private bool showLineOfSight = false;

    [Space(3)]

    [SerializeField] private float gizmosRadius = 1f;
    [SerializeField] private Color32 radiusColour;
    [SerializeField] private Color32 otherRadiusColour;
    [SerializeField] private Color32 lineOfSightColourDetected;
    [SerializeField] private Color32 lineOfSightColourUndetected;

    public GameObject DetectedTarget { get; set; }

    #endregion

    private void Start()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _sphereCollider.isTrigger = true;
        _sphereCollider.radius = detectionRadius;

        gizmosRadius = _sphereCollider.radius;

        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (var c in colliders)
        {
            ProcessTrigger(c, transform => detectedObjects.Add(transform));
        }
    }

    #region OnTriggerEnter and OnTriggerExit

    private void OnTriggerEnter(Collider other)
    {
        ProcessTrigger(other, transform => detectedObjects.Add(transform));
    }

    private void OnTriggerExit(Collider other)
    {
        ProcessTrigger(other, transform => detectedObjects.Remove(transform));
    }

    #endregion

    private void ProcessTrigger(Collider other, Action<Transform> action)
    {
        if (other.CompareTag("Untagged"))
        {
            return;
        }

        foreach (string t in targetTags)
        {
            if (other.CompareTag(t))
            {
                action(other.transform);
            }
        }
    }

    public Transform GetNearestTarget(string tag)
    {
        //Calculate distance between this.transform and target.transform, then return with the nearest
        if (detectedObjects.Count == 0)
        {
            return null;
        }

        Transform nearestTarget = null;
        float nearestDistanceSquare = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform potentialTarget in detectedObjects)
        {
            if (potentialTarget.CompareTag(tag))
            {
                Vector3 directionToNearestTarget = potentialTarget.position - currentPosition;
                float directionSquareToTarget = directionToNearestTarget.sqrMagnitude;

                if (directionSquareToTarget < nearestDistanceSquare)
                {
                    nearestDistanceSquare = directionSquareToTarget;
                    nearestTarget = potentialTarget;
                }
            }
        }

        return nearestTarget;
    }

    public GameObject UpdateSensor()
    {
        //Sphere collider check
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

    public GameObject OnDetectionPerformed(GameObject target)
    {
        Vector3 originPoint = transform.position + Vector3.up * detectionRangeHeight;

        RaycastHit hit;
        Vector3 directionToTarget = target.transform.position - originPoint;
        float maxDistance = Vector3.Distance(target.transform.position, originPoint);
        Physics.Raycast(originPoint, directionToTarget.normalized ,out hit, maxDistance * 2, detectionMask);

        Debug.Log("RangeSensor: " + target.transform.position);
        Debug.DrawLine(originPoint, target.transform.position);

        //Debug.Log("RangeSensor: " + hit.collider);
        //Debug.Log("RangeSensor: " + target);

        if (hit.collider != null)
        {
            Debug.Log($"Raycast hit. {hit.transform.tag}");
        }

        if (hit.collider != null && hit.collider.gameObject == target)
        {
            return hit.collider.gameObject;
            #region Debug

            if (GameManager.Instance.toggleDebug)
            {
                if (showLineOfSight && this.enabled)
                {
                    Debug.DrawLine(transform.position + Vector3.up * detectionRangeHeight, target.transform.position, lineOfSightColourDetected);
                }
            }

            #endregion
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

        if (showDetectionRadius)
        {
            Gizmos.color = radiusColour;
            Gizmos.DrawWireSphere(transform.position, gizmosRadius);
        }

        if (showOtherDetectionRadius)
        {
            Gizmos.color = DetectedTarget ? lineOfSightColourDetected : lineOfSightColourUndetected;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }

        if (showLineOfSight)
        {
            Gizmos.color = otherRadiusColour;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * detectionRangeHeight, 0.3f);
        }
    }

    #endregion
}
