using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonLobby : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public static PhotonLobby lobby; 

    //UI Buttons.
    [SerializeField] GameObject lobbyConnectButton; //Button clicked to join lobby.
    [SerializeField] GameObject logInPanel; //Panel for displaying main menu.
    [SerializeField] GameObject lobbyPanel; //Panel for displaying lobby.

    [SerializeField] private InputField playerNameInput; //Input field for player name. 

    //RoomInfo.
    private string roomName; //String to save room name. 
    private int maxRoomSize = 4; //Max number of players that can connect to one room.

    public List<RoomInfo> roomListings; //List of currently active rooms.

    [SerializeField] private Transform roomsContainer; //Container for holding room listings.
    [SerializeField] private GameObject roomListingPrefab; //Prefab for displaying each available room.

    //Create singleton.
    private void Awake()
    {
        lobby = this; 
    }

    //Connect to master photon server.
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        roomListings = new List<RoomInfo>(); 
    }

    //If connection master server is successful. 
    public override void OnConnectedToMaster()
    {
        Debug.Log("[INFO] You are now connected to the " + PhotonNetwork.CloudRegion + " server!");

        PhotonNetwork.AutomaticallySyncScene = true; //Whenever the host loads a level, it loads for everyone.

        lobbyConnectButton.SetActive(true); //Active play button.

        //Check if player has a nickname saved.
        if(PlayerPrefs.HasKey("Nickname"))
        {
            if(PlayerPrefs.GetString("Nickname") == "")
            {
                PhotonNetwork.NickName = "Player" + Random.Range(0, 1000); //Create random player name. 
            }
            else
            {
                PhotonNetwork.NickName = PlayerPrefs.GetString("Nickname"); //Get saved player name. 
            }
        }
        else
        {
            PhotonNetwork.NickName = "Player" + Random.Range(0, 1000); //Create random player name. 
        }

        playerNameInput.text = PhotonNetwork.NickName; //Update input field with the name typed. 
    }

    //Joins the main lobby with name settings. 
    public void OnPlayButtonClicked()
    {
        Debug.Log("[INFO] Loading server browser...");

        if (!PhotonNetwork.InLobby)
        {
            logInPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            PhotonNetwork.JoinLobby();
        }
    }

    //Update player name input.
    public void PlayerNameUpdate(string nameInput)
    {
        PhotonNetwork.NickName = nameInput;
        PlayerPrefs.SetString("Nickname", nameInput);
        playerNameInput.text = nameInput;
    }

    //Update list of available rooms when Lobby is joined.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        //RemoveRoomListing();

        int tempIndex;

        foreach(RoomInfo room in roomList)
        {
            if(roomListings != null)
            {
                tempIndex = roomListings.FindIndex(ByName(room.Name));
            }
            else
            {
                tempIndex = -1;
            }
            if(tempIndex != -1)
            {
                roomListings.RemoveAt(tempIndex);
                Destroy(roomsContainer.GetChild(tempIndex).gameObject);
            }          
            roomListings.Add(room);
            ListRoom(room);
            
        }
    }

    //Lists each available room.
    void ListRoom(RoomInfo room)
    {
        if (room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsContainer);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.roomName = room.Name;
            tempButton.roomSize = room.MaxPlayers;
            tempButton.playerCount = room.PlayerCount; 

            tempButton.SetRoom(); 
        }
    }

    //Function for changing room name. 
    public void OnRoomNameChanged(string nameIn)
    {
        roomName = nameIn;
    }

    //When a room is closed, remove it from the list. 
    void RemoveRoomListing()
    {
        int i = 0; 
        while(roomsContainer.childCount != 0)
        {
            Destroy(roomsContainer.GetChild(i).gameObject);
            i++;
        }
    }

    //Button to create new room.
    public void CreateRoom()
    {
        Debug.Log("[INFO] New room created.");
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)maxRoomSize };
        PhotonNetwork.CreateRoom(roomName, roomOps);
    }

    //If room creation fails. 
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("[ERROR] Failed to create a room.");
    }

    //If player cancels matchmaking.
    public void MatchmakingCancel()
    {
        Debug.Log("[INFO] Matchmaking cancelled.");
        PhotonNetwork.LeaveLobby();
        logInPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }

    //Predicate function for searching for a room by name.
    static System.Predicate<RoomInfo> ByName(string name)
    {
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
    }
}
