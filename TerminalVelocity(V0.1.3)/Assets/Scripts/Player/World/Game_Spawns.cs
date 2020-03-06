using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Spawns : MonoBehaviour
{
    public static Game_Spawns GS;

    public Transform[] spawnPoints;

    private void OnEnable()
    {
        if(Game_Spawns.GS == null)
        {
            Game_Spawns.GS = this; 
        }
    }
}
