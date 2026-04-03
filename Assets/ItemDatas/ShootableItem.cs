using UnityEngine;

public class ShootableItem : ItemObject
{
    [Header("Refs")]
    public Transform firePoint;

    [Header("Fire")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public int poolSize = 20;

    GameObject[] pool;
    int poolIndex;

    public override void Tick()
    {
        // 발사 아이템은 Tick에서 할 일이 없음
    }

    public override void Init(PlayerHand hand, ItemData data)
    {
        base.Init(hand, data);

        if (firePoint == null) firePoint = transform;

        BuildPool();
        poolIndex = 0;
    }

    void OnDisable()
    {
        // 풀 오브젝트 정리(선택: 풀을 계속 재사용하고 싶으면 제거해도 됨)
        // 장착 해제할 때 Destroy 되니까 굳이 없어도 됨.
    }

    public override void Execute()
    {
        Fire();
    }

    void BuildPool()
    {
        if (projectilePrefab == null || poolSize <= 0) return;

        pool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            pool[i] = obj;
        }
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
