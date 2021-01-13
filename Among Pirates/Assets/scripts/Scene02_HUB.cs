using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scene02_HUB : MonoBehaviour
{
    [Header("Preset")]
    public TMPro.TMP_InputField General_SetName;
    public Scrollbar Skin_SetSizeX;
    public Scrollbar Skin_SetSizeY;
    public Scrollbar Skin_SetSizeZ;
    public GameObject[] Main_Panels;
    public GameObject[] Controller_Buttons;

    //[Header("Set on Start")]
    //public GameObject LocalChar.myObj;

    #region Buttons

    #region General_Buttons

    public void Panel_Toggle(int panelIndex)
    {
        Main_Panels[panelIndex].SetActive(!Main_Panels[panelIndex].activeInHierarchy);
    }

    public void Chat_SendMessage()
    {
        LocalChar.myObj.GetComponent<CharProfile>().SendChatMsg();
    }

    public void RoleSet_OnClick(int roleID)
    {
        LocalChar.myObj.GetComponent<CharProfile>().Send_SetRole(roleID);
        LocalChar.myObj.GetComponent<CharProfile>().Send_SetName(LocalChar.myObj.GetComponent<CharProfile>().playerName);
    }

    #endregion

    #region Profile_Buttons

    public void Profile_General_SetName()
    {
        if (General_SetName.text.Trim() == "") return;
        
        LocalChar.myObj.GetComponent<CharProfile>().Send_SetName(General_SetName.text);
    }

    public void Profile_Skin_SetSizes()
    {
        LocalChar.myObj.GetComponent<CharProfile>().TriggerChangeScale(Skin_SetSizeX.value, Skin_SetSizeY.value, Skin_SetSizeZ.value);
    }

    public void Profile_Skin_SetSkin()
    {

    }
    #endregion

    #region Player_Buttons

    public void Button_Kill()
    {
        LocalChar.myObj.GetComponent<CharProfile>().Send_KillChar();
    }
    public void Button_Revive(bool StartRevive)
    {
        LocalChar.myObj.GetComponent<CharProfile>().Send_ReviveChar(StartRevive);
    }

    public void Button_Kick()
    {
        LocalChar.myObj.GetComponent<CharProfile>().Send_KickBag();
    }
    
    public void Button_JumpOut()
    {
        LocalChar.myObj.GetComponent<CharProfile>().Send_JumpInBag(false, null);
    }
    
    public void Button_JumpIn()
    {
        LocalChar.myObj.GetComponent<CharProfile>().Send_JumpInBag(true, null);
    }
    
    public void Button_PickUp()
    {
        LocalChar.myObj.GetComponent<CharProfile>().Send_PickUpBag(true);
    }

    public void Button_PutDown()
    {
        LocalChar.myObj.GetComponent<CharProfile>().Send_PickUpBag(false);
    }
    #endregion

    #endregion
}
