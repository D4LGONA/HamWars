using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Physics")]
    public float lifeTime = 3f;
    public float damage = 10f;

    [Header("Impact VFX")]
    public ParticleSystem impactDustPrefab; 
    public float vfxLife = 2f;              

    Rigidbody rb;
    float disableAt;
    bool launched;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 물리 총알 추천 설정
        rb.useGravity = true;                 // 탄도 중력 쓸 거면 true
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 고속 탄 튐 방지
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void OnEnable()
    {
        // 풀에서 꺼내질 때 상태 리셋
        launched = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
    }

    public void Launch(Vector3 dir, float speed)
    {
        launched = true;

        dir = dir.normalized;

        // Sleep 해제
        rb.WakeUp();

        // 위치/회전은 발사자가 맞춰주고, 여기서는 속도만
        rb.velocity = dir * speed;

        disableAt = Time.time + lifeTime;
    }

    void Update()
    {
        if (launched && Time.time >= disableAt)
            gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!launched) return;

        var cp = collision.GetContact(0);

        if (impactDustPrefab != null)
        {
            var ps = Instantiate(
                impactDustPrefab,
                cp.point,
                Quaternion.LookRotation(cp.normal)
            );

            ps.Play();
            Destroy(ps.gameObject, vfxLife);
        }

        var dmg = collision.collider.GetComponentInParent<IDamageable>();
        if (dmg != null)
        {
            dmg.Hit(damage, cp.point, cp.normal);
        }

    }
}
