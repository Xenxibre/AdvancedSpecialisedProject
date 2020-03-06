using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    //UI info.
    [SerializeField] private Text nameText;
    [SerializeField] private Text sizeText;

    //Room info.
    public string roomName;
    public int roomSize;
    public int playerCount;
 
    public void SetRoom()
    {
        nameText.text = roomName;
        sizeText.text = playerCount.ToString() + "/" + roomSize.ToString(); 
    }

    public void JoinRoomOnClick()
    {
        PhotonNetwork.JoinRoom(roomName);
    }

}
