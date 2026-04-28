using UnityEngine;
using UnityEngine.AI;

public class EnemyDamageReceiver : MonoBehaviour, IDamageable
{
    [SerializeField] private Health health;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider bodyCollider;

    [Header("Death")]
    [SerializeField] private float destroyDelay = 3f;

    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private bool isDead;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (bodyCollider == null)
            bodyCollider = GetComponent<Collider>();
    }

    public void Hit(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (isDead) return;

        if (health != null)
            health.TakeDamage(Mathf.RoundToInt(damage));

        if (health != null && health.currentHp <= 0)
        {
            Die();
            return;
        }

        PlayHit();
    }

    private void PlayHit()
    {
        if (animator == null) return;

        animator.SetTrigger(HitHash);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (bodyCollider != null)
            bodyCollider.enabled = false;

        if (animator != null)
        {
            animator.ResetTrigger(HitHash);
            animator.SetTrigger(DieHash);
        }

    }
}