using UnityEngine;
using System.Collections.Generic;

public class TEMPGameManager : MonoBehaviour
{
    public static TEMPGameManager Instance { get; private set; }

    private List<GameObject> activeBullets = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        CheckBulletCollisions();
    }

    public void RegisterBullet(GameObject bullet)
    {
        if (bullet != null && !activeBullets.Contains(bullet))
        {
            activeBullets.Add(bullet);
        }
    }

    public void UnregisterBullet(GameObject bullet)
    {
        if (bullet != null && activeBullets.Contains(bullet))
        {
            activeBullets.Remove(bullet);
        }
    }

    private void CheckBulletCollisions()
    {
        List<GameObject> bullets = new List<GameObject>(activeBullets);

        foreach (GameObject bullet in bullets)
        {
            if (bullet == null)
            {
                activeBullets.Remove(bullet);
                continue;
            }

            BulletMovement bulletMovement = bullet.GetComponent<BulletMovement>();
            if (bulletMovement != null && bulletMovement.enemyShooter != null)
            {
                CheckBulletHitObjects(bullet, bulletMovement.enemyShooter);
            }
            else
            {
                CheckBulletHitObjects(bullet, null);
            }
        }
    }

    private void CheckBulletHitObjects(GameObject bullet, GameObject shooter)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (player != null && player != shooter)
        {
            if (IsColliding(bullet, player))
            {
                Destroy(player);
                DestroyBullet(bullet);
                return;
            }
        }

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null && enemy != shooter)
            {
                if (IsColliding(bullet, enemy))
                {
                    Destroy(enemy);
                    DestroyBullet(bullet);
                    return;
                }
            }
        }
    }

    private bool IsColliding(GameObject obj1, GameObject obj2)
    {
        Collider col1 = obj1.GetComponent<Collider>();
        Collider col2 = obj2.GetComponent<Collider>();

        if (col1 != null && col2 != null)
        {
            return col1.bounds.Intersects(col2.bounds);
        }
        return false;
    }

    private void DestroyBullet(GameObject bullet)
    {
        if (bullet != null)
        {
            UnregisterBullet(bullet);
            Destroy(bullet);
        }
    }

    public void OnBulletCollision(GameObject bullet, GameObject hitObject)
    {
        if (hitObject.CompareTag("Player") || hitObject.CompareTag("Enemy"))
        {
            BulletMovement bulletMovement = bullet.GetComponent<BulletMovement>();
            if (bulletMovement != null && bulletMovement.enemyShooter == hitObject)
            {
                return;
            }
            Destroy(hitObject);
        }
    }
}
