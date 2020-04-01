using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun; 

public class ZombieController : MonoBehaviour
{
    //Parent Object. 
    private ZombieManager zombManager;

    //Last player that hit zombie.
    private Player_Stats lastPlayer; 

    //Own agent. 
    private NavMeshAgent agent;

    //Zombie Info. 
    private float health;
    private float damage;

    //Positions of players that are alive. 
    private List<Transform> playerList;

    //Position of the closest player. 
    private Vector3 closestPlayer;

    //Photon view. 
    private PhotonView PV; 

    private void Start()
    {
        PV = GetComponent<PhotonView>(); 
        zombManager = GameObject.Find("ZombieManager").GetComponent<ZombieManager>();
        agent = GetComponent<NavMeshAgent>();

        damage = 25;

        Physics.IgnoreLayerCollision(9, 10); 
    }

    private void Update()
    {
        //Debug.Log("My health is " + health); 

        playerList = zombManager.GetPlayerPosList();

        if (playerList.Count != 0)
        {
            UpdateClosestPlayer();
        }        

        MoveTowards(); 

        if(health <= 0)
        {
            zombManager.ZombieDied(this);
            lastPlayer.Money += 100; 
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

    //-------------------------------------------------
    //Collision. 
    //-------------------------------------------------
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "PlayerAlive")
        {
            collision.gameObject.GetComponent<Player_Stats>().TakeDamage(damage); 
        }
    }
    

    //-------------------------------------------------
    //Getters & Setters.
    //-------------------------------------------------
    public float Health { get => health; set => health = value; }

    public Player_Stats LastPlayer { get => lastPlayer; set => lastPlayer = value; }

    public void SetSpeed(float speed)
    {
        if (agent != null)
        {
            agent.speed = speed; 
        }
    }
}
