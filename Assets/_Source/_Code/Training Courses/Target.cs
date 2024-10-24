using System;
using UnityEngine;

public class Target : MonoBehaviour
{
    #region Variables

    public bool hasTargetBeenHit = false;
    public bool isPlayerTraining = false;

    [Header("Score Value for This Target")]
    [SerializeField] public int targetScoreValue;

    [Space(15)]

    [SerializeField, Tooltip("If you want the target to survive more than one bullet")] public float setTargetHealth;
    private float defaultTargetHealth;

    public Guid targetGuid;
    public int targetListID;

    [Space(15)]

    private bool destroyThisTarget = false;

    public Guid GenerateGuid()
    {
        return Guid.NewGuid();
    }

    public void SetTargetID(int indexPosition)
    {
        targetListID = indexPosition;
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
        defaultTargetHealth = setTargetHealth;
    }

    private void Start()
    {
        GameManager.Instance.OnTargetHit += ThisTargetHasBeenHit;
    }

    public void ReportTarget()
    {
        //This is a function for debugging
        //Each target is being called by the TrainingCourseManager to report back in the console
        Debug.Log("Target number ID: " + targetListID + " , GUID: " + targetGuid + " reporting!");
    }

    private void ThisTargetHasBeenHit (Guid guid, int damage)
    {
        if (guid != targetGuid)
        {
            return;
        }

        //Find where the bullet hit the target and set the final score
        defaultTargetHealth -= damage;

        if (defaultTargetHealth <= 0)
        {
            destroyThisTarget = true;
        }

        guid = targetGuid;
        Debug.Log("Target " + targetGuid + " hit: Score is " + targetScoreValue);

        hasTargetBeenHit = true;

        DisableThisTarget();
    }

    public void OnHitTriggerEvent(int damage)
    {
        GameManager.Instance.TargetHit(targetGuid, damage);
    }

    public void DisableThisTarget()
    {
        TrainingCourseManager.Instance.UpdateScore(targetScoreValue);
        
        Debug.Log("Target " + targetListID + " hit.");

        if (hasTargetBeenHit && destroyThisTarget)
        {
            Destroy(this.gameObject);
        }
        else if (hasTargetBeenHit && !destroyThisTarget)
        {
            Debug.Log("Target has been hit but not destroyed.");
        }
    }
}
