using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Refs")]
    public InputManager input;
    public Transform cam; // 플레이어 자식인 카메라 Transform
    public GameObject firstCam;
    public GameObject thirdCam;
    public Transform heldCube;   // 들고있는 큐브
    public Transform targetHead;

    [Header("Move")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f * 2f;
    public float jumpHeight = 1.2f;

    [Header("Mouse Look")]
    public bool useMouseLook = true;
    public float pitchMin = -35f;
    public float pitchMax = 60f;
    public float mouseSensitivity = 1f; // 필요하면


    private bool isFirst = false;
    private CharacterController controller;
    private Vector3 velocity;
    float pitch; // 누적 pitch

    public bool Move = true;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (input == null) input = GetComponent<InputManager>();

        if (cam == null)
            cam = Camera.main != null ? Camera.main.transform : null;
    }

    void Update()
    {
        if (input == null) return;
        if (false == Move) return;

        HandleMouseLook();     // 마우스로만 회전
        HandleMove();          // WASD는 회전 없이 이동만
        HandleGravityAndJump();
        SetMode(!input.IsThird);
    }
    void LateUpdate()
    {
        if (heldCube == null || cam == null) return;

        heldCube.rotation = cam.rotation;
        firstCam.transform.rotation = heldCube.rotation;
    }

    void HandleMouseLook()
    {
        if (!useMouseLook || cam == null) return;

        Vector2 look = input.LookDelta * mouseSensitivity;

        transform.Rotate(0f, look.x, 0f);

        pitch -= look.y; // 보통 위로 마우스 올리면 위를 보게 하려면 '-'가 자연스러움
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        targetHead.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMove()
    {
        Vector2 mv = input.Move;
        if (mv.sqrMagnitude < 0.0001f) return;

        Vector3 moveDir = transform.right * mv.x + transform.forward * mv.y;
        moveDir = Vector3.ClampMagnitude(moveDir, 1f);

        controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }

    void HandleGravityAndJump()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        if (controller.isGrounded && input.JumpDown)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void SetMode(bool firstPerson)
    {
        isFirst = firstPerson;
        if (firstCam) firstCam.SetActive(isFirst);
        if (thirdCam) thirdCam.SetActive(!isFirst);
    }

}
