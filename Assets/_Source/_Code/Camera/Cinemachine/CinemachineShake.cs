using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    #region Variables

    public static CinemachineShake Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera vCam;

    private float shakeTimer;
    private float shakeTimerLength;
    private float shakeStartingIntensity;

    #endregion

    private void Awake()
    {
        Instance = this;
        GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

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

            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            Mathf.Lerp(shakeStartingIntensity, 0f, 1 - (shakeTimer / shakeTimerLength));
        }
    }
}
