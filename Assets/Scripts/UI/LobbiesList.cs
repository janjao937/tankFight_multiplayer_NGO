using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private Transform lobbyItemParent;
    [SerializeField] private LobbyItem lobbyItemPrefab;
    
    private bool isRefreshing = false;

    private void OnEnable()
    {
        RefreshListAsync();
    }
    public async void RefreshListAsync()
    {
        if (isRefreshing) return;
        isRefreshing = true;
        try
        {
            QueryLobbiesOptions lobbyOptions = new QueryLobbiesOptions();
            lobbyOptions.Count = 25;
            lobbyOptions.Filters = new List<QueryFilter>(){
            new QueryFilter(field:QueryFilter.FieldOptions.AvailableSlots,op:QueryFilter.OpOptions.GT,value:"0"),//GT = Greater than
            new QueryFilter(field:QueryFilter.FieldOptions.IsLocked,op:QueryFilter.OpOptions.EQ,value:"0"),//EQ = Equal
            };
            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(lobbyOptions);

            foreach(Transform child in lobbyItemParent){
                Destroy(child.gameObject);
            }
            foreach(Lobby lobby in lobbies.Results){
                LobbyItem lobbyItem = Instantiate(lobbyItemPrefab,lobbyItemParent);
                lobbyItem.Initialize(this,lobby);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        isRefreshing = false;
    }
    public void JoinAsync(Lobby lobby)
    {
        // if (isJoining) return;
        // isJoining = true;
        // try
        // {
        //     Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
        //     string joinCode = joiningLobby.Data["JoinCode"].Value;
        //     await ClientSingleton.Instance.ClientGameManager.StartClientAsync(joinCode);
        // }
        // catch (LobbyServiceException esE)
        // {
        //     Debug.Log(esE);
        // }
        // isJoining = false;

        mainMenu.JoinAsync(lobby);
    }
}
