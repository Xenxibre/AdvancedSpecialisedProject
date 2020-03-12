using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField] private List<Player_Stats> activePlayers;
    private List<Transform> alivePlayers; 

    void Start()
    {
        activePlayers = new List<Player_Stats>();
    }

    public void AddToPlayerList(Player_Stats playerStats)
    {
        activePlayers.Add(playerStats);
        
        if(playerStats.IsAlive)
        {
            alivePlayers.Add(playerStats.Position);
        }
    }

    public List<Transform> GetPlayerPosList()
    {
        return alivePlayers; 
    }
}
