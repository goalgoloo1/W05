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

        Destroy(gameObject, 5f);
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


    /// <summary>   
    /// HS: Player bullet doesn't kill the enemies so ADD this code, must have to remove this stuff soon.
    /// </summary>

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            //do nothing
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnDestroy()
    {
        Instantiate((GameObject)Resources.Load("HW/BulletDestroyParticle"), transform.position, Quaternion.identity);


    }

    /// <summary>
    /// HS: code above is for player bullet TEST only , must have to remove this stuff soon.
    /// </summary>

}