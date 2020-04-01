using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipGunButton : MonoBehaviour
{
    private InventoryUI UI;
    private GunManager GM;

    [SerializeField] private Text listingText;

    private string gunName; 
    private int gunNum; 
    private int slot;

    public string GunName { get => gunName; set => gunName = value; }

    private void Start()
    {
        UI = GetComponentInParent<InventoryUI>();
        GM = GetComponentInParent<GunManager>(); 
    }

    public void Setup(string name, int id, int pos)
    {
        listingText.text = name;
        slot = pos;
        gunNum = id;
    }

    public void EquipButtonClicked()
    {       
        if(slot == 1)
        {
            UI.UpdatePrimary(gunNum);
            Destroy(this.gameObject);
        }

        if(slot == 2)
        {
            UI.UpdateSecondary(gunNum);
            Destroy(this.gameObject);
        }
    }
}
