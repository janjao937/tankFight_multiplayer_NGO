using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkObject playerPrefab;
    private NetworkManager networkManager;
    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();
    public Action<UserData> OnUserJoined;
    public Action<UserData> OnUserLeft;
    public Action<string> OnClientLeft;
    public NetworkServer(NetworkManager networkManager, NetworkObject playerPrefab)
    {
        this.networkManager = networkManager;
        this.playerPrefab = playerPrefab;

        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += OnNetworkReady;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse res)
    {
        string payload = System.Text.Encoding.UTF8.GetString(req.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        clientIdToAuth[req.ClientNetworkId] = userData.UserAuthId;
        authIdToUserData[userData.UserAuthId] = userData;

        OnUserJoined?.Invoke(userData);

        _ = SpawnPlayerDelayed(req.ClientNetworkId);
        res.Approved = true;
        res.CreatePlayerObject = false;

        // Debug.Log(userData.UserName);
    }

    private async Task SpawnPlayerDelayed(ulong clientId)
    {
        await Task.Delay(1000);
        NetworkObject playerInit = GameObject.Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        playerInit.SpawnAsPlayerObject(clientId);
    }

    private void OnNetworkReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong id)
    {
        if (clientIdToAuth.TryGetValue(id, out string autId))
        {
            clientIdToAuth.Remove(id);
            OnUserLeft?.Invoke(authIdToUserData[autId]);
            authIdToUserData.Remove(autId);
            OnClientLeft?.Invoke(autId);
        }
    }
    public bool OpenConnection(string ip, int port)
    {
        UnityTransport transport = networkManager.gameObject.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, (ushort)port);
        return networkManager.StartServer();
    }
    public UserData GetUserDataByClientId(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            if (authIdToUserData.TryGetValue(authId, out UserData data))
            {
                return data;
            }
            return null;
        }
        return null;
    }
    public void Dispose()
    {
        if (networkManager == null) { return; }
        networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        networkManager.OnServerStarted -= OnNetworkReady;
        if (networkManager.IsListening)
        {
            networkManager.Shutdown();
        }
    }
}
