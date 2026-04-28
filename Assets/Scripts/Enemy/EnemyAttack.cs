using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAttack : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Range")]
    public float detectRange = 8f;
    public float attackRange = 1.5f;

    [Header("Attack")]
    public int damage = 10;
    public float attackCooldown = 1.2f;

    private NavMeshAgent agent;
    private EnemyMovement enemyMovement;
    private Animator animator;
    private Health playerHealth;

    private float lastAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyMovement = GetComponent<EnemyMovement>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                player = playerObj.transform;
        }

        if (player != null)
            playerHealth = player.GetComponent<Health>();
    }

    private void Update()
    {
        if (player == null || playerHealth == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectRange)
        {
            if (enemyMovement != null)
                enemyMovement.enabled = false;

            if (distance > attackRange)
            {
                ChasePlayer();
            }
            else
            {
                StopAndAttack();
            }
        }
        else
        {
            ReturnToWander();
        }
    }

    private void ChasePlayer()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        agent.isStopped = false;
        agent.SetDestination(player.position);

        if (animator != null)
            animator.SetBool("IsMoving", true);
    }

    private void StopAndAttack()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        agent.isStopped = true;
        agent.ResetPath();

        if (animator != null)
            animator.SetBool("IsMoving", false);

        LookAtPlayer();
        TryAttack();
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;

        if (animator != null)
            animator.SetTrigger("Attack");

        playerHealth.TakeDamage(damage);
    }

    private void ReturnToWander()
    {
        if (enemyMovement != null && !enemyMovement.enabled)
            enemyMovement.enabled = true;

        if (agent.enabled && agent.isOnNavMesh)
            agent.isStopped = false;
    }

    private void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        transform.rotation = Quaternion.LookRotation(direction);
    }
}