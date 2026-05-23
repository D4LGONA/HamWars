using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private InputManager input;

    [Header("Parameters")]
    [SerializeField] private string isMovingParam = "IsMoving";

    [Header("Options")]
    [SerializeField] private float moveThreshold = 0.01f;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (input == null)
            input = GetComponent<InputManager>();
    }

    private void Update()
    {
        if (animator == null || input == null) return;

        bool isMoving = input.Move.sqrMagnitude > moveThreshold;
        animator.SetBool(isMovingParam, isMoving);
    }
}
