using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    [SerializeField]
    float speed;
    Rigidbody rb;
    public GameObject enemyShooter; //쏜놈
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

}
