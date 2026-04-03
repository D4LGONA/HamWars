using UnityEngine;

public class PlaceableItem : ItemObject
{
    [Header("Placement")]
    public float placeDistance = 5f;
    public float surfaceOffset = 0.03f;

    [Header("Snap")]
    public float gridSize = 1f;

    [Header("Ghost")]
    public Material ghostMaterial;

    [Header("Ghost Color")]
    public Color freeCanPlaceColor = new Color(0.20f, 0.95f, 0.75f, 0.45f);
    public Color snapCanPlaceColor = new Color(1.00f, 0.85f, 0.20f, 0.45f);
    public Color cannotPlaceColor = new Color(1.00f, 0.35f, 0.25f, 0.45f);

    GameObject ghost;
    Renderer[] ghostRenderers;
    BoxCollider[] ghostBoxes;
    MaterialPropertyBlock mpb;

    static readonly int ColorID = Shader.PropertyToID("_Color");

    void OnDisable()
    {
        if (ghost != null)
            Destroy(ghost);
    }

    public override void Tick()
    {
        if (Data == null || Data.heldPrefab == null || !Data.placeable)
        {
            SetGhost(false);
            return;
        }

        EnsureGhost();

        bool hitOk = TryGetPose(out var pos, out var rot);

        SetGhost(true);
        ghost.transform.SetPositionAndRotation(pos, rot);

        bool snapMode = hand != null && hand.input != null && hand.input.SnapMode;
        bool canPlace = hitOk && CanPlaceAtCurrentGhost();

        if (!canPlace)
            SetGhostColor(cannotPlaceColor);
        else
            SetGhostColor(snapMode ? snapCanPlaceColor : freeCanPlaceColor);
    }

    public override void Execute()
    {
        if (Data == null || Data.heldPrefab == null || !Data.placeable) return;
        if (hand == null || hand.cam == null || hand.inv == null) return;

        EnsureGhost();

        if (!TryGetPose(out var pos, out var rot)) return;

        ghost.transform.SetPositionAndRotation(pos, rot);

        if (!CanPlaceAtCurrentGhost()) return;
        if (!hand.inv.Remove(Data, 1)) return;

        Instantiate(Data.heldPrefab, pos, rot);
    }

    bool TryGetPose(out Vector3 pos, out Quaternion rot)
    {
        pos = default;
        rot = Quaternion.identity;

        if (hand == null || hand.cam == null)
            return false;

        Ray ray = hand.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (!Physics.Raycast(ray, out RaycastHit hit, placeDistance))
        {
            pos = ray.origin + ray.direction * placeDistance;
            return false;
        }

        bool snapMode = hand.input != null && hand.input.SnapMode;

        // 나중에 회전 추가하면 여기서 rot 계산
        rot = Quaternion.identity;

        // pushOut 계산용으로 ghost를 먼저 hit.point 위치에 둠
        ghost.transform.SetPositionAndRotation(hit.point, rot);

        float pushOut = GetPushOutDistance(hit.normal);
        Vector3 candidatePos = hit.point + hit.normal * (pushOut + surfaceOffset);

        if (snapMode)
        {
            candidatePos.x = SnapFloor(candidatePos.x, gridSize);
            candidatePos.y = SnapFloor(candidatePos.y, gridSize) + 0.5f;
            candidatePos.z = SnapFloor(candidatePos.z, gridSize);
        }

        pos = candidatePos;
        return true;
    }

    float SnapFloor(float value, float size)
    {
        if (size <= 0f) return value;
        return Mathf.Floor(value / size) * size;
    }

    float GetPushOutDistance(Vector3 worldNormal)
    {
        if (ghost == null || ghostBoxes == null || ghostBoxes.Length == 0)
            return 0f;

        Vector3 rootPos = ghost.transform.position;
        float maxPush = 0f;

        foreach (var box in ghostBoxes)
        {
            if (box == null) continue;

            // 루트 pivot -> box 중심까지의 월드 오프셋
            Vector3 centerOffset = box.bounds.center - rootPos;

            // normal 방향으로 center가 얼마나 나가 있는지
            float centerProjection = Vector3.Dot(worldNormal, centerOffset);

            // world normal 방향으로 box가 차지하는 반경
            Vector3 scaledSize = Vector3.Scale(box.size, box.transform.lossyScale);
            Vector3 extents = new Vector3(
                Mathf.Abs(scaledSize.x) * 0.5f,
                Mathf.Abs(scaledSize.y) * 0.5f,
                Mathf.Abs(scaledSize.z) * 0.5f
            );

            Vector3 localNormal = box.transform.InverseTransformDirection(worldNormal);
            localNormal = new Vector3(
                Mathf.Abs(localNormal.x),
                Mathf.Abs(localNormal.y),
                Mathf.Abs(localNormal.z)
            );

            float supportRadius =
                extents.x * localNormal.x +
                extents.y * localNormal.y +
                extents.z * localNormal.z;

            float push = centerProjection + supportRadius;

            if (push > maxPush)
                maxPush = push;
        }

        return maxPush;
    }

    bool CanPlaceAtCurrentGhost()
    {
        if (ghost == null) return false;
        if (ghostBoxes == null || ghostBoxes.Length == 0) return true;

        foreach (var box in ghostBoxes)
        {
            if (box == null) continue;

            Bounds b = box.bounds;

            Collider[] hits = Physics.OverlapBox(
                b.center,
                b.extents,
                box.transform.rotation,
                ~0,
                QueryTriggerInteraction.Ignore
            );

            foreach (var h in hits)
            {
                if (h == null) continue;

                // 고스트 자신 무시
                if (h.transform.IsChildOf(ghost.transform))
                    continue;

                // 트리거 무시
                if (h.isTrigger)
                    continue;

                return false;
            }
        }

        return true;
    }

    void EnsureGhost()
    {
        if (ghost != null) return;

        ghost = Instantiate(Data.heldPrefab);
        ghost.name = Data.heldPrefab.name + "_Ghost";

        foreach (var col in ghost.GetComponentsInChildren<Collider>(true))
            col.enabled = false;

        foreach (var rb in ghost.GetComponentsInChildren<Rigidbody>(true))
            rb.isKinematic = true;

        ghostRenderers = ghost.GetComponentsInChildren<Renderer>(true);
        ghostBoxes = ghost.GetComponentsInChildren<BoxCollider>(true);
        mpb = new MaterialPropertyBlock();

        if (ghostMaterial != null)
        {
            foreach (var r in ghostRenderers)
                r.sharedMaterial = ghostMaterial;
        }

        ghost.SetActive(false);
    }

    void SetGhost(bool on)
    {
        if (ghost != null && ghost.activeSelf != on)
            ghost.SetActive(on);
    }

    void SetGhostColor(Color c)
    {
        if (ghostRenderers == null) return;

        foreach (var r in ghostRenderers)
        {
            if (r == null) continue;

            r.GetPropertyBlock(mpb);
            mpb.SetColor(ColorID, c);
            r.SetPropertyBlock(mpb);
        }
    }
}