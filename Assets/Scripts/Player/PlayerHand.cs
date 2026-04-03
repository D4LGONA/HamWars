using Unity.VisualScripting;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public Inventory inv;
    public Transform itemAnchor;   // Camera 밑 빈 오브젝트
    public Camera cam;
    public InputManager input;

    GameObject currentHeld;
    ItemObject currentItemObj;

    [Header("Cooldown")]
    public float execCooldownTime = 0.5f;
    float lastExecTime = 0.0f;

    public bool UIOpen = false;

    void OnEnable()
    {
        if (inv == null) inv = GetComponentInParent<Inventory>();
        if (cam == null) cam = Camera.main;
        if (input == null) input = GetComponentInParent<InputManager>();

        if (inv != null)
        {
            inv.OnSlotChanged += HandleSlotChanged;
            EquipFromSlot(inv.currentSlotIndex);
        }
    }

    void OnDisable()
    {
        if (inv != null) inv.OnSlotChanged -= HandleSlotChanged;
    }

    void Update()
    {
        if (currentItemObj == null) return;

        currentItemObj.Tick();

        if (input == null || input.Firedown == false) return;
        if (Time.time < lastExecTime) return;
        lastExecTime = Time.time + execCooldownTime;
        if (false == UIOpen)
            currentItemObj.Execute();
    }

    // ---------------- Equip ----------------
    void HandleSlotChanged(int idx) => EquipFromSlot(idx);

    void EquipFromSlot(int idx)
    {
        if (inv == null) return;

        var slots = inv.GetSlots();
        if (idx < 0 || idx >= slots.Length) return;

        ItemStack stack = slots[idx];
        ItemData item = stack.IsEmpty ? null : stack.item;
        Equip(item);
    }
    
    void Equip(ItemData item) // 손에 든 장비 교체
    {
        if (currentHeld != null) Destroy(currentHeld);
        currentHeld = null;
        currentItemObj = null;

        if (item == null || item.heldPrefab == null) return;

        currentHeld = Instantiate(item.heldPrefab, itemAnchor);
        currentHeld.transform.localPosition = Vector3.zero;

        currentItemObj = currentHeld.GetComponentInChildren<ItemObject>();
        if (currentItemObj != null)
            currentItemObj.Init(this, item);
    }
}
