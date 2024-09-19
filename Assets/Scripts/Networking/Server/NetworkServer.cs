using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;
    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();

    public Action<string> OnClientLeft;
    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += OnNetworkReady;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse res)
    {
        string payload = System.Text.Encoding.UTF8.GetString(req.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        clientIdToAuth[req.ClientNetworkId] = userData.UserAuthId;
        authIdToUserData[userData.UserAuthId] = userData;

        res.Approved = true;
        res.Position = SpawnPoint.GetRandomSpawnPos();
        res.Rotation = Quaternion.identity;
        res.CreatePlayerObject = true;

        // Debug.Log(userData.UserName);
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
            authIdToUserData.Remove(autId);

            OnClientLeft?.Invoke(autId);
        }
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
