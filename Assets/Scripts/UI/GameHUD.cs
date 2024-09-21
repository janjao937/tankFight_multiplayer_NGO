using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameHUD : NetworkBehaviour
{
    [SerializeField] private TMP_Text lobbyCodeText = default;
    private NetworkVariable<FixedString32Bytes> lobbyCode = new NetworkVariable<FixedString32Bytes>("");

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            lobbyCode.OnValueChanged += HandleLobbyCodeChange;
            HandleLobbyCodeChange(string.Empty,lobbyCode.Value);
        }
        if (!IsHost) return;

        lobbyCode.Value = HostSingleton.Instance.HostGameManager.JoinCode;
    }
    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            lobbyCode.OnValueChanged -= HandleLobbyCodeChange;
        }
    }

    private void HandleLobbyCodeChange(FixedString32Bytes oldCode, FixedString32Bytes newCode)
    {
        lobbyCodeText.text = newCode.ToString();
    }

    public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.HostGameManager.ShutDown();
        }

        ClientSingleton.Instance.ClientGameManager.Disconnect();
    }
}
