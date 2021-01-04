using System;
using UnityEngine;
using Mirror;
using TMPro;

public class CharProfile : NetworkBehaviour
{
    [Header("General")]
    public string playerName = "Hesus";

    public int role = -1;                   //0=crew, 1=Killer, 2=Thief

    public bool isAlive = true;             //is this player alive

    [Header("Stats")]
    public float movementSpeed = 3f;
    public float rotationSpeed = 90f;

    [Header("Bag Options")]
    public bool inBag = false;              //if character is in a bag or not
    public bool hasBag = false;             //if character is holding a bag or not

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

        //ChatBehaviour
        OnMessage += HandleNewMessage;
    }

    #region GENERAL-Name

    public void Send_SetName(string newName)
    {
        CmdSetName(newName);
    }

    //Ovo tu client traži izmejnu svojih parametara na serveru
    [Command]
    private void CmdSetName(string playerName1)
    {
        playerName = playerName1;
        RpcSetName(playerName1);
    }

    //Ovo tu server radi na clientima
    [ClientRpc]
    private void RpcSetName(string playerName1)
    {
        playerName = playerName1;
    }

    #endregion

    #region GENERAL-Role

    public void Send_SetRole(int roleID)
    {
        CmdSetRole(roleID);
    }

    //Ovo tu client traži izmjenu svojih parametara na serveru
    [Command]
    private void CmdSetRole(int roleID)
    {
        role = roleID;
        RpcSetRole(roleID);
    }

    //Ovo client traži od servera da server radi na clientima
    [TargetRpc]
    private void RpcSetRole(int roleID)
    {
        role = roleID;
    }

    #endregion

    #region ACTION-PickUpBag

    public void Send_PickUpBag(bool hasBag1)
    {
        CmdPickUpBag(hasBag1);
    }

    //Ovo tu client traži izmjenu svojih parametara na serveru
    [Command]
    private void CmdPickUpBag(bool hasBag1)
    {
        hasBag = hasBag1;
        RpcPickUpBag(hasBag1);
    }

    //Ovo client traži od servera da server radi na clientima
    [ClientRpc]
    private void RpcPickUpBag(bool hasBag1)
    {
        hasBag = hasBag1;
    }

    #endregion

    #region ACTION-JumpInBag

    public void Send_JumpInBag(bool inBag1)
    {
        CmdJumpInBag(inBag1);
    }

    //Ovo tu client traži izmjenu svojih parametara na serveru
    [Command]
    private void CmdJumpInBag(bool inBag1)
    {
        inBag = inBag1;
        if (inBag) GetComponent<Animator>().Play("anim_char_hide");
        else if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("anim_char_hide")) GetComponent<Animator>().Play("anim_char_unhide");
        RpcJumpInBag(inBag1);
    }

    //Ovo client traži od servera da server radi na clientima
    [ClientRpc]
    private void RpcJumpInBag(bool inBag1)
    {
        inBag = inBag1;
        if (inBag) GetComponent<Animator>().Play("anim_char_hide");
        else if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("anim_char_hide")) GetComponent<Animator>().Play("anim_char_unhide");
    }

    #endregion

    #region CHAT

    //Napravimo Custom Event i customly ga triggeramo/handlamo
    //Netko će priložiti poruku i ovaj event se treba triggerati
    private static event Action<CharProfile, string> OnMessage;

    //Kada se event triggera, poziva se ova metoda, preuzima se message iz eventa
    private void HandleNewMessage(CharProfile profile, string message)
    {
        GameObject.Find("Chat/Text").GetComponent<TMP_Text>().text += "\n<b><color=red>" + profile.playerName + ":</color></b> ";
        GameObject.Find("Chat/Text").GetComponent<TMP_Text>().text += message;
    }

    //Pritiskom na tipku pokreće se slanje poruke
    public void SendChatMsg()
    {
        string message = GameObject.Find("Chat/InputField").GetComponent<TMP_InputField>().text;

        CmdSendMessage(message);

    }

    //Ovo tu client traži da server na sebi zapiše promjene koje je client napravio
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