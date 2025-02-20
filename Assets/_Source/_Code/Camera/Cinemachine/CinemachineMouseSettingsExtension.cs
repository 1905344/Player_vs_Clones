using UnityEngine;
using Unity.Cinemachine;
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

        //Referencing the Cinemachine camera POV
        //camPOV = GetComponent<CinemachinePOV>();

        //if (inputManager.mouseAcceleration)
        //{
        //    camPOV.m_VerticalAxis.m_SpeedMode = AxisState.SpeedMode.MaxSpeed;
        //}

        //if (inputManager.invertMouse)
        //{
        //    camPOV.m_HorizontalAxis.m_InvertInput = true;
        //}
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
    }
}
