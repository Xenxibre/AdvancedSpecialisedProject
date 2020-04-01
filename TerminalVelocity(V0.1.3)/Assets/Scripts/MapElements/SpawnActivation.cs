using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnActivation : MonoBehaviour
{
    private ZombieManager zombManager;

    [SerializeField] private int set;

    private bool isActivated; 

    void Start()
    {
        zombManager = GameObject.Find("ZombieManager").GetComponent<ZombieManager>();
        isActivated = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isActivated)
        {
            zombManager.ActivateSpawns(set);
            isActivated = true; 
        }
    }
}
