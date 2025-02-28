using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class P2_CameraID : MonoBehaviour
{
    #region Variables

    [SerializeField] public Guid cameraID;

    public Guid GetCameraID()
    {
        return cameraID;
    }

    #endregion

    void Start()
    {
        
    }

    public void SetCameraID(Guid guid)
    {
        cameraID = guid;

        Debug.Log($"cameraID = {cameraID}");
    }

    void Update()
    {
        
    }
}
