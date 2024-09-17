using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [Header("Setting")]
    [SerializeField] private int ownerProprity = 15;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    public override void OnNetworkSpawn()
    {
        if(IsServer){
            UserData userData = HostSingleton.Instance.HostGameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            PlayerName.Value = userData.UserName;

        }
        if (IsOwner)
        {
            virtualCamera.Priority = ownerProprity;
        }
    }
}
