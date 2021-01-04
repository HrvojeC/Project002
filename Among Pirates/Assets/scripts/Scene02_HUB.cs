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
    public GameObject[] Controller_Buttons;

    //[Header("Set on Start")]
    //public GameObject LocalChar.myObj;

    #region Buttons

    #region General_Buttons
    public void Canvas_Chat_Button_OnClick()
    {
        LocalChar.myObj.GetComponent<CharProfile>().SendChatMsg();
    }
    public void Canvas_RoleSet_OnClick(int roleID)
    {
        LocalChar.myObj.GetComponent<CharProfile>().Send_SetRole(roleID);
        LocalChar.myObj.GetComponent<CharProfile>().Send_SetName(LocalChar.myObj.GetComponent<CharProfile>().playerName);
    }

    public void Canvas_Profile_General_SetName_OnClick()
    {
        if (General_SetName.text.Trim() == "") return;
        
        LocalChar.myObj.GetComponent<CharProfile>().Send_SetName(General_SetName.text);
    }

    public void Canvas_Profile_Skin_SetSizes_OnClick()
    {
        LocalChar.myObj.GetComponent<CharProfile>().TriggerChangeScale(Skin_SetSizeX.value, Skin_SetSizeY.value, Skin_SetSizeZ.value);
    }

    public void Canvas_Profile_Skin_SetSkin_OnClick()
    {

    }

    public void Canvas_Controller_Kill_OnClick()
    {

    }

    #endregion

    #region Player_Buttons

    public void Button_Kill()
    {

    }

    public void Button_Kick()
    {

    }
    
    public void Button_JumpOut()
    {
        LocalChar.myObj.GetComponent<CharProfile>().Send_JumpInBag(false);
    }
    
    public void Button_JumpIn()
    {
        LocalChar.myObj.GetComponent<CharProfile>().Send_JumpInBag(true);
    }
    
    public void Button_PickUp()
    {
        LocalChar.myObj.GetComponent<CharProfile>().Send_PickUpBag(true);
    }

    public void Button_DropDown()
    {
        LocalChar.myObj.GetComponent<CharProfile>().Send_PickUpBag(false);
    }

    #endregion
    #endregion
}
