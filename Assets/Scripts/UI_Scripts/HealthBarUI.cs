using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    public Health target;
    public Transform fill; // FillPanel

    void Start()
    {
        if (target != null)
        {
            target.OnHpChanged += UpdateBar;
            UpdateBar(target.currentHp, target.maxHp);
        }
    }

    void OnDestroy()
    {
        if (target != null)
            target.OnHpChanged -= UpdateBar;
    }

    void UpdateBar(int current, int max)
    {
        float ratio = (float)current / max;

        Vector3 scale = fill.localScale;
        scale.x = ratio;
        fill.localScale = scale;
    }
}