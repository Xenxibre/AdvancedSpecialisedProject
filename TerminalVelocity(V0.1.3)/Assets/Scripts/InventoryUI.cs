using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    //Input Manager.
    private Player_Input_Handler m_input;

    //UI Game Objects.
    [SerializeField] private GameObject m_shopPanel;
    [SerializeField] private GameObject m_inventoryPanel;

    [SerializeField] private Text m_buttonDeagle;

    //Bools.
    private bool isWindowOpen = false;
    private bool isDeaglePurchased = false; 

    void Start()
    {
        m_input = GetComponentInParent<Player_Input_Handler>(); 

        m_shopPanel.SetActive(false);
        m_inventoryPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        OpenWindow(); 
    }

    private void OpenWindow()
    {
        if(m_input.GetInventoryPressed())
        {
            if(!isWindowOpen)
            {
                Cursor.lockState = CursorLockMode.None;

                m_shopPanel.SetActive(true);

                isWindowOpen = true; 
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;

                m_shopPanel.SetActive(false);

                isWindowOpen = false;
            }
        }
    }

    public void DeaglePurchased()
    {
        m_buttonDeagle.text = "Sold!";
        isDeaglePurchased = true;
    }
}
