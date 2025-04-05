using System;
using UnityEngine;

public class P2_CameraID : MonoBehaviour
{
    #region Variables

    public string cameraID;

    public string GetCameraID()
    {
        return cameraID;
    }

    #endregion

    public void SetCameraID(string getID)
    {
        cameraID = getID;
        Debug.Log($"cameraID = {cameraID}");
    }
}
