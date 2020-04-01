using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyGunButton : MonoBehaviour
{
    private InventoryUI UI;
    private Player_Stats stats;

    [SerializeField] private int gunNum;
    [SerializeField] private string gunName;
    [SerializeField] private int slot; 
    [SerializeField] private int price;

    [SerializeField] private Text purchaseText;

    private bool isPurchased = false; 

    public void Start()
    {
        UI = GetComponentInParent<InventoryUI>();
        stats = GetComponentInParent<Player_Stats>(); 
        purchaseText.text = price.ToString(); 
    }

    public void PurchaseGun()
    {
        if (stats.Money >= price && isPurchased == false)
        {
            UI.AddToInventory(gunNum);
            stats.Money = stats.Money - price; 
            purchaseText.text = "Sold!";
            isPurchased = true; 
        }
    }   
}
