using UnityEngine;

//[ExecuteInEditMode]
public class CharSkin : MonoBehaviour
{
    [Header("Core Settings")]
    private Vector3 minScale = new Vector3(0.5f, 0.5f, 0.8f);    //Ovo može biti u settingsima ili glavnim game varijablama
    private Vector3 maxScale = new Vector3(1.0f, 1.0f, 1.0f);    //Ovo može biti u settingsima ili glavnim game varijablama

    [Header("ScriptSetup")]
    public GameObject[] myChar;
    public PlayerProfileSettings pps;


    public Material matCharacter;

    public void ResetSkinMaterial(Material matC)
    {
        matCharacter = matC;
        if (myChar[0]) foreach (GameObject a in myChar) a.GetComponent<MeshRenderer>().material = matCharacter;
    }

    public void ResetTransformSize(float bsX, float bsY, float bsZ)
    {
        pps.bodyScaleX = bsX;
        pps.bodyScaleY = bsY;
        pps.bodyScaleZ = bsZ;
        float xa = minScale.x + (maxScale.x - minScale.x) * pps.bodyScaleX;
        float ya = minScale.y + (maxScale.y - minScale.y) * pps.bodyScaleY;
        float za = minScale.z + (maxScale.z - minScale.z) * pps.bodyScaleZ;
        transform.Find("Pelvis/Body").localScale = new Vector3(xa, ya, za);
    }
    
    public Vector3 GetScaleFromValues()
    {
        float xa = minScale.x + (maxScale.x - minScale.x) * pps.bodyScaleX;
        float ya = minScale.y + (maxScale.y - minScale.y) * pps.bodyScaleY;
        float za = minScale.z + (maxScale.z - minScale.z) * pps.bodyScaleZ;

        return new Vector3(xa, ya, za);
    }

#if UNITY_EDITOR

    void Reset()
    {
        pps.bodyScaleX = (transform.Find("Pelvis/Body").localScale.x - minScale.x) / (maxScale.x - minScale.x);
        pps.bodyScaleY = (transform.Find("Pelvis/Body").localScale.y - minScale.y) / (maxScale.y - minScale.y);
        pps.bodyScaleZ = (transform.Find("Pelvis/Body").localScale.z - minScale.z) / (maxScale.z - minScale.z);
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