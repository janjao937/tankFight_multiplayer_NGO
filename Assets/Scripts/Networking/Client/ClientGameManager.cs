using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    private const string _menuSceneName = "Menu";
     private const string _connectionType = "dtls";
    private JoinAllocation allocation;
    public async Task<bool> InitAsync()
    {
        //Authenticate player
        await UnityServices.InitializeAsync();
        AuthState authState = await AuthenticationWrapper.GetAuth();
        if (authState == AuthState.Authenticated)
        {
            return true;
        }
        return false;
    }
    public async Task StartClientAsync(string joinCode){
        try{
        allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

        }
        catch(Exception ex){
            Debug.Log(ex);
            return;
        }
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(allocation,_connectionType);
        transport.SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartClient();

    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(_menuSceneName);
    }
}
