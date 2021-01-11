using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;


public static class LocalChar
{
    public static GameObject myObj;

    public static CharProfile myProfile;
    public static CharNetwork myNetwork;
    public static CharSkin mySkin;

    public static GameObject myHUB_Obj;
    public static Scene02_HUB myHUB_Scr;
}

public static class Console
{
    public static void Send (string text)
    {
        if (!GameObject.Find("Console"))
        {
            Debug.Log("Console not found");
            return;
        }

        GameObject.Find("Console/Text").GetComponent<TMP_Text>().text = text;
    }
}
