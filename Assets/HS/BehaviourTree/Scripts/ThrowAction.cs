using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Throw", story: "[Agent] Throw [Bomb] to [Target] from [FirePoint]", category: "Action", id: "ce9c4d096ee1c64a677a9be179f0a748")]
public partial class ThrowAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Bomb;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> FirePoint;

    // 회전 관련 변수
    private float rotationSpeed = 20f;
    private float aimOffset = 0.5f;
    private float alignmentThreshold = 0.95f;

    // 투척 관련 변수
    private float throwForce = 12f;
    private float throwCooldown = 3f;
    private float throwTimer = 0f;
    private float explosionDelay = 2f;
    private float explosionRadius = 2f;
    private float explosionDamage = 50f;

    // 투척 궤적 계산용 변수
    private Vector3 currentAimDirection;

    protected override Status OnStart()
    {
        // 필요한 객체들이 모두 존재하는지 확인
        if (Agent.Value == null || Target.Value == null ||
            FirePoint.Value == null || Bomb.Value == null)
        {
            Debug.LogError("Required GameObjects not set in blackboard variables");
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // 필요한 객체들이 모두 존재하는지 확인
        if (Agent.Value == null || Target.Value == null ||
            FirePoint.Value == null || Bomb.Value == null)
        {
            return Status.Failure;
        }

        // 적 회전 처리
        bool isAligned = RotateTowardsTarget();

        // 타이머 업데이트 및 투척
        throwTimer += Time.deltaTime;
        if (throwTimer >= throwCooldown && isAligned)
        {
            ThrowGrenade();
            throwTimer = 0f;
        }

        // 계속 실행되도록 Running 반환
        return Status.Running;
    }

    protected override void OnEnd()
    {
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

    private void ThrowGrenade()
    {
        if (Bomb.Value == null || FirePoint.Value == null) return;

        // 투척 방향 계산 (약간 포물선을 그리도록 상방향 추가)
        Vector3 targetPosition = Target.Value.transform.position + Vector3.up * aimOffset;
        Vector3 firePointPosition = FirePoint.Value.transform.position;

        // 거리에 따라 위쪽으로 던지는 각도 조절
        float distanceToTarget = Vector3.Distance(firePointPosition, targetPosition);
        float upwardForce = Mathf.Clamp(distanceToTarget * 0.5f, 1f, 10f);

        currentAimDirection = (targetPosition - firePointPosition).normalized + new Vector3(0, upwardForce * 0.1f, 0);

        // 수류탄 생성 및 투척
        GameObject grenade = GameObject.Instantiate(Bomb.Value, firePointPosition, Quaternion.identity);
        Rigidbody grenadeRb = grenade.GetComponent<Rigidbody>();

        if (grenadeRb != null)
        {
            // 물리력으로 수류탄 투척
            grenadeRb.AddForce(currentAimDirection * throwForce, ForceMode.Impulse);

            // 2초 후에 폭발하는 코루틴 시작
            StartGrenadeExplosion(grenade);
        }
        else
        {
            Debug.LogWarning("Bomb prefab has no Rigidbody component!");
        }
    }

    private void StartGrenadeExplosion(GameObject grenade)
    {
        // MonoBehaviour가 아니므로 직접 코루틴을 실행할 수 없음
        // 따라서 폭발 컴포넌트를 추가하거나 새로운 MonoBehaviour를 생성해서 처리해야 함
        GrenadeExplosion explosionComponent = grenade.GetComponent<GrenadeExplosion>();

        if (explosionComponent == null)
        {
            explosionComponent = grenade.AddComponent<GrenadeExplosion>();
        }

        explosionComponent.InitializeExplosion(explosionDelay, explosionRadius, explosionDamage);
    }
}

// 수류탄 폭발을 처리하는 컴포넌트
public class GrenadeExplosion : MonoBehaviour
{
    private float explosionDelay;
    private float explosionRadius;
    private float explosionDamage;
    private float timer = 0f;

    public void InitializeExplosion(float delay, float radius, float damage)
    {
        explosionDelay = delay;
        explosionRadius = radius;
        explosionDamage = damage;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= explosionDelay)
        {
            Explode();
        }
    }

    void Explode()
    {
        // 폭발 반경 내의 콜라이더 찾기
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // 폭발 효과 처리
        foreach (Collider hit in colliders)
        {
            // 플레이어 체력 감소 등의 처리
            if (hit.CompareTag("Player"))
            {
                // 플레이어에게 데미지 주기
                // hit.GetComponent<PlayerHealth>()?.TakeDamage(explosionDamage);
                Debug.Log($"Player hit by explosion! Damage: {explosionDamage}");
            }

            // 리지드바디가 있으면 폭발력 적용
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(700f, transform.position, explosionRadius);
            }
        }

        // 폭발 효과 표시 (파티클 등)
        // GameObject explosionEffect = GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // 폭발 사운드 재생
        // AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        // 수류탄 오브젝트 제거
        Destroy(gameObject);
    }

    // 디버깅용: 폭발 반경 보기
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

