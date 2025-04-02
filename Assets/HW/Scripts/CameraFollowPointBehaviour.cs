using UnityEngine;

public class CameraFollowPointBehaviour : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
