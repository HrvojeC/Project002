using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene02_HUB : MonoBehaviour
{
    public ChatBehaviour myChat;

    public void Canvas_Chat_Button_OnClick()
    {
        Console.Send("tried to send text (" + ")");
        myChat.Send();
    }

    public void Canvas_Profile_ContentSkin_Button_OnClick ()
    {

    }
}
