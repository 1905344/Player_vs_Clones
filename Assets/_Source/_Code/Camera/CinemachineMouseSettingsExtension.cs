using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CinemachineMouseSettingsExtension : CinemachineExtension
{
    #region Variables

    //POV (Point Of View)
    private CinemachinePOV camPOV;

    //InputManager reference
    private InputManager inputManager;

    #endregion

    protected override void Awake()
    {
        //Referencing the InputManager by calling the instance
        inputManager = InputManager.Instance;

        //Call the Awake function of the InputManager
        base.Awake();

        if (inputManager.mouseAcceleration)
        {
            camPOV.m_VerticalAxis.m_SpeedMode = AxisState.SpeedMode.MaxSpeed;
        }
        else
        {
            camPOV.m_VerticalAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
        }

        if (inputManager.invertMouse)
        {
            camPOV.m_HorizontalAxis.m_InvertInput = true;
        }

        camPOV.m_HorizontalAxis.m_MaxSpeed = inputManager.mouseVerticalSensitivity;
        camPOV.m_VerticalAxis.m_MaxSpeed = inputManager.mouseHorizontalSensitivity;
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
    }
}
