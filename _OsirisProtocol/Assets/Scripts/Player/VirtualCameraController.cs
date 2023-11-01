using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class VirtualCameraController : MonoBehaviour
{
    public float VibrationIntensity = 1.0f;
    public int lastCamera;

    public List<CinemachineVirtualCamera> VirtualCameras;
    public CinemachineFreeLook FreeLookCamera;

    void Start()
    {
        lastCamera = 0; // Asegúrate de establecer una cámara inicial
        
        if (FreeLookCamera == null )
        {
            Debug.LogError("Missing Free Look");
        }
    }

    public void setCameraVC(int index)
    {
        if (lastCamera != index)
        {
            VirtualCameras[index].Priority = 1;
            VirtualCameras[lastCamera].Priority = 0;
            CinemachineBasicMultiChannelPerlin noise = VirtualCameras[lastCamera].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            noise.m_AmplitudeGain = 0;
            lastCamera = index;
        }
    }


    public void ApplyVibrationToCurrentCamera(float maxVibrationIntensity)
    {
        CinemachineVirtualCamera currentCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;

        if (currentCamera != null)
        {
            CinemachineBasicMultiChannelPerlin noise = VirtualCameras[lastCamera].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            // Para otras cámaras virtuales
            if (noise != null)
            {
                noise.m_AmplitudeGain = maxVibrationIntensity;
            }
        }
    }
}
