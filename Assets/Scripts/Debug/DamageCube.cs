using System.Collections.Generic;
using UnityEngine;

public class DamageCube : MonoBehaviour
{
    public int damage = 5;
    public float damageInterval = 0.5f;

    private float timer = 0f;
    private readonly List<Health> targets = new List<Health>();

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < damageInterval)
            return;

        timer = 0f;

        for (int i = targets.Count - 1; i >= 0; i--)
        {
            if (targets[i] == null)
            {
                targets.RemoveAt(i);
                continue;
            }

            targets[i].TakeDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Health health = other.GetComponentInParent<Health>();
        if (health == null)
            return;

        if (!targets.Contains(health))
            targets.Add(health);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Health health = other.GetComponentInParent<Health>();
        if (health == null)
            return;

        targets.Remove(health);
    }
}