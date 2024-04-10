using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Target : MonoBehaviour
{
    #region Variables

    public bool OnTargetHit = false;

    private TrainingCourseManager courseManager;

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

    #endregion

    private void Awake()
    {
        outerRingCollider = GetComponent<Collider>();
        innerRingCollider = GetComponent<Collider>();
        centreRingCollider = GetComponent<Collider>();
        bullsEyeCollider = GetComponent<Collider>();
    }

    public void TargetHit()
    {
        //Find where the bullet hit the target and set the final score
        //finalScore == 
        courseManager.OnTargetHit(finalScore);
        OnTargetHit = true;
    }

    void Update()
    {
        if (OnTargetHit)
        {
            this.gameObject.SetActive(false);
        }
    }
}
