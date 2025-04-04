using UnityEngine;

public class EnemyRotationController : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float aimOffset = 0.5f;

    public void RotateTowardsTargetHorizontally(Transform target)
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetPosition = target.position + Vector3.up * aimOffset;
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0;

        if (directionToTarget.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}