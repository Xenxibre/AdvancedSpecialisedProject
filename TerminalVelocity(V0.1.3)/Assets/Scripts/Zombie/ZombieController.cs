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

    //Zombie Info. 
    private float health; 

    //Positions of players that are alive. 
    private List<Transform> playerList;

    //Position of the closest player. 
    private Vector3 closestPlayer;

   

    private void Start()
    {        
        zombManager = GameObject.Find("ZombieManager").GetComponent<ZombieManager>();
        agent = GetComponent<NavMeshAgent>();
        playerList = zombManager.GetPlayerPosList();

        health = 100; 
    }

    private void Update()
    {
        if(playerList.Count != 0)
        {
            UpdateClosestPlayer();
        }        

        MoveTowards(); 

        if(health <= 0)
        {
            Destroy(this.gameObject); 
        }
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

    //Getters & Setters.
    public float Health { get => health; set => health = value; }
}
