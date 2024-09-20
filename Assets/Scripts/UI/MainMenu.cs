using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private TMP_Text findMatchButtonText;
    [SerializeField] private TMP_Text queTimerText;
    [SerializeField] private TMP_Text queStateText;

    private bool isMatchmaking = false;
    private bool isCancelling = false;

    private void Start()
    {
        if (ClientSingleton.Instance == null) return;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        queStateText.text = string.Empty;
        queTimerText.text = string.Empty;
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

            await ClientSingleton.Instance.ClientGameManager.CancelMatchmaking(); //Cancle match

            isCancelling = false;
            isMatchmaking = false;

            findMatchButtonText.text = "Find Match";
            queStateText.text = string.Empty;
            return;
        }

        ClientSingleton.Instance.ClientGameManager.MatchmakeAsync(OnMatchMake); //start queue
        findMatchButtonText.text = "Cancel";
        queStateText.text = "Searching...";

        isMatchmaking = true;

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
            case MatchmakerPollingResult.TicketRetrievalError:
                queStateText.text = "TicketRetrieval Error ";
                break;
            case MatchmakerPollingResult.TicketCancellationError:
                queStateText.text = "TicketCancellation Error";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                queStateText.text = "MatchAssignmentError Error";
                break;
        }
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.HostGameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.ClientGameManager.StartClientAsync(joinCodeField.text);
    }
}
