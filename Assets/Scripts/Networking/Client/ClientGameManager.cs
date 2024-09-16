using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    private const string _menuSceneName = "Menu";
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

    public void GoToMenu()
    {
        SceneManager.LoadScene(_menuSceneName);
    }
}
