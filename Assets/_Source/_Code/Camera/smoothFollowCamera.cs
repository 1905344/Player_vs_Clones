using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smoothFollowCamera : MonoBehaviour
{
    //Code sourced from: https://github.com/t4guw/100-Unity-Mechanics-for-Programmers/tree/master/programs/smooth_camera_follow

    //This script is for the camera for following/tracking an object or the player character
    //The cinemachine camera can be used instead of this script

    //The target to track
    [SerializeField] private Transform target;

    [SerializeField, Range(0.1f, 1f)] [Tooltip("How smoothly the camera follows or 'tracks' the object")] private float smoothSpeed = 0.125f;
    
    private Vector3 offset;

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        //For 3D projects
        //transform.LookAt(target);
    }

}
