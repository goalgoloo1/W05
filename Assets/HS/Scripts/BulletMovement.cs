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

        if (TEMPGameManager.Instance != null)
        {
            TEMPGameManager.Instance.RegisterBullet(gameObject);
        }
    }

    void Update()
    {
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (TEMPGameManager.Instance != null)
        //{
        //    TEMPGameManager.Instance.OnBulletCollision(gameObject, collision.gameObject);
        //}
        Debug.Log(collision.gameObject);

        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if(!playerController.evadeSuccess)
            {
                playerController.OnDeath();
            }
        }


        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (TEMPGameManager.Instance != null)
        {
            TEMPGameManager.Instance.UnregisterBullet(gameObject);
        }
    }
}
