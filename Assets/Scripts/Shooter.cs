using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Refs")]
    public InputManager input;
    public Transform firePoint;

    [Header("Fire")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public float fireDelay = 0.5f;
    public int poolSize = 20;

    private float nextFireTime;
    private GameObject[] pool;
    private int poolIndex;

    void Awake()
    {
        if (input == null) input = GetComponentInParent<InputManager>();

        pool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            pool[i] = obj;
        }

        nextFireTime = 0f;
        poolIndex = 0;
    }

    void Update()
    {
        if (false == input.Firedown) return;
        if (Time.time < nextFireTime) return;

        Fire();
        nextFireTime = Time.time + fireDelay;
    }

    void Fire()
    {
        var obj = GetFromPool();
        obj.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
        obj.SetActive(true);

        if (obj.TryGetComponent<Projectile>(out var proj))
        {
            proj.Launch(firePoint.forward, projectileSpeed);
        }
        else if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = firePoint.forward * projectileSpeed;
        }
    }

    GameObject GetFromPool()
    {
        var obj = pool[poolIndex];
        poolIndex = (poolIndex + 1) % pool.Length;
        return obj;
    }
}
