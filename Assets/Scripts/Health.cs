using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHp = 100;
    public int currentHp;

    public event Action<int, int> OnHpChanged; // current, max
    public event Action OnDie;

    void Awake()
    {
        currentHp = maxHp;
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void TakeDamage(int dmg)
    {
        currentHp -= dmg;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        OnHpChanged?.Invoke(currentHp, maxHp);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} »ç¸Á");
        OnDie?.Invoke();
    }
}