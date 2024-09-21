using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private TMP_Text findMatchButtonText;
    [SerializeField] private TMP_Text queTimerText;
    [SerializeField] private TMP_Text queStateText;

    private bool isMatchmaking = false;
    private bool isCancelling = false;
    private bool isBusy = false;
    private float timeInQueue = default;



    private void Start()
    {
        if (ClientSingleton.Instance == null) return;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        queStateText.text = string.Empty;
        queTimerText.text = string.Empty;
    }
    private void Update()
    {
        if (isMatchmaking)
        {
            timeInQueue += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeInQueue);
            queTimerText.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }
    public async void FindMatchPressed()
    {
        if (isCancelling)
        {
            return;
        }
        if (isMatchmaking)
        {
            queStateText.text = "Cancelling...";
            isCancelling = true;

            await ClientSingleton.Instance.ClientGameManager.CancelMatchmaking(); //Cancel match

            isCancelling = false;
            isMatchmaking = false;
            isBusy = false;
            findMatchButtonText.text = "Find Match";
            queStateText.text = string.Empty;
            queTimerText.text = string.Empty;
            return;
        }

        if (isBusy) { return; }

        ClientSingleton.Instance.ClientGameManager.MatchmakeAsync(OnMatchMake); //start queue
        findMatchButtonText.text = "Cancel";
        queStateText.text = "Searching...";
        timeInQueue = 0f;//reset queue time
        isMatchmaking = true;
        isBusy = true;
    }

    private void OnMatchMake(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                queStateText.text = "Connecting...";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                queStateText.text = "TicketCreation Error";
                break;
            case MatchmakerPollingResult.TicketCancellationError:
                queStateText.text = "TicketCancellation Error";
                break;
            case MatchmakerPollingResult.TicketRetrievalError:
                queStateText.text = "TicketRetrieval Error ";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                queStateText.text = "MatchAssignmentError Error";
                break;
        }
    }

    public async void StartHost()
    {
        if (isBusy) { return; }
        isBusy = true;
        await HostSingleton.Instance.HostGameManager.StartHostAsync();
        isBusy = false;
    }

    public async void StartClient()
    {
        if (isBusy) { return; }
        isBusy = true;
        await ClientSingleton.Instance.ClientGameManager.StartClientAsync(joinCodeField.text);
        isBusy = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isBusy) return;
        isBusy = true;
        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;
            await ClientSingleton.Instance.ClientGameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException esE)
        {
            Debug.Log(esE);
        }
        isBusy = false;
    }
}
