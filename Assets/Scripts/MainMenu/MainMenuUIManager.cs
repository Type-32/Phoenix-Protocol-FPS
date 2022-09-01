using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager instance;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button createRoomButton;

    [Space]
    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject multiplayerMenu;
    public GameObject roomMenu;
    public GameObject findRoomMenu;
    public GameObject loadingMenu;

    [Space]
    [Header("Misc Components")]
    [SerializeField] private Text connectionIndicator;
    [SerializeField] private Text invalidInputField;
    [SerializeField] private Text roomTitle;
    [SerializeField] private InputField roomInputField;
    [SerializeField] private InputField playerNameInputField;

    [Space]
    [Header("Menu States")]
    public bool openedMainMenu = false;
    public bool openedMultiplayerMenu = false;
    public bool openedRoomMenu = false;
    public bool openedFindRoomMenu = false;
    public bool openedLoadingMenu = false;
    public bool usingCreateRooomInputField = false;

    [Space]
    [Header("Menu Online States")]
    [SerializeField] private bool joinedMasterLobby = false;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        JoiningMasterLobby(false);
        SetCreateRoomInputField(false);
        SetConnectionIndicatorText("Attempting to connect to Multiplayer Services...");
        MainMenuUIManager.instance.SetInvalidInputFieldText(" ", Color.red);
        CloseRoomMenu();
        CloseMultiplayerMenu();
        CloseLoadingMenu();
        CloseFindRoomMenu();
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
        if (value) createRoomButton.interactable = false;
        else createRoomButton.interactable = true;
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
    public string GetPlayerFieldName()
    {
        return playerNameInputField.text;
    }
}