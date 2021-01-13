using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class CharProfile : NetworkBehaviour
{

    [Header("General")]
    public string playerName = "Hesus";
    public int role = -1;                   //0=crew, 1=Killer, 2=Thief
    public bool isAlive = true;             //is this player alive
    public List<NetworkIdentity> revive_helpers = new List<NetworkIdentity>(); //players that are reviving this char if it's dead
    public float revive_timer = 0;          //time.time when revivers started channeling. 0 = off


    [Header("Stats")]
    public float movementSpeed = 3f;
    public float rotationSpeed = 90f;

    [Header("Bag Options")]
    public bool inBag = false;              //if character is in a bag or not
    public bool hasBag = false;             //if character is holding a bag or not

    [Header("Skin & Size")]
    [Range(0, 1.0f)] public float bodyScaleX = 0.5f;
    [Range(0, 1.0f)] public float bodyScaleY = 0.5f;
    [Range(0, 1.0f)] public float bodyScaleZ = 0.5f;

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

    public void Send_JumpInBag(bool inBag1, NetworkIdentity myBagID)
    {
        //dolazi do pogreške u network characterima kada se koristi LocalChar, jer u error slučaju ne-lokalni char radi akciju a gleda se LocalChar što sruši igru
        if (myBagID == null) CmdJumpInBag(inBag1, LocalChar.myNetwork.selection_Bag.GetComponent<NetworkIdentity>(), this.gameObject.GetComponent<NetworkIdentity>());
        else CmdJumpInBag(inBag1, myBagID, this.gameObject.GetComponent<NetworkIdentity>());
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
            //Ako je netko u BAGu odradi animacije:
            //Zatresi Bag
            myBagID.GetComponent<Animator>().Play("anim_Bag_Kicked");
            //Killer kick animacija
            GetComponent<Animator>().Play("anim_char_kickbag");
            //natjeraj lika u bag-u da izađe
            NetworkIdentity targetID = myBagID.gameObject.GetComponent<Bag_main>().GuyInside;
            myBagID.gameObject.GetComponent<Bag_main>().GuyInside.gameObject.GetComponent<CharProfile>().Send_JumpInBag(false, myBagID);
        }
        else
        {
            //Ako nitko nije u BAGu odradi animacije:
            //Zatresi Bag
            myBagID.GetComponent<Animator>().Play("anim_Bag_Kicked");
            //Killer kick animacija
            GetComponent<Animator>().Play("anim_char_kickbag");
            //i dalje ništa
        }
        RpcKickBag(myBagID);
    }

    [ClientRpc]
    private void RpcKickBag(NetworkIdentity myBagID)
    {
        if (myBagID.gameObject.GetComponent<Bag_main>().isSomeoneInside)
        {
            //Ako je netko u BAGu odradi animacije:
            //Zatresi Bag
            //Killer kick animacija
            //natjeraj lika u bag-u da izađe
            myBagID.gameObject.GetComponent<Bag_main>().GuyInside.gameObject.GetComponent<CharProfile>().Send_JumpInBag(false, myBagID);
        }
        else
        {
            //Ako nitko nije u BAGu odradi animacije:
            //Zatresi Bag
            //Killer kick animacija
            //i dalje ništa
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
        myTargetID.gameObject.GetComponent<CharProfile>().isAlive = false;
        RpcKillChar(myTargetID);
    }

    [ClientRpc]
    private void RpcKillChar(NetworkIdentity myTargetID)
    {
        myTargetID.gameObject.GetComponent<CharProfile>().isAlive = false;
    }

    #endregion

    #region ACTION-ReviveCharacter

    /// <summary>
    /// To Revive character 2 players must channel revive action for 10seconds
    /// When second player starts reviving action, timer starts counting on server. 
    /// </summary>

    //This is first part where players assign themselves as revivers (they start REVIVE action)
    public void Send_ReviveChar(bool startRevive)
    {
        //Start Revive true/false, who are you reviving?
        CmdReviveChar(startRevive, LocalChar.myNetwork.selection_CharDead.GetComponent<NetworkIdentity>());
    }

    [Command]
    private void CmdReviveChar(bool startRevive, NetworkIdentity myTargetID)
    {
        CharProfile a = myTargetID.gameObject.GetComponent<CharProfile>();

        //da li počinjemo revive ili prestajemo
        if (startRevive)
        {
            //je li ovaj igrač ubrojan kao reviver ako nije ubroji ga
            if (!a.revive_helpers.Contains(this.GetComponent<NetworkIdentity>()))
                a.revive_helpers.Add(this.GetComponent<NetworkIdentity>());
        }
        else
        {
            //je li ovaj igrač ubrojan kao reviver ako je izbaci ga
            if (a.revive_helpers.Contains(this.GetComponent<NetworkIdentity>()))
                a.revive_helpers.Remove(this.GetComponent<NetworkIdentity>());
        }

        RpcReviveChar(startRevive, myTargetID);
    }

    [ClientRpc]
    private void RpcReviveChar(bool startRevive, NetworkIdentity myTargetID)
    {
        CharProfile a = myTargetID.gameObject.GetComponent<CharProfile>();

        //da li počinjemo revive ili prestajemo
        if (startRevive)
        {
            //je li ovaj igrač ubrojan kao reviver ako nije ubroji ga
            if (!a.revive_helpers.Contains(this.GetComponent<NetworkIdentity>()))
                a.revive_helpers.Add(this.GetComponent<NetworkIdentity>());
        }
        else
        {
            //je li ovaj igrač ubrojan kao reviver ako je izbaci ga
            if (a.revive_helpers.Contains(this.GetComponent<NetworkIdentity>()))
                a.revive_helpers.Remove(this.GetComponent<NetworkIdentity>());
        }
    }

    //This is second part when server finishes revive countdown (REVIVE SUCCESSFUL)
    [Command]
    public void CmdReviveSuccess()
    {
        isAlive = true;
        //unassign helping players
        revive_helpers.Clear();
        //animacija anim_char_idle 1
        GetComponent<Animator>().Play("anim_char_idle 1");

        RpcReviveSuccess();
    }

    [ClientRpc]
    private void RpcReviveSuccess()
    {
        isAlive = true;
        //unassign helping players
        revive_helpers.Clear();
        //animacija anim_char_idle 1
        GetComponent<Animator>().Play("anim_char_idle 1");
    }
    #endregion

    #region CHAT

    //Napravimo Custom Event i customly ga triggeramo/handlamo
    //Netko će priložiti poruku i ovaj event se treba triggerati
    private static event Action<NetworkIdentity, string> OnMessage;

    //Kada se event triggera, poziva se ova metoda, preuzima se message iz eventa
    private void HandleNewMessage(NetworkIdentity profile, string message)
    {
        GameObject.Find("Chat/Text").GetComponent<TMP_Text>().text += "\n<b><color=red>" + profile.GetComponent<CharProfile>().playerName + ":</color></b> ";
        GameObject.Find("Chat/Text").GetComponent<TMP_Text>().text += message;
        Console.Send("new message arrived (" + message + ")");
    }

    //Pritiskom na tipku pokreće se slanje poruke
    public void SendChatMsg()
    {
        string message = GameObject.Find("Chat/InputField").GetComponent<TMP_InputField>().text;
        GameObject.Find("Chat/InputField").GetComponent<TMP_InputField>().text = string.Empty;

        if (message.Trim() == "") { Console.Send("could not send empty message"); return; }
        Console.Send("tried to send text (" + message + ")");

        CmdSendMessage(message);
    }

    //Ovo tu client traži da server na sebi zapiše promjene koje je client napravio
    [Command]
    private void CmdSendMessage(string message)
    {
        //u osnovi client pita: "Serveru, možeš li ovaj moj info izmjeniti na sebi?"
        //Server može odobravati ili odbiti ovdje
        //Ako server odobri onda pozova RPC na svim clientima
        RpcHandleMessage(message.Trim());
    }

    //Ovo tu server šalje (radi na) svim clientima
    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        //Proslijedi (clientu/svima) trigger na event 
        OnMessage?.Invoke(GetComponent<NetworkIdentity>(), message);
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