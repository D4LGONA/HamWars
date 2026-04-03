using UnityEngine;

public class PlaceableItem : ItemObject
{
    [Header("Placement")]
    public float placeDistance = 4f;

    [Header("Snap")]
    public float gridSize = 1f;
    public float surfaceOffset = 0.01f;
    public bool snapYToGrid = false;

    [Header("Ghost")]
    public Material ghostMaterial;

    [Header("Ghost Color")]
    public Color canPlaceColor = new Color(0.20f, 0.95f, 0.75f, 0.45f);
    public Color cannotPlaceColor = new Color(1.00f, 0.35f, 0.25f, 0.45f);

    GameObject ghost;
    Renderer[] ghostRenderers;
    MaterialPropertyBlock mpb;

    static readonly int ColorID2 = Shader.PropertyToID("_Color");

    void OnDisable()
    {
        if (ghost != null) Destroy(ghost);
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

        SetGhostColor(hitOk ? canPlaceColor : cannotPlaceColor);
    }

    public override void Execute()
    {
        if (Data == null || Data.heldPrefab == null || !Data.placeable) return;
        if (hand == null || hand.cam == null || hand.inv == null) return;

        if (!TryGetPose(out var pos, out var rot)) return;

        if (false == hand.inv.Remove(Data, 1)) return;

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

        if (snapMode)
            pos = GetSnappedPosition(hit);
        else
            pos = hit.point + hit.normal * surfaceOffset;

        rot = Quaternion.identity;
        return true;
    }

    Vector3 GetSnappedPosition(RaycastHit hit)
    {
        Vector3 p = hit.point + hit.normal * surfaceOffset;

        p.x = Snap(p.x, gridSize);
        p.z = Snap(p.z, gridSize);

        if (snapYToGrid)
            p.y = Snap(p.y, gridSize);

        return p;
    }

    float Snap(float value, float size)
    {
        if (size <= 0f) return value;
        return Mathf.Round(value / size) * size;
    }

    void EnsureGhost()
    {
        if (ghost != null) return;

        ghost = Instantiate(Data.heldPrefab);
        ghost.transform.localPosition = Vector3.zero;

        foreach (var col in ghost.GetComponentsInChildren<Collider>(true))
            col.enabled = false;

        foreach (var rb in ghost.GetComponentsInChildren<Rigidbody>(true))
            rb.isKinematic = true;

        ghostRenderers = ghost.GetComponentsInChildren<Renderer>(true);
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
            r.GetPropertyBlock(mpb);
            mpb.SetColor(ColorID2, c);
            r.SetPropertyBlock(mpb);
        }
    }
}