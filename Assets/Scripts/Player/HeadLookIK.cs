using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeadLookIK : MonoBehaviour
{
    [Header("Refs")]
    public Transform headPivot;

    [Header("IK")]
    public bool enableIK = true;
    public float lookDistance = 5f;

    [Range(0f, 1f)] public float lookWeight = 1f;
    [Range(0f, 1f)] public float bodyWeight = 0.15f;
    [Range(0f, 1f)] public float headWeight = 0.85f;
    [Range(0f, 1f)] public float eyesWeight = 0f;
    [Range(0f, 1f)] public float clampWeight = 0.5f;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        if (!enableIK || headPivot == null)
        {
            animator.SetLookAtWeight(0f);
            return;
        }

        Vector3 lookPosition = headPivot.position + headPivot.forward * lookDistance;

        animator.SetLookAtWeight(
            lookWeight,
            bodyWeight,
            headWeight,
            eyesWeight,
            clampWeight
        );

        animator.SetLookAtPosition(lookPosition);
    }
}