using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Wander")]
    public float wanderRadius = 8f;                    // 적 위치 기준 반경
    public float repathInterval = 1.0f;                // 목적지 갱신 최소 간격(성능)
    public float arriveDistance = 0.6f;                // 도착 판정
    public Vector2 waitRange = new Vector2(1.0f, 3.0f);// 도착 후 대기 시간

    [Header("NavMesh")]
    public float sampleMaxDistance = 2.0f;             // 샘플 스냅 거리(작으면 실패↑)
    public int sampleTries = 8;                        // 샘플 시도 횟수
    public int areaMask = NavMesh.AllAreas;

    [Header("Stuck Recover")]
    public float stuckCheckInterval = 0.5f;            // 막힘 체크 주기
    public float minProgress = 0.05f;                  // 이만큼은 가까워져야 "진행"
    public float stuckTimeToRepath = 1.0f;             // 이 시간동안 진행 없으면 재선정

    NavMeshAgent agent;
    float nextRepathTime;
    float nextMoveTime;

    float lastDist = Mathf.Infinity;
    float lastStuckCheckTime;
    float stuckAccum;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        if (NavMesh.SamplePosition(transform.position, out var hit, 10f, areaMask))
            agent.Warp(hit.position);

        nextMoveTime = Time.time;
        nextRepathTime = 0f;

        lastDist = Mathf.Infinity;
        lastStuckCheckTime = Time.time;
        stuckAccum = 0f;

        PickAndGo();
    }

    void Update()
    {
        if (!agent.enabled) return;

        // NavMesh 위가 아니면 이동 자체가 이상해질 수 있음
        if (!agent.isOnNavMesh) return;

        // 대기중
        if (Time.time < nextMoveTime) return;

        // 경로가 없으면 새 목적지
        if (!agent.pathPending && !agent.hasPath && Time.time >= nextRepathTime)
        {
            PickAndGo();
            return;
        }

        // 도착 판정(remainingDistance 중심)
        if (IsArrived())
        {
            float wait = Random.Range(waitRange.x, waitRange.y);
            nextMoveTime = Time.time + wait;
            agent.ResetPath();
            return;
        }

        // 막힘 체크해서 회복
        CheckStuckAndRecover();
    }

    bool IsArrived()
    {
        if (agent.pathPending) return false;
        if (!agent.hasPath) return true;

        float stop = Mathf.Max(arriveDistance, agent.stoppingDistance);
        return agent.remainingDistance <= stop;
    }

    void CheckStuckAndRecover()
    {
        if (Time.time - lastStuckCheckTime < stuckCheckInterval) return;
        lastStuckCheckTime = Time.time;

        if (agent.pathPending || !agent.hasPath) return;

        float d = agent.remainingDistance;

        // 목적지까지의 거리가 줄어들지 않으면 stuck 누적
        if (lastDist - d < minProgress)
            stuckAccum += stuckCheckInterval;
        else
            stuckAccum = 0f;

        lastDist = d;

        // 일정 시간 이상 진행 없으면 목적지 다시 뽑기
        if (stuckAccum >= stuckTimeToRepath && Time.time >= nextRepathTime)
        {
            stuckAccum = 0f;
            PickAndGo();
        }
    }

    void PickAndGo()
    {
        nextRepathTime = Time.time + repathInterval;

        if (TryGetRandomNavPoint(transform.position, wanderRadius, out Vector3 dest))
        {
            agent.isStopped = false;
            agent.SetDestination(dest);

            // stuck 추적 초기화
            lastDist = Mathf.Infinity;
            stuckAccum = 0f;
        }
        else
        {
            // 목적지 못 찾으면 짧게 기다렸다 재시도
            nextMoveTime = Time.time + 0.5f;
        }
    }

    bool TryGetRandomNavPoint(Vector3 origin, float radius, out Vector3 result)
    {
        float snapDist = Mathf.Max(sampleMaxDistance, radius); 

        for (int i = 0; i < sampleTries; i++)
        {
            Vector2 r = Random.insideUnitCircle * radius;
            Vector3 candidate = origin + new Vector3(r.x, 0f, r.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, snapDist, areaMask))
            {
                result = hit.position;
                return true;
            }
        }

        result = origin;
        return false;
    }
}