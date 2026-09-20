using UnityEngine;
using Unity.Cinemachine;

public class CameraDirector : MonoBehaviour
{
    [SerializeField] private CinemachineCamera[] cameras;
    [SerializeField] private int activePriority = 100;
    [SerializeField] private int inactivePriority = 0;

    public void SetCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
            cameras[i].Priority = i == index ? activePriority : inactivePriority;
    }
}