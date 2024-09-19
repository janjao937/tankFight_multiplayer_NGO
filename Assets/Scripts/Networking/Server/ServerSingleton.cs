using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

public class ServerSingleton : MonoBehaviour
{
    private static ServerSingleton instance;
    public ServerGameManager ServerGameManager { get; private set; }

    public static ServerSingleton Instance
    {
        get
        {
            if (instance != null) { return instance; }
            instance = FindObjectOfType<ServerSingleton>();

            if (instance == null)
            {
                Debug.LogWarning("No ServerSingleton in scene");
                return null;
            }
            return instance;
        }

    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public async Task CreateServer()
    {
        await UnityServices.InitializeAsync();
        ServerGameManager = new ServerGameManager(
            ApplicationData.IP(),
            ApplicationData.Port(),
            ApplicationData.QPort(),
            NetworkManager.Singleton
            );
    }
    private void OnDestroy()
    {
        ServerGameManager?.Dispose();
    }
}
