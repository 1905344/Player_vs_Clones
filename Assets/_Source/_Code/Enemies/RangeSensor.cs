using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SphereCollider))]
public class RangeSensor : MonoBehaviour
{
    #region Variables

    [SerializeField] public float detectionRadius = 10f;
    [SerializeField] public List<string> targetTags = new();

    readonly List<Transform> detectedObjects = new(10);
    private SphereCollider _sphereCollider;

    [Space(10)]

    [Header("Debug: Show Gizmos")]
    [SerializeField] private bool showGizmos;
    
    [Space(3)]

    [SerializeField] private float gizmosRadius = 1f;
    [SerializeField] private Color32 radiusColour;

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


    #region Debug: Showing Gizmos

    private void OnDrawGizmosSelected()
    {
        //Visualising the radius of the sphere collider

        if (showGizmos)
        {
            Gizmos.color = radiusColour;
            Gizmos.DrawWireSphere(transform.position, gizmosRadius);
        }
    }

    #endregion
}
