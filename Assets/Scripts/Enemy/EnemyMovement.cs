using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Wander")]
    public float wanderRadius = 8f;
    public float waitTime = 2f;
    public float arriveDistance = 0.6f;

    private NavMeshAgent agent;
    private Animator animator;
    private float waitTimer;

    private const int SampleTries = 10;
    private const float SampleMaxDistance = 2f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        PickAndGo();
    }

    private void Update()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        bool isMoving = agent.velocity.sqrMagnitude > 0.01f && !agent.pathPending;
        animator.SetBool("IsMoving", isMoving);

        if (agent.pathPending)
            return;

        if (agent.remainingDistance <= arriveDistance)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                waitTimer = 0f;
                PickAndGo();
            }
        }
    }

    private void PickAndGo()
    {
        if (TryGetRandomPoint(out Vector3 destination))
        {
            agent.SetDestination(destination);
        }
    }

    private bool TryGetRandomPoint(out Vector3 result)
    {
        for (int i = 0; i < SampleTries; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
            Vector3 randomPoint = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, SampleMaxDistance, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = transform.position;
        return false;
    }
}