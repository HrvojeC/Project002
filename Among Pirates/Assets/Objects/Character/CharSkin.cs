using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class CharSkin : MonoBehaviour
{
    private Vector3 minScale = new Vector3(0.5f, 0.5f, 0.8f);    //Ovo može biti u settingsima ili glavnim game varijablama
    private Vector3 maxScale = new Vector3(1.0f, 1.0f, 1.0f);    //Ovo može biti u settingsima ili glavnim game varijablama
    public GameObject[] myChar;

    [Range (0,1.0f)]
    public float bodyScaleX = 0.5f;

    [Range(0, 1.0f)]
    public float bodyScaleY = 0.5f;

    [Range(0, 1.0f)]
    public float bodyScaleZ = 0.5f;

    public Material matCharacter;

    
    Vector3 GetScaleFromValues()
    {
        float xa = minScale.x + (maxScale.x - minScale.x) * bodyScaleX;
        float ya = minScale.y + (maxScale.y - minScale.y) * bodyScaleY;
        float za = minScale.z + (maxScale.z - minScale.z) * bodyScaleZ;
        transform.Find("Pelvis/Body").localScale = new Vector3(xa, ya, za);

        //bodyScaleX = (transform.Find("Pelvis/Body").localScale.x - minScale.x) / (maxScale.x - minScale.x);

        return new Vector3(xa, ya, za);
    }

#if UNITY_EDITOR

    void Reset()
    {
        bodyScaleX = (transform.Find("Pelvis/Body").localScale.x - minScale.x) / (maxScale.x - minScale.x);
        bodyScaleY = (transform.Find("Pelvis/Body").localScale.y - minScale.y) / (maxScale.y - minScale.y);
        bodyScaleZ = (transform.Find("Pelvis/Body").localScale.z - minScale.z) / (maxScale.z - minScale.z);
    }

    void OnValidate()
    {
        if (transform.Find("Pelvis/Body").localScale != GetScaleFromValues())
        {
            UnityEditor.Undo.RecordObject(transform, "change scale via variable.");
            transform.Find("Pelvis/Body").localScale = GetScaleFromValues();
        }
        if (myChar[0]) foreach (GameObject a in myChar) a.GetComponent<MeshRenderer>().material = matCharacter;
    }

    void Update()
    {
        if (transform.Find("Pelvis/Body").localScale != GetScaleFromValues())
        {
            UnityEditor.Undo.RecordObject(this, "change scale via Transform.");
            transform.Find("Pelvis/Body").localScale = GetScaleFromValues();
        }
        if (myChar[0]) foreach (GameObject a in myChar) a.GetComponent<MeshRenderer>().material = matCharacter;
    }
#endif
}