using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Networking : MonoBehaviour
{
    private PhotonView PV;
    private Player_Controller PC; 

    [SerializeField] private GameObject playerAvatarPrefab;

    [SerializeField] private Player_Controller playerController;
    [SerializeField] private Player_Input_Handler inputHandler;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject playerBody;

    void Start()                                                                        
    {   
        PV = GetComponent<PhotonView>();
        PC = GetComponent<Player_Controller>();

        SpawnAvatar();

        PC.SetupForMovement();

        UI = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/UI").gameObject;

        playerBody = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/Body").gameObject;
        
        playerCamera = transform.Find("/PhotonPlayer(Clone)/PhotonPlayerAvatar(Clone)/GFX/RotationHelper/CameraObject").gameObject;

        if (!PV.IsMine)
        {
            bool yo = PV.IsMine;
            Debug.Log("PV is mine: " + yo); 
            playerController.enabled = false;
            inputHandler.enabled = false;           
        }
        if(PV.IsMine)
        {
            playerCamera.gameObject.SetActive(true);
            UI.gameObject.SetActive(true);
            playerBody.SetActive(false);
        }
   
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
