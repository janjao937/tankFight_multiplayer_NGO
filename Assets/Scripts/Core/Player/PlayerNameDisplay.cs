using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TankPlayer tankPlayer;
    [SerializeField] private TMP_Text nameText;


    private void Start()
    {
        HandlePlayerNameChange(string.Empty,tankPlayer.PlayerName.Value);
        tankPlayer.PlayerName.OnValueChanged += HandlePlayerNameChange;
    }

    private void HandlePlayerNameChange(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        nameText.text = newValue.ToString();

    }

    private void OnDestroy()
    {
        tankPlayer.PlayerName.OnValueChanged -= HandlePlayerNameChange;
    }
}
