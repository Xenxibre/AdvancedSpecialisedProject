using System.Collections;
using System.Collections.Generic;
using Photon.Pun; 
using UnityEngine;

public class Door : MonoBehaviour
{
    private PhotonView PV; 

    [SerializeField] private int cost;
    [SerializeField] private float openSpeed;

    private Quaternion rotation1;
    private Quaternion rotation2;
    private float duration; 

    private bool isOpen = false;

    public int Cost { get => cost; set => cost = value; }
    public bool IsOpen { get => isOpen; set => isOpen = value; }

    private void Start()
    {
        PV = GetComponent<PhotonView>(); 

        rotation1 = transform.rotation;
        rotation2 = Quaternion.Euler(0, -90, 0);

        duration = 1f / openSpeed; 
    }

    public void OpenDoor()
    {
        Debug.Log("[INFO] Door opening."); 

        if (IsOpen == false)
        {
            IsOpen = true; 
            PV.RPC("RPC_OpenDoor", RpcTarget.AllBuffered);         
        }
    }

    private IEnumerator RotateOverTime()
    {        
        if (duration > 0f)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            transform.rotation = rotation1;
            yield return null;
            while (Time.time < endTime)
            {
                float progress = (Time.time - startTime) / duration;
                // progress will equal 0 at startTime, 1 at endTime.
                transform.rotation = Quaternion.Slerp(rotation1, rotation2, progress);
                yield return null;
            }
        }
        transform.rotation = rotation2;
    }

    //-------------------------------------------------------------
    //Pun Functions. 
    //-------------------------------------------------------------
    [PunRPC]
    void RPC_OpenDoor()
    {
        StartCoroutine(RotateOverTime()); 
    }
}
