using UnityEngine;

public interface IDroppable
{
    void Drop(Vector3 hitPoint, Vector3 hitNormal);
}

public interface IDamageable
{
    void Hit(Vector3 hitPoint, Vector3 hitNormal);
}

public interface IPickup
{
    void TryPickup(Inventory inv);
}
