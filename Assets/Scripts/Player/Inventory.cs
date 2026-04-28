using System;
using UnityEngine;

public class Inventory : MonoBehaviour // 플레이어 자원 + 인벤토리 통합
{
    [Header("Inventory")]
    public int slotCount = 6;
    [SerializeField] private ItemStack[] slots;

    [Header("Default Item")]
    public ItemData gun;

    public int currentSlotIndex = 0;

    public event Action OnChanged;          // UI 갱신
    public event Action<int> OnSlotChanged; // 손 아이템 갱신

    [Header("Coins")]
    [SerializeField] private int currentCoins = 0;
    public int CurrentCoins => currentCoins;

    void Awake()
    {
        slots = new ItemStack[slotCount];

        if (gun != null)
        {
            slots[0].item = gun;
            slots[0].amount = 1;
            currentSlotIndex = 0;
        }

        OnChanged?.Invoke();
        OnSlotChanged?.Invoke(currentSlotIndex);
    }

    public ItemStack[] GetSlots() => slots;

    public void SelectNext()
    {
        currentSlotIndex = (currentSlotIndex + 1) % slots.Length;
        OnChanged?.Invoke();
        OnSlotChanged?.Invoke(currentSlotIndex);
    }

    public void SelectPrev()
    {
        currentSlotIndex = (currentSlotIndex - 1 + slots.Length) % slots.Length;
        OnChanged?.Invoke();
        OnSlotChanged?.Invoke(currentSlotIndex);
    }

    public bool Add(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return false;
        int left = amount;

        // 1) 기존 스택 채우기
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item && slots[i].amount < item.maxStack)
            {
                int space = item.maxStack - slots[i].amount;
                int add = Mathf.Min(space, left);
                slots[i].amount += add;
                left -= add;
                if (left <= 0)
                {
                    OnChanged?.Invoke();
                    OnSlotChanged?.Invoke(currentSlotIndex);
                    return true;
                }
            }
        }

        // 2) 빈 슬롯에 넣기
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty)
            {
                int add = Mathf.Min(item.maxStack, left);
                slots[i].item = item;
                slots[i].amount = add;
                left -= add;
                if (left <= 0)
                {
                    OnChanged?.Invoke();
                    OnSlotChanged?.Invoke(currentSlotIndex);
                    return true;
                }
            }
        }

        OnChanged?.Invoke();
        OnSlotChanged?.Invoke(currentSlotIndex);
        return false;
    }

    public bool Remove(ItemData item, int number)
    {
        if (item == null || number <= 0) return false;

        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];
            if (slot.IsEmpty) continue;

            if (slot.item == item)
            {
                slot.amount -= number;
                if (slot.amount <= 0) slot.Clear();

                slots[i] = slot;
                OnChanged?.Invoke();
                OnSlotChanged?.Invoke(i);
                return true;
            }
        }
        return false;
    }

    // 코인 관련

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        currentCoins += amount;
        OnChanged?.Invoke(); // UI 갱신 필요하면
    }

    public bool UseCoins(int amount)
    {
        if (amount <= 0 || currentCoins < amount) return false;

        currentCoins -= amount;
        OnChanged?.Invoke();
        return true;
    }
}