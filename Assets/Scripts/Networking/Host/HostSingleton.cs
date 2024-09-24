using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{

    private static HostSingleton instance;
    public HostGameManager HostGameManager { get; private set; }
    public static HostSingleton Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindObjectOfType<HostSingleton>();
            if (instance == null)
            {
                // Debug.LogWarning("No HostSingleton in this scene");
                return null;

            }
            return instance;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    
    public void CreateHost(NetworkObject playerPrefab)
    {
        HostGameManager = new HostGameManager(playerPrefab);
    }
    private void OnDestroy()
    {
        HostGameManager?.Dispose();
    }
}
