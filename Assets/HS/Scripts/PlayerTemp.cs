using UnityEngine;

public class PlayerTemp : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    private Rigidbody rb;
    private Vector3 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("플레이어 오브젝트에 Rigidbody 컴포넌트가 없습니다!", gameObject);
        }
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(horizontalInput, 0f, verticalInput).normalized;
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}
