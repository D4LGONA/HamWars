using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    public Inventory inv;
    public HotbarSlotUI[] slotUIs; // Panel(1)~Panel(11)에 붙인 SlotUI들 드래그로 넣기
    public int startSlotIndex = 0; // 인벤 슬롯 0번부터 보여줄거면 0

    void OnEnable()
    {
        if (inv == null) inv = FindFirstObjectByType<Inventory>();
        if (inv == null) return;

        inv.OnChanged += RefreshAll;
        inv.OnSlotChanged += Highlight;

        RefreshAll();
        Highlight(inv.currentSlotIndex);
    }

    void OnDisable()
    {
        if (inv != null)
        {
            inv.OnChanged -= RefreshAll;
            inv.OnSlotChanged -= Highlight;
        }
    }

    void RefreshAll()
    {
        var slots = inv.GetSlots();

        for (int ui = 0; ui < slotUIs.Length; ui++)
        {
            int invIdx = startSlotIndex + ui;
            var stack = (invIdx >= 0 && invIdx < slots.Length) ? slots[invIdx] : default;
            slotUIs[ui].SetIndex(invIdx);
            slotUIs[ui].SetStack(stack);
        }
    }

    void Highlight(int invSelectedIndex)
    {
        for (int ui = 0; ui < slotUIs.Length; ui++)
        {
            int invIdx = startSlotIndex + ui;
            slotUIs[ui].SetSelected(invIdx == invSelectedIndex);
        }
    }
}
