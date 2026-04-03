using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class ShopUI : MonoBehaviour
{
    [Header("Root")]
    public GameObject root;
    public PlayerHand hand;

    [Header("Texts")]
    public TMP_Text coinText;
    public TMP_Text infoText;

    [Header("Exchange")]
    public ItemData stoneItem;
    public int coinCost = 1;
    public int stoneAmount = 1;

    Inventory currentInventory;
    CharacterMovement currentMovement;

    bool isOpen = false;
    public bool IsOpen => isOpen;

    void Start()
    {
        CloseImmediate();
    }

    void Update()
    {
        if (!isOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void Open(Inventory inventory, CharacterMovement movement)
    {
        currentInventory = inventory;
        currentMovement = movement;
        isOpen = true;

        if (root != null)
            root.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (currentMovement != null)
            currentMovement.Move = false;

        RefreshUI();

        if (infoText != null)
            infoText.text = "";
    }

    public void Close()
    {
        isOpen = false;
        hand.UIOpen = false;

        if (root != null)
            root.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (currentMovement != null)
            currentMovement.Move = true;
    }

    void CloseImmediate()
    {
        isOpen = false;

        if (root != null)
            root.SetActive(false);
    }

    public void OnClickExchangeStone()
    {
        if (currentInventory == null)
            return;

        if (stoneItem == null)
        {
            if (infoText != null)
                infoText.text = "돌 아이템이 연결되지 않았습니다.";
            return;
        }

        // 1) 코인 차감
        bool paid = currentInventory.UseCoins(coinCost);
        if (!paid)
        {
            if (infoText != null)
                infoText.text = "Not enough Coin!";
            RefreshUI();
            return;
        }

        // 2) 돌 지급
        bool added = currentInventory.Add(stoneItem, stoneAmount);

        // 3) 인벤토리 꽉 찼으면 환불
        if (!added)
        {
            currentInventory.AddCoins(coinCost);

            if (infoText != null)
                infoText.text = "인벤토리가 가득 차서 교환할 수 없습니다.";

            RefreshUI();
            return;
        }

        if (infoText != null)
            infoText.text = $"Coin {coinCost} -> Block {stoneAmount}";

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (currentInventory == null) return;

        if (coinText != null)
            coinText.text = $"Coin : {currentInventory.CurrentCoins}";
    }
}