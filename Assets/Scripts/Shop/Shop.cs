using UnityEngine;

public class Shop : MonoBehaviour
{
    public ShopUI shopUI;

    public void OpenShop(Inventory inventory, CharacterMovement movement)
    {
        if (shopUI == null) return;
        shopUI.Open(inventory, movement);
    }
}