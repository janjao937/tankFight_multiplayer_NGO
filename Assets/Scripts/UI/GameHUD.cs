using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;


public class GameHUD : NetworkBehaviour
{
    [SerializeField] private TMP_Text lobbyCodeText = default;
    private NetworkVariable<FixedString32Bytes> lobbyCode = new NetworkVariable<FixedString32Bytes>(""); 
    // private string hostLobbyCode = "";

  
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            lobbyCode.OnValueChanged += HandleLobbyCodeChange;
            HandleLobbyCodeChange(string.Empty, lobbyCode.Value);
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
        ClientSingleton.Instance.ClientGameManager.UserData.UserGamePreferences.GameQueue = GameQueue.Solo;//test fixbug leaderbord
    }

    private void HandleLobbyCodeChange(FixedString32Bytes oldCode, FixedString32Bytes newCode)
    {
        lobbyCodeText.text = newCode.ToString();
    }

    private void UpdateLobbyCodeUI(string newCode)
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
