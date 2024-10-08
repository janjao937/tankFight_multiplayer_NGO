using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

//have error null reference after played host on leaderboard script
public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private Transform teamLeaderboardEntityHolder;
    [SerializeField] private GameObject teamleaderboardPanel;
    [SerializeField] private LeaderboardEntityDisplay leaderboardPrefab;
    [SerializeField] private int entitysToDisplay = 8;
    [SerializeField] private Color ownerColor = Color.black;
    [SerializeField] private string[] teamName;
    [SerializeField] private TeamColorLookup teamColorLookup;

    private NetworkList<LeaderboardEntityState> leaderboardEntities;
    private List<LeaderboardEntityDisplay> entityDisplays = new List<LeaderboardEntityDisplay>();
    private List<LeaderboardEntityDisplay> teamEntityDisplays = new List<LeaderboardEntityDisplay>();

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            if (ClientSingleton.Instance.ClientGameManager.UserData.UserGamePreferences.GameQueue == GameQueue.Team)
            {
                // Debug.Log("QQQQQQ  "+ClientSingleton.Instance.ClientGameManager.UserData.UserGamePreferences.GameQueue);
                teamleaderboardPanel.SetActive(true);
                for (int i = 0; i < teamName.Length; i++)
                {
                    LeaderboardEntityDisplay teamLeaderboardEntityDisplay = Instantiate(leaderboardPrefab, teamLeaderboardEntityHolder);
                    teamLeaderboardEntityDisplay.Initialize(i, teamName[i], 0);

                    Color teamColor = teamColorLookup.GetTeamColor(i);
                    teamLeaderboardEntityDisplay.SetColor(teamColor);
                    teamEntityDisplays.Add(teamLeaderboardEntityDisplay);
                }

            }
            leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;
            foreach (LeaderboardEntityState entity in leaderboardEntities)
            {
                HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntityState>
                {
                    Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }

        if (IsServer)
        {
            TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
            foreach (TankPlayer player in players)
            {
                HandlePlayerSpawned(player);
            }
            TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
        }
    }

    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        if (!gameObject.scene.isLoaded) { return; }
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                if (!entityDisplays.Any(e => e.ClientId == changeEvent.Value.ClientId))
                {
                    LeaderboardEntityDisplay leaderboardEntityDisplay = Instantiate(leaderboardPrefab, leaderboardEntityHolder);
                    leaderboardEntityDisplay.Initialize(changeEvent.Value.ClientId, changeEvent.Value.PlayerName, changeEvent.Value.Coins);
                    entityDisplays.Add(leaderboardEntityDisplay);
                    if (NetworkManager.Singleton.LocalClientId == changeEvent.Value.ClientId)
                    {
                        leaderboardEntityDisplay.SetColor(ownerColor);
                    }
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                LeaderboardEntityDisplay displayRemove = entityDisplays.FirstOrDefault(e => e.ClientId == changeEvent.Value.ClientId);
                if (displayRemove != null)
                {
                    displayRemove.transform.SetParent(null);
                    Destroy(displayRemove.gameObject);
                    entityDisplays.Remove(displayRemove);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                LeaderboardEntityDisplay displayUpdate = entityDisplays.FirstOrDefault(e => e.ClientId == changeEvent.Value.ClientId);
                if (displayUpdate != null)
                {
                    displayUpdate.UpdateCoin(changeEvent.Value.Coins);
                }
                break;
        }
        entityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));
        for (int i = 0; i < entityDisplays.Count; i++)
        {
            entityDisplays[i].transform.SetSiblingIndex(i);
            entityDisplays[i].UpdateText();

            entityDisplays[i].gameObject.SetActive(i <= entitysToDisplay - 1);
        }

        LeaderboardEntityDisplay myDisplay = entityDisplays.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);
        if (myDisplay != null)
        {
            if (myDisplay.transform.GetSiblingIndex() >= entitysToDisplay)
            {
                leaderboardEntityHolder.GetChild(entitysToDisplay - 1).gameObject.SetActive(false);
                myDisplay.gameObject.SetActive(true);
            }
        }

        if (!teamleaderboardPanel.activeSelf) return;
        LeaderboardEntityDisplay teamDisplay = teamEntityDisplays.FirstOrDefault(x => x.TeamIndex == changeEvent.Value.TeamIndex);
        if (teamDisplay != null)
        {
            if (changeEvent.Type == NetworkListEvent<LeaderboardEntityState>.EventType.Remove)
            {
                teamDisplay.UpdateCoin(teamDisplay.Coins - changeEvent.Value.Coins);
            }
            else
            {
                // teamCoin+(current-previous) Increse  => [10 + (8-7)] or Decrese =>  [10+(9-10)] team coin
                teamDisplay.UpdateCoin(teamDisplay.Coins + (changeEvent.Value.Coins - changeEvent.PreviousValue.Coins));
            }
            teamEntityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));

            for (int i = 0; i < teamEntityDisplays.Count; i++)
            {
                teamEntityDisplays[i].transform.SetSiblingIndex(i);
                teamEntityDisplays[i].UpdateText();//update coin text
            }
        }

    }


    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;
        }
        if (IsServer)
        {
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
        }

    }
    private void HandlePlayerSpawned(TankPlayer player)
    {
        leaderboardEntities.Add(new LeaderboardEntityState
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.PlayerName.Value,
            TeamIndex = player.TeamIndex.Value,
            Coins = 0,
        });
        player.CoinWallet.TotalCoins.OnValueChanged += (oldCoins, newCoins) => HandleCoinsChange(player.OwnerClientId, newCoins);
    }
    private void HandlePlayerDespawned(TankPlayer player)
    {
        if (IsServer && player.OwnerClientId == OwnerClientId) return;//hold error on leave game
        if (leaderboardEntities == null) { return; }

        foreach (LeaderboardEntityState entity in leaderboardEntities)
        {
            if (entity.ClientId != player.OwnerClientId) { continue; }
            leaderboardEntities.Remove(entity);
            break;
        }

        player.CoinWallet.TotalCoins.OnValueChanged -= (oldCoins, newCoins) => HandleCoinsChange(player.OwnerClientId, newCoins);
    }
    private void HandleCoinsChange(ulong clientId, int newCoins)
    {
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].ClientId != clientId) continue;
            leaderboardEntities[i] = new LeaderboardEntityState
            {
                ClientId = leaderboardEntities[i].ClientId,
                PlayerName = leaderboardEntities[i].PlayerName,
                TeamIndex = leaderboardEntities[i].TeamIndex,
                Coins = newCoins
            };
            return;
        }

    }
}
