using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemyShoot", story: "[Agent] Shoot [Bullet] to [Target] from [FirePoint] and draw [LaserPointer]", category: "Action", id: "accfc193e286d18cccec69c3399bb833")]
public partial class EnemyShootAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Bullet;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> FirePoint;
    [SerializeReference] public BlackboardVariable<GameObject> LaserPointer;
    // 회전 관련 변수
    private float rotationSpeed = 20f;
    private float aimOffset = 0.5f;
    private float alignmentThreshold = 0.95f;

    // 발사 관련 변수
    private GameObject bulletPrefab;
    private float fireRate = 1f;
    private float shootTimer = 0f;

    // 레이저 관련 변수
    private float laserMaxLength = 100f;
    private float laserWidth = 0.05f;
    private Vector3 currentAimDirection;

    protected override Status OnStart()
    {
        // 필요한 객체들이 모두 존재하는지 확인
        if (Agent.Value == null || Target.Value == null ||
            FirePoint.Value == null || LaserPointer.Value == null)
        {
            Debug.LogError("Required GameObjects not set in blackboard variables");
            return Status.Failure;
        }

        // 라인 렌더러가 있는지 확인
        LineRenderer lineRenderer = LaserPointer.Value.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LaserPointer must have a LineRenderer component");
            return Status.Failure;
        }

        // 라인렌더러 두께 설정
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // 필요한 객체들이 모두 존재하는지 확인
        if (Agent.Value == null || Target.Value == null ||
            FirePoint.Value == null || LaserPointer.Value == null)
        {
            return Status.Failure;
        }

        // 적 회전 처리
        bool isAligned = RotateTowardsTarget();

        // 조준선 업데이트
        UpdateLaserSight();

        // 발사 처리
        shootTimer += Time.deltaTime;
        if (shootTimer >= 1f / fireRate && isAligned)
        {
            Shoot();
            shootTimer = 0f;
        }

        // 계속 실행되도록 Running 반환
        return Status.Running;
    }

    protected override void OnEnd()
    {
        // 종료 시 레이저 비활성화
        if (LaserPointer.Value != null)
        {
            LineRenderer lineRenderer = LaserPointer.Value.GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }
    }

    private bool RotateTowardsTarget()
    {
        Transform agentTransform = Agent.Value.transform;
        Transform targetTransform = Target.Value.transform;

        // 타겟 위치 계산 (머리 높이에 맞춤)
        Vector3 targetPosition = targetTransform.position + Vector3.up * aimOffset;
        Vector3 directionToTarget = targetPosition - agentTransform.position;
        directionToTarget.y = 0; // 수평 회전만

        if (directionToTarget.sqrMagnitude > 0.001f)
        {
            // 회전 적용
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            agentTransform.rotation = Quaternion.Slerp(agentTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 정렬 여부 계산
            float alignment = Vector3.Dot(agentTransform.forward, directionToTarget.normalized);
            return alignment > alignmentThreshold;
        }

        return true;
    }

    private void UpdateLaserSight()
    {
        LineRenderer lineRenderer = LaserPointer.Value.GetComponent<LineRenderer>();
        if (lineRenderer == null) return;

        // 조준 방향 계산
        Vector3 targetPosition = Target.Value.transform.position + Vector3.up * aimOffset;
        Vector3 firePointPosition = FirePoint.Value.transform.position;
        currentAimDirection = (targetPosition - firePointPosition).normalized;

        // 레이저 라인 업데이트
        //lineRenderer.enabled = true;
        // 랜더러 활성화 여부를 shootTimer에 따라 결정
        lineRenderer.enabled = shootTimer > 0.7f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, firePointPosition);

        RaycastHit hit;
        if (Physics.Raycast(firePointPosition, currentAimDirection, out hit, laserMaxLength))
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1, firePointPosition + currentAimDirection * laserMaxLength);
        }
    }

    private void Shoot()
    {
        if (Bullet != null && FirePoint.Value != null)
        {
            Quaternion bulletRotation = Quaternion.LookRotation(currentAimDirection);
            Transform firePointTransform = FirePoint.Value.transform;
            BulletMovement newBulletMovement = GameObject.Instantiate(Bullet, firePointTransform.position, bulletRotation).GetComponent<BulletMovement>();
            newBulletMovement.enemyShooter = this.GameObject;
        }
    }
}
