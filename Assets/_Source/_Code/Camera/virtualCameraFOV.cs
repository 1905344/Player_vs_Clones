using UnityEngine;
using Unity.Cinemachine;

public static class virtualCameraFOV
{
    public static void SetFocalLength(this CinemachineVirtualCamera vcam, float focalLength)
    {
        //vcam.m_Lens.FieldOfView = Camera.FocalLengthToFieldOfView(focalLength, vcam.m_Lens.SensorSize.y);
        vcam.m_Lens.FieldOfView = focalLength;
    }

    public static float GetFocalLength(this CinemachineVirtualCamera vcam)
    {
        //return Camera.FieldOfViewToFocalLength(vcam.m_Lens.FieldOfView, vcam.m_Lens.SensorSize.y);
        return vcam.m_Lens.FieldOfView;
    }
}
