using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    public List<MapItemInfo> mapItemInfo = new List<MapItemInfo>();
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private GameObject startGameButton;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    private void Awake()
    {
        Instance = this;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        MainMenuUIManager.instance.JoiningMasterLobby(true);
    }
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(MainMenuUIManager.instance.GetRoomInputFieldText()))
        {
            Debug.LogWarning("Cannot Create a room with a null Input Field! ");
            MainMenuUIManager.instance.SetInvalidInputFieldText("Invalid Name: Input Field Cannot be Null", Color.red);
            MainMenuUIManager.instance.RoomInputFieldText(" ");
            return;
        }
        //roomInfo.CustomProperties.Add("roomIcon", roomIcon.sprite);
        //roomInfo.CustomProperties.Add("roomMode", roomMode.text);
        //roomInfo.CustomProperties.Add("roomHostName", roomHostName.text);
        PhotonNetwork.CreateRoom(MainMenuUIManager.instance.GetRoomInputFieldText(), MainMenuUIManager.instance.GetGeneratedRoomOptions());
        Debug.Log("Trying to create a room with the name " + MainMenuUIManager.instance.GetRoomInputFieldText());
        MainMenuUIManager.instance.SetRoomTitle(MainMenuUIManager.instance.GetRoomInputFieldText());
        MainMenuUIManager.instance.SetInvalidInputFieldText("Creating Room...", Color.black);
        MainMenuUIManager.instance.CloseCreateRoomMenu();
        MainMenuUIManager.instance.OpenLoadingMenu();
    }
    public void QuickMatch()
    {
        MainMenuUIManager.instance.OpenLoadingMenu();
        MainMenuUIManager.instance.CloseMultiplayerMenu();
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        MainMenuUIManager.instance.CloseLoadingMenu();
        MainMenuUIManager.instance.CloseMultiplayerMenu();
        MainMenuUIManager.instance.OpenCreateRoomMenu();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MainMenuUIManager.instance.CloseLoadingMenu();
        MainMenuUIManager.instance.OpenMultiplayerMenu();
        Debug.Log("Failed to create room, Message: " + message);
        MainMenuUIManager.instance.SetInvalidInputFieldText("Invalid Session, returned with message: " + message, Color.red);
    }
    public override void OnCreatedRoom()
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(MainMenuUIManager.instance.GetGeneratedRoomOptions().CustomRoomProperties);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Connected to Room");
        MainMenuUIManager.instance.OpenRoomMenu();
        MainMenuUIManager.instance.CloseFindRoomMenu();
        MainMenuUIManager.instance.CloseMultiplayerMenu();
        MainMenuUIManager.instance.CloseLoadingMenu();
        MainMenuUIManager.instance.SetRoomTitle(PhotonNetwork.CurrentRoom.Name);
        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Disconnected from Room");
        MainMenuUIManager.instance.CloseRoomMenu();
        MainMenuUIManager.instance.CloseFindRoomMenu();
        MainMenuUIManager.instance.CloseLoadingMenu();
        MainMenuUIManager.instance.OpenMultiplayerMenu();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for(int i = 0;i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList) continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MainMenuUIManager.instance.OpenLoadingMenu();
        MainMenuUIManager.instance.CloseFindRoomMenu();
        MainMenuUIManager.instance.CloseMultiplayerMenu();
        Debug.Log("Loading Room Info...");
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MainMenuUIManager.instance.CloseRoomMenu();
        MainMenuUIManager.instance.OpenLoadingMenu();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
    public void QuitApplication()
    {
        Application.Quit();
    }
}
