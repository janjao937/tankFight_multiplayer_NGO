using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorDisplay : MonoBehaviour
{
    [SerializeField] private TeamColorLookup teamColorLookup;
    [SerializeField] private TankPlayer player;
    [SerializeField] private SpriteRenderer[] playerRenderers;

    private void Start()
    {
        HandleTeamChange(-1, player.TeamIndex.Value);
        player.TeamIndex.OnValueChanged += HandleTeamChange;
    }

    private void HandleTeamChange(int previousValue, int newValue)
    {
        Color teamColor = teamColorLookup.GetTeamColor(newValue);
        foreach (SpriteRenderer spriteRenderer in playerRenderers)
        {
            spriteRenderer.color = teamColor;
        }
    }

    private void OnDestroy()
    {
        player.TeamIndex.OnValueChanged -= HandleTeamChange;
    }
}
