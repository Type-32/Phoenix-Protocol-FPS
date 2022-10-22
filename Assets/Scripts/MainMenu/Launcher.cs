using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    public List<MapItemInfo> mapItemInfo = new List<MapItemInfo>();
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private GameObject startGameButton;
    public LoadoutSelectionScript loadoutSelection;
    public string startKey = "103274803";
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
        SetLoadoutValuesToPlayer();
    }
    public override void OnConnected()
    {
        SetLoadoutValuesToPlayer();
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
        SetLoadoutValuesToPlayer();
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
        startGameButton.SetActive(CheckIfStartAllowed());
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(CheckIfStartAllowed());
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

    public bool CheckIfStartAllowed()
    {
        bool flag = false;
        if (PhotonNetwork.IsMasterClient)
        {
            flag = true;
            if (flag)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount >= 1 || PhotonNetwork.MasterClient.NickName == startKey) startGameButton.GetComponent<Button>().interactable = true;
                else startGameButton.GetComponent<Button>().interactable = false;
            }
        }
        return flag;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
        startGameButton.SetActive(CheckIfStartAllowed());
    }

    public int FindGlobalWeaponIndex(WeaponData data)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            if (GlobalDatabase.singleton.allWeaponDatas[i] == data) return i;
        }
        return -1;
    }
    public int FindGlobalAttachmentIndex(WeaponAttachmentData data)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponAttachmentDatas.Count; i++)
        {
            if (GlobalDatabase.singleton.allWeaponAttachmentDatas[i] == data) return i;
        }
        return -1;
    }
    public void SetLoadoutValuesToPlayer()
    {
        Hashtable temp = new Hashtable();
        //PhotonNetwork.LocalPlayer.CustomProperties = new Hashtable();
        int selectedMainWeaponIndex = FindGlobalWeaponIndex(loadoutSelection.loadoutDataList[PlayerPrefs.GetInt("selectedLoadoutIndex")].weaponData[0]);
        int selectedSecondWeaponIndex = FindGlobalWeaponIndex(loadoutSelection.loadoutDataList[PlayerPrefs.GetInt("selectedLoadoutIndex")].weaponData[1]);
        Debug.LogWarning(selectedMainWeaponIndex);
        Debug.LogWarning(selectedSecondWeaponIndex);
        temp.Add("selectedMainWeaponIndex", selectedMainWeaponIndex);
        temp.Add("selectedSecondWeaponIndex", selectedSecondWeaponIndex);

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
        PhotonNetwork.LoadLevel((int)PhotonNetwork.CurrentRoom.CustomProperties["roomMapIndex"]);
    }
    public void QuitApplication()
    {
        Application.Quit();
    }
}
