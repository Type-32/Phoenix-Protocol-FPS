using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Michsky.MUIP;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using LauncherManifest;

public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager instance;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button createRoomButton;
    [Space]
    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject updateLogsMenu;
    public GameObject multiplayerMenu;
    public GameObject roomMenu;
    public GameObject findRoomMenu;
    public GameObject loadingMenu;
    public GameObject settingsMenu;
    public GameObject cosmeticsMenu;
    public GameObject createRoomMenu;
    public GameObject loadoutSelectionMenu;
    public GameObject popupMenu;
    public GameObject shopMenu;

    [Space]
    [Header("Misc Components")]
    [SerializeField] private Text connectionIndicator;
    [SerializeField] private Text invalidInputField;
    [SerializeField] private Text roomTitle;
    [SerializeField] private Text findRoomIndicator;
    [SerializeField] private InputField findRoomInputField;
    public TMP_InputField playerNameInputField;

    [Space]
    [Header("Menu States")]
    public bool openedMainMenu = false;
    public bool openedUpdateLogsMenu = false;
    public bool openedCosmeticsMenu = false;
    public bool openedMultiplayerMenu = false;
    public bool openedRoomMenu = false;
    public bool openedFindRoomMenu = false;
    public bool openedLoadingMenu = false;
    public bool openedSettingsMenu = false;
    public bool openedCreateRoomMenu = false;
    public bool openedLoadoutSelectionMenu = false;
    public bool openedPopupMenu = false;
    public bool openedShopMenu = false;
    public bool usingCreateRooomInputField = false;

    [Space]
    [Header("Menu Online States")]
    [SerializeField] private bool joinedMasterLobby = false;

    [Space]
    [Header("Room Creation")]
    [SerializeField] private TMP_InputField roomInputField;
    [SerializeField] private int roomMapSelectionIndex;
    [SerializeField] private Gamemodes selectedGamemodes = Gamemodes.FFA;
    [SerializeField] private int maxPlayerCount = 10;
    [SerializeField] private int maxKillLimitNumber = 30;
    [SerializeField] private bool roomVisibility = true;
    [SerializeField] private Transform popupHolder;
    [SerializeField] private Transform notificationHolder;

    [Header("Room Visual")]
    [SerializeField] Text maxPlayers;
    [SerializeField] Text gamemodes;
    [SerializeField] Text visibility;
    [SerializeField] Text maxKillLimit;

    [Header("Room Preview")]
    [SerializeField] Text selectedRoomName;
    [SerializeField] Text selectedMap;
    [SerializeField] Text selectedGamemode;
    [SerializeField] Text selectedMaxPlayers;
    [SerializeField] Text selectedMaxKillLimit;
    public List<GameObject> FFAGamemodeOptions = new();
    public List<GameObject> TDMPlayersOptions = new();
    public List<GameObject> KOTHPlayersOptions = new();
    public List<GameObject> DZPlayersOptions = new();

    [Space,Header("Version Manifests")]
    [SerializeField] private List<Text> versionTexts = new();

    public enum PopupQueue
    {
        OnMainMenuLoad,
        OnOpenedGame
    };
    public struct PopupData
    {
        public string title;
        public string content;
        public PopupQueue queueType;
    };
    [Space]
    [Header("Popups")]
    public List<ModalWindowManager> modalWindowList = new();
    public List<PopupData> queuedModalWindows = new();
    public List<NotificationManager> notificationList = new();
    public List<PopupData> queuedNotifications = new();
    public GameObject modalWindowPrefab;
    public GameObject notificationPrefab;

    [Space]
    [Header("User Stats")]
    public Text username;
    public Text userLevel;
    public Text userCoins;
    public Slider userLevelProgress;

    public enum Gamemodes
    {
        FFA = 1,
        TDM = 2,
        KOTH = 3,
        DZ = 4
    }
    private void Awake()
    {
        instance = this;
        Debug.Log("Initializing Awake Main menu");
    }
    void Start()
    {
        Debug.Log("Loaded Scene from Main Menu");
        JoiningMasterLobby(false);
        SetCreateRoomInputField(true);
        SetConnectionIndicatorText("Attempting to connect to Multiplayer Services...");
        SetInvalidInputFieldText(" ", Color.red);
        CloseRoomMenu();
        CloseMultiplayerMenu();
        CloseLoadingMenu();
        CloseFindRoomMenu();
        CloseCosmeticsMenu();
        CloseCreateRoomMenu();
        CloseSettingsMenu();
        CloseUpdateLogsMenu();
        //CloseLoadoutSelectionMenu();
        OpenMainMenu();
        for(int i = 0; i < versionTexts.Count; i++)
        {
            versionTexts[i].text = "V" + LocalLaunchedClient.LocalGameVersion.ToString();
        }
        if(LauncherConfig.CompareLocalVersionWithCache())
        {
            
        }
        //AddPopup("test", "testlol");

    }
    private void OnEnable()
    {
        MainMenuUIManager.instance.SetUserGUIData(PlayerPrefs.GetString("Username"), UserDatabase.Instance.GetUserXPLevelValue(), UserDatabase.Instance.GetUserXPValue(), UserDatabase.Instance.GetUserCoinValue());
    }

    #region Main Menus
    public void OpenMainMenu()
    {
        openedMainMenu = true;
        mainMenu.SetActive(openedMainMenu);
    }
    public void CloseMainMenu()
    {
        openedMainMenu = false;
        mainMenu.SetActive(openedMainMenu);
    }
    #endregion

    #region Multiplayer Menus
    public void OpenMultiplayerMenu()
    {
        openedMultiplayerMenu = true;
        multiplayerMenu.SetActive(openedMultiplayerMenu);
    }
    public void CloseMultiplayerMenu()
    {
        openedMultiplayerMenu = false;
        multiplayerMenu.SetActive(openedMultiplayerMenu);
    }
    public void ToggleMultiplayerMenu()
    {
        if (openedMultiplayerMenu)
        {
            CloseMultiplayerMenu();
        }
        else
        {
            OpenMultiplayerMenu();
        }
    }
    #endregion

    #region Update Logs Menus
    public void OpenUpdateLogsMenu()
    {
        openedUpdateLogsMenu = true;
        updateLogsMenu.SetActive(openedUpdateLogsMenu);
    }
    public void CloseUpdateLogsMenu()
    {
        openedUpdateLogsMenu = false;
        updateLogsMenu.SetActive(openedUpdateLogsMenu);
    }
    public void ToggleUpdateLogsMenu(bool toggle)
    {
        openedUpdateLogsMenu = toggle;
        updateLogsMenu.SetActive(toggle);
    }
    #endregion

    #region Cosmetics Menu
    public void OpenCosmeticsMenu()
    {
        openedCosmeticsMenu = true;
        cosmeticsMenu.SetActive(openedCosmeticsMenu);
    }
    public void CloseCosmeticsMenu()
    {
        openedCosmeticsMenu = false;
        cosmeticsMenu.SetActive(openedCosmeticsMenu);
    }
    public void ToggleCosmeticsMenu()
    {
        if (openedMultiplayerMenu)
        {
            CloseMultiplayerMenu();
        }
        else
        {
            OpenMultiplayerMenu();
        }
    }
    #endregion

    #region Room Menus
    public void OpenRoomMenu()
    {
        openedRoomMenu = true;
        roomMenu.SetActive(openedRoomMenu);
    }
    public void CloseRoomMenu()
    {
        openedRoomMenu = false;
        roomMenu.SetActive(openedRoomMenu);
    }
    public void ToggleRoomMenu()
    {
        if (openedRoomMenu)
        {
            CloseRoomMenu();
        }
        else
        {
            OpenRoomMenu();
        }
    }
    #endregion

    #region Find Room Menus
    public void OpenFindRoomMenu()
    {
        openedFindRoomMenu = true;
        findRoomMenu.SetActive(openedFindRoomMenu);
    }
    public void CloseFindRoomMenu()
    {
        openedFindRoomMenu = false;
        findRoomMenu.SetActive(openedFindRoomMenu);
    }
    public void ToggleFindRoomMenu()
    {
        if (openedFindRoomMenu)
        {
            CloseFindRoomMenu();
        }
        else
        {
            OpenFindRoomMenu();
        }
    }
    public void SetFindRoomText(string text)
    {
        findRoomIndicator.text = text;
    }
    public string GetFindRoomText()
    {
        return findRoomIndicator.text;
    }
    public int GetRoomCodeInputField()
    {
        return int.Parse(findRoomInputField.text);
    }
    #endregion

    #region Loading Menus
    public void OpenLoadingMenu()
    {
        openedLoadingMenu = true;
        loadingMenu.SetActive(openedLoadingMenu);
    }
    public void CloseLoadingMenu()
    {
        openedLoadingMenu = false;
        loadingMenu.SetActive(openedLoadingMenu);
    }
    public void ToggleLoadingMenu()
    {
        if (openedLoadingMenu)
        {
            CloseLoadingMenu();
        }
        else
        {
            OpenLoadingMenu();
        }
    }
    #endregion

    #region Settings Menus
    public void OpenSettingsMenu()
    {
        openedSettingsMenu = true;
        settingsMenu.SetActive(openedSettingsMenu);
    }
    public void CloseSettingsMenu()
    {
        openedSettingsMenu = false;
        settingsMenu.SetActive(openedSettingsMenu);
    }
    public void ToggleSettingsMenu()
    {
        if (openedLoadingMenu)
        {
            CloseSettingsMenu();
        }
        else
        {
            OpenSettingsMenu();
        }
    }
    #endregion

    #region Create Room Menu
    public void OpenCreateRoomMenu()
    {
        openedCreateRoomMenu = true;
        createRoomMenu.SetActive(openedCreateRoomMenu);
        OnChangedMaxPlayers(int.Parse(maxPlayers.text));
        OnSelectedGamemode(selectedGamemodes);
        OnSelectedGamemode(selectedGamemodes);
    }
    public void CloseCreateRoomMenu()
    {
        openedCreateRoomMenu = false;
        createRoomMenu.SetActive(openedCreateRoomMenu);
    }
    public void ToggleCreateRoomMenu()
    {
        if (openedLoadingMenu)
        {
            CloseCreateRoomMenu();
        }
        else
        {
            OpenCreateRoomMenu();
        }
    }
    #endregion

    #region Loadout Selection Menu
    public void OpenLoadoutSelectionMenu()
    {
        openedLoadoutSelectionMenu = true;
        loadoutSelectionMenu.SetActive(openedLoadoutSelectionMenu);
    }
    public void CloseLoadoutSelectionMenu()
    {
        openedLoadoutSelectionMenu = false;
        loadoutSelectionMenu.SetActive(openedLoadoutSelectionMenu);
    }
    public void ToggleLoadoutSelectionMenu()
    {
        if (openedLoadingMenu)
        {
            CloseLoadoutSelectionMenu();
        }
        else
        {
            OpenLoadoutSelectionMenu();
        }
    }
    #endregion

    #region Main
    public Gamemodes GetGamemode()
    {
        return selectedGamemodes;
    }
    public void UseCreateRoomInputField()
    {
        if (usingCreateRooomInputField)
        {
            SetCreateRoomInputField(false);
        }
        else
        {
            SetCreateRoomInputField(true);
        }
    }
    public void SetCreateRoomInputField(bool value)
    {
        usingCreateRooomInputField = value;
        roomInputField.gameObject.SetActive(value);
        //if (value) createRoomButton.interactable = false;
        //else createRoomButton.interactable = true;
    }

    public void JoiningMasterLobby(bool value)
    {
        joinedMasterLobby = value;
        multiplayerButton.interactable = value;
        if (!value) connectionIndicator.text = "Connection Failed! Multiplayer Functions are now unavailable.";
        else connectionIndicator.text = "Connection Successful! Multiplayer Functions are now available.";
    }
    public string SetConnectionIndicatorText(string content)
    {
        if(content != null) connectionIndicator.text = content;
        else return connectionIndicator.text;
        return null;
    }
    public void RoomInputFieldText(string content)
    {
        roomInputField.text = content;
    }
    public string GetRoomInputFieldText()
    {
        return roomInputField.text;
    }
    public void SetInvalidInputFieldText(string content, Color color)
    {
        invalidInputField.text = content;
        invalidInputField.color = color;
    }
    public void SetRoomTitle(string text)
    {
        roomTitle.text = text;
    }
    public string GetRoomTitle()
    {
        return roomTitle.text;
    }
    #endregion

    #region Room Creation
    public RoomOptions GenerateRoomOptionsFromData(string roomName, string roomHostName, int mapInfoIndex, Gamemodes roomModes, int maxPlayer, int mapIndex, bool roomVisibility, int maxKillLimit)
    {
        Hashtable hash = new();
        RoomOptions roomOptions = new RoomOptions();
        int roomCode = Random.Range(10000000, 99999999);
        string[] tempValues = { "roomName", "roomHostName", "mapInfoIndex", "maxPlayer", "gameStarted", "roomMode", "roomMapIndex", "roomVisibility", "roomCode", "maxKillLimit" }; //Expose values to main lobby
        roomOptions.CustomRoomPropertiesForLobby = tempValues;
        roomOptions.CustomRoomProperties = new Hashtable();
        roomOptions.CustomRoomProperties.Add("roomName", roomName);
        roomOptions.CustomRoomProperties.Add("roomHostName", roomHostName);
        roomOptions.CustomRoomProperties.Add("mapInfoIndex", mapInfoIndex);
        roomOptions.CustomRoomProperties.Add("maxPlayer", maxPlayer);
        roomOptions.CustomRoomProperties.Add("gameStarted", false);
        switch (roomModes)
        {
            case Gamemodes.FFA:
                roomOptions.CustomRoomProperties.Add("roomMode", "Free For All");
                break;
            case Gamemodes.TDM:
                roomOptions.CustomRoomProperties.Add("roomMode", "Team Deathmatch");
                break;
            case Gamemodes.KOTH:
                roomOptions.CustomRoomProperties.Add("roomMode", "King of the Hills");
                break;
            case Gamemodes.DZ:
                roomOptions.CustomRoomProperties.Add("roomMode", "Drop Zones");
                break;
        }
        roomOptions.CustomRoomProperties.Add("roomMapIndex", mapIndex);
        roomOptions.CustomRoomProperties.Add("roomVisibility", roomVisibility);
        roomOptions.CustomRoomProperties.Add("roomCode", roomCode);
        roomOptions.CustomRoomProperties.Add("maxKillLimit", maxKillLimit);
        roomOptions.MaxPlayers = (byte)maxPlayer;

        /*
        hash.Add("roomName", roomName);
        hash.Add("roomHostName", roomHostName);
        hash.Add("mapInfoIndex", mapInfoIndex);
        hash.Add("maxPlayer", maxPlayer);
        switch (roomModes)
        {
            case Gamemodes.FFA:
                hash.Add("roomMode", "Free For All");
                break;
            case Gamemodes.TDM:
                hash.Add("roomMode", "Team Deathmatch");
                break;
            case Gamemodes.KOTH:
                hash.Add("roomMode", "King of the Hills");
                break;
            case Gamemodes.DZ:
                hash.Add("roomMode", "Drop Zones");
                break;
        }
        hash.Add("roomMapIndex", mapIndex);
        hash.Add("roomVisibility", roomVisibility);
        hash.Add("roomCode", roomCode);
        roomOptions.CustomRoomProperties = hash;*/
        return roomOptions;
    }
    public RoomOptions GetGeneratedRoomOptions()
    {
        RoomOptions roomOptionsTemp = new RoomOptions();
        roomOptionsTemp.CustomRoomProperties = new Hashtable();
        roomOptionsTemp = GenerateRoomOptionsFromData(GetRoomInputFieldText(), PhotonNetwork.NickName, roomMapSelectionIndex, selectedGamemodes, maxPlayerCount, MapListItemHolder.Instance.selectedMapIndex, roomVisibility, maxKillLimitNumber);
        return roomOptionsTemp;
    }
    public void OnCreateRoomInputSubmit(string roomInput)
    {
        selectedRoomName.text = "Room Name: " + roomInput;
    }
    public void OnSelectedMap(string mapNameInput)
    {
        selectedMap.text = "Selected Map: " + mapNameInput;
    }
    public void OnSelectedGamemode(Gamemodes gmInput)
    {
        string temp = "None";
        switch (gmInput)
        {
            case Gamemodes.TDM:
                temp = "Team Deathmatch";
                break;
            case Gamemodes.KOTH:
                temp = "King of the Hills";
                break;
            case Gamemodes.DZ:
                temp = "Drop Zones";
                break;
            case Gamemodes.FFA:
                temp = "Free For All";
                break;
        }
        selectedGamemode.text = "Selected Gamemode: " + temp;
    }
    public void OnChangedMaxPlayers(int maxPlayersInput)
    {
        selectedMaxPlayers.text = "Max Players: " + maxPlayersInput.ToString();
    }
    public void OnChangedMaxKillLimit(int input)
    {
        selectedMaxKillLimit.text = "Max Kill Limit: " + input.ToString();
    }
    public void OnChangedVisibility(bool visible)
    {
        if (visible) visibility.text = "Public";
        else visibility.text = "Private";
    }

    public void DecreasePlayerCount()
    {
        int temp = int.Parse(maxPlayers.text);
        if (temp - 1 < 2) return;
        maxPlayerCount = temp - 1;
        maxPlayers.text = (temp - 1).ToString();
        OnChangedMaxPlayers(temp - 1);
    }
    public void IncreasePlayerCount()
    {
        int temp = int.Parse(maxPlayers.text);
        if (temp + 1 > 20) return;
        maxPlayerCount = temp + 1;
        maxPlayers.text = (temp + 1).ToString();
        OnChangedMaxPlayers(temp + 1);
    }
    public void DecreaseKillLimit()
    {
        int temp = int.Parse(maxKillLimit.text);
        if (temp - 1 < 10) return;
        maxKillLimitNumber = temp - 1;
        maxKillLimit.text = (temp - 1).ToString();
        OnChangedMaxKillLimit(temp - 1);
    }
    public void IncreaseKillLimit()
    {
        int temp = int.Parse(maxKillLimit.text);
        if (temp + 1 > 50) return;
        maxKillLimitNumber = temp + 1;
        maxKillLimit.text = (temp + 1).ToString();
        OnChangedMaxKillLimit(temp + 1);
    }


    public void NextGamemodeSelect()
    {
        switch (selectedGamemodes)
        {
            case Gamemodes.FFA:
                gamemodes.text = "TDM";
                selectedGamemodes = Gamemodes.TDM;
                break;
            case Gamemodes.TDM:
                gamemodes.text = "KOTH";
                selectedGamemodes = Gamemodes.KOTH;
                break;
            case Gamemodes.KOTH:
                gamemodes.text = "DZ";
                selectedGamemodes = Gamemodes.DZ;
                break;
            case Gamemodes.DZ:
                gamemodes.text = "FFA";
                selectedGamemodes = Gamemodes.FFA;
                break;
        }
        OnSelectedGamemode(selectedGamemodes);
    }
    public void PreviousGamemodeSelect()
    {
        switch (selectedGamemodes)
        {
            case Gamemodes.FFA:
                gamemodes.text = "DZ";
                selectedGamemodes = Gamemodes.DZ;
                break;
            case Gamemodes.DZ:
                gamemodes.text = "KOTH";
                selectedGamemodes = Gamemodes.KOTH;
                break;
            case Gamemodes.KOTH:
                gamemodes.text = "TDM";
                selectedGamemodes = Gamemodes.TDM;
                break;
            case Gamemodes.TDM:
                gamemodes.text = "FFA";
                selectedGamemodes = Gamemodes.FFA;
                break;
        }
        OnSelectedGamemode(selectedGamemodes);
    }

    public void ToggleVisibilitySelect()
    {
        if (roomVisibility) roomVisibility = false;
        else roomVisibility = true;
        OnChangedVisibility(roomVisibility);
    }
    #endregion

    #region Shop Menu
    public void ToggleShopMenu(bool value)
    {
        openedShopMenu = value;
        shopMenu.SetActive(value);
        if (value)
        {
            shopMenu.GetComponent<ShopMenuScript>().ToggleWeaponsMenu(value);
            shopMenu.GetComponent<ShopMenuScript>().TogglePreviewUI(value);
        }
    }
    #endregion

    #region Popup Menu
    public void TogglePopupMenu(bool value)
    {
        openedPopupMenu = value;
        popupMenu.SetActive(value);
    }
    public void AddModalWindow(string title, string content)
    {
        ModalWindowManager item = Instantiate(modalWindowPrefab, popupHolder).GetComponent<ModalWindowManager>();
        item.gameObject.SetActive(true);
        item.titleText = title;
        item.descriptionText = content;
        item.UpdateUI();
        item.Open();
        //popupWindows.Add(item)
        //Debug.Log("Popup Instantiated");
    }
    public void QueueModalWindow(string title, string content, PopupQueue queueType)
    {
        PopupData temp;
        temp.title = title;
        temp.content = content;
        temp.queueType = queueType;
        queuedModalWindows.Add(temp);
    }
    public bool RemoveModalWindow(ModalWindowManager i)
    {
        if (modalWindowList.Contains(i))
        {
            modalWindowList.Remove(i);
            Destroy(i.gameObject);
            return true;
        }
        return false;
    }
    public void AddNotification(string title, string content)
    {
        NotificationManager item = Instantiate(notificationPrefab, notificationHolder).GetComponent<NotificationManager>();
        item.gameObject.SetActive(true);
        item.title = title;
        item.description = content;
        item.UpdateUI();
        item.Open();
        Destroy(item.gameObject, 5f);
        //popupWindows.Add(item)
        //Debug.Log("Popup Instantiated");
    }
    public void QueueNotification(string title, string content, PopupQueue queueType)
    {
        PopupData temp;
        temp.title = title;
        temp.content = content;
        temp.queueType = queueType;
        queuedNotifications.Add(temp);
    }
    public bool RemoveNotification(NotificationManager i)
    {
        if (notificationList.Contains(i))
        {
            notificationList.Remove(i);
            Destroy(i.gameObject);
            return true;
        }
        return false;
    }
    #endregion

    #region User GUI
    public void SetUserGUIData(string name, int level, float levelProgress, int coin)
    {
        username.text = name;
        userLevel.text = level.ToString();
        userLevelProgress.value = levelProgress / (float)(level * UserDatabase.Instance.levelLimiter);
        userCoins.text = "$" + coin.ToString();
    }
    public void UpdateName(string name)
    {
        username.text = name;
    }
    public void UpdateCoin(int coin)
    {
        userCoins.text = "$" + coin.ToString();
    }
    public void UpdateLevels(int level, float levelProgress)
    {
        userLevel.text = level.ToString();
        userLevelProgress.value = levelProgress;
    }
    #endregion
}
