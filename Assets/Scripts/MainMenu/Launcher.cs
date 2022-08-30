using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
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
            MainMenuUIManager.instance.RoomInputFieldText("InvalidName");
            return;
        }
        PhotonNetwork.CreateRoom(MainMenuUIManager.instance.GetRoomInputFieldText());
        Debug.Log("Trying to create a room with the name " + MainMenuUIManager.instance.GetRoomInputFieldText());
        MainMenuUIManager.instance.SetInvalidInputFieldText("Creating Room...", Color.black);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room, Message: " + message);
        MainMenuUIManager.instance.SetInvalidInputFieldText("Invalid Session, returned with message: " + message, Color.red);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Connected to Room");
        MainMenuUIManager.instance.OpenRoomMenu();
        MainMenuUIManager.instance.CloseMultiplayerMenu();
        MainMenuUIManager.instance.CloseMainMenu();
    }
}
