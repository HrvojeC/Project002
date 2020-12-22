using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
