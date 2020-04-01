using System.Collections.Generic;
using System.IO; 
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    //Input Manager.
    private Player_Input_Handler input;

    //HUD Object.
    private GameObject hud; 

    //Master Panel (Window).
    private GameObject masterPanel;

    //Master panel stuff. 
    private GameObject shopButtonUnselected;
    private GameObject inventoryButtonUnselected;

    //Shop UI Game Objects. 
    private GameObject shopPanel;

    [SerializeField] private List<GameObject> categoryPanels;

    private int currentPanel;

    //Inventory UI Game Objects.
    private GameObject inventoryPanel;
    private GameObject stashPanel;

    [SerializeField] private List<Sprite> gunImages;

    private Image primaryImage;
    private Image secondaryImage;

    private Text primaryText;
    private Text secondaryText;

    //Bools.
    private bool isWindowOpen = false;

    private struct GunInfo
    {
        public GameObject gunComponents;
        public Gun gunController; 
    }

    //List of guns the player owns. 
    private GunManager GM; 
    private Gun_Controller GC;

    private List<GunInfo> allGuns;
    private List<GunInfo> gunsInInventory;

    private GunInfo primary;
    private GunInfo secondary; 

    void Start()
    {
        input = GetComponentInParent<Player_Input_Handler>(); 
        GM = GetComponent<GunManager>();

        allGuns = new List<GunInfo>();
        gunsInInventory = new List<GunInfo>();

        //Get Panels.
        hud = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/InGame").gameObject;

        masterPanel = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window").gameObject;
        inventoryPanel = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window/Inventory").gameObject;
        shopPanel = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window/Shop").gameObject;
        stashPanel = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window/Inventory/StashPanel").gameObject;

        primaryImage = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window/Inventory/PrimaryPanel").gameObject.GetComponent<Image>();
        secondaryImage = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window/Inventory/SecondaryPanel").gameObject.GetComponent<Image>();

        primaryText = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window/Inventory/PrimaryText").gameObject.GetComponent<Text>();
        secondaryText = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window/Inventory/SecondaryText").gameObject.GetComponent<Text>(); 

        //Get Master Panel Buttons.      
        shopButtonUnselected = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window/ShopButton/Unselected").gameObject;       
        inventoryButtonUnselected = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window/InventoryButton/Unselected").gameObject;

        //Populate all guns.
        for (int t = 0; t < transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/RotationHelper/WeaponManager").gameObject.transform.childCount; t++)
        {
            GameObject temp;
            temp = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/RotationHelper/WeaponManager").gameObject.transform.GetChild(t).gameObject;

            GunInfo tempInfo;
            tempInfo.gunController = temp.GetComponent<Gun>();
            tempInfo.gunComponents = temp.transform.Find("Components").gameObject;

            allGuns.Add(tempInfo);
        }

        //Populate category panels list. 
        for (int i = 0; i < transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window/Shop/StockPanel").gameObject.transform.childCount; i++)
        {
            categoryPanels.Add(transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window/Shop/StockPanel").gameObject.transform.GetChild(i).gameObject);
        }

        currentPanel = 0; 
    }

    void Update()
    {
        OpenWindow();
    }

    public void SetupForShooting()  //Called after avatar is instantiated.
    {
        GC = GetComponentInChildren<Gun_Controller>();
    }

    //------------------------------------------------------
    //Master Panel Functions. 
    //------------------------------------------------------
    private void OpenWindow()
    {
        if (input.GetInventoryPressed())
        {
            if (!isWindowOpen)
            {
                hud.SetActive(false); 

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true; 

                masterPanel.SetActive(true);
                inventoryPanel.SetActive(true);

                isWindowOpen = true;
            }
            else
            {
                hud.SetActive(true);

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                masterPanel.SetActive(false);

                isWindowOpen = false;
            }
        }
    }

    public void ShopButtonPressed()
    {       
        shopButtonUnselected.SetActive(false);
        inventoryButtonUnselected.SetActive(true); 

        inventoryPanel.SetActive(false);
        shopPanel.SetActive(true);    
    }

    public void InventoryButtonPressed()
    {
        shopButtonUnselected.SetActive(true);
        inventoryButtonUnselected.SetActive(false);

        inventoryPanel.SetActive(true);
        shopPanel.SetActive(false);

        OpenInventory(); 
    }

    //--------------------------------------------------------
    //Shop Menu Functions.
    //--------------------------------------------------------
    public void CategoryButtonPress(int index)
    {
        categoryPanels[currentPanel].SetActive(false);
        currentPanel = index; 
        categoryPanels[currentPanel].SetActive(true); 
    }

    public void AddToInventory(int gunNum)
    {
        int index;
        GunInfo tempInfo;

        index = allGuns.FindIndex(ByNumber(gunNum));
        tempInfo = allGuns[index]; 

        gunsInInventory.Add(tempInfo);
    }

    //---------------------------------------------------------
    //Inventory Menu Functions. 
    //---------------------------------------------------------
    private void OpenInventory()
    {
        UpdateInventory(); 
    }

    public void UpdatePrimary(int gunNum)
    {       
        int index;       
        index = SearchInventory(gunNum);

        if(index != -1)
        {     
            if (primary.gunController != null)
            {
                gunsInInventory.Add(primary); 
            }

            primary = gunsInInventory[index];
            gunsInInventory.Remove(gunsInInventory[index]);

            if (GM == null)
            {
                GM = transform.Find("/PhotonPlayer(Clone)").gameObject.GetComponent<GunManager>();
            }
            if(GM != null)
            {
                GM.SwitchPrimary(primary.gunComponents, primary.gunController);
                UpdateInventory();

                primaryImage.sprite = gunImages[primary.gunController.Id];
                primaryImage.color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                primaryText.text = primary.gunController.Name; 

                Debug.Log("[INFO] Secondary weapon changed."); 
            }
            else
            {
                Debug.Log("[ERROR] Gun manager not retrieved.");
            }
        }
        else
        {
            Debug.Log("[ERROR] Could not find gun in list.");
        }
    }

    public void UpdateSecondary(int gunNum)
    {     
        int index; 
        index = SearchInventory(gunNum);

        if (index != -1)
        {
            if (secondary.gunController != null)
            {
                gunsInInventory.Add(secondary);
            }

            secondary = gunsInInventory[index];
            gunsInInventory.Remove(gunsInInventory[index]); 

            if(GM == null)
            {               
                GM = transform.Find("/PhotonPlayer(Clone)").gameObject.GetComponent<GunManager>(); 
            }
            if (GM != null)
            {
                GM.SwitchSecondary(secondary.gunComponents, secondary.gunController);
                UpdateInventory();

                Debug.Log(secondary.gunController.Id); 

                if(gunImages[secondary.gunController.Id] == null)
                {
                    Debug.Log("[ERROR] Could not find image in list."); 
                }

                secondaryImage.sprite = gunImages[secondary.gunController.Id];
                secondaryImage.color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                secondaryText.text = secondary.gunController.Name; 

                Debug.Log("[INFO] Primary weapon changed.");
            }
            else
            {
                Debug.Log("[ERROR] Gun manager not retrieved."); 
            }
        }
        else
        {
            Debug.Log("[ERROR] Could not find gun in list."); 
        }
    }

    private int SearchInventory(int gunNum)
    {
        if (gunsInInventory.Count == 0)
        {
            Debug.Log("[INFO] Inventory is empty."); 
        }

        int index = 0;
        index = gunsInInventory.FindIndex(ByNumber(gunNum));
        return index;
    }

    private void UpdateInventory()
    {
        for (int t = 0; t < stashPanel.transform.childCount; t++)
        {
            Destroy(stashPanel.transform.GetChild(t).gameObject);
        }

        if (gunsInInventory != null)
        {
            for(int i = 0; i < gunsInInventory.Count; i++)
            {                
                GameObject temp;
                temp = (GameObject) Instantiate(Resources.Load("StashItemListing"), stashPanel.transform);            

                GunInfo tempGun;
                tempGun = gunsInInventory[i]; 

                EquipGunButton tempButton;
                tempButton = temp.gameObject.GetComponent<EquipGunButton>();
                tempButton.Setup(tempGun.gunController.Name, tempGun.gunController.Id, tempGun.gunController.Slot); 
            }
        }
    }

    //-------------------------------------------------------
    //Predicate Functions. 
    //-------------------------------------------------------
    static System.Predicate<GunInfo> ByNumber(int gunNum)
    {
        return delegate (GunInfo gunInfo)
        {
            return gunInfo.gunController.Id == gunNum;
        }; 
    }

    //-------------------------------------------------------
    //Getters & Setters. 
    //-------------------------------------------------------

}
