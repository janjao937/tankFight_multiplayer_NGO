using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using System.Text;
using Unity.Services.Authentication;

public class HostGameManager : IDisposable
{
    private const string _gameSceneName = "Game";
    private const string _connectionType = "dtls";
    private Allocation allocation;
    private string joinCode = default;
    private string lobbyId = default;
    private const int MaxConnections = 20;

    public NetworkServer NetworkServer{get;private set;}
    public async Task StartHostAsync()
    {
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        try
        {
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(allocation, _connectionType);
        transport.SetRelayServerData(relayServerData);

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>(){
                {
                    "JoinCode",new DataObject(visibility:DataObject.VisibilityOptions.Member,value:joinCode)
                }
            };

            string playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Unknow");
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", MaxConnections, lobbyOptions);
            this.lobbyId = lobby.Id;
            HostSingleton.Instance.StartCoroutine(HearbeatLobby(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return;
        }
        NetworkServer = new NetworkServer(NetworkManager.Singleton);

        UserData userData = new UserData()
        {
            UserName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
            UserAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);
        byte[] payloadByte = Encoding.UTF8.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadByte;

        NetworkManager.Singleton.StartHost();
        NetworkServer.OnClientLeft += HandleClientLeft;

        NetworkManager.Singleton.SceneManager.LoadScene(_gameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private IEnumerator HearbeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    public void Dispose()
    {
       ShutDown();
    }

    public async void ShutDown()
    {
         HostSingleton.Instance?.StopCoroutine(nameof(HearbeatLobby));//fix error is dont have host singleton

        if (!string.IsNullOrEmpty(lobbyId))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
            lobbyId = string.Empty;
        }
        NetworkServer.OnClientLeft -= HandleClientLeft;
        NetworkServer?.Dispose();
    }

    private async void HandleClientLeft(string authId)
    {
       try{
            await LobbyService.Instance.RemovePlayerAsync(lobbyId,authId);

       }
       catch(LobbyServiceException e){
        Debug.Log(e);
       }
    }
}
