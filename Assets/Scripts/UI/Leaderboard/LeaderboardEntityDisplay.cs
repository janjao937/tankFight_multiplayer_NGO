using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    // private Color myColor = Color.green;
    private FixedString32Bytes displayName;

    public int TeamIndex { get; private set; }
    public ulong ClientId { get; private set; }
    public int Coins { get; private set; }



    public void Initialize(ulong clientId, FixedString32Bytes displayName, int coins)
    {
        this.ClientId = clientId;
        this.displayName = displayName;
        this.Coins = coins;
        // if (clientId == NetworkManager.Singleton.LocalClientId)
        // {
        //     displayText.color = myColor;
        // }
        UpdateText();

    }
    public void Initialize(int teamIndex, FixedString32Bytes displayName, int coins)
    {
        this.TeamIndex = teamIndex;
        this.displayName = displayName;
        this.Coins = coins;

        UpdateText();

    }
    public void SetColor(Color color){
        displayText.color = color;
    }
    public void UpdateCoin(int coins)
    {
        this.Coins = coins;
        UpdateText();
    }
    public void UpdateText()
    {
        displayText.text = $"{transform.GetSiblingIndex() + 1}) {this.displayName} - {this.Coins}";
    }

}
