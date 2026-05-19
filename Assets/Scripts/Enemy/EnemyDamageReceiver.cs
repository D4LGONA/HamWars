using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDamageReceiver : MonoBehaviour, IDamageable
{
    [SerializeField] private Health health;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider bodyCollider;

    [Header("Death")]
    [SerializeField] private float disappearDelay = 3f;
    [SerializeField] private float shrinkDuration = 1f;
    [SerializeField] private Transform visualRoot;

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

        // 실제 보이는 모델 쪽만 줄이기
        if (visualRoot == null && animator != null)
            visualRoot = animator.transform;
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
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        if (bodyCollider != null)
            bodyCollider.enabled = false;

        if (animator != null)
        {
            animator.ResetTrigger(HitHash);
            animator.SetTrigger(DieHash);
        }

        StartCoroutine(DisappearCoroutine());
    }

    private IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(disappearDelay);

        if (visualRoot == null)
        {
            Destroy(gameObject);
            yield break;
        }

        float timer = 0f;
        Vector3 startScale = visualRoot.localScale;

        while (timer < shrinkDuration)
        {
            timer += Time.deltaTime;
            float t = timer / shrinkDuration;

            visualRoot.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null;
        }

        visualRoot.localScale = Vector3.zero;
        Destroy(gameObject);
    }
}