using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class P2_CameraManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject[] cameras;

    private bool changeCamera = false;

    #endregion

    void Start()
    {
        P2_GameManager.Instance.changePlayerCharacter += OnCameraChanged;
    }

    private void OnCameraChanged(Guid cameraID)
    {
        foreach (GameObject camera in cameras)
        {
            //if (camera.GetGuid == cameraID)
            //{
            //    //Change to the camera
                    //cameras[].SetActive(true);
            //      changeCamera = true;
            //}
            //else
            //{
            //    return;
            //}
        }
    }

    void Update()
    {
        //Apply camera change in update
        //May not be needed
        if (changeCamera)
        {
            
            changeCamera = false;
        }
    }
}
