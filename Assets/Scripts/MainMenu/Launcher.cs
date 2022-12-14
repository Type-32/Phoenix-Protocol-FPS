using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Michsky.MUIP;
using UserConfiguration;

public class Launcher : MonoBehaviourPunCallbacks
{
    public List<RoomInfo> rl = new List<RoomInfo>();
    [Space]
    public static Launcher Instance;
    public List<MapItemInfo> mapItemInfo = new List<MapItemInfo>();
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private Text roomCodeText;
    public LoadoutSelectionScript loadoutSelection;
    public string startKey = "103274803";
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        MenuManager.instance.SetFindRoomText("");
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
        MenuManager.instance.JoiningMasterLobby(true);
    }
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(MenuManager.instance.GetRoomInputFieldText()))
        {
            Debug.LogWarning("Cannot Create a room with a null Input Field! ");
            MenuManager.instance.SetInvalidInputFieldText("Invalid Name: Input Field Cannot be Null", Color.red);
            MenuManager.instance.RoomInputFieldText(" ");
            return;
        }
        //roomInfo.CustomProperties.Add("roomIcon", roomIcon.sprite);
        //roomInfo.CustomProperties.Add("roomMode", roomMode.text);
        //roomInfo.CustomProperties.Add("roomHostName", roomHostName.text);
        PhotonNetwork.CreateRoom(MenuManager.instance.GetRoomInputFieldText(), MenuManager.instance.GetGeneratedRoomOptions());
        Debug.Log("Trying to create a room with the name " + MenuManager.instance.GetRoomInputFieldText());
        MenuManager.instance.SetRoomTitle(MenuManager.instance.GetRoomInputFieldText());
        MenuManager.instance.SetInvalidInputFieldText("Creating Room...", Color.black);
        MenuManager.instance.CloseCreateRoomMenu();
        MenuManager.instance.OpenLoadingMenu();
    }
    public void QuickMatch()
    {
        MenuManager.instance.OpenLoadingMenu();
        MenuManager.instance.CloseMultiplayerMenu();
        PhotonNetwork.JoinRandomRoom();
        SetLoadoutValuesToPlayer();
    }
    public override void OnConnected()
    {
        SetLoadoutValuesToPlayer();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        MenuManager.instance.CloseLoadingMenu();
        MenuManager.instance.CloseMultiplayerMenu();
        MenuManager.instance.OpenCreateRoomMenu();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MenuManager.instance.CloseLoadingMenu();
        MenuManager.instance.OpenMultiplayerMenu();
        Debug.Log("Failed to create room, Message: " + message);
        MenuManager.instance.SetInvalidInputFieldText("Invalid Session, returned with message: " + message, Color.red);
    }
    public override void OnCreatedRoom()
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(MenuManager.instance.GetGeneratedRoomOptions().CustomRoomProperties);
        if ((bool)MenuManager.instance.GetGeneratedRoomOptions().CustomRoomProperties["roomVisibility"])
        {
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }
        else
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }
    public override void OnJoinedRoom()
    {
        SetLoadoutValuesToPlayer();
        Debug.Log("Connected to Room");
        MenuManager.instance.OpenRoomMenu();
        MenuManager.instance.CloseFindRoomMenu();
        MenuManager.instance.CloseMultiplayerMenu();
        MenuManager.instance.CloseLoadingMenu();
        MenuManager.instance.SetRoomTitle(PhotonNetwork.CurrentRoom.Name);
        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        startGameButton.SetActive(CheckIfStartAllowed());
        Debug.Log((int)PhotonNetwork.CurrentRoom.CustomProperties["roomCode"]);
        int tmp = (int)PhotonNetwork.CurrentRoom.CustomProperties["roomCode"];
        roomCodeText.text = (bool)PhotonNetwork.CurrentRoom.CustomProperties["roomVisibility"] ? "" : ("Room Code: " + tmp.ToString());
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(CheckIfStartAllowed());
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Disconnected from Room");
        MenuManager.instance.CloseMainMenu();
        MenuManager.instance.CloseLoadingMenu();
        MenuManager.instance.CloseFindRoomMenu();
        MenuManager.instance.CloseLoadingMenu();
        MenuManager.instance.CloseMultiplayerMenu();
        MenuManager.instance.CloseRoomMenu();
        MenuManager.instance.CloseSettingsMenu();
        MenuManager.instance.CloseUpdateLogsMenu();
        MenuManager.instance.CloseCreateRoomMenu();
        MenuManager.instance.CloseLoadoutSelectionMenu();
        MenuManager.instance.CloseCosmeticsMenu();
        MenuManager.instance.OpenMultiplayerMenu();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //rl = roomList;
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            rl.Add(roomList[i]);
            if (roomList[i].RemovedFromList)
            {
                //rl.Add(roomList[i]);
                continue;
            }
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
        Debug.Log(rl.Count);
    }

    public void FindRoomThroughCode()
    {
        int code = MenuManager.instance.GetRoomCodeInputField();
        for (int i = 0; i < rl.Count; i++)
        {
            if ((int)rl[i].CustomProperties["roomCode"] == code)
            {
                MenuManager.instance.SetFindRoomText("");
                JoinRoom(rl[i]);
                return;
            }
        }
        MenuManager.instance.SetFindRoomText("Room with Code " + code + " Not Found.");
    }
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenLoadingMenu();
        MenuManager.instance.CloseFindRoomMenu();
        MenuManager.instance.CloseMultiplayerMenu();
        Debug.Log("Loading Room Info...");
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.CloseRoomMenu();
        MenuManager.instance.OpenLoadingMenu();
    }

    public bool CheckIfStartAllowed()
    {
        bool flag = false;
        if (PhotonNetwork.IsMasterClient)
        {
            flag = true;
            if (flag)
            {
                if (MenuManager.instance.GetGamemode() == MenuManager.Gamemodes.FFA)
                {
                    if (PhotonNetwork.CurrentRoom.PlayerCount >= 1 || PhotonNetwork.MasterClient.NickName == startKey) startGameButton.gameObject.SetActive(true);
                }
                else if (MenuManager.instance.GetGamemode() == MenuManager.Gamemodes.TDM)
                {
                    if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 || PhotonNetwork.MasterClient.NickName == startKey) startGameButton.gameObject.SetActive(true);
                }
                else startGameButton.gameObject.SetActive(false);
            }
        }
        return flag;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
        startGameButton.SetActive(CheckIfStartAllowed());
    }

    public void SetLoadoutValuesToPlayer()
    {
        Hashtable temp = new Hashtable();
        //PhotonNetwork.LocalPlayer.CustomProperties = new Hashtable();
        int selectedMainWeaponIndex = Database.FindWeaponDataIndex(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[0]);
        int selectedSecondWeaponIndex = Database.FindWeaponDataIndex(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[1]);
        int selectedEquipmentIndex1 = Database.FindEquipmentDataIndex(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].equipmentData[0]);
        int selectedEquipmentIndex2 = Database.FindEquipmentDataIndex(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].equipmentData[1]);
        Debug.Log("EQ 1: " + selectedEquipmentIndex1 + "    EQ 2: " + selectedEquipmentIndex2);
        //Debug.LogWarning(selectedMainWeaponIndex);
        //Debug.LogWarning(selectedSecondWeaponIndex);
        temp.Add("selectedMainWeaponIndex", selectedMainWeaponIndex);
        temp.Add("selectedSecondWeaponIndex", selectedSecondWeaponIndex);
        temp.Add("selectedEquipmentIndex1", selectedEquipmentIndex1);
        temp.Add("selectedEquipmentIndex2", selectedEquipmentIndex2);

        int SMWA_SightIndex1 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSightIndex[0];
        int SMWA_SightIndex2 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSightIndex[1];
        int SMWA_BarrelIndex1 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedBarrelIndex[0];
        int SMWA_BarrelIndex2 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedBarrelIndex[1];
        int SMWA_UnderbarrelIndex1 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedUnderbarrelIndex[0];
        int SMWA_UnderbarrelIndex2 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedUnderbarrelIndex[1];
        int SMWA_LeftbarrelIndex1 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelLeftIndex[0];
        int SMWA_LeftbarrelIndex2 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelLeftIndex[1];
        int SMWA_RightbarrelIndex1 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelRightIndex[0];
        int SMWA_RightbarrelIndex2 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelRightIndex[1];
        int SMWA_AppearanceIndex1 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedAppearanceDataIndex[0];
        int SMWA_AppearanceIndex2 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedAppearanceDataIndex[1];
        temp.Add("SMWA_SightIndex1", SMWA_SightIndex1);
        temp.Add("SMWA_SightIndex2", SMWA_SightIndex2);
        temp.Add("SMWA_BarrelIndex1", SMWA_BarrelIndex1);
        temp.Add("SMWA_BarrelIndex2", SMWA_BarrelIndex2);
        temp.Add("SMWA_UnderbarrelIndex1", SMWA_UnderbarrelIndex1);
        temp.Add("SMWA_UnderbarrelIndex2", SMWA_UnderbarrelIndex2);
        temp.Add("SMWA_LeftbarrelIndex1", SMWA_LeftbarrelIndex1);
        temp.Add("SMWA_LeftbarrelIndex2", SMWA_LeftbarrelIndex2);
        temp.Add("SMWA_RightbarrelIndex1", SMWA_RightbarrelIndex1);
        temp.Add("SMWA_RightbarrelIndex2", SMWA_RightbarrelIndex2);
        temp.Add("SMWA_AppearanceIndex1", SMWA_AppearanceIndex1);
        temp.Add("SMWA_AppearanceIndex2", SMWA_AppearanceIndex2);

        PhotonNetwork.LocalPlayer.SetCustomProperties(temp);
    }
    public void StartGame()
    {
        SetLoadoutValuesToPlayer();
        PhotonNetwork.CurrentRoom.CustomProperties["gameStarted"] = true;
        PhotonNetwork.LoadLevel((int)PhotonNetwork.CurrentRoom.CustomProperties["roomMapIndex"]);
    }
    public void QuitApplication()
    {
        Application.Quit();
    }
}
