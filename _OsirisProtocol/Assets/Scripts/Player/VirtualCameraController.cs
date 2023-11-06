using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class VirtualCameraController : MonoBehaviour
{
    public int lastCamera;
    public List<CinemachineVirtualCamera> VirtualCameras;
    public CinemachineFreeLook FreeLookCamera;

    void Start()
    {
        lastCamera = 0; // Aseg�rate de establecer una c�mara inicial

        if (FreeLookCamera == null)
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
            if (noise != null)
            {
                noise.m_AmplitudeGain = 0;
            }
            else
            {
                Debug.LogError("The VC Camera at index " + lastCamera + " is missing noise profile");
            }

            lastCamera = index;
        }
    }

    public void ApplyVibrationToCurrentCamera()
    {
        float maxVibrationIntensity = 0.5f;
        CinemachineVirtualCamera currentCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;

        if (currentCamera != null)
        {
            CinemachineBasicMultiChannelPerlin noise = VirtualCameras[lastCamera].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            StartCoroutine(rumbleCamera(noise, maxVibrationIntensity));
        }
    }

    IEnumerator rumbleCamera(CinemachineBasicMultiChannelPerlin noise, float maxVibrationIntensity)
    {
        // Para otras camaras virtuales
        if (noise != null)
        {
            noise.m_AmplitudeGain = maxVibrationIntensity;
        }

        yield return new WaitForSeconds(0.25f);

        if (noise != null)
        {
            noise.m_AmplitudeGain = 0;
        }
    }
}
