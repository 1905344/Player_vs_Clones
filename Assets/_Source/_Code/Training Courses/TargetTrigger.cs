using System;
using UnityEngine;

public class TargetTrigger : MonoBehaviour
{
    //This script triggers the event for when a target is hit

    #region Variables

    [SerializeField] private GameObject targetGameObject;

    //Identifying this target
    public int targetListID;
    public Guid targetGuid;

    public void SetTargetID(int indexPosition)
    {
        targetListID = indexPosition;
    }

    private static Guid GenerateGuid()
    {
        return Guid.NewGuid();
    }

    public int GetThisTargetsID()
    {
        return targetListID;
    }

    public Guid GetThisTargetsGuid()
    {
        return targetGuid;
    }

    #endregion

    private void Awake()
    {
        targetGuid = GenerateGuid();
    }

    public void Start()
    {
        //targetGameObject.GetComponent<Target>().targetGuid = targetGuid;
    }

    public void ReportTarget()
    {
        //This is a function for debugging
        //Each target is being called by the TrainingCourseManager to report back in the console
        Debug.Log("Target " + targetListID + " reporting!");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gun")
        {
            //GameManager.Instance.TargetHit(targetGuid, 0);
            Debug.Log("TargetTrigger: Target hit!");
        }
    }
}
