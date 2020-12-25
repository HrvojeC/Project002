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

    [Header("Set on Start")]
    public GameObject myLocalPlayer;

    public void Canvas_Chat_Button_OnClick()
    {
        myLocalPlayer.GetComponent<PlayerProfileSettings>().SendChatMsg();
    }

    public void Canvas_Profile_General_SetName_OnClick()
    {
        if (General_SetName.text.Trim() == "") return;
        
        myLocalPlayer.GetComponent<PlayerProfileSettings>().Send_SetName(General_SetName.text);
    }

    public void Canvas_Profile_Skin_SetSizes_OnClick()
    {
        myLocalPlayer.GetComponent<PlayerProfileSettings>().TriggerChangeScale(Skin_SetSizeX.value, Skin_SetSizeY.value, Skin_SetSizeZ.value);
    }

    public void Canvas_Profile_Skin_SetSkin_OnClick()
    {

    }
}
