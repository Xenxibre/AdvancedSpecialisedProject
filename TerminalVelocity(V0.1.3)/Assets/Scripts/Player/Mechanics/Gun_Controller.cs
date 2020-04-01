using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gun_Controller : MonoBehaviour
{
    [SerializeField] private List<GameObject> gunsList;

    public GameObject GetGunObject(int index)
    {
        return gunsList[index];
    }
}
    
