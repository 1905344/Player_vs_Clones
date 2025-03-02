using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class P2_CameraManager : MonoBehaviour
{
    #region Variables

    private static P2_CameraManager instance;

    public static P2_CameraManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] private List<GameObject> cameras = new List<GameObject>();
    [SerializeField] private int currentActiveCamera = 0;
    [SerializeField] private string currentCameraID = string.Empty;

    private bool changeCamera = false;

    public int getCurrentActiveCamera()
    {
        return currentActiveCamera;
    }

    #endregion

    void Awake()
    {
        P2_GameManager.Instance.changePlayerCharacter += OnCameraChanged;
        //P2_GameManager.Instance.changePlayerCharacter -= OnCameraChanged;

        P2_GameManager.Instance.playerCharacterKilled += RemoveCamera;
        //P2_GameManager.Instance.playerCharacterKilled -= RemoveCamera;

        currentCameraID = cameras[0].GetComponent<P2_CameraID>().GetCameraID();
    }

    private void OnCameraChanged()
    {
        cameras[currentActiveCamera].gameObject.SetActive(false);
        currentCameraID = string.Empty;

        if (currentActiveCamera >= (cameras.Count - 1))
        {
            currentActiveCamera = 0;
        }
        else
        {
            currentActiveCamera++;
        }

        for (int i = 0; i < cameras.Count; i++)
        {
            //if (cameras[i].gameObject.activeInHierarchy)
            //{
            //    //cameras[currentActiveCamera].gameObject.SetActive(false);
            //    //currentCameraID = string.Empty;

            //    i++;
            //}
            
            if (i == currentActiveCamera)
            {
                string checkGuid = cameras[i].GetComponent<P2_CameraID>().GetCameraID();

                cameras[i].gameObject.SetActive(true);
                currentCameraID = checkGuid;
            }
        }
    }

    private void RemoveCamera(Guid cameraGuid)
    {
        if (cameraGuid == null)
        {
            Debug.Log($"RemoveCamera received no guid: {cameraGuid}");
            return;
        }

        GameObject camera = cameras[currentActiveCamera].gameObject;
        cameras.Remove(camera);
    }
}
