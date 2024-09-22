using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;


public class GameHUD : NetworkBehaviour
{
    [SerializeField] private TMP_Text lobbyCodeText = default;
    private NetworkVariable<int> lobbyCode = new NetworkVariable<int>(0);

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            lobbyCode.OnValueChanged += HandleLobbyCodeChange;
            HandleLobbyCodeChange(0, lobbyCode.Value);
        }
        if (!IsHost) return;
        
        // lobbyCode.Value = HostSingleton.Instance.HostGameManager.JoinCode;
    }
    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            lobbyCode.OnValueChanged -= HandleLobbyCodeChange;
        }
    }

    private void HandleLobbyCodeChange(int oldCode, int newCode)
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
