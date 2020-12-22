using System;
using UnityEngine;
using Mirror;
using TMPro;


public class ChatBehaviour : NetworkBehaviour
{
    [SerializeField] private TMP_Text chatText = null;
    [SerializeField] private TMP_InputField inputField = null;


    private void Start()
    {
        if (!hasAuthority) return;
        chatText = GameObject.Find("Chat/Text").GetComponent<TMP_Text>();
        inputField = GameObject.Find("Chat/InputField").GetComponent<TMP_InputField>();
        GameObject.Find("HUB").GetComponent<Scene02_HUB>().myChat = this;
    }



    #region Zaprimanje_Poruke

    private static event Action<string> OnMessage;

    public override void OnStartAuthority()
    {
        OnMessage += HandleNewMessage;
    }

    [ClientCallback]
    private void OnDestroy()
    {
        if (!hasAuthority) { return; }

        OnMessage -= HandleNewMessage;
    }

    private void HandleNewMessage(string message)
    {
        chatText.text += message;
    }

    #endregion

    [Client]
    public void Send()
    {
        string message = inputField.text;

        
        if (string.IsNullOrWhiteSpace(message)) { return; Console.Send("could not send empty message"); }
        Console.Send("tried to send text (" + message + ")");
        CmdSendMessage(message);

        inputField.text = string.Empty;
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        RpcHandleMessage($"[{connectionToClient.connectionId}]: {message}");
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }
}
