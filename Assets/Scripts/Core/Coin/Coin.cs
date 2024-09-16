using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Coin : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    protected int coinValue = 10;
    protected bool alreadyCollected = false;

    public abstract int Collect();

    public void SetValue(int value){
        this.coinValue = value;
    }

    protected void Show(bool show){
        spriteRenderer.enabled = show;
    }
}
