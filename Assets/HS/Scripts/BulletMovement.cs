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
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

}
