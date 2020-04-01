using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITabButton : MonoBehaviour
{
    private InventoryUI UI;

    private void Start()
    {
        UI = GetComponentInParent<InventoryUI>(); 
    }

    public void InventoryButtonClicked()
    {
        UI.InventoryButtonPressed(); 
    }

    public void ShopButtonClicked()
    {
        UI.ShopButtonPressed(); 
    }
}
