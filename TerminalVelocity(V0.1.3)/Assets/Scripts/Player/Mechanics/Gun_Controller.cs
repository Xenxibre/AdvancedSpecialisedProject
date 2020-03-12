using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gun_Controller : MonoBehaviour
{
    [SerializeField] private List<GameObject> gunsList;

    private void Start()
    {
        for (int i = 0; i < gunsList.Count; i++)
        {
            Debug.Log("At position " + i.ToString() + " is " + gunsList[i].gameObject.name); 
        }
    }

    public GameObject GetGunObject(int index)
    {
        Debug.Log("[INFO] " + gunsList[index].name + "is the new active weapon.");
        return gunsList[index];
    }
}
    
