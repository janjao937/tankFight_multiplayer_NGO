using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectionButtons : MonoBehaviour
{
    public void StartJoin(){
        NetworkManager.Singleton.StartClient();
    }
    public void StartHost(){

        NetworkManager.Singleton.StartHost();
    }
}
