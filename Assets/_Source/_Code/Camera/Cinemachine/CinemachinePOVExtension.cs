using UnityEngine;
using Cinemachine;

public class CinemachinePOVExtension : CinemachineExtension
{
    //This script allows the use of the "new" input system to move a cinemachine virtual camera
    //within the aim stage.

    #region Variables

    [SerializeField, Tooltip("How sensitive the mouse is on the horizontal axis")] 
    private float mouseHorizontalSensitivity = 10f;

    [SerializeField, Tooltip("How sensitive the mouse is on the vertical axis")] 
    private float mouseVerticalSensitivity = 10f;

    [SerializeField, Tooltip("How far the mouse can vertically move the camera up or down on the 'y' axis before reaching the defined limit.")] 
    private float clampYAngle = 85f;

    //private InputManager inputManager;
    private Vector3 startingRotation;

    #endregion

    protected override void Awake()
    {
        //Referencing the InputManager by calling the instance
        //inputManager = InputManager.Instance;

        //Call the Awake function of the InputManager
        base.Awake();
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow)
        {
            if (stage == CinemachineCore.Stage.Aim)
            {
                if (startingRotation == null)
                {
                    //Get the starting rotation of the camera
                    startingRotation = transform.localRotation.eulerAngles;

                    //Get the mouse delta value from the Input Manager as a vector2 value
                    Vector2 deltaInput = InputManager.Instance.GetMouseDelta();

                    //Update the camera rotation using the mouse delta values from the InputManager
                    //startingRotation.x += deltaInput.x * mouseVerticalSensitivity * Time.deltaTime;
                    //startingRotation.y += deltaInput.y * mouseHorizontalSensitivity * Time.deltaTime;

                    //Clamping the 'y' value of the camera when moving the mouse vertically
                    startingRotation.y = Mathf.Clamp(startingRotation.y, -clampYAngle, clampYAngle);

                    //Setting the camera orientation when moving it with the mouse
                    //startingRotation.y is in the 'X' value to rotate horizontally (look left or right)
                    //startingRotation.x is in the 'Y' value to rotate vertically (look up or down)
                    //state.RawOrientation = Quaternion.Euler(startingRotation.y, startingRotation.x, 0f);
                }
            }
        }
    }
}
