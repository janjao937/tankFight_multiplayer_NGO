using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Image healPowerBar;
    [Header("Setting")]
    [SerializeField] private int maxHealPower = 30;
    [SerializeField] private float healCooldown = 60f;
    [SerializeField] private float healTickRate = 1f;
    [SerializeField] private int coinsPerTick = 10;
    [SerializeField] private int healthPerTick = 10;

    private List<TankPlayer> playersInZone = new List<TankPlayer>();


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsServer) { return; }
        if (!col.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }
        playersInZone.Add(player);
        Debug.Log("Entered Healing Zone:"+player.PlayerName.Value);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!IsServer) { return; }
        if (!col.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }
        playersInZone.Remove(player);
        Debug.Log("Exit Healing Zone:"+player.PlayerName.Value);
    }
}
