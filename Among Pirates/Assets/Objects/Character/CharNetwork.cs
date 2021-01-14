using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharNetwork : NetworkBehaviour
{
    [Header ("Prefab Library")]
    public NetworkIdentity pref_BagOfGold;

    [Header("My Bodyparts")]
    public TMP_Text nameText = null;
    public GameObject myCanvas = null;
    public GameObject myBag = null;

    [Header("Actions")]
    public GameObject selection_Bag = null;
    public GameObject selection_Char = null;
    public GameObject selection_CharDead = null;

    private void Start()
    {
        if (!isLocalPlayer) return;

        //Initialization of Local character

        LocalChar.myObj = this.gameObject;

        LocalChar.myProfile = GetComponent<CharProfile>();
        LocalChar.mySkin = GetComponent<CharSkin>();
        LocalChar.myNetwork = GetComponent<CharNetwork>();

        LocalChar.myHUB_Obj = GameObject.Find("HUB");
        LocalChar.myHUB_Scr = LocalChar.myHUB_Obj.GetComponent<Scene02_HUB>();

        // Setting up parameters

        LocalChar.myProfile.playerName = "Player " + UnityEngine.Random.Range(0, 999);
        LocalChar.myProfile.Send_SetName(LocalChar.myProfile.playerName);
    }

    public void Update()
    {
//ovo vrijedi i za local i non-local playera
        ServerUpdate();
        myBag.SetActive(GetComponent<CharProfile>().hasBag);
        myCanvas.transform.rotation = Camera.main.transform.rotation;

        if (!isLocalPlayer)
        {
//ovo vrijedi samo za non-local playera
            nameText.text = GetComponent<CharProfile>().playerName;
            return;
        }
//ovo vrijedi samo za local playera

        //switches
        if (LocalChar.myProfile.isAlive == false)
        {
            Death();
            return;
        }
        if (LocalChar.myProfile.inBag == true) 
        {
            if (LocalChar.myProfile.role != -1) SetMyController();
            return;
        }

        //actions
        MovePlayerAndCamera();
        DetectAction();
    }

    public void MovePlayerAndCamera() //called from NetworkBehaviour
    {
        

        int movem = 0;
        int rotat = 0;

        if (Input.GetKey(KeyCode.W)) movem += 1;
        if (Input.GetKey(KeyCode.S)) movem -= 1;
        if (Input.GetKey(KeyCode.A)) rotat -= 1;
        if (Input.GetKey(KeyCode.D)) rotat += 1;

        transform.Rotate(Vector3.up * rotat * LocalChar.myProfile.rotationSpeed * Time.deltaTime);
        //controller.Move(Vector3.forward * movem * movementSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * movem * LocalChar.myProfile.movementSpeed * Time.deltaTime);

        GetComponent<Animator>().SetBool("Walking", movem != 0);

        GameObject.Find("CameraObj").transform.position = transform.position;
        GameObject.Find("CameraObj").transform.eulerAngles = transform.eulerAngles;
    }

    public void DetectAction()
    {
        //Svaki frame napravi collider check i odblokiraj akcije koje su moguće
        //AKCIJE:
        // Crew - Take Bag, Drop Bag, Call Meeting
        // Killer - Kill, Call Meeting
        // Thief - Take Bag, Drop Bag, Toss Bag

        //Hide&Seek akcije:
        // Crew - Take Bag, Drop Bag, Jump in bag, Jump out of the bag
        // Killer - Kill, Kick Bag(takes 2sec, 50% to miss; force guy inside to Jumpout)

        //Create Collider
        Vector3 sphereCenter = transform.position + transform.forward * 0.2f + transform.up * 0.2f;

        //Get Hits
        Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, 0.05f);

        //Initialize variables
        selection_Char = null;
        selection_Bag = null;
        selection_CharDead = null;

        //check collisions & find possible selections
        foreach (var hitCollider in hitColliders)
        {
            //ako je player unutra treba zabraniti pickup
            //ne prepoznaje BAG kao interaktivni objekt ako je netko unutra
            if (hitCollider.tag == "BagGold")
            {
                if (LocalChar.myProfile.role == 0 && !hitCollider.GetComponent<Bag_main>().isSomeoneInside) selection_Bag = hitCollider.gameObject;
                if (LocalChar.myProfile.role == 1) selection_Bag = hitCollider.gameObject;
            }
            if (hitCollider.tag == "Player" && hitCollider.gameObject != this.gameObject)
            {
                if (hitCollider.GetComponent<CharProfile>().isAlive)
                    selection_Char = hitCollider.gameObject;
                else
                    selection_CharDead = hitCollider.gameObject;
            }
        }

        //activate buttons and input actions
        SetMyController();
    }

    void SetMyController ()
    {
        // Prvo ugasi sve tipke a onda pali po potrebi
        foreach (GameObject a in LocalChar.myHUB_Scr.Controller_Buttons) a.SetActive(false);

        // Controller_Buttons [0] = KillCharacter, [1] = KickBag, [2] = PickUpBag, [3] = PutDownBag, [4] = JumpInBag, [5] = JumpOutBag, [6] = ReviveCharacter, [7] = StopRevivingCharacter
        switch (LocalChar.myProfile.role)
        {
            case 0:     //------CREW------
                LocalChar.myHUB_Scr.Controller_Buttons[5].SetActive(LocalChar.myProfile.inBag);
                LocalChar.myHUB_Scr.Controller_Buttons[4].SetActive(!LocalChar.myProfile.inBag && !LocalChar.myProfile.hasBag);

                LocalChar.myHUB_Scr.Controller_Buttons[3].SetActive(LocalChar.myProfile.hasBag);
                LocalChar.myHUB_Scr.Controller_Buttons[2].SetActive(!LocalChar.myProfile.hasBag && !LocalChar.myProfile.inBag);

                if (LocalChar.myProfile.hasBag && !LocalChar.myProfile.inBag)
                {
                    LocalChar.myHUB_Scr.Controller_Buttons[3].GetComponent<Button>().interactable = true;
                    LocalChar.myHUB_Scr.Controller_Buttons[4].GetComponent<Button>().interactable = false;
                }
                else if (!LocalChar.myProfile.hasBag && !LocalChar.myProfile.inBag)
                {
                    LocalChar.myHUB_Scr.Controller_Buttons[2].GetComponent<Button>().interactable = selection_Bag;
                    LocalChar.myHUB_Scr.Controller_Buttons[4].GetComponent<Button>().interactable = selection_Bag;
                }

                if (selection_CharDead)
                {
                    //Jeli ovaj player već zapisan u listi helpera
                    if (selection_CharDead.GetComponent<CharProfile>().revive_helpers.Contains(this.GetComponent<NetworkIdentity>()))
                        LocalChar.myHUB_Scr.Controller_Buttons[7].SetActive(true); //StopRevive
                    else
                        LocalChar.myHUB_Scr.Controller_Buttons[6].SetActive(true); //StartRevive
                }
                break;
            case 1:     //------KILLER------
                LocalChar.myHUB_Scr.Controller_Buttons[0].SetActive(true);
                LocalChar.myHUB_Scr.Controller_Buttons[1].SetActive(true);
                LocalChar.myHUB_Scr.Controller_Buttons[0].GetComponent<Button>().interactable = selection_Char;
                LocalChar.myHUB_Scr.Controller_Buttons[1].GetComponent<Button>().interactable = selection_Bag;
                break;
            case 2:
                break;
            default:
                print("role not set");
                break;

        }
    }

    void Death()
    {
        if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("anim_charDead_Pose01")) GetComponent<Animator>().Play("anim_charDead_Pose01");
    }

    #region SERVER ONLY

    [ServerCallback]
    public void ServerUpdate()
    {
        CharProfile myProfile = GetComponent<CharProfile>();

        if (!myProfile.isAlive)
        {
            //print("name: " + myProfile.playerName + ", before " + myProfile.revive_helpers.Count);
            //for (int i = myProfile.revive_helpers.Count-1; i >= 0; i--) { if (myProfile.revive_helpers[i] == null) myProfile.revive_helpers.RemoveAt(i); }
            //print("name: " + myProfile.playerName + ", after " + myProfile.revive_helpers.Count);
            if (myProfile.revive_helpers.Count >= 2)
            {
                print("timer: " + Time.time + " - " + myProfile.revive_timer);

                if (myProfile.revive_timer == 0)
                    myProfile.revive_timer = Time.time;
                else if (Time.time - myProfile.revive_timer > 10.0f)
                {
                    myProfile.revive_timer = 0;
                    myProfile.CmdReviveSuccess();
                }
            }
            else myProfile.revive_timer = 0;
        }
    }


    #endregion
}
