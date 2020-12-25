using System;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerProfileSettings : NetworkBehaviour
{
    [Header("General")]
    public string playerName;

    //[SyncVar]
    //public Color32 color = Color.black;

    [Header("Skin & Size")]
    [Range(0, 1.0f)]
    public float bodyScaleX = 0.5f;

    [Range(0, 1.0f)]
    public float bodyScaleY = 0.5f;

    [Range(0, 1.0f)]
    public float bodyScaleZ = 0.5f;


    private void Start()
    {
        if (!isLocalPlayer) return;

        playerName = "Player " + UnityEngine.Random.Range(0, 999);
        Send_SetName(playerName);

        GameObject.Find("HUB").GetComponent<Scene02_HUB>().myLocalPlayer = this.gameObject;

        //ChatBehaviour
        OnMessage += HandleNewMessage;
    }

    #region SETTINGS-Name

    public void Send_SetName(string newName)
    {
        CmdStartSettings(newName);
    }

    //Ovo tu client traži izmejnu svojih parametara na serveru
    [Command]
    private void CmdStartSettings(string playerName1)
    {
        playerName = playerName1;
        RpcStartSettings(playerName1);
    }

    //Ovo tu server radi na clientima
    [ClientRpc]
    private void RpcStartSettings(string playerName1)
    {
        playerName = playerName1;
    }

    #endregion


    #region CHAT

    //Napravimo Custom Event i customly ga triggeramo/handlamo
    //Netko će priložiti poruku i ovaj event se treba triggerati
    private static event Action<PlayerProfileSettings, string> OnMessage;

    //Kada se event triggera, poziva se ova metoda, preuzima se message iz eventa
    private void HandleNewMessage(PlayerProfileSettings profile, string message)
    {
        //Upiši u chat novu poruku
        if (GameObject.Find("Chat/Text") == null) { print("nemremo naći ChatText objekt"); return; }
        else print("našli smo ChatText objekt");
        if (GameObject.Find("Chat/Text").GetComponent<TMP_Text>() == null)  { print("nemremo naći TMP_Text"); return; }
        else print("našli smo TMP_Text");

        print("\n<b><color=red>" + profile.playerName + ":</color></b> " + message);

        GameObject.Find("Chat/Text").GetComponent<TMP_Text>().text += "\n<b><color=red>" + profile.playerName + ":</color></b> ";
        GameObject.Find("Chat/Text").GetComponent<TMP_Text>().text += message;
    }

    //Pritiskom na tipku pokreće se slanje poruke
    public void SendChatMsg()
    {
        string message = GameObject.Find("Chat/InputField").GetComponent<TMP_InputField>().text;

        CmdSendMessage(message);

    }

    //Ovo tu client traži da server zapiše promjene koje je client napravio
    [Command]
    private void CmdSendMessage(string message)
    {
        //u osnovi client pita: "Serveru, možeš li ovaj moj info izmjeniti na sebi?"
        //Server može odobravati ili odbiti ovdje
        if (message.Trim() == "") { Console.Send("could not send empty message"); return; }
        Console.Send("tried to send text (" + message + ")");
        //Ako server odobri onda pozova RPC na svim clientima
        GameObject.Find("Chat/InputField").GetComponent<TMP_InputField>().text = string.Empty;
        RpcHandleMessage(message.Trim());
    }

    //Ovo tu server šalje (radi na) svim clientima
    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        //Proslijedi (clientu/svima) trigger na event 
        OnMessage?.Invoke(this, message);
    }

    #endregion


    #region SETTINGS-Size

    //ovo tu pozivamo kada želimo poslati info svima
    public void TriggerChangeScale(float bsX, float bsY, float bsZ)
    {
        CmdChangeScale(bsX, bsY, bsZ);
    }

    //Ovo tu client traži da server zapiše promjene koje je client napravio
    [Command]
    private void CmdChangeScale(float bsX, float bsY, float bsZ)
    {
        bodyScaleX = bsX;
        bodyScaleY = bsY;
        bodyScaleZ = bsZ;
        GetComponent<CharSkin>().ResetTransformSize(bsX, bsY, bsZ);
        RpcChangeScale(bsX, bsY, bsZ);
    }

    //Ovo tu server šalje (radi na) svim clientima
    [ClientRpc]
    private void RpcChangeScale(float bsX, float bsY, float bsZ)
    {
        bodyScaleX = bsX;
        bodyScaleY = bsY;
        bodyScaleZ = bsZ;
        GetComponent<CharSkin>().ResetTransformSize(bsX, bsY, bsZ);
    }

    #endregion



}