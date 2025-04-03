using UnityEngine;

public class CameraFollowPointBehaviour : MonoBehaviour
{
    private void LateUpdate()
    {
        // 메인 카메라의 회전 가져오기
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Vector3 cameraEulerAngles = cameraRotation.eulerAngles;

        // X축 회전을 -50도 ~ 50도로 제한
        float clampedX = Mathf.Clamp(cameraEulerAngles.x, -50f, 50f);

        // 제한된 X축과 원래의 Y, Z축으로 새 회전 생성
        transform.rotation = Quaternion.Euler(clampedX, cameraEulerAngles.y, cameraEulerAngles.z);
    }
}