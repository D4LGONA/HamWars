using UnityEngine;

public enum HamsterTeam
{
    Ally,
    Enemy
}

public class HamsterTeamInfo : MonoBehaviour
{
    public HamsterTeam team = HamsterTeam.Enemy;
}