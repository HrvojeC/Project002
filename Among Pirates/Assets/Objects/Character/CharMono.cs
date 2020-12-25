using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharMono : MonoBehaviour
{
    public PlayerProfileSettings myPPS;
    public TMP_Text nameText;
    public GameObject myCanvas;





    void Update()
    {
        nameText.text = myPPS.playerName;
        myCanvas.transform.rotation = Camera.main.transform.rotation;
    }
}
