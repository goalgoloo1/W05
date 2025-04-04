using UnityEngine;

public class PlayerBulletMovement : MonoBehaviour
{
    [SerializeField]
    float speed;
    Rigidbody rb;
    Vector3 moveDirection; // ������ �̵� ����
    Vector3 targetPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Rigidbody�� ȸ������ �ʵ��� ���� (���� ����)
        if (rb != null)
        {
            rb.freezeRotation = true; // ȸ�� ����
        }
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        Debug.Log("Aim Target: " + targetPosition);
        // ������ �� �� ����ϰ� ����
        moveDirection = (targetPosition - transform.position).normalized;
        transform.forward = moveDirection; // �ð��� ���� ���� (���� ����)
    }

    void Update()
    {
        // ������ �������� �̵�
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }
}