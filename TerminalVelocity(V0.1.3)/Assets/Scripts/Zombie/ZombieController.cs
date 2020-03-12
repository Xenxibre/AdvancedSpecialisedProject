using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class ZombieController : MonoBehaviour
{
    //Parent Object. 
    private ZombieManager zombManager;

    //Own agent. 
    private NavMeshAgent agent; 

    //Positions of players that are alive. 
    private List<Transform> playerList;

    //Position of the closest player. 
    private Vector3 closestPlayer;

    private void Start()
    {        
        zombManager = GameObject.Find("ZombieManager").GetComponent<ZombieManager>();
        agent = GetComponent<NavMeshAgent>();
        playerList = zombManager.GetPlayerPosList();
    }

    private void Update()
    {
        if(playerList.Count != 0)
        {
            UpdateClosestPlayer();
        }        
        MoveTowards(); 
    }

    //Search through the player list and calculate the closest.
    private void UpdateClosestPlayer()
    {
        float previous = 0;
        float current = 0;
        Vector3 closestPos = Vector3.zero; 

        for (int i = 0; i < playerList.Count; i++)
        {
            current = Vector3.Distance(playerList[i].position, transform.position);
            
            if (previous != 0)
            {
                if(current < previous)
                {
                    closestPos = playerList[i].position; 
                    previous = current; 
                }
                else
                {
                    previous = current; 
                }
            }
            else
            {
                closestPos = playerList[i].position; 
                previous = current; 
            }
        }
        closestPlayer = closestPos; 
    }

    private void MoveTowards()
    {
        agent.SetDestination(closestPlayer); 
    }
}
