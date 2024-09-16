using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform turretTransfrom;

    private void LateUpdate(){
        if(!IsOwner){return;}
        Vector2 aimScreenPosition = inputReader.AimPosition;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);
        turretTransfrom.up = new Vector2(aimWorldPosition.x-turretTransfrom.position.x
        ,aimWorldPosition.y-turretTransfrom.position.y);


        
    }
}
