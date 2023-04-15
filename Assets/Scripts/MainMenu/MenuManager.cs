using System.Collections;
using System.IO;
using System;
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
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using PrototypeLib.Modules.FileOperations.IO;
using PrototypeLib.OnlineServices.PUNMultiplayer.ConfigurationKeys;
using UserConfiguration;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    //[SerializeField] private Button multiplayerButton;
    public delegate void ToggleMenu(bool state = true, string name = "null", int id = -1);
    public delegate MenuIdentifier SearchMenu(string name = "null", int id = -1);
    public static event ToggleMenu OnMenuToggled;
    public static event SearchMenu OnSearchMenu;
    [Space]
    [Header("Menus")]
    public GameObject login;
    public GameObject main;
    public RoomMenuComponent RoomMenuComp;
    public Button multiplayerMenuButton;
    public GameObject roomMenu, mainMenu;
    public GameObject loadoutSelectionMenu;
    public GameObject popupMenu;
    public GameObject shopMenu;
    public GameObject gunsmithMenu;
    public GameObject enterAnimMenu;
    public List<GameObject> setActiveMenuList = new();
    public List<MenuIdentifier> MenuIdentifiers = new();

    [Space]
    [Header("Misc Components")]
    [SerializeField] private InputField findRoomInputField;
    public TMP_InputField playerNameInputField;
    public Animator enterAnim;

    [Space]
    [Header("Menu States")]
    public bool openedMainMenu = false;
    public bool openedMultiplayerMenu = false;
    public bool openedRoomMenu = false;
    public bool openedFindRoomMenu = false;
    public bool openedLoadingMenu = false;
    public bool openedCreateRoomMenu = false;
    public bool openedLoadoutSelectionMenu = false;
    public bool openedPopupMenu = false;
    public bool openedShopMenu = false;
    public bool openedGunsmithMenu = false;
    public bool usingCreateRooomInputField = false;
    public bool isConnected = false;

    [Space]
    [Header("Menu Online States")]
    [SerializeField] private bool joinedMasterLobby = false;

    [Space]
    [Header("Room Creation")]
    [SerializeField] private InputField roomInputField;
    [SerializeField] private int roomMapSelectionIndex;
    [SerializeField] private Gamemodes selectedGamemodes = Gamemodes.FFA;
    [SerializeField] private int maxPlayerCount = 10;
    [SerializeField] private int maxKillLimitNumber = 30;
    [SerializeField] private bool roomVisibility = true;
    [SerializeField] private bool allowDownedState = false;
    [SerializeField] private Transform popupHolder;
    [SerializeField] private Transform notificationHolder;

    [Header("Room Visual")]
    [SerializeField] Text maxPlayers;
    [SerializeField] Text gamemodes;
    [SerializeField] Text visibility;
    [SerializeField] Text maxKillLimit;
    [SerializeField] Text downedState;

    [Header("Top Navi-Bar UI")]
    [SerializeField] GameObject quitButton;
    [SerializeField] GameObject backButton;
    [Header("QuickMatch UI HUD")]
    [SerializeField] Text passedTime, modeMessage;
    [SerializeField] public GameObject quitMatchmakingButton;

    [Space, Header("Version Manifests")]
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
    public Image userLevelProgress;
    private bool startQMTimer = false;
    private float QMTimerSeconds, QMTimerMinutes;
    [Serializable, SerializeField]
    public enum Gamemodes
    {
        FFA = 1,
        TDM = 2,
        CTF = 3,
        DZ = 4
    }
    private void Awake()
    {
        FileOps<UserDataJSON>.OperatedFile += DetectIfUserDataModified;
        foreach (GameObject tp in setActiveMenuList)
        {
            tp.SetActive(true);
        }
        Instance = this;
        gunsmithMenuState += OnGunsmithMenuToggled;
        int tmep = 0;
        foreach (MenuIdentifier id in MenuIdentifiers)
        {
            id.SetID(tmep);
            tmep++;
        }
        //login.SetActive(true);
    }
    private void DetectIfUserDataModified(string strPath, UserDataJSON tmp)
    {
        if (strPath == UserSystem.UserDataPath)
        {
            username.text = tmp.username;
            userLevel.text = tmp.userLevel.ToString();
            userLevelProgress.fillAmount = (float)tmp.userLevelXP / (float)(tmp.userLevel * UserDatabase.Instance.levelLimiter);
            userCoins.text = tmp.userCoins.ToString();
        }
    }
    public void SetQuitButtonState(bool state)
    {
        if (quitButton == null || backButton == null) return;
        quitButton.SetActive(state);
        backButton.SetActive(!state);
    }
    public void SetMainMenuState(bool state)
    {
        if (mainMenu == null || roomMenu == null) return;
        mainMenu.SetActive(state);
        roomMenu.SetActive(!state);
    }
    public void CloseMenu(int id) { OnMenuToggled?.Invoke(false, "null", id); }
    public void CloseMenu(string id) { OnMenuToggled?.Invoke(false, id); }
    public void CloseMenu(MenuIdentifier id) { OnMenuToggled?.Invoke(false, id.menuName, id.menuID); }
    public void OpenMenu(int id) { OnMenuToggled?.Invoke(true, "null", id); }
    public void OpenMenu(string id) { OnMenuToggled?.Invoke(true, id); }
    public void OpenMenu(MenuIdentifier id) { OnMenuToggled?.Invoke(true, id.menuName, id.menuID); }
    public MenuIdentifier FindMenu(int id) { return OnSearchMenu?.Invoke("null", id); }
    public MenuIdentifier FindMenu(string id) { return OnSearchMenu?.Invoke(id); }
    public MenuIdentifier FindMenu(bool state) { return OnSearchMenu?.Invoke(state ? "true" : "false"); }
    public void OnInstructedMenuIdentifier(bool val, string nm)
    {
        if (val && nm == "main") SetQuitButtonState(true);
        else SetQuitButtonState(false);
        switch (nm)
        {
            case "main":
                if (roomMenu == null) break;
                if (!roomMenu.activeSelf) { openedMainMenu = val; }
                else { openedRoomMenu = val; }
                if (roomMenu.activeSelf) openedMainMenu = false;
                if (!roomMenu.activeSelf) openedRoomMenu = false;
                break;
            case "multiplayer":
                openedMultiplayerMenu = val;
                break;
            case "findRoom":
                openedFindRoomMenu = val;
                break;
            case "createRoom":
                openedCreateRoomMenu = val;
                break;
            case "loadout":
                openedLoadoutSelectionMenu = val;
                break;
            case ("room" or ""):
                break;
        }
    }
    void Start()
    {
        //RefreshMenu();
        //Debug.Log("Loaded Scene from Main Menu");
        RefreshMenu();
        JoiningMasterLobby(false);
        CloseLoadingMenu();
        CloseFindRoomMenu();
        CloseCosmeticsMenu();
        CloseCreateRoomMenu();
        CloseSettingsMenu();
        CloseMultiplayerMenu();
        ToggleGunsmithMenu(false);
        SetMainMenuState(true);
        OpenMainMenu();
        quitMatchmakingButton.SetActive(false);
        for (int i = 0; i < versionTexts.Count; i++)
        {
            versionTexts[i].text = "V" + LocalLaunchedClient.LocalGameVersion.ToString();
        }
        if (LauncherConfig.CompareLocalVersionWithCache())
        {

        }
    }
    private void OnEnable()
    {
    }
    public void SetQuickMatchUIInfo(string topMessage, bool startTimer)
    {
        startQMTimer = startTimer;
        if (!startQMTimer)
        {
            QMTimerMinutes = 0;
            QMTimerSeconds = 0;
            passedTime.text = Launcher.Instance.foundMatch ? "Match Found" : "Pasued";
        }
        modeMessage.text = topMessage;
    }
    public void CloseCurrentMenu()
    {
        OnMenuToggled?.Invoke(false,"command.CloseAllMenus",-1);
    }
    public void RefreshMenu()
    {
        foreach (MenuIdentifier ip in MenuIdentifiers)
        {
            ip.gameObject.SetActive(true);
            CloseMenu(ip);
        }
    }
    private void FixedUpdate()
    {
        if (startQMTimer)
        {
            QMTimerSeconds += Time.fixedDeltaTime;
            if ((int)QMTimerSeconds >= 60)
            {
                QMTimerSeconds = 0;
                QMTimerMinutes++;
            }
            passedTime.text = ((int)QMTimerMinutes < 10 ? $"0{(int)QMTimerMinutes}:" : $"{(int)QMTimerMinutes}:") + ((int)QMTimerSeconds < 10 ? $"0{(int)QMTimerSeconds}" : $"{(int)QMTimerSeconds}");
        }
    }
    #region Main Menus
    public void OpenMainMenu()
    {
        OpenMenu("main");
    }
    public void CloseMainMenu()
    {
        CloseMenu("main");
    }

    public void OpenCosmeticsMenu()
    {
        OpenMenu("cosmetics");
    }
    public void CloseCosmeticsMenu()
    {
        CloseMenu("cosmetics");
    }

    public void OpenMultiplayerMenu()
    {
        OpenMenu("multiplayer");
    }
    public void CloseMultiplayerMenu()
    {
        CloseMenu("multiplayer");
    }

    public void OpenFindRoomMenu()
    {
        OpenMenu("findRoom");
    }
    public void CloseFindRoomMenu()
    {
        CloseMenu("findRoom");
    }
    public int GetRoomCodeInputField()
    {
        return int.Parse(findRoomInputField.text);
    }

    public void OpenLoadingMenu()
    {
        OpenMenu("loading");
    }
    public void CloseLoadingMenu()
    {
        CloseMenu("loading");
    }

    public void OpenSettingsMenu()
    {
        OpenMenu("settings");
    }
    public void CloseSettingsMenu()
    {
        CloseMenu("settings");
    }

    public void OpenCreateRoomMenu()
    {
        OpenMenu("createRoom");
    }
    public void CloseCreateRoomMenu()
    {
        CloseMenu("createRoom");
    }

    public void OpenLoadoutSelectionMenu()
    {
        OpenMenu("loadout");
    }
    public void CloseLoadoutSelectionMenu()
    {
        CloseMenu("loadout");
    }

    public Action gunsmithMenuState;
    public void ToggleGunsmithMenu(bool state)
    {
        gunsmithMenuState.Invoke();
        openedGunsmithMenu = state;
        gunsmithMenu.SetActive(openedGunsmithMenu);
    }
    public virtual void OnGunsmithMenuToggled()
    {

    }

    public Gamemodes GetGamemode()
    {
        return selectedGamemodes;
    }

    public void JoiningMasterLobby(bool value)
    {
        joinedMasterLobby = value;
        isConnected = value;
        enterAnim.SetBool("isConnected", isConnected);
        StartCoroutine(DelayDisableEnterAnim(2f));
    }
    IEnumerator DelayDisableEnterAnim(float time)
    {
        yield return new WaitForSeconds(time);
        if (true) StartCoroutine(DelayDisableEnterAnim(1f));
        //else RoomManager.Instance.loadEnterAnim = false;
    }
    public string SetConnectionIndicatorText(string content)
    {
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

    public RoomOptions GenerateRoomOptionsFromData(string roomName, string roomHostName, int mapInfoIndex, Gamemodes roomModes, int maxPlayer, int mapIndex, bool roomVisibility, int maxKillLimit, bool allowDownedState = false, bool randomRespawn = true)
    {
        Hashtable hash = new();
        RoomOptions roomOptions = new RoomOptions();
        int roomCode = UnityEngine.Random.Range(10000000, 99999999);
        string[] tempValues = { RoomKeys.RoomName, RoomKeys.RoomHostName, RoomKeys.MapInfoIndex, RoomKeys.MaxPlayer, RoomKeys.GameStarted, RoomKeys.RandomRespawn, RoomKeys.RoomMode, RoomKeys.RoomMapIndex, RoomKeys.RoomVisibility, RoomKeys.RoomCode, RoomKeys.MaxKillLimit, RoomKeys.AllowDownedState }; //Expose values to main lobby
        roomOptions.CustomRoomPropertiesForLobby = tempValues;
        roomOptions.CustomRoomProperties = new Hashtable();
        roomOptions.CustomRoomProperties.Add(RoomKeys.RoomName, roomName);
        roomOptions.CustomRoomProperties.Add(RoomKeys.RoomHostName, roomHostName);
        roomOptions.CustomRoomProperties.Add(RoomKeys.MapInfoIndex, mapInfoIndex);
        roomOptions.CustomRoomProperties.Add(RoomKeys.MaxPlayer, maxPlayer);
        roomOptions.CustomRoomProperties.Add(RoomKeys.GameStarted, false);
        roomOptions.CustomRoomProperties.Add(RoomKeys.RandomRespawn, randomRespawn);
        switch (roomModes)
        {
            case Gamemodes.FFA:
                roomOptions.CustomRoomProperties.Add(RoomKeys.RoomMode, "Free For All");
                break;
            case Gamemodes.TDM:
                roomOptions.CustomRoomProperties.Add(RoomKeys.RoomMode, "Team Deathmatch");
                break;
            case Gamemodes.CTF:
                roomOptions.CustomRoomProperties.Add(RoomKeys.RoomMode, "Capture The Flag");
                break;
            case Gamemodes.DZ:
                roomOptions.CustomRoomProperties.Add(RoomKeys.RoomMode, "Drop Zones");
                break;
        }
        roomOptions.CustomRoomProperties.Add(RoomKeys.RoomMapIndex, mapIndex);
        roomOptions.CustomRoomProperties.Add(RoomKeys.RoomVisibility, roomVisibility);
        roomOptions.CustomRoomProperties.Add(RoomKeys.RoomCode, roomCode);
        roomOptions.CustomRoomProperties.Add(RoomKeys.MaxKillLimit, maxKillLimit);
        roomOptions.CustomRoomProperties.Add(RoomKeys.AllowDownedState, allowDownedState);
        roomOptions.MaxPlayers = (byte)maxPlayer;
        return roomOptions;
    }
    public RoomOptions GetGeneratedRoomOptions()
    {
        RoomOptions roomOptionsTemp = new RoomOptions();
        roomOptionsTemp.CustomRoomProperties = new Hashtable();

        //Force Gamemode to FFA
        roomOptionsTemp = GenerateRoomOptionsFromData(GetRoomInputFieldText(), PhotonNetwork.NickName, roomMapSelectionIndex, selectedGamemodes, maxPlayerCount, MapListItemHolder.Instance.selectedMapIndex, roomVisibility, maxKillLimitNumber, allowDownedState);
        return roomOptionsTemp;
    }
    public void OnChangedVisibility(bool visible)
    {
        if (visible) visibility.text = "Public";
        else visibility.text = "Private";
    }
    public void OnChangedAllowDownedState(bool allow)
    {
        if (allow) downedState.text = "On";
        else downedState.text = "Off";
    }
    public void DecreasePlayerCount()
    {
        int temp = int.Parse(maxPlayers.text);
        if (temp - 1 < 2) return;
        maxPlayerCount = temp - 1;
        maxPlayers.text = (temp - 1).ToString();
    }
    public void IncreasePlayerCount()
    {
        int temp = int.Parse(maxPlayers.text);
        if (temp + 1 > 20) return;
        maxPlayerCount = temp + 1;
        maxPlayers.text = (temp + 1).ToString();
    }
    public void DecreaseKillLimit()
    {
        int temp = int.Parse(maxKillLimit.text);
        if (temp - 1 < 10) return;
        maxKillLimitNumber = temp - 1;
        maxKillLimit.text = (temp - 1).ToString();
    }
    public void IncreaseKillLimit()
    {
        int temp = int.Parse(maxKillLimit.text);
        if (temp + 1 > 50) return;
        maxKillLimitNumber = temp + 1;
        maxKillLimit.text = (temp + 1).ToString();
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
                gamemodes.text = "CTF";
                selectedGamemodes = Gamemodes.CTF;
                break;
            case Gamemodes.CTF:
                gamemodes.text = "DZ";
                selectedGamemodes = Gamemodes.DZ;
                break;
            case Gamemodes.DZ:
                gamemodes.text = "FFA";
                selectedGamemodes = Gamemodes.FFA;
                break;
        }
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
                gamemodes.text = "CTF";
                selectedGamemodes = Gamemodes.CTF;
                break;
            case Gamemodes.CTF:
                gamemodes.text = "TDM";
                selectedGamemodes = Gamemodes.TDM;
                break;
            case Gamemodes.TDM:
                gamemodes.text = "FFA";
                selectedGamemodes = Gamemodes.FFA;
                break;
        }
    }

    public void ToggleVisibilitySelect()
    {
        if (roomVisibility) roomVisibility = false;
        else roomVisibility = true;
        OnChangedVisibility(roomVisibility);
    }
    public void ToggleAllowDownedStateSelect()
    {
        if (allowDownedState) allowDownedState = false;
        else allowDownedState = true;
        OnChangedAllowDownedState(allowDownedState);
    }

    public void ToggleShopMenu(bool value)
    {
        openedShopMenu = value;
        shopMenu.SetActive(value);
    }
    public void TogglePopupMenu(bool value)
    {
        openedPopupMenu = value;
        popupMenu.SetActive(value);
    }
    public ModalWindowManager AddModalWindow(string title, string content)
    {
        ModalWindowManager item = Instantiate(modalWindowPrefab, popupHolder).GetComponent<ModalWindowManager>();
        item.gameObject.SetActive(true);
        item.titleText = title;
        item.descriptionText = content;
        item.UpdateUI();
        item.Open();
        return item;
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
}
