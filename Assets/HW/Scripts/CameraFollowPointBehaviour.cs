using UnityEngine;

public class CameraFollowPointBehaviour : MonoBehaviour
{
    private void LateUpdate()
    {
        // ���� ī�޶��� ȸ�� ��������
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Vector3 cameraEulerAngles = cameraRotation.eulerAngles;

        // X�� ȸ���� -50�� ~ 50���� ����
        float clampedX = Mathf.Clamp(cameraEulerAngles.x, -50f, 50f);

        // ���ѵ� X��� ������ Y, Z������ �� ȸ�� ����
        transform.rotation = Quaternion.Euler(clampedX, cameraEulerAngles.y, cameraEulerAngles.z);
    }
}