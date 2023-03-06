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

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    //[SerializeField] private Button multiplayerButton;
    public delegate void ToggleMenu(bool state = true, string name = "null", int id = -1);
    public delegate MenuIdentifier SearchMenu(string name = "null", int id = -1);
    public static event ToggleMenu OnMenuToggled;
    public static event SearchMenu OnSearchMenu;
    [Space]
    [Header("Menus")]
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
    [SerializeField] private Transform popupHolder;
    [SerializeField] private Transform notificationHolder;

    [Header("Room Visual")]
    [SerializeField] Text maxPlayers;
    [SerializeField] Text gamemodes;
    [SerializeField] Text visibility;
    [SerializeField] Text maxKillLimit;

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
    public Slider userLevelProgress;
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
        foreach (GameObject tp in setActiveMenuList)
        {
            tp.SetActive(true);
        }
        instance = this;
        gunsmithMenuState += OnGunsmithMenuToggled;
        RefreshMenu();
        int tmep = 0;
        foreach (MenuIdentifier id in MenuIdentifiers)
        {
            id.SetID(tmep);
            tmep++;
        }
    }
    public void SetQuitButtonState(bool state)
    {
        quitButton.SetActive(state);
        backButton.SetActive(!state);
    }
    public void SetMainMenuState(bool state)
    {
        mainMenu.SetActive(state);
        roomMenu.SetActive(!state);
    }
    public void CloseMenu(int id) { OnMenuToggled?.Invoke(false, "null", id); }
    public void CloseMenu(string id) { OnMenuToggled?.Invoke(false, id); }
    public void CloseMenu(MenuIdentifier id) { OnMenuToggled?.Invoke(false, id.menuName, id.menuID); Debug.Log($"{id.menuName}"); }
    public void OpenMenu(int id) { OnMenuToggled?.Invoke(true, "null", id); }
    public void OpenMenu(string id) { OnMenuToggled?.Invoke(true, id); }
    public void OpenMenu(MenuIdentifier id) { OnMenuToggled?.Invoke(true, id.menuName, id.menuID); }
    public MenuIdentifier FindMenu(int id) { return OnSearchMenu?.Invoke("null", id); }
    public MenuIdentifier FindMenu(string id) { return OnSearchMenu?.Invoke(id); }
    public MenuIdentifier FindMenu(bool state) { return OnSearchMenu?.Invoke(state ? "true" : "false"); }
    public static void OnInstructedMenuIdentifier(bool val, string nm)
    {
        if (val && nm == "main") MenuManager.instance.SetQuitButtonState(true);
        else MenuManager.instance.SetQuitButtonState(false);
    }
    void Start()
    {
        //RefreshMenu();
        Debug.Log("Loaded Scene from Main Menu");
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
        foreach (MenuIdentifier ip in MenuIdentifiers)
        {
            CloseMenu(ip);
        }
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
        RoomManager.Instance.loadEnterAnim = false;
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

    public RoomOptions GenerateRoomOptionsFromData(string roomName, string roomHostName, int mapInfoIndex, Gamemodes roomModes, int maxPlayer, int mapIndex, bool roomVisibility, int maxKillLimit, bool randomRespawn = true)
    {
        Hashtable hash = new();
        RoomOptions roomOptions = new RoomOptions();
        int roomCode = UnityEngine.Random.Range(10000000, 99999999);
        string[] tempValues = { "roomName", "roomHostName", "mapInfoIndex", "maxPlayer", "gameStarted", "randomRespawn", "roomMode", "roomMapIndex", "roomVisibility", "roomCode", "maxKillLimit" }; //Expose values to main lobby
        roomOptions.CustomRoomPropertiesForLobby = tempValues;
        roomOptions.CustomRoomProperties = new Hashtable();
        roomOptions.CustomRoomProperties.Add("roomName", roomName);
        roomOptions.CustomRoomProperties.Add("roomHostName", roomHostName);
        roomOptions.CustomRoomProperties.Add("mapInfoIndex", mapInfoIndex);
        roomOptions.CustomRoomProperties.Add("maxPlayer", maxPlayer);
        roomOptions.CustomRoomProperties.Add("gameStarted", false);
        roomOptions.CustomRoomProperties.Add("randomRespawn", randomRespawn);
        switch (roomModes)
        {
            case Gamemodes.FFA:
                roomOptions.CustomRoomProperties.Add("roomMode", "Free For All");
                break;
            case Gamemodes.TDM:
                roomOptions.CustomRoomProperties.Add("roomMode", "Team Deathmatch");
                break;
            case Gamemodes.CTF:
                roomOptions.CustomRoomProperties.Add("roomMode", "Capture The Flag");
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
        return roomOptions;
    }
    public RoomOptions GetGeneratedRoomOptions()
    {
        RoomOptions roomOptionsTemp = new RoomOptions();
        roomOptionsTemp.CustomRoomProperties = new Hashtable();

        //Force Gamemode to FFA
        roomOptionsTemp = GenerateRoomOptionsFromData(GetRoomInputFieldText(), PhotonNetwork.NickName, roomMapSelectionIndex, Gamemodes.FFA, maxPlayerCount, MapListItemHolder.Instance.selectedMapIndex, roomVisibility, maxKillLimitNumber);
        return roomOptionsTemp;
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
