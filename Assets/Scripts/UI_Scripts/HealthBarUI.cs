using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Health target;
    public Image fill; // 빨간 체력바 이미지

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
        fill.fillAmount = ratio;
    }
}