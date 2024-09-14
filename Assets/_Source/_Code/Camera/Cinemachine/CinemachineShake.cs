using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    #region Variables

    public static CinemachineShake Instance { get; private set; }

    private CinemachineVirtualCamera vCam;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    private float shakeTimer;
    private float shakeTimerLength;
    private float shakeStartingIntensity;

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
        shakeTimer = time;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            //Debug.Log("CinemachineShake: Shaking the camera!");
            
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(shakeStartingIntensity, 0f, 1 - (shakeTimer / shakeTimerLength));
        }
    }
}
