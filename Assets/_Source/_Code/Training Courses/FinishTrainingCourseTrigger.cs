using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FinishTrainingCourseTrigger : MonoBehaviour
{
    #region Variables

    [Header("Training Course Number ID")]
    [SerializeField] private int courseID;

    [Space(15)]

    [Header("Training Course Ending Position Collider")]
    private BoxCollider trainingCourseFinishedCollider;

    #endregion

    private void Awake()
    {
        trainingCourseFinishedCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (courseID < 3 && courseID != 0)
            {
                GameManager.Instance.OnTrainingCourseEnd(courseID);
            }
            else if (courseID == 3 && courseID != 0)
            {
                GameManager.Instance.OnPlayerFinishedTraining();
            }
        }
        else
        {
            return;
        }
    }
}
