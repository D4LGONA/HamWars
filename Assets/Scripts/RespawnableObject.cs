using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class RespawnableObject : MonoBehaviour, IDamageable, IDroppable
{
    [Header("설정")]
    public int hitsToBreak = 3;     // n
    public float respawnCooldown = 10f;

    [Header("쿨다운 표시(UI)")]
    public TMP_Text cooldownText;      // 오브젝트 위에 띄울 텍스트
    public Transform textPivot;        // 텍스트 위치(옵션). 비우면 transform 기준
    public float showDistance = 10.0f;  // 가까이 가면 표시

    [Header("아이템 드롭 설정")]
    public GameObject dropItem;

    int hitCount = 0;
    bool broken = false;

    Collider col;
    Renderer[] rends;

    // 쿨다운 계산용
    float respawnEndTime = -1f;
    Transform player;

    void Awake()
    {
        col = GetComponent<Collider>();
        rends = GetComponentsInChildren<Renderer>(true);

        // 플레이어 찾기(플레이어 오브젝트에 Tag를 Player로 해줘)
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;

        if (cooldownText)
            cooldownText.gameObject.SetActive(false);
    }



    void Break()
    {
        broken = true;

        // "없어짐" 처리 (Destroy 안 하고 숨김)
        if (col) col.enabled = false;
        foreach (var r in rends) r.enabled = false;

        // 쿨다운 끝나는 시각 기록
        respawnEndTime = Time.time + respawnCooldown;

        // 쿨다운 시작
        StartCoroutine(CoRespawn());
    }

    IEnumerator CoRespawn()
    {
        yield return new WaitForSeconds(respawnCooldown);

        // 리스폰(다시 보이기) + 초기화
        hitCount = 0;
        broken = false;
        respawnEndTime = -1f;

        if (col) col.enabled = true;
        foreach (var r in rends) r.enabled = true;

        if (cooldownText)
            cooldownText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!cooldownText || player == null) return;

        // 파괴 상태일 때만 표시 의미 있음
        if (!broken)
        {
            if (cooldownText.gameObject.activeSelf)
                cooldownText.gameObject.SetActive(false);
            return;
        }

        float dist = Vector3.Distance(player.position, transform.position);
        if (dist > showDistance)
        {
            if (cooldownText.gameObject.activeSelf)
                cooldownText.gameObject.SetActive(false);
            return;
        }

        // 남은 시간(초)
        float remain = Mathf.Max(0f, respawnEndTime - Time.time);
        int remainSec = Mathf.CeilToInt(remain);

        cooldownText.gameObject.SetActive(true);
        cooldownText.text = $"{remainSec}s";

        // 카메라를 바라보게(빌보드)
        if (player)
        {
            Vector3 dir = player.transform.position - cooldownText.transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
                cooldownText.transform.rotation = Quaternion.LookRotation(-dir);
        }

    }

    // --------------------- Interfaces ---------------------

    public void Drop(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dropItem) return;

        Vector3 spawnPos = hitPoint + hitNormal * 0.05f;
        GameObject go = Instantiate(dropItem, spawnPos, Quaternion.Euler(90f, 0f, 0f));
        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb)
        {
            // 랜덤 옆방향(수평)
            Vector3 side = Random.insideUnitSphere;
            side.y = 0f;
            if (side.sqrMagnitude < 0.0001f) side = Vector3.forward;
            side.Normalize();

            float upForce = 3.5f;
            float sideForce = 3.5f;

            Vector3 force = Vector3.up * upForce + side * sideForce;
            rb.AddForce(force, ForceMode.Impulse);

            // 살짝 회전도 주면 더 "뿅"
            rb.AddTorque(Random.insideUnitSphere * 0.5f, ForceMode.Impulse);
        }
    }

    public void Hit(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (broken) return;
        hitCount++;

        Drop(hitPoint, hitNormal);

        if (hitCount >= hitsToBreak)
            Break();
    }
}
