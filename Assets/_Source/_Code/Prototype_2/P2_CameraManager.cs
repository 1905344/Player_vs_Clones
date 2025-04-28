using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private List<GameObject> cameras = new (3);
    [SerializeField] private int currentActiveCamera = 0;
    [SerializeField] private string currentCameraID = string.Empty;

    public int getCurrentActiveCamera()
    {
        return currentActiveCamera;
    }

    #endregion

    void Awake()
    {
        P2_GameManager.Instance.changePlayerCharacter += OnCameraChanged;
        P2_GameManager.Instance.playerCharacterKilled += RemoveCamera;

        currentCameraID = cameras[0].GetComponent<P2_CameraID>().GetCameraID();
    }

    private void OnCameraChanged()
    {
        if (cameras[currentActiveCamera].gameObject != null)
        {
            cameras[currentActiveCamera].gameObject.SetActive(false);
        }
        
        currentCameraID = string.Empty;
        currentActiveCamera++;
        currentActiveCamera %= cameras.Count;

        for (int i = 0; i < cameras.Count; i++)
        {
            if (i == currentActiveCamera)
            {
                string checkGuid = cameras[i].GetComponent<P2_CameraID>().GetCameraID();

                cameras[i].gameObject.SetActive(true);
                currentCameraID = checkGuid;
            }
        }
    }

    private void RemoveCamera(string cameraGuid)
    {
        if (cameraGuid == null || cameraGuid != currentCameraID)
        {
            Debug.Log($"RemoveCamera received no guid: {cameraGuid}");
            return;
        }

        cameras.Remove(cameras[currentActiveCamera].gameObject);
    }
}
