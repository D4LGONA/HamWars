using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Refs")]
    public InputManager input;

    [Header("Visual")]
    public Transform characterVisual;     // SDCharacter 넣기

    [Header("Camera Refs")]
    public GameObject firstCam;
    public GameObject thirdCam;

    public Transform headPivot;           // 1인칭 카메라 부모
    public Transform thirdCameraPivot;    // 3인칭 카메라 공전 중심

    [Header("Optional")]
    public Transform heldCube;
    public HeadLookIK headLookIK;          // 머리 IK 스크립트

    [Header("Move")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f * 2f;
    public float jumpHeight = 1.2f;

    [Header("Rotation")]
    public float characterRotateSpeed = 12f; // 3인칭 이동 방향 회전 속도

    [Header("Mouse Look")]
    public bool useMouseLook = true;
    public float mouseSensitivity = 1f;

    [Header("First Person Look")]
    public float firstPitchMin = -35f;
    public float firstPitchMax = 60f;

    [Header("Third Person Look")]
    public float thirdPitchMin = -20f;
    public float thirdPitchMax = 60f;

    [Header("First Person Camera Follow")]
    public Transform firstCamPoint; // Head 본 아래 빈 오브젝트
    public bool followHeadPointInFirstPerson = true;

    private CharacterController controller;
    private Vector3 velocity;

    private bool isFirst;

    private float firstPitch;
    private float thirdYaw;
    private float thirdPitch;

    public bool Move = true;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (input == null)
            input = GetComponent<InputManager>();
    }

    private void Start()
    {
        if (input != null)
            SetMode(!input.IsThird);
    }

    private void Update()
    {
        if (input == null) return;
        if (!Move) return;

        SetMode(!input.IsThird);

        HandleMouseLook();
        HandleMove();
        HandleGravityAndJump();
    }

    private void LateUpdate()
    {
        UpdateFirstPersonCameraPosition();
    }

    private void UpdateFirstPersonCameraPosition()
    {
        if (!isFirst) return;
        if (!followHeadPointInFirstPerson) return;
        if (firstCam == null || firstCamPoint == null) return;

        firstCam.transform.position = firstCamPoint.position;
    }

    private void HandleMouseLook()
    {
        if (!useMouseLook) return;

        Vector2 look = input.LookDelta * mouseSensitivity;

        if (isFirst)
            HandleFirstPersonLook(look);
        else
            HandleThirdPersonLook(look);
    }

    private void HandleFirstPersonLook(Vector2 look)
    {
        // 1인칭 좌우: PlayerCharacter 루트 회전
        // 1인칭에서는 카메라/이동 기준이 몸 방향과 같아야 해서 루트 회전이 맞음
        transform.Rotate(0f, look.x, 0f);

        // 1인칭 상하: HeadPivot만 회전
        firstPitch -= look.y;
        firstPitch = Mathf.Clamp(firstPitch, firstPitchMin, firstPitchMax);

        if (headPivot != null)
            headPivot.localRotation = Quaternion.Euler(firstPitch, 0f, 0f);

        // 1인칭일 때 모델 방향은 루트 방향과 같게 정렬
        if (characterVisual != null)
            characterVisual.localRotation = Quaternion.identity;
    }

    private void HandleThirdPersonLook(Vector2 look)
    {
        // 3인칭: 마우스는 캐릭터를 돌리지 않고 카메라 피벗만 공전
        thirdYaw += look.x;
        thirdPitch -= look.y;
        thirdPitch = Mathf.Clamp(thirdPitch, thirdPitchMin, thirdPitchMax);

        if (thirdCameraPivot != null)
            thirdCameraPivot.localRotation = Quaternion.Euler(thirdPitch, thirdYaw, 0f);
    }

    private void HandleMove()
    {
        Vector2 mv = input.Move;
        if (mv.sqrMagnitude < 0.0001f) return;

        Vector3 moveDir;

        if (isFirst)
        {
            // 1인칭: PlayerCharacter 방향 기준 이동
            moveDir = transform.right * mv.x + transform.forward * mv.y;
        }
        else
        {
            // 3인칭: 카메라 기준 이동
            moveDir = GetThirdPersonMoveDirection(mv);

            // 중요:
            // PlayerCharacter 루트가 아니라 SDCharacter만 이동 방향으로 회전
            RotateVisualToMoveDirection(moveDir);
        }

        moveDir = Vector3.ClampMagnitude(moveDir, 1f);
        controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }

    private Vector3 GetThirdPersonMoveDirection(Vector2 mv)
    {
        Transform camTr = thirdCam != null ? thirdCam.transform : null;

        if (camTr == null)
            return transform.right * mv.x + transform.forward * mv.y;

        Vector3 camForward = camTr.forward;
        Vector3 camRight = camTr.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        return camRight * mv.x + camForward * mv.y;
    }

    private void RotateVisualToMoveDirection(Vector3 moveDir)
    {
        moveDir.y = 0f;

        if (moveDir.sqrMagnitude < 0.001f) return;
        if (characterVisual == null) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDir);

        characterVisual.rotation = Quaternion.Slerp(
            characterVisual.rotation,
            targetRotation,
            characterRotateSpeed * Time.deltaTime
        );
    }

    private void HandleGravityAndJump()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        if (controller.isGrounded && input.JumpDown)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void SetMode(bool firstPerson)
    {
        if (isFirst == firstPerson)
        {
            if (firstCam != null) firstCam.SetActive(isFirst);
            if (thirdCam != null) thirdCam.SetActive(!isFirst);
            return;
        }

        isFirst = firstPerson;

        if (firstCam != null) firstCam.SetActive(isFirst);
        if (thirdCam != null) thirdCam.SetActive(!isFirst);

        if (headLookIK != null)
            headLookIK.enableIK = isFirst;

        if (isFirst)
        {
            // 1인칭 진입 시 모델 방향을 루트와 맞춤
            if (characterVisual != null)
                characterVisual.localRotation = Quaternion.identity;

            if (headPivot != null)
                headPivot.localRotation = Quaternion.Euler(firstPitch, 0f, 0f);
        }
        else
        {
            // 3인칭 진입 시 1인칭 머리 상하 회전 초기화
            firstPitch = 0f;

            if (headPivot != null)
                headPivot.localRotation = Quaternion.identity;

            // 3인칭 카메라는 현재 루트 방향 기준에서 시작
            thirdYaw = 0f;

            if (thirdCameraPivot != null)
                thirdCameraPivot.localRotation = Quaternion.Euler(thirdPitch, thirdYaw, 0f);
        }
    }

    private Transform GetActiveCameraTransform()
    {
        if (isFirst && firstCam != null)
            return firstCam.transform;

        if (!isFirst && thirdCam != null)
            return thirdCam.transform;

        return null;
    }
}