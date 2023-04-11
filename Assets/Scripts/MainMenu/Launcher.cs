using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Michsky.MUIP;
using UserConfiguration;
using System.Threading.Tasks;
using InfoTypes.InRoomPreview;
using PrototypeLib.Modules.FileOperations.IO;
using PrototypeLib.OnlineServices.PUNMultiplayer.ConfigurationKeys;

public class Launcher : MonoBehaviourPunCallbacks
{
    public List<RoomInfo> rl = new();
    [Space]
    public static Launcher Instance;
    public List<MapItemInfo> mapItemInfo = new List<MapItemInfo>();
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private GameObject startGameButton;
    public LoadoutSelectionScript loadoutSelection;
    public string startKey = "103274803";
    [SerializeField] Animator matchmakingAnimator;
    private List<IEnumerator<bool>> matchFindPeriods = new();
    private bool isMatchmaking = false;
    public bool foundMatch = false;
    public RoomInfo stashedSelectedRoomInfo, roomListSelectedInfo;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        RoomManager.Instance.SetLoadingScreenState(false, 0);
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
        MenuManager.Instance.JoiningMasterLobby(true);
        Hashtable temp = new();
        temp.Add("userLevel", UserConfiguration.UserSystem.LocalUserLevel);
        PhotonNetwork.LocalPlayer.SetCustomProperties(temp);
        MenuManager.Instance.multiplayerMenuButton.interactable = true;
        RoomManager.Instance.loadEnterAnim = false;
    }
    public void CreateRoom()
    {
        if (MapListItemHolder.Instance.selectedMapIndex == -1)
        {
            Debug.LogWarning("Cannot Create a room with an invalid map selection! ");
            MenuManager.Instance.AddModalWindow("Error", "Cannot Create Room: Invalid Map Selection");
            return;
        }
        if (string.IsNullOrEmpty(MenuManager.Instance.GetRoomInputFieldText()))
        {
            Debug.LogWarning("Cannot Create a room with a null Input Field! ");
            //MenuManager.Instance.SetInvalidInputFieldText("Invalid Name: Input Field Cannot be Null", Color.red);
            MenuManager.Instance.AddModalWindow("Error", "Cannot Create Room: Invalid Room Name");
            MenuManager.Instance.RoomInputFieldText("");
            return;
        }
        //roomInfo.CustomProperties.Add("roomIcon", roomIcon.sprite);
        //roomInfo.CustomProperties.Add(RoomKeys.RoomMode, roomMode.text);
        //roomInfo.CustomProperties.Add(RoomKeys.RoomHostName, roomHostName.text);
        PhotonNetwork.CreateRoom(MenuManager.Instance.GetRoomInputFieldText(), MenuManager.Instance.GetGeneratedRoomOptions());
        Debug.Log("Trying to create a room with the name " + MenuManager.Instance.GetRoomInputFieldText());
        //MenuManager.Instance.SetInvalidInputFieldText("Creating Room...", Color.black);
        MenuManager.Instance.CloseCreateRoomMenu();
        MenuManager.Instance.OpenLoadingMenu();
    }
    IEnumerator JoinRoomDelayed(int tryJoinDelay, string roomName, string[] expectedUsers = null)
    {
        yield return new WaitForSeconds(tryJoinDelay);
        PhotonNetwork.JoinRoom(roomName, expectedUsers);
        isMatchmaking = false;
        MenuManager.Instance.quitMatchmakingButton.SetActive(false);
        matchmakingAnimator.SetBool("isMatchmaking", isMatchmaking);
        MenuManager.Instance.SetQuickMatchUIInfo("Joining Match...", false);
        MenuManager.Instance.AddNotification("Matchmaking", "You have joined a Match.");
    }
    private async Task MatchmakingAsync(MenuManager.Gamemodes gm)
    {
        isMatchmaking = true;
        matchmakingAnimator.SetBool("isMatchmaking", isMatchmaking);
        MenuManager.Instance.SetQuickMatchUIInfo($"Attempting to find a {gm.ToString()} match...", true);
        //MenuManager.Instance.OpenLoadingMenu();
        //? MenuManager.Instance.CloseMainMenu();
        // TODO PhotonNetwork.JoinRandomRoom();
        SetLoadoutValuesToPlayer();
        MenuManager.Instance.quitMatchmakingButton.SetActive(true);
        foundMatch = (bool)await PeriodicFindMatch(14, 4, gm); // (14 + 1) * 4 = 60 seconds, until automatically quit matchmaking to save performance.
        if (foundMatch)
        {
            MenuManager.Instance.quitMatchmakingButton.SetActive(false);
            MenuManager.Instance.SetQuickMatchUIInfo("Joining Match...", false);
            StartCoroutine(JoinRoomDelayed(3, stashedSelectedRoomInfo.Name));
        }
        else
        {
            if (isMatchmaking)
            {
                isMatchmaking = false;
                MenuManager.Instance.quitMatchmakingButton.SetActive(false);
                matchmakingAnimator.SetBool("isMatchmaking", isMatchmaking);
                MenuManager.Instance.SetQuickMatchUIInfo("Stopping Matchmaking...", false);
                MenuManager.Instance.AddModalWindow("Not Found", $"There are no available matches with the {gm.ToString()} gamemode. Matchmaking is stopped in order to preserve threading performance.");
                MenuManager.Instance.AddNotification("Matchmaking", "You have quitted matchmaking.");
            }
        }
    }
    public async void QuickMatch(int gmIndex)
    {
        MenuManager.Instance.multiplayerMenuButton.interactable = false;
        await MatchmakingAsync(gmIndex == 1 ? MenuManager.Gamemodes.TDM : gmIndex == 2 ? MenuManager.Gamemodes.FFA : gmIndex == 3 ? MenuManager.Gamemodes.CTF : gmIndex == 4 ? MenuManager.Gamemodes.DZ : MenuManager.Gamemodes.FFA);
    }
    public void StopQuickMatch()
    {
        ModalWindowManager tmp = MenuManager.Instance.AddModalWindow("Leave Matchmaking", "Are you sure you want to leave Matchmaking?");
        tmp.showCancelButton = true;
        tmp.UpdateUI();
        tmp.onConfirm.AddListener(LeaveQMListener);
    }
    private void LeaveQMListener()
    {
        MenuManager.Instance.multiplayerMenuButton.interactable = true;
        MenuManager.Instance.SetQuickMatchUIInfo("Leaving Matchmaking...", false);
        foundMatch = false;
        isMatchmaking = false;
        MenuManager.Instance.quitMatchmakingButton.SetActive(false);
        MenuManager.Instance.AddNotification("Matchmaking", "You have quitted matchmaking.");
        matchmakingAnimator.SetBool("isMatchmaking", isMatchmaking);
    }
    private string CheckAvailableRooms(MenuManager.Gamemodes gamemodes)
    {
        Debug.Log($"Room Counts {rl.Count}");
        foreach (RoomInfo tp in rl)
        {
            if (tp.MaxPlayers > tp.PlayerCount)
            {
                Debug.Log($"Room Counts {rl.Count}, Room Mode {(string)tp.CustomProperties[RoomKeys.RoomMode]}");
                switch (gamemodes)
                {
                    case MenuManager.Gamemodes.FFA:
                        if ((string)tp.CustomProperties[RoomKeys.RoomMode] == "Free For All")
                        {
                            stashedSelectedRoomInfo = tp;
                            return tp.Name;
                        }
                        break;
                    case MenuManager.Gamemodes.TDM:
                        if ((string)tp.CustomProperties[RoomKeys.RoomMode] == "Team Deathmatch")
                        {
                            stashedSelectedRoomInfo = tp;
                            return tp.Name;
                        }
                        break;
                    case MenuManager.Gamemodes.DZ:
                        if ((string)tp.CustomProperties[RoomKeys.RoomMode] == "Drop Zones")
                        {
                            stashedSelectedRoomInfo = tp;
                            return tp.Name;
                        }
                        break;
                    case MenuManager.Gamemodes.CTF:
                        if ((string)tp.CustomProperties[RoomKeys.RoomMode] == "Capture The Flag")
                        {
                            stashedSelectedRoomInfo = tp;
                            return tp.Name;
                        }
                        break;
                }
            }
        }
        return "";
    }
    public async Task<bool?> PeriodicFindMatch(int maxTimes, int secPerTime, MenuManager.Gamemodes gamemodes, int times = 0)
    {
        if (!isMatchmaking) return false;
        await Task.Delay(secPerTime * 1000);
        if (!isMatchmaking) return false;
        string ret = CheckAvailableRooms(gamemodes);//! Undeletable
        bool pfm = false;
        if (times < maxTimes)
        {
            if (string.IsNullOrEmpty(ret))
                pfm = (bool)await PeriodicFindMatch(maxTimes, secPerTime, gamemodes, times + 1);
            else
                pfm = true;
        }
        return pfm;
        //return false;
    }
    public override void OnConnected()
    {
        SetLoadoutValuesToPlayer();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        MenuManager.Instance.multiplayerMenuButton.interactable = true;
        MenuManager.Instance.CloseCurrentMenu();
        MenuManager.Instance.OpenMenu("main");
        MenuManager.Instance.AddModalWindow("Error Match", $"An Error Returned Whilst Joining Match:\n{message}\n\nReturn Code: {returnCode}");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        MenuManager.Instance.multiplayerMenuButton.interactable = true;
        MenuManager.Instance.CloseCurrentMenu();
        MenuManager.Instance.OpenMenu("main");
        MenuManager.Instance.AddModalWindow("Error Match", $"An Error Returned Whilst Joining Match:\n{message}\n\nReturn Code: {returnCode}");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MenuManager.Instance.multiplayerMenuButton.interactable = true;
        MenuManager.Instance.CloseLoadingMenu();
        MenuManager.Instance.OpenCreateRoomMenu();
        MenuManager.Instance.AddModalWindow("Error", "Failed to create room. Server returned a message: " + message + "\nFail code " + returnCode.ToString());
        Debug.Log("Failed to create room, Message: " + message);
    }
    public override void OnCreatedRoom()
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(MenuManager.Instance.GetGeneratedRoomOptions().CustomRoomProperties);
        if ((bool)MenuManager.Instance.GetGeneratedRoomOptions().CustomRoomProperties[RoomKeys.RoomVisibility])
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
        MenuManager.Instance.multiplayerMenuButton.interactable = false;
        MenuManager.Instance.SetMainMenuState(false);
        MenuManager.Instance.CloseCurrentMenu();
        MenuManager.Instance.OpenMenu("main");
        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        LoadoutDataJSON tp = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath).LoadoutData;
        MenuManager.Instance.RoomMenuComp.SetRoomInfoPreview(
            PhotonNetwork.CurrentRoom.Name,
            new MapPreviewInfo(mapItemInfo[(int)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomMapIndex] - 1].mapName, mapItemInfo[(int)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomMapIndex] - 1].mapIcon),
            new StatisticsPreviewInfo((string)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomMode], PhotonNetwork.CurrentRoom.MaxPlayers, (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomCode], (bool)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomVisibility], (bool)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.AllowDownedState]),
            new LoadoutPreviewInfo(GlobalDatabase.Instance.allWeaponDatas[tp.Slots[tp.SelectedSlot].Weapon1], GlobalDatabase.Instance.allWeaponDatas[tp.Slots[tp.SelectedSlot].Weapon2], GlobalDatabase.Instance.allEquipmentDatas[tp.Slots[tp.SelectedSlot].Equipment1], GlobalDatabase.Instance.allEquipmentDatas[tp.Slots[tp.SelectedSlot].Equipment2])
        );
        startGameButton.SetActive(CheckIfStartAllowed());
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(CheckIfStartAllowed());
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Disconnected from Room");
        MenuManager.Instance.CloseCurrentMenu();
        MenuManager.Instance.OpenMenu("main");
        MenuManager.Instance.SetMainMenuState(true);
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
                rl.Remove(roomList[i]);
                continue;
            }
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
        //Debug.Log(rl.Count);
    }

    public void FindRoomThroughCode()
    {
        int code = MenuManager.Instance.GetRoomCodeInputField();
        for (int i = 0; i < rl.Count; i++)
        {
            if ((int)rl[i].CustomProperties[RoomKeys.RoomCode] == code)
            {
                JoinRoom(rl[i]);
                return;
            }
        }
        MenuManager.Instance.AddModalWindow("Error", "Room with Code " + code + " Not Found.");
    }
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
        Debug.Log("Loading Room Info...");
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.SetMainMenuState(true);
        MenuManager.Instance.OpenLoadingMenu();
    }

    public bool CheckIfStartAllowed()
    {
        bool flag = false;
        if (PhotonNetwork.IsMasterClient)
        {
            flag = true;
            if (flag)
            {
                if (MenuManager.Instance.GetGamemode() == MenuManager.Gamemodes.FFA)
                {
                    if (PhotonNetwork.CurrentRoom.PlayerCount >= 1 || PhotonNetwork.MasterClient.NickName == startKey) startGameButton.gameObject.SetActive(true);
                }
                else if (MenuManager.Instance.GetGamemode() == MenuManager.Gamemodes.TDM)
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
        int selectedWeaponIndex1 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[0].GlobalWeaponIndex;
        int selectedWeaponIndex2 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[1].GlobalWeaponIndex;
        int selectedEquipmentIndex1 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].equipmentData[0].GlobalEquipmentIndex;
        int selectedEquipmentIndex2 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].equipmentData[1].GlobalEquipmentIndex;
        temp.Add(LoadoutKeys.SelectedWeaponIndex(1), selectedWeaponIndex1);
        temp.Add(LoadoutKeys.SelectedWeaponIndex(2), selectedWeaponIndex2);
        temp.Add(LoadoutKeys.SelectedEquipmentIndex(1), selectedEquipmentIndex1);
        temp.Add(LoadoutKeys.SelectedEquipmentIndex(2), selectedEquipmentIndex2);

        int smwaSightIndex1 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSightIndex[0];
        int smwaSightIndex2 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSightIndex[1];
        int smwaBarrelIndex1 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedBarrelIndex[0];
        int smwaBarrelIndex2 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedBarrelIndex[1];
        int smwaUnderbarrelIndex1 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedUnderbarrelIndex[0];
        int smwaUnderbarrelIndex2 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedUnderbarrelIndex[1];
        int smwaLeftbarrelIndex1 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelLeftIndex[0];
        int smwaLeftbarrelIndex2 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelLeftIndex[1];
        int smwaRightbarrelIndex1 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelRightIndex[0];
        int smwaRightbarrelIndex2 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelRightIndex[1];
        int smwaAppearanceIndex1 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedAppearanceDataIndex[0];
        int smwaAppearanceIndex2 = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedAppearanceDataIndex[1];
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Sight, 1), smwaSightIndex1);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Sight, 2), smwaSightIndex2);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Barrel, 1), smwaBarrelIndex1);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Barrel, 2), smwaBarrelIndex2);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Underbarrel, 1), smwaUnderbarrelIndex1);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Underbarrel, 2), smwaUnderbarrelIndex2);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Leftbarrel, 1), smwaLeftbarrelIndex1);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Leftbarrel, 2), smwaLeftbarrelIndex2);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Rightbarrel, 1), smwaRightbarrelIndex1);
        temp.Add(LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Rightbarrel, 2), smwaRightbarrelIndex2);
        temp.Add(LoadoutKeys.SelectedWeaponAppearance(1), smwaAppearanceIndex1);
        temp.Add(LoadoutKeys.SelectedWeaponAppearance(2), smwaAppearanceIndex2);

        PhotonNetwork.LocalPlayer.SetCustomProperties(temp);
    }
    public async void StartGame()
    {
        SetLoadoutValuesToPlayer();
        RoomManager.Instance.SetLoadingPreviewRPC((int)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomMapIndex] - 1, true);
        PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.GameStarted] = true;
        await Task.Delay(3000);
        PhotonNetwork.LoadLevel((int)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomMapIndex]);
    }
    public void QuitApplication()
    {
        Application.Quit();
    }
}
