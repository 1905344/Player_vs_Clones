using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class P2_CameraManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject[] cameras;
    [SerializeField] private GameObject[] cameraIDs;

    private bool changeCamera = false;

    Guid currentGuid = Guid.Empty;

    #endregion

    void Start()
    {
        P2_GameManager.Instance.changePlayerCharacter += OnCameraChanged;
        P2_GameManager.Instance.changePlayerCharacter -= OnCameraChanged;

    }

    private void OnCameraChanged(Guid cameraID)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            Guid checkGuid = cameraIDs[i].GetComponent<P2_CameraID>().GetCameraID();

            if (checkGuid == cameraID)
            {
                //Change to the camera
                cameras[i].SetActive(true);
                changeCamera = true;

                currentGuid = checkGuid;

                //foreach (GameObject camera in cameras)
                //{
                //    if (camera.activeInHierarchy && checkGuid != cameraID)
                //    {
                //        camera.SetActive(false);
                //    }
                //}
            }
            else
            {
                return;
            }
        }

        
    }

    void Update()
    {
        //Apply camera change in update
        //May not be needed
        if (changeCamera)
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                Guid checkGuid = cameraIDs[i].GetComponent<P2_CameraID>().cameraID;

                foreach (GameObject camera in cameras)
                {
                    if (!camera.activeInHierarchy && checkGuid == currentGuid)
                    {
                        camera.SetActive(true);

                        if (camera.activeInHierarchy && checkGuid != currentGuid)
                        {
                            camera.SetActive(false);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            currentGuid = Guid.Empty;
            changeCamera = false;
        }
    }
}
