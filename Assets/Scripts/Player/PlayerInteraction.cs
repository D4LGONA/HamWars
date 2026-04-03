using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Camera cam;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Inventory Inventory;
    [SerializeField] private PlayerHand hand;

    [Header("Interact")]
    [SerializeField] private float interactDistance = 10f;

    void Awake()
    {
        if (inputManager == null)
            inputManager = GetComponent<InputManager>();

        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        if (inputManager == null) return;

        if (inputManager.RightClickDown)
        {
            Debug.Log("rclickdown");
            TryInteract();
        }
    }

    void TryInteract()
    {
        if (cam == null)
            return;

        Ray ray = cam.ScreenPointToRay(
            new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f)
        );

        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red, 10f);

        if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            Debug.Log($"Raycast Fail | origin: {ray.origin}, dir: {ray.direction}, dist: {interactDistance}");
            return;
        }

        Debug.Log($"Hit: {hit.collider.name}");

        CoinSpawner spawner = hit.collider.GetComponentInParent<CoinSpawner>();
        if (spawner != null)
        {
            int amount = spawner.CollectAllCoins();

            if (amount > 0 && Inventory != null)
            {
                Inventory.AddCoins(amount);
                Debug.Log($"코인 {amount} 획득");
            }
            else
            {
                Debug.Log("회수할 코인이 없습니다.");
            }

            return;
        }

        Shop shop = hit.collider.GetComponentInParent<Shop>();
        if (shop != null)
        {
            CharacterMovement movement = GetComponent<CharacterMovement>();
            if (movement == null)
                movement = GetComponentInParent<CharacterMovement>();

            if (Inventory != null && movement != null)
            {
                hand.UIOpen = true;
                shop.OpenShop(Inventory, movement);
                Debug.Log("상점 UI 열기");
            }
            else
            {
                Debug.LogWarning("Inventory 또는 CharacterMovement가 없습니다.");
            }

            return;
        }
    }
}