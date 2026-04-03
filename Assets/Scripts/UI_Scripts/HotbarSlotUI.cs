using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlotUI : MonoBehaviour
{
    [Header("Refs")]
    public Image background;          // 슬롯 배경 이미지(패널 Image)
    public TMP_Text nameText;         // name 텍스트(TMP)

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(0.2f, 0.55f, 1f, 1f); // 파란색

    int index;

    public void SetIndex(int i) => index = i;

    public void SetStack(ItemStack stack)
    {
        if (stack.IsEmpty || stack.item == null)
        {
            if (nameText) nameText.text = "";
            return;
        }

        if (nameText) nameText.text = stack.item.itemName;
    }

    public void SetSelected(bool selected)
    {
        if (background) background.color = selected ? selectedColor : normalColor;
    }
}
