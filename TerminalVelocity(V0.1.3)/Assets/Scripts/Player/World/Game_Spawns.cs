using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

public class Game_Spawns : MonoBehaviour
{
    public static Game_Spawns GS;

    public Transform[] spawnPoints;

    private void Start()
    {
        if (Game_Spawns.GS = null)
        {
            Game_Spawns.GS = this;
        }
        else
        {
            if (Game_Spawns.GS != this)
            {
                Destroy(Game_Spawns.GS);
                Game_Spawns.GS = this; 
            }
        }
    }

    private void OnEnable()
    {
        if(Game_Spawns.GS == null)
        {
            Game_Spawns.GS = this;
        }
    }
}
