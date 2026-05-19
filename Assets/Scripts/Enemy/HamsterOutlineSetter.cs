using UnityEngine;

[RequireComponent(typeof(HamsterTeamInfo))]
public class HamsterOutlineSetter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HamsterTeamInfo teamInfo;
    [SerializeField] private Outline outline;

    [Header("Outline Settings")]
    [SerializeField] private Color allyColor = Color.blue;
    [SerializeField] private Color enemyColor = Color.red;
    [SerializeField] private float outlineWidth = 3f;

    private void Awake()
    {
        if (teamInfo == null)
            teamInfo = GetComponent<HamsterTeamInfo>();

        if (outline == null)
            outline = GetComponent<Outline>();

        ApplyOutline();
    }

    private void OnValidate()
    {
        if (teamInfo == null)
            teamInfo = GetComponent<HamsterTeamInfo>();

        if (outline == null)
            outline = GetComponent<Outline>();

        ApplyOutline();
    }

    public void ApplyOutline()
    {
        if (teamInfo == null || outline == null)
            return;

        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineWidth = outlineWidth;

        switch (teamInfo.team)
        {
            case HamsterTeam.Ally:
                outline.OutlineColor = allyColor;
                break;

            case HamsterTeam.Enemy:
                outline.OutlineColor = enemyColor;
                break;
        }
    }
}