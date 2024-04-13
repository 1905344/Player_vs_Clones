using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    #region Variables

    public Target instance;

    public bool OnTargetHit = false;
    public bool isPlayerTraining = false;

    private TrainingCourseManager courseManager;

    [Header("Score Values")]
    [SerializeField] public float outerRingScore;    
    [SerializeField] public float innerRingScore;    
    [SerializeField] public float centreRingScore;
    [SerializeField] public float bullsEyeScore;

    private float finalScore;

    [Space(15)]

    [Header("Colliders")]
    [SerializeField] private Collider outerRingCollider;
    [SerializeField] private Collider innerRingCollider;
    [SerializeField] private Collider centreRingCollider;
    [SerializeField] private Collider bullsEyeCollider;

    [Space(15)]

    [SerializeField] public float setTargetHealth;
    private float defaultTargetHealth;

    //Identifying this target
    public float targetNumberID;
    public Guid targetGuid;

    public float GetThisTargetsID()
    {
        return targetNumberID;
    }

    public Guid GetThisTargetsGuid()
    {
        return targetGuid;
    }

    #endregion

    public void SetTargetID(float indexPosition)
    {
        targetNumberID = indexPosition;
    }

    private static Guid GenerateGuid()
    {
        return Guid.NewGuid();
    }

    public void ReportTarget()
    {
        //This is a function for debugging
        //Each target is being called by the TrainingCourseManager to report back in the console
        Debug.Log("Target " + targetNumberID + " reporting!");
    }

    private void Awake()
    {
        instance = this;
        targetGuid = GenerateGuid();

        outerRingCollider = GetComponent<Collider>();
        innerRingCollider = GetComponent<Collider>();
        centreRingCollider = GetComponent<Collider>();
        bullsEyeCollider = GetComponent<Collider>();

        defaultTargetHealth = setTargetHealth;
    }

    public void TargetHit(float damage)
    {
        //Find where the bullet hit the target and set the final score
        //finalScore == 
        courseManager.OnTargetHit(finalScore);

        defaultTargetHealth -= damage;
        OnTargetHit = true;
    }

    public void DisableThisTarget()
    {
        setTargetHealth = defaultTargetHealth;
        Debug.Log("Target " + targetNumberID + " hit.");
    }

    void Update()
    {
        if (OnTargetHit)
        {
            this.gameObject.SetActive(false);
        }
    }
}
