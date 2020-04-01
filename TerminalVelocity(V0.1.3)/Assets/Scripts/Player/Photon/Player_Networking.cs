using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Player_Networking : MonoBehaviour
{
    [SerializeField] private GameObject playerAvatarPrefab;

    private PhotonView PV;

    //Scripts. 
    private Player_Controller playerController; 
    private Player_Input_Handler inputHandler;
    private GunManager gunManager;
    private Player_Stats playerStats;
    private Player_Networking networking;
    private InventoryUI uiScript;
   
    //GameObjects. 
    private GameObject playerCamera;
    private GameObject UI;
    private GameObject playerBody;

    void Start()                                                                        
    {   
        PV = GetComponent<PhotonView>();
        playerController = GetComponent<Player_Controller>();
        inputHandler = GetComponent<Player_Input_Handler>();
        gunManager = GetComponent<GunManager>();
        playerStats = GetComponent<Player_Stats>();
        networking = GetComponent<Player_Networking>();
        uiScript = GetComponent<InventoryUI>();

        SpawnAvatar();
       
        UI = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI").gameObject;

        playerBody = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/Body").gameObject;
        
        playerCamera = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/RotationHelper/CameraObject").gameObject;

        if (!PV.IsMine)
        {
            playerController.enabled = false;
            inputHandler.enabled = false;
            //gunManager.enabled = false;
            playerStats.enabled = false;
            networking.enabled = false;
            uiScript.enabled = false; 
        }
        if(PV.IsMine)
        {
            playerCamera.gameObject.SetActive(true);
            UI.gameObject.SetActive(true);
            playerBody.SetActive(false);
        }

        playerController.SetupForMovement();

        gunManager.SetupForShooting();

        uiScript.SetupForShooting();

        playerStats.Setup(); 
    }

    private void SpawnAvatar()
    {
        if (PV.IsMine)
        {       
            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, transform.position);
        }
    }

    [PunRPC]
    void RPC_AddCharacter(Vector3 pos)
    {
        Debug.Log("[INFO] Instantiating avatar.");
        Instantiate(playerAvatarPrefab, pos, Quaternion.identity, transform);
    }
}
