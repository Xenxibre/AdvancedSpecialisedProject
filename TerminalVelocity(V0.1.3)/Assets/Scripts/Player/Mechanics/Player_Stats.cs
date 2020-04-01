using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Stats : MonoBehaviour
{
    //Input Handler. 
    private Player_Input_Handler inputHandler; 

    //Stats. 
    private float iFrames; 
    private float health;
    private float money;
    [SerializeField] private float rayDistance; 

    private Transform position;

    //UI Elements. 
    private GameObject healthBar;
    [SerializeField] private GameObject buyTextObject;

    private Text buyText; 
    private Text moneyText;
    private Text roundText; 
   
    //Zombie Handle. 
    [SerializeField] private ZombieManager zombManager;

    private void Start()
    { 
        position = GetComponent<Transform>();
        inputHandler = GetComponent<Player_Input_Handler>();
        zombManager = GameObject.FindGameObjectWithTag("ZombieManager").GetComponent<ZombieManager>(); 
     
        health = 100;
        iFrames = 2;
        money = 10000; 
    }

    private void Update()
    {                 
        moneyText.text = money.ToString();          
        roundText.text = zombManager.RoundNum.ToString();     
                
        InteractRay();         
    }

    //------------------------------------------------------------------------------
    //Interact Ray.
    //------------------------------------------------------------------------------
    private void InteractRay()
    {
        Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.green); //Debug. 

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance)) //Ray in front of player. 
        {
            if (hit.transform.gameObject.tag == "Door")
            {
                GameObject door = hit.transform.gameObject;
                Door doorScript = door.GetComponent<Door>(); //Get Door Object.

                float doorCost = doorScript.Cost;
                bool isOpen = doorScript.IsOpen; 

                if (!isOpen) //Enable UI component. 
                {
                    buyTextObject.SetActive(true);
                    buyText.text = "Buy: " + doorCost;
                }

                if (inputHandler.GetInteractPressed()) //If E pressed and player has enough money. 
                {                    
                    if (money >= doorCost)
                    {
                        buyTextObject.SetActive(false);
                        money = money - doorCost; //Subtract cost from inventory.
                        doorScript.OpenDoor(); //Open door. 
                    }
                }
            }
            else
            {
                buyTextObject.SetActive(false);
            }
        }
    }

    //---------------------------------------------------------------------------
    //Health Functions.
    //---------------------------------------------------------------------------
    public void TakeDamage(float amount)
    {
        if (healthBar == null)
        {
            healthBar = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/InGame/HealthBar/BarObject").gameObject;
        }
        health = health - amount;

        UpdateHealthBar();

        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Invincibility());
        }
    }

    private void UpdateHealthBar()
    {
        healthBar.transform.localScale = new Vector3(health / 100, 1, 1); 
    }

    private void Die()
    {
        Debug.Log("[INFO] You have died.");
    }

    //-----------------------------------------------------
    //Getters & Setters.
    //-----------------------------------------------------
    public float Health { get => health; set => health = value; }
    public float Money { get => money; set => money = value; }
    public Transform Position { get => position; set => position = value; }

    public void Setup()
    {
       moneyText = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/InGame/Score/Money_Num").GetComponent<Text>();
       roundText = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/InGame/RoundNum").GetComponent<Text>();
       buyTextObject = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/InGame/BuyText").gameObject;
       buyText = buyTextObject.GetComponent<Text>();

       buyTextObject.SetActive(false);
    }

    //---------------------------------------------
    //Co-Routines.
    //---------------------------------------------
    private IEnumerator Invincibility()
    {
        yield return new WaitForSeconds(iFrames); 
    }
}
