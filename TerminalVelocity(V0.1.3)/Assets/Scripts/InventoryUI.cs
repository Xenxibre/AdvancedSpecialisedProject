using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    //Input Manager.
    private Player_Input_Handler m_input;

    //Master Panel.
    private GameObject m_masterPanel;

    //Inventory Game Objects.    
    private GameObject m_inventoryPanel;

    //Shop UI Game Objects. 
    private GameObject m_shopButton; 
    private GameObject m_shopPanel;

    [SerializeField] private List<GameObject> m_categoryPanels;

    private int m_currentPanel;

    //Bools.
    private bool isWindowOpen = false;

    void Start()
    {
        m_input = GetComponentInParent<Player_Input_Handler>();

        m_masterPanel = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window").gameObject;
        m_shopPanel = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window/Shop").gameObject;

        for (int i = 0; i < transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window/Shop/StockPanel").gameObject.transform.childCount; i++)
        {
            m_categoryPanels.Add(transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI/Canvas/External/Parent/Window/Shop/StockPanel").gameObject.transform.GetChild(i).gameObject);
        }

        m_currentPanel = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        OpenWindow();
    }

    private void OpenWindow()
    {
        if (m_input.GetInventoryPressed())
        {
            if (!isWindowOpen)
            {
                Cursor.lockState = CursorLockMode.None;

                m_masterPanel.SetActive(true);
                m_shopPanel.SetActive(true);

                isWindowOpen = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;

                m_masterPanel.SetActive(false);
                m_shopPanel.SetActive(false);

                isWindowOpen = false;
            }
        }
    }

    //--------------------------------------------------------
    //Shop Menu Functions.
    //--------------------------------------------------------
    public void ShopButtonPressed()
    {
        //Deactivate inventory panel
        //Activate shop panel. 
    }

    public void CategoryButtonPress(int index)
    {
        //m_categoryButtons[m_currentButton].ChanegColorOrSomething(); 
        //m_categoryButtons[index].ChangeColorOrSomething(); 
        m_categoryPanels[m_currentPanel].SetActive(false);
        m_currentPanel = index; 
        m_categoryPanels[m_currentPanel].SetActive(true); 
    }

    public void AddToInventory()
    {

    }
}
