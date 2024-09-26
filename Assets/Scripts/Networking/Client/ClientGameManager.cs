using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ClientGameManager : IDisposable
{
    private const string _menuSceneName = "Menu";
    private const string _connectionType = "dtls";
    private JoinAllocation allocation;
    private NetworkClient networkClient;
    private MatchplayMatchmaker matchmaker;
    public UserData UserData { get; private set; }
    public async Task<bool> InitAsync()
    {
        //Authenticate player
        await UnityServices.InitializeAsync();

        networkClient = new NetworkClient(NetworkManager.Singleton);
        matchmaker = new MatchplayMatchmaker();

        AuthState authState = await AuthenticationWrapper.GetAuth();

        if (authState == AuthState.Authenticated)
        {
            this.UserData = new UserData
            {
                UserName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
                UserAuthId = AuthenticationService.Instance.PlayerId
            };
            return true;
        }
        return false;
    }
    private void ConnectClient()
    {
        string payload = JsonUtility.ToJson(UserData);
        byte[] payloadByte = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadByte;
        NetworkManager.Singleton.StartClient();
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            return;
        }
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(allocation, _connectionType);
        transport.SetRelayServerData(relayServerData);

        // string payload = JsonUtility.ToJson(userData);
        // byte[] payloadByte = Encoding.UTF8.GetBytes(payload);

        // NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadByte;
        // NetworkManager.Singleton.StartClient();
        ConnectClient();

    }
    private async Task<MatchmakerPollingResult> GetMatchAsync()
    {
        MatchmakingResult matchmakingResult = await matchmaker.Matchmake(UserData);
        if (matchmakingResult.result == MatchmakerPollingResult.Success)
        {
            //connect server
            StartClient(matchmakingResult.ip, matchmakingResult.port);
        }
        return matchmakingResult.result;

    }
    public async void MatchmakeAsync(bool isTeamQueue, Action<MatchmakerPollingResult> onMatchmakeResponse)
    {
        if (matchmaker.IsMatchmaking) { return; }

        UserData.UserGamePreferences.GameQueue = isTeamQueue ? GameQueue.Team : GameQueue.Solo;
        MatchmakerPollingResult matchResult = await GetMatchAsync();
        onMatchmakeResponse?.Invoke(matchResult);
    }
    public async Task CancelMatchmaking()
    {
        await matchmaker.CancelMatchmaking();
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(_menuSceneName);
    }

    public void StartClient(string ip, int port)
    {

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, (ushort)port);
        ConnectClient();
    }

    public void Disconnect()
    {
        networkClient.Disconnect();
    }

    public void Dispose()
    {
        networkClient?.Dispose();
    }


}
