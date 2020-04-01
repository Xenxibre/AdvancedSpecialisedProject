using Photon.Pun; 
using System.Collections;
using System.Collections.Generic;
using System.IO; 
using UnityEngine;
using UnityEngine.UI;

public class ZombieManager : MonoBehaviour
{
    public static ZombieManager zombieManager; //Singleton. 

    private PhotonView PV; //Networking component. 

    [SerializeField] private GameObject zombiePrefab; //Zombie prefab. 

    [SerializeField] private List<Transform> alivePlayers; //Alive players.

    //Zombie info.
    [SerializeField] private List<ZombieController> zombies; 

    private const int maxZombiesInRoom = 2; //Max zombies that can spawn at once. 

    private int roundNum; //Round number; 
    private int zombiesInRound; //Total number of zombies that will spawn this round.
    private int zombiesKilled; //Zombies killed this round.
    private int zombiesInRoom; //Zombies that have been spawned this round.  

    private float zombieHealth;
    private float zombieSpeed; 

    private GameObject zombieToDie;

    //Zombie spawn system. 
    [SerializeField] private List<Transform> activeSpawns; //Active zombie spawn points. 

    [SerializeField] private GameObject lobbySpawns;
    [SerializeField] private GameObject staffRoomSpawns;
    [SerializeField] private GameObject securitySpawns;
 
    private Collider corridorCollider;
    private Collider securityCollider;

    private bool canSpawn;

    private const int spawnDelay = 2;
    private const int roundDelay = 5;

    void Start()
    {
        if(ZombieManager.zombieManager == null)
        {
            ZombieManager.zombieManager = this;
        }
        else
        {
            if (ZombieManager.zombieManager != this)
            {
                Destroy(ZombieManager.zombieManager.gameObject);
                ZombieManager.zombieManager = this; 
            }
        }

        PV = GetComponent<PhotonView>(); 

        alivePlayers = new List<Transform>();
        zombies = new List<ZombieController>(); 

        for(int i = 0; i < transform.childCount; i++)
        {
            activeSpawns.Add(transform.GetChild(i));
        }

        roundNum = 0;
        zombiesInRound = 10;
        zombiesKilled = 0;
        zombiesInRoom = 0;

        zombieHealth = 20;
        zombieSpeed = 1;

        canSpawn = true;

        Debug.Log(lobbySpawns.transform.childCount); 

        for(int i = 0; i < lobbySpawns.transform.childCount; i++)
        {
            activeSpawns.Add(lobbySpawns.transform.GetChild(i)); 
        }

        StartCoroutine(WaitForNextRound());
    }

    private void Update()
    {
        SpawnZombie();
    }

    private void GetAlivePlayers()
    {
        alivePlayers.Clear(); 

        for(int i = 0; i < GameObject.FindGameObjectsWithTag("PlayerAlive").Length; i ++)
        {
            alivePlayers.Add(GameObject.FindGameObjectsWithTag("PlayerAlive")[i].transform); 
        }
    }

    private void StartNextRound()
    {
        GetAlivePlayers(); 

        roundNum++;

        zombiesInRound = 10 + (roundNum * 2);
        zombieHealth = zombieHealth + (roundNum * 2);
        zombieSpeed = zombieSpeed + (roundNum / 100); 

        zombiesKilled = 0;
        canSpawn = true;

        Debug.Log("[INFO] Next round started, " + zombiesInRound + " remaining."); 
    }
    
    private void SpawnZombie() //Spawn zombie at a random active spawn location. 
    {
        if (PV.IsMine)
        {
            if(canSpawn && zombiesInRoom < maxZombiesInRoom && (zombiesKilled + zombiesInRoom) < zombiesInRound)
            {
                int spawnPicker = Random.Range(0, activeSpawns.Count);
                Vector3 spawnPos = activeSpawns[spawnPicker].position;

                if(PV.IsMine)
                {
                    PV.RPC("RPC_SpawnZombie", RpcTarget.AllBuffered, spawnPos); 
                }                                            
            }       
        }
    }

    //----------------------------------------------------
    //Co-Routines.
    //----------------------------------------------------
    private IEnumerator WaitForSpawn() //Delay between zombies spawning.
    {
        canSpawn = false; 

        yield return new WaitForSeconds(spawnDelay);

        canSpawn = true; 
    }

    private IEnumerator WaitForNextRound() //Delay been rounds finishing and starting. 
    {
        canSpawn = false;
        yield return new WaitForSeconds(roundDelay);
        StartNextRound();  
    }

    //---------------------------------------------------
    //Public Functions. 
    //---------------------------------------------------
    public List<Transform> GetPlayerPosList() //Return positions of alive players. 
    {
        return alivePlayers; 
    }

    public void ZombieDied(ZombieController zombie) //Called when a zombie is killed.
    {
        int tempIndex;
        tempIndex = zombies.FindIndex(ByZombie(zombie)); 

        PV.RPC("RPC_DestroyZombie", RpcTarget.AllBuffered, tempIndex); 
    }

    public void ActivateSpawns(int set) //Activate the spawners in a section of the map when it is unlocked. 
    {
        if(set == 1)
        {
            for(int i = 0; i < staffRoomSpawns.transform.childCount; i++)
            {
                activeSpawns.Add(staffRoomSpawns.transform.GetChild(i).transform);
            }
        }
        else if (set == 2)
        {
            for(int i = 0; i < securitySpawns.transform.childCount; i++)
            {
                activeSpawns.Add(securitySpawns.transform.GetChild(i).transform);
            }
        }
    }

    public int RoundNum { get => roundNum; set => roundNum = value; }

    //------------------------------------------------
    //Predicate Functions. 
    //------------------------------------------------
    static System.Predicate<ZombieController> ByZombie(ZombieController zombieController)
    {
        return delegate (ZombieController zombie)
        {
            return zombie == zombieController;
        };
    }

    //-------------------------------------------------
    //PUN Functions
    //-------------------------------------------------
    [PunRPC]
    void RPC_SpawnZombie(Vector3 pos) //Spawn a zombie.
    {
        GameObject temp;
        ZombieController tempCon; 

        temp = Instantiate(zombiePrefab, pos, Quaternion.identity, transform);

        tempCon = temp.GetComponent<ZombieController>();

        tempCon.SetSpeed(zombieSpeed);
        tempCon.Health = zombieHealth;

        Debug.Log(tempCon.Health); 

        zombies.Add(tempCon);

        zombiesInRoom++;

        Debug.Log("[INFO] Zombie spawned.");
        StartCoroutine(WaitForSpawn());
    }

    [PunRPC]
    void RPC_DestroyZombie(int tempIndex) //Destroy a zombie. 
    {
        Destroy(zombies[tempIndex].gameObject); 
        zombies.Remove(zombies[tempIndex]);

        zombiesInRoom--;

        zombiesKilled++;

        Debug.Log("[INFO] Zombies remaining: " + (zombiesInRound - zombiesKilled));

        if (zombiesKilled == zombiesInRound)
        {
            StartCoroutine(WaitForNextRound());
            Debug.Log("[INFO] Round complete, next round starting...");
        }
    }
}
