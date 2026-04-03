using UnityEngine;

public class HotbarScrollController : MonoBehaviour
{
    public InputManager input;
    public Inventory inv;

    void Awake()
    {
        if (input == null) input = GetComponent<InputManager>();
        if (inv == null) inv = GetComponent<Inventory>();

        if (input == null) input = GetComponentInParent<InputManager>();
        if (inv == null) inv = GetComponentInParent<Inventory>();
    }

    void Update()
    {
        if (input == null || inv == null) return;

        int d = input.ScrollDelta;
        if (d == 1) inv.SelectPrev();        // ÈÙ¾÷
        else if (d == -1) inv.SelectNext();  // ÈÙ´Ù¿î
    }
}
