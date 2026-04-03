using System.Xml.Serialization;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickItem : MonoBehaviour, IPickup
{
    public ItemData item;
    public int amount = 1;

    void OnEnable()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (false == other.gameObject.CompareTag("Player")) return;
        var inv = other.gameObject.GetComponentInParent<Inventory>();
        if (inv == null) return;
        TryPickup(inv);
    }

    //----------- interfaces
    public void TryPickup(Inventory inv)
    {
        if (inv.Add(item, amount))
        {
            gameObject.SetActive(false);
        }
    }
}
