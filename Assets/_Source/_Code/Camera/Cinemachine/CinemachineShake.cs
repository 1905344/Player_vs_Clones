using Cinemachine;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    #region Variables

    public static CinemachineShake Instance { get; private set; }

    private CinemachineVirtualCamera vCam;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    private float shakeTimer;
    private float shakeTimerLength;
    private float shakeStartingIntensity;

    private bool startShaking = false;

    #endregion

    private void Awake()
    {
        Instance = this;
        vCam = GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensity, float time)
    {
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

        shakeStartingIntensity = intensity;
        shakeTimerLength = time;
        //shakeTimer = time;
        shakeTimer += Time.deltaTime;
        startShaking = true;

        #region Debug

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("CinemachineShake: Shaking the camera!");
        }

        #endregion
    }

    private void StopShaking()
    {
        shakeTimer = 0;
        startShaking = false;
    }

    private void Update()
    {
        if (startShaking)
        {
            if (shakeTimer > 0)
            {
                shakeTimer += Time.deltaTime;

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(shakeStartingIntensity, 0f, 1 - (shakeTimer / shakeTimerLength));

                if (shakeTimer > shakeTimerLength)
                {
                    StopShaking();
                }
            }
        }

    }
}
