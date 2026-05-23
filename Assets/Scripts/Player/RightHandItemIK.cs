using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RightHandItemIK : MonoBehaviour
{
    [Header("Refs")]
    public Transform rightHandTarget;

    [Header("IK")]
    public bool enableIK = false;

    [Range(0f, 1f)] public float positionWeight = 1f;
    [Range(0f, 1f)] public float rotationWeight = 1f;

    [Header("Smooth")]
    public float weightLerpSpeed = 12f;

    private Animator animator;
    private float currentWeight;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float targetWeight = enableIK ? 1f : 0f;

        currentWeight = Mathf.Lerp(
            currentWeight,
            targetWeight,
            weightLerpSpeed * Time.deltaTime
        );
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        if (rightHandTarget == null)
        {
            SetRightHandIKWeight(0f);
            return;
        }

        float finalPositionWeight = currentWeight * positionWeight;
        float finalRotationWeight = currentWeight * rotationWeight;

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, finalPositionWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, finalRotationWeight);

        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
    }

    private void SetRightHandIKWeight(float weight)
    {
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
    }
}