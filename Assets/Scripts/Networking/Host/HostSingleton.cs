using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{

    private static HostSingleton instance;
    public HostGameManager HostGameManager;
    public static HostSingleton Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindObjectOfType<HostSingleton>();
            if (instance == null)
            {
                Debug.LogWarning("No Host singleton in this scene");
                return null;

            }
            return instance;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    
    public void CreateHost()
    {
        HostGameManager = new HostGameManager();
    }
    private void OnDestroy()
    {
        HostGameManager?.Dispose();
    }
}
