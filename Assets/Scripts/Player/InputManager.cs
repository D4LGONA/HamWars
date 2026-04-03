using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Mouse")]
    public bool useMouseLook = true;
    public float mouseSensitivity = 2.0f;

    public Vector2 Move { get; private set; }
    public bool JumpDown { get; private set; }
    public Vector2 LookDelta { get; private set; }
    public bool Firedown { get; private set; }
    public bool RightClickDown { get; private set; }
    public bool IsThird { get; private set; }
    public bool InventoryOpen { get; private set; }
    public int ScrollDelta { get; private set; }

    public bool SnapMode { get; private set; }   // Q 토글 스냅 모드

    void Awake()
    {
        if (useMouseLook)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
            ScrollDelta = 1;
        else if (Input.mouseScrollDelta.y < 0)
            ScrollDelta = -1;
        else
            ScrollDelta = 0;

        if (Input.GetKeyDown(KeyCode.V))
            IsThird = !IsThird;

        if (Input.GetKeyDown(KeyCode.E))
            InventoryOpen = !InventoryOpen;

        if (Input.GetKeyDown(KeyCode.Q))
            SnapMode = !SnapMode;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Move = new Vector2(h, v);
        Move = Vector2.ClampMagnitude(Move, 1f);

        Firedown = Input.GetMouseButton(0);
        RightClickDown = Input.GetMouseButtonDown(1);

        JumpDown = Input.GetKey(KeyCode.Space);

        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        LookDelta = new Vector2(mx, my) * mouseSensitivity;
    }
}