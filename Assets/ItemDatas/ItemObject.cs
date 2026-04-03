using UnityEngine;

public abstract class ItemObject : MonoBehaviour
{
    [field: SerializeField] public ItemData Data { get; private set; }
    protected PlayerHand hand;

    public virtual void Init(PlayerHand hand, ItemData data)
    {
        this.hand = hand;
        this.Data = data;
    }

    public abstract void Tick();

    public abstract void Execute();
}
