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
    private UserData userData;
    public async Task<bool> InitAsync()
    {
        //Authenticate player
        await UnityServices.InitializeAsync();

        networkClient = new NetworkClient(NetworkManager.Singleton);
        matchmaker = new MatchplayMatchmaker();

        AuthState authState = await AuthenticationWrapper.GetAuth();

        this.userData = new UserData
        {
            UserName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
            UserAuthId = AuthenticationService.Instance.PlayerId
        };

        if (authState == AuthState.Authenticated)
        {
            return true;
        }
        return false;
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

        // UserData userData = new UserData
        // {
        //     UserName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
        //     UserAuthId = AuthenticationService.Instance.PlayerId
        // };


        string payload = JsonUtility.ToJson(userData);
        byte[] payloadByte = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadByte;
        NetworkManager.Singleton.StartClient();

    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(_menuSceneName);
    }
    private async Task<MatchmakerPollingResult> GetMatchAsync()
    {
        MatchmakingResult matchmakingResult = await matchmaker.Matchmake(userData);
        if (matchmakingResult.result == MatchmakerPollingResult.Success)
        {
            //connect server

        }
        return matchmakingResult.result;

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
