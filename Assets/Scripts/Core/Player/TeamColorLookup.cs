using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewTeamColorLookup", menuName ="Team Color Lookup")]
public class TeamColorLookup : ScriptableObject
{
    private Color[] teamColors = { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.gray, Color.white, Color.cyan };

    public Color GetTeamColor(int teamIndex)
    {
        if (teamIndex < 0 || teamIndex > teamColors.Length)
        {
            return Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
        else
        {
            return teamColors[teamIndex];
        }
    }
}
