using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager networkManager;
    private const string _menuSceneName = "Menu";


    public NetworkClient(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong id)
    {
        if (id != 0 && id != networkManager.LocalClientId) { return; }

        Disconnect();
    }
    public void Disconnect()
    {
        if (SceneManager.GetActiveScene().name != _menuSceneName)
        {
            SceneManager.LoadScene(_menuSceneName);
        }

        if (networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        if (networkManager != null)
        {
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }
}
