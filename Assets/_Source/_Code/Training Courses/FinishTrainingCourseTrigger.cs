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
            if (courseID <= 3 && courseID != 0)
            {
                GameManager.Instance.OnTrainingCourseEnd(courseID);
            }
            
            if (courseID == 3 && courseID != 0)
            {
                TrainingCourseManager.Instance.isTrainingComplete = true;
                GameManager.Instance.OnPlayerFinishedTraining();
                Debug.Log("Player has crossed the final finishing line.");
            }

            gameObject.SetActive(false);
        }
        else
        {
            return;
        }
    }
}
