using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake instance { get; private set; }

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    private void Awake()
    {
        instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        //cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        //cinemachineBasicMultiChannelPerlin.m_FrequencyGain = 5;
    }

    public void ShakeCamera(float intensity, float time)
    {
        //CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
            //cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>(); //Why don't I just cache this?

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            //Turn timer off
            //CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                //cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 
                Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
        }
    }
}
