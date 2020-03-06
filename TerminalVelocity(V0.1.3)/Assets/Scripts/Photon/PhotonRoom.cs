using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    //Room info.
    public static PhotonRoom room;
    private PhotonView PV;

    public bool isGameLoaded;
    public int currentScene;

    private int playersInRoom;
    private int playersInGame;

    [SerializeField] private int multiplayerSceneIndex; //Build index of the game scene. 

    //UI Elements.
    [SerializeField] private GameObject lobbyPanel; //In lobby display.
    [SerializeField] private GameObject roomPanel; //In room display.
    [SerializeField] private GameObject startButton; //Available only for host. 

    [SerializeField] private Transform playersContainer; //Displays all players in room.
    [SerializeField] private GameObject playerListingPrefab; //Player list prefab.

    [SerializeField] private Text roomName;

    private void Awake()
    {
        if(PhotonRoom.room == null)
        {
            PhotonRoom.room = this; 
        }
        else
        {
            if(PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this; 
            }
        }
        DontDestroyOnLoad(this.gameObject);
        PV = GetComponent<PhotonView>(); 
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading; 
    }

    //When a player successfuly joins a room.
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("[INFO] Joined room: " + PhotonNetwork.CurrentRoom.Name); 

        roomPanel.SetActive(true); //Activate room UI.
        lobbyPanel.SetActive(false); //Disable lobby UI.

        roomName.text = PhotonNetwork.CurrentRoom.Name; //Update room name text.

        if (PhotonNetwork.IsMasterClient) //Allow host to start the game.
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }

        ClearPlayerListings(); //Remove old player listing.
        ListPlayers(); //Refresh player listing with new players. 
    }

    //Called whenever a new player enters the room.
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        playersInRoom++; 

        ClearPlayerListings(); //Remove old player listing.
        ListPlayers(); //Refresh player listing with new players.
    }

    //Called whenever a new player leaves the room.
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer); 

        ClearPlayerListings(); //Remove old player listing.
        ListPlayers(); //Refresh player listing with new players. 

        if (PhotonNetwork.IsMasterClient) //Support for host migration.
        {
            startButton.SetActive(true);
        }

        playersInRoom--; 
    }

    //Clear player list. 
    void ClearPlayerListings()
    {
        if(playersContainer.gameObject != null)
        {
            for (int i = playersContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(playersContainer.GetChild(i).gameObject);
            }
        }
    }

    //Draw player list. 
    void ListPlayers()
    {
        if (PhotonNetwork.InRoom)
        {
            foreach (Player player in PhotonNetwork.PlayerList) //Loop through each player and create a text object.
            {
                GameObject tempListing = Instantiate(playerListingPrefab, playersContainer);
                Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
                tempText.text = player.NickName;
            }
        }
    }

    //When the game is ready to start.
    public void StartGame()
    {
        isGameLoaded = true; 

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[INFO] Starting game.");

            PhotonNetwork.CurrentRoom.IsOpen = false; //Remove room from room list once game has started.

            roomPanel.SetActive(false);

            PhotonNetwork.LoadLevel(multiplayerSceneIndex); //Autosync being enabled causes all players to load together.
        }
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if(currentScene == multiplayerSceneIndex)
        {
            isGameLoaded = true;
            CreateNetworkedPlayer();  
        }
    }

    private void CreateNetworkedPlayer()
    {
        Debug.Log("[INFO] Network player created.");
        int spawnPicker = Random.Range(0, Game_Spawns.GS.spawnPoints.Length);
        Vector3 spawnPos = Game_Spawns.GS.spawnPoints[spawnPicker].position;
        Destroy(Game_Spawns.GS.spawnPoints[spawnPicker].gameObject);
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), spawnPos, Quaternion.identity, 0);
    }

    IEnumerator rejoinLobby()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.JoinLobby();
    }

    public void BackOnClick()
    {
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();

        StartCoroutine(rejoinLobby());
    }
}