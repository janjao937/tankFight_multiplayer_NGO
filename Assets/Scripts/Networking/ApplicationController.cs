using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ServerSingleton serverPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicateServer)
    {
        if (isDedicateServer)
        {
            ServerSingleton serverSingleton = Instantiate(serverPrefab);
            await serverSingleton.CreateServer();
            await serverSingleton.ServerGameManager.StartGameServerAsync();
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost();

            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool authenticated = await clientSingleton.CreateClient();



            if (authenticated)
            {
                //change scene
                clientSingleton.ClientGameManager.GoToMenu();
            }

        }
    }

}
