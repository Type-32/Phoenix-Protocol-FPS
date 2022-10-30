using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    [Space]
    [Header("Misc Components")]
    [SerializeField] private Text connectionIndicator;
    [SerializeField] private Text invalidInputField;
    [SerializeField] private Text roomTitle;
    [SerializeField] private Text findRoomIndicator;
    [SerializeField] private InputField findRoomInputField;
    public InputField playerNameInputField;

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
    public bool usingCreateRooomInputField = false;

    [Space]
    [Header("Menu Online States")]
    [SerializeField] private bool joinedMasterLobby = false;

    [Space]
    [Header("Room Creation")]
    [SerializeField] private InputField roomInputField;
    [SerializeField] private int roomMapSelectionIndex;
    [SerializeField] private Gamemodes selectedGamemodes = Gamemodes.FFA;
    [SerializeField] private int maxPlayerCount = 8;
    [SerializeField] private bool roomVisibility = true;

    [Header("Room Visual")]
    [SerializeField] Text maxPlayers;
    [SerializeField] Text gamemodes;
    [SerializeField] Text visibility;

    [Header("Room Preview")]
    [SerializeField] Text selectedRoomName;
    [SerializeField] Text selectedMap;
    [SerializeField] Text selectedGamemode;
    [SerializeField] Text selectedMaxPlayers;


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
    public RoomOptions GenerateRoomOptionsFromData(string roomName, string roomHostName, int mapInfoIndex, Gamemodes roomGamemodes, int maxPlayer, int mapIndex, bool roomVisibility)
    {
        Hashtable hash = new();
        RoomOptions roomOptions = new RoomOptions();
        int roomCode = Random.Range(10000000, 99999999);
        string[] tempValues = { "roomName", "roomHostName", "mapInfoIndex", "maxPlayer", "roomMode", "roomMapIndex", "roomVisibility", "roomCode" }; //Expose values to main lobby
        roomOptions.CustomRoomPropertiesForLobby = tempValues;
        roomOptions.CustomRoomProperties = new Hashtable();
        roomOptions.CustomRoomProperties.Add("roomName", roomName);
        roomOptions.CustomRoomProperties.Add("roomHostName", roomHostName);
        roomOptions.CustomRoomProperties.Add("mapInfoIndex", mapInfoIndex);
        roomOptions.CustomRoomProperties.Add("maxPlayer", maxPlayer);
        switch (roomGamemodes)
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
        roomOptions.MaxPlayers = (byte)maxPlayer;

        /*
        hash.Add("roomName", roomName);
        hash.Add("roomHostName", roomHostName);
        hash.Add("mapInfoIndex", mapInfoIndex);
        hash.Add("maxPlayer", maxPlayer);
        switch (roomGamemodes)
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
        roomOptionsTemp = GenerateRoomOptionsFromData(GetRoomInputFieldText(), PhotonNetwork.NickName, roomMapSelectionIndex, selectedGamemodes, maxPlayerCount, MapListItemHolder.Instance.selectedMapIndex, roomVisibility);
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
        if (temp + 1 > 12) return;
        maxPlayerCount = temp + 1;
        maxPlayers.text = (temp + 1).ToString();
        OnChangedMaxPlayers(temp + 1);
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

}
