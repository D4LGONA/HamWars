using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    public Inventory inventory;
    public TMP_Text coinText;

    void Start()
    {
        if (inventory != null)
        {
            inventory.OnChanged += Refresh;
            Refresh();
        }
    }

    void OnDestroy()
    {
        if (inventory != null)
            inventory.OnChanged -= Refresh;
    }

    void Refresh()
    {
        if (inventory == null || coinText == null) return;

        coinText.text = inventory.CurrentCoins.ToString();
    }
}