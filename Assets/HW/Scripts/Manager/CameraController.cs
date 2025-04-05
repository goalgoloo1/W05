using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance => _instance;
    static CameraController _instance;

    private void Awake()
    {
        _instance = this;
    }

    CinemachineCamera prevCamera;

    public void ChangeCamera(CinemachineCamera newCamera)
    {
        if(prevCamera != null)
        {
            prevCamera.Priority = 0;
            newCamera.Priority = 10;
        }
        else
        {
            newCamera.Priority = 10;
        }
        

        prevCamera = newCamera;
    }

    public void ReturnCamera(CinemachineCamera currentCamera)
    {
        if(prevCamera != null)
        {
            prevCamera.Priority = 10;
            currentCamera.Priority = 0;
        }
        else
        {

        }
    }
}
