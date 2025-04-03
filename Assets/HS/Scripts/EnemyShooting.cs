using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    private Transform playerTransform;
    public float aimSpeed = 5f;
    public float aimOffset = 0.5f;
    private float shootTimer = 0f;

    public GameObject bulletPrefab;
    public Transform spawnPoint; 
    public float fireRate = 1f;

    public float laserMaxLength = 100f;
    public float laserWidth = 0.05f;

    private LineRenderer laserLineRenderer; 
    private Vector3 currentAimDirection; 

    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

            laserLineRenderer = spawnPoint.GetComponent<LineRenderer>();
                laserLineRenderer.positionCount = 2;
                laserLineRenderer.startWidth = laserWidth;
                laserLineRenderer.endWidth = laserWidth;
    }

    void Update()
    {
        if (playerTransform == null || spawnPoint == null || laserLineRenderer == null) return;

        AimAtPlayerHorizontally();

        CalculateAimDirection();

        UpdateLaserSight();

        shootTimer += Time.deltaTime;
        if (shootTimer >= 1f / fireRate)
        {
            print("shoot");
            Shoot();
            shootTimer = 0f; // Reset timer after shooting
        }
        
        
    }
    Vector3 GetAimTargetPosition()
    {
        return playerTransform.position + Vector3.up * aimOffset;
    }

    void AimAtPlayerHorizontally()
    {
        Vector3 targetPosition = GetAimTargetPosition();
        Vector3 directionToPlayer = targetPosition - transform.position;
        directionToPlayer.y = 0; 

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, aimSpeed * Time.deltaTime);
        }
    }

    void CalculateAimDirection()
    {
        Vector3 aimTarget = GetAimTargetPosition();
        currentAimDirection = (aimTarget - spawnPoint.position).normalized;
    }

    void UpdateLaserSight()
    {
        laserLineRenderer.SetPosition(0, spawnPoint.position);

        RaycastHit hit;
        if (Physics.Raycast(spawnPoint.position, currentAimDirection, out hit, laserMaxLength))
        {
            laserLineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            laserLineRenderer.SetPosition(1, spawnPoint.position + currentAimDirection * laserMaxLength);
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null)
        {
            Quaternion bulletRotation = Quaternion.identity; 
            if (currentAimDirection != Vector3.zero) 
            {
                bulletRotation = Quaternion.LookRotation(currentAimDirection);
            }
            Instantiate(bulletPrefab, spawnPoint.position, bulletRotation);
        }
    }
}