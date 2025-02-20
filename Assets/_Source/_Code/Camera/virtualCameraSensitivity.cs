using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public static class virtualCameraSensitivity
{
    public static void SetCameraPOV(this CinemachineVirtualCamera vcam, float sensitivityX, float sensitivityY, bool mouseAcceleration, bool invertYAxis)
    {
        if (!mouseAcceleration)
        {
            vcam.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
            vcam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
        }
        else
        {
            vcam.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_SpeedMode = AxisState.SpeedMode.MaxSpeed;
            vcam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_SpeedMode = AxisState.SpeedMode.MaxSpeed;
        }

        if (invertYAxis)
        {
            vcam.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput = true;
        }
        else
        {
            vcam.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput = false;
        }

        vcam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = sensitivityX;
        vcam.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = sensitivityY;
    }

    public static float GetMouseHorizontalSensitivity(this CinemachinePOV vcamPOV)
    {
        return vcamPOV.m_HorizontalAxis.m_MaxSpeed;
    }

    public static float GetMouseVerticalSensitivity(this CinemachinePOV vcamPOV)
    {
        return vcamPOV.m_VerticalAxis.m_MaxSpeed;
    }

    public static bool GetMouseAcceleration(this CinemachinePOV vcamPOV)
    {
        if (vcamPOV.m_HorizontalAxis.m_SpeedMode == AxisState.SpeedMode.MaxSpeed && vcamPOV.m_VerticalAxis.m_SpeedMode == AxisState.SpeedMode.MaxSpeed)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool GetInvertMouseY(this CinemachinePOV vcamPOV)
    {
        return vcamPOV.m_VerticalAxis.m_InvertInput;
    }
}
