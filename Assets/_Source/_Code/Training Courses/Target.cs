using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;

[Serializable]
public class Target : MonoBehaviour
{
    #region Variables

    //private Target instance;

    public bool hasTargetBeenHit = false;
    public bool isPlayerTraining = false;

    private TrainingCourseManager courseManager;

    [Header("Score Value for This Target")]
    [SerializeField] private int finalScore;

    [Space(15)]

    [SerializeField, Tooltip("If you want the target to survive more than one bullet")] public float setTargetHealth;
    private float defaultTargetHealth;

    public int targetNumberID;
    public Guid targetGuid;

    [Space(15)]

    [SerializeField] private TargetTrigger targetTriggerScript;

    private bool destroyThisTarget = false;

    #endregion

    private void Awake()
    {
        defaultTargetHealth = setTargetHealth;
    }

    private void Start()
    {
        GameManager.Instance.OnTargetHit += ThisTargetHasBeenHit;
        GameManager.Instance.AfterTargetHit += DisableThisTarget;
        targetTriggerScript = GetComponent<TargetTrigger>();
        targetGuid = targetTriggerScript.targetGuid;

        //defaultTargetHealth = setTargetHealth;
    }

    private void ThisTargetHasBeenHit (Guid guid, int damage)
    {
        //Find where the bullet hit the target and set the final score
        defaultTargetHealth -= damage;

        if (defaultTargetHealth <= 0)
        {
            destroyThisTarget = true;
        }

        guid = targetGuid;
        Debug.Log("Target " + targetGuid + " hit: Score is " + finalScore);

        hasTargetBeenHit = true;
    }

    public void DisableThisTarget()
    {
        courseManager.UpdateScore(finalScore);

        //setTargetHealth = defaultTargetHealth;
        Debug.Log("Target " + targetNumberID + " hit.");

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
