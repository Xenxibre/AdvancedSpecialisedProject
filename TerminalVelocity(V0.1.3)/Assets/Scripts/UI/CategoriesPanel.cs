using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoriesPanel : MonoBehaviour
{
    [SerializeField] private int m_panelNum;

    private InventoryUI m_UI; 
    private Button m_button; 

    private void Start()
    {
        m_UI = GetComponentInParent<InventoryUI>(); 
        m_button = GetComponentInChildren<Button>(); 
    }

    public void OnButtonClicked()
    {
        m_UI.CategoryButtonPress(m_panelNum); 
    }
}
