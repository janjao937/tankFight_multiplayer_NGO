using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ServerSingleton serverPrefab;
    [SerializeField] private NetworkObject playerPrefab;

    private const string _gameSceneName = "Game";
    private ApplicationData appData;

    
    private async void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicateServer)
    {
        if (isDedicateServer)
        {
            Application.targetFrameRate = 60;
            appData = new ApplicationData();

            ServerSingleton serverSingleton = Instantiate(serverPrefab);

            StartCoroutine(LoadGameSceneAsync(serverSingleton));
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost(playerPrefab);

            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool authenticated = await clientSingleton.CreateClient();



            if (authenticated)
            {
                //change scene
                clientSingleton.ClientGameManager.GoToMenu();
            }

        }
    }

    private IEnumerator LoadGameSceneAsync(ServerSingleton serverSingleton)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_gameSceneName);

        while (!asyncOperation.isDone)//check next frame
        {
            yield return null;
        }

        Task createServerTask = serverSingleton.CreateServer(this.playerPrefab);
        yield return new WaitUntil(() => createServerTask.IsCompleted);

        Task startServerTask = serverSingleton.ServerGameManager.StartGameServerAsync();
        yield return new WaitUntil(() => startServerTask.IsCompleted);
    }
}
