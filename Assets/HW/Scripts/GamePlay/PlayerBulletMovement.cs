using UnityEngine;

public class PlayerBulletMovement : MonoBehaviour
{
    [SerializeField]
    float speed;
    Rigidbody rb;
    Vector3 moveDirection; // 고정된 이동 방향
    Vector3 targetPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Rigidbody가 회전하지 않도록 설정 (선택 사항)
        if (rb != null)
        {
            rb.freezeRotation = true; // 회전 고정
        }

        Destroy(gameObject, 5f);
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        Debug.Log("Aim Target: " + targetPosition);
        // 방향을 한 번 계산하고 고정
        moveDirection = (targetPosition - transform.position).normalized;
        transform.forward = moveDirection; // 시각적 방향 설정 (선택 사항)
    }
    
    void Update()
    {
        // 고정된 방향으로 이동
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