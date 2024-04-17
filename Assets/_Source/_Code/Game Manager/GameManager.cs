using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    //This script will check the relevant player behaviours to then 
    //set the states for the finite state machine / A.I.

    #region Variables

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    //Events for when a target is hit
    public event Action<Guid, int> OnTargetHit;
    public event Action AfterTargetHit;

    //Ending training courses
    public event Action TrainingCourseStarted;
    public event Action TrainingCourseEnded;

    //Events for A.I. Behaviours
    public event Action SetAiBehaviour;

    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #region Event Functions

    public void TargetHit(Guid guid, int damage)
    {
        if (OnTargetHit != null)
        {
            OnTargetHit(guid, damage);
        }
    }

    public void OnAfterTargetHit()
    {
        if (AfterTargetHit != null)
        {
            AfterTargetHit();
        }
    }

    public void OnTrainingCourseStart()
    {
        if (TrainingCourseStarted != null)
        {
            TrainingCourseStarted();
        }
    }

    public void OnTrainingCourseEnd()
    {
        if (TrainingCourseEnded != null)
        {
            TrainingCourseEnded();
        }
    }

    public void OnSetAiBehaviour()
    {
        if (SetAiBehaviour != null)
        {
            SetAiBehaviour();
        }
    }

    #endregion
}
