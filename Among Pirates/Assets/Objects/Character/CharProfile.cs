﻿using System;
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
        //Ako želi pokupiti bag znači da vidi bag, ako želi spustiti bag onda ga nema ispred
        if (LocalChar.myNetwork.selection_Bag != null)
            CmdPickUpBag(hasBag1, LocalChar.myNetwork.selection_Bag.GetComponent<NetworkIdentity>());
        else
            CmdPickUpBag(hasBag1, null);
    }

    //Ovo tu client traži izmjenu svojih parametara na serveru
    [Command]
    private void CmdPickUpBag(bool hasBag1, NetworkIdentity myBagID)
    {
        //ovdje bi server trebao napraviti istu provjeru jer trenutno samo client radi provjeru a server vjeruje

        hasBag = hasBag1;

        if (hasBag)
        {
            //ako imam bag znači da sam ga uzeo
            NetworkServer.Destroy(myBagID.gameObject);
        }
        else
        {
            //ako nemam bag znači da sam ga ostavio
            GameObject newBag = Instantiate(LocalChar.myNetwork.pref_BagOfGold.gameObject, transform.position + transform.forward * 0.2f, Quaternion.identity);
            NetworkServer.Spawn(newBag);
        }


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
        CmdJumpInBag(inBag1, LocalChar.myNetwork.selection_Bag.GetComponent<NetworkIdentity>(), this.gameObject.GetComponent<NetworkIdentity>());
    }

    //Ovo tu client traži izmjenu svojih parametara na serveru
    [Command]
    private void CmdJumpInBag(bool inBag1, NetworkIdentity myBagID, NetworkIdentity myID)
    {
        inBag = inBag1;
        myBagID.gameObject.GetComponent<Bag_main>().isSomeoneInside = inBag1;
        if (inBag)
        {
            GetComponent<Animator>().Play("anim_char_hide");
            myBagID.gameObject.GetComponent<Bag_main>().GuyInside = myID;
        }
        else if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("anim_char_hide"))
        {
            GetComponent<Animator>().Play("anim_char_unhide");
            myBagID.gameObject.GetComponent<Bag_main>().GuyInside = null;
        }
        RpcJumpInBag(inBag1, myBagID, myID);
    }

    //Ovo client traži od servera da server radi na clientima
    [ClientRpc]
    private void RpcJumpInBag(bool inBag1, NetworkIdentity myBagID, NetworkIdentity myID)
    {
        inBag = inBag1;
        myBagID.gameObject.GetComponent<Bag_main>().isSomeoneInside = inBag1;
        if (inBag)
        {
            GetComponent<Animator>().Play("anim_char_hide");
            myBagID.gameObject.GetComponent<Bag_main>().GuyInside = myID;
        }
        else if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("anim_char_hide"))
        {
            GetComponent<Animator>().Play("anim_char_unhide");
            myBagID.gameObject.GetComponent<Bag_main>().GuyInside = null;
        }
    }

    #endregion

    #region ACTION-KickBag

    public void Send_KickBag()
    {
        CmdKickBag(LocalChar.myNetwork.selection_Bag.GetComponent<NetworkIdentity>());
    }

    [Command]
    private void CmdKickBag(NetworkIdentity myBagID)
    {
        if (myBagID.gameObject.GetComponent<Bag_main>().isSomeoneInside)
        {
            //zatresi Bag, odradi animaciju i
            //natjeraj lika u bag-u da izađe
            myBagID.gameObject.GetComponent<Bag_main>().GuyInside.gameObject.GetComponent<CharProfile>().Send_JumpInBag(false);
        }
        else
        {
            //zatresi Bag, odradi animaciju i dalje ništa
        }
        RpcKickBag(myBagID);
    }

    [ClientRpc]
    private void RpcKickBag(NetworkIdentity myBagID)
    {
        if (myBagID.gameObject.GetComponent<Bag_main>().isSomeoneInside)
        {
            //zatresi Bag, odradi animaciju i
            //natjeraj lika u bag-u da izađe
            myBagID.gameObject.GetComponent<Bag_main>().GuyInside.gameObject.GetComponent<CharProfile>().Send_JumpInBag(false);
        }
        else
        {
            //zatresi Bag, odradi animaciju i dalje ništa
        }
    }

    #endregion

    #region ACTION-KillCharacter

    public void Send_KillChar()
    {
        CmdKillChar(LocalChar.myNetwork.selection_Char.GetComponent<NetworkIdentity>());
    }

    [Command]
    private void CmdKillChar(NetworkIdentity myTargetID)
    {
        
        RpcKickBag(myTargetID);
    }

    [ClientRpc]
    private void RpcKillChar(NetworkIdentity myTargetID)
    {
        
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