using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{

    private static ClientSingleton instance;
    public ClientGameManager ClientGameManager{get;private set;}
    public static ClientSingleton Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindObjectOfType<ClientSingleton>();
            if (instance == null)
            {
                Debug.LogError("No Client singleton in this scene");
                return null;

            }
            return instance;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnDestroy(){
        ClientGameManager.Dispose();
    }


    public async Task<bool> CreateClient()
    {
        ClientGameManager = new ClientGameManager();
        return await ClientGameManager.InitAsync();

    }


}
