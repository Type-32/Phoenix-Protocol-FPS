using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    public PhotonView pv;
    public GameObject controller;
    [SerializeField] GameObject deathUI;
    [SerializeField] Text deathCountdown;
    public Camera cameraObject;
    [SerializeField] UIManager playerUI;
    [SerializeField] PlayerControllerManager player;
    [SerializeField] GameObject skipCountdownIndicator;
    [SerializeField] GameObject spawnpointUISelection;
    [SerializeField] Button respawnButton;
    public LoadoutSlotHolder slotHolderScript;
    public ChoiceHolderScript choiceHolderScript;
    public AudioListener audioListener;
    [SerializeField] bool hasRespawned = false;
    [SerializeField] bool respawning = false;
    [SerializeField] float temp;
    [SerializeField] float secondFill = 0f;
    [SerializeField] float secondCount = 0f;
    [SerializeField] float returnTemp = 2f;
    [SerializeField] int respawnCountdown = 4;

    [Space]
    [Header("UI")]
    public GameObject loadout;
    public LoadoutMenu loadoutMenu;
    [SerializeField] CanvasGroup deathGUICanvas;

    [Space]
    public bool openedLoadoutMenu = false;
    public bool openedOptions = false;
    public bool openedSettingsSection = false;
    public bool enabledButtonHolder = false;
    public bool openedInventory = false;

    [Space]
    [Header("RespawnUI")]
    public RespawningUI respawnUI;
    public Transform spawnpoint;
    public GameObject spawnpointSelectionUI;
    public SpawnpointUI spawnpointUI;
    public KillStatsHUD killStatsHUD;
    public KillMessagesHUD killMessagesHUD;

    [Space]
    [Header("Settings Related")]
    public SettingsMenu settingsMenu;
    public GameObject buttonHolder;

    [Space]
    [Header("Options Elements")]
    public GameObject optionsUI;
    public SpawnpointCamera spawnpointCamera;

    [Space]
    [Header("Player Infos")]
    public int kills = 0;
    public int deaths = 0;
    public bool randomSpawnpoint = false;
    public bool nightVisionState = false;
    public bool IsTeam = false;
    [HideInInspector] public int streakKills = 0;
    [HideInInspector] public int recordKills = 0;
    [HideInInspector] public int totalGainedXP = 0;

    [Space]
    [Header("Kill Messages")]
    public Transform killMSGHolder;
    public GameObject killMSGPrefab;

    int selectedSPIndex = 0;
    Vector3 tempVelocity;
    //float fov = 60f;
    private Color randomPlayerColor;
    public RoomManager roomManager;
    public CurrentMatchManager cmm;

    public WeaponData FindWeaponDataFromIndex(int index)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            if (index == i) return GlobalDatabase.singleton.allWeaponDatas[i];
        }
        return null;
    }
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        Hashtable temp = new();
        temp.Add("team", false);
        pv.Owner.CustomProperties.TryAdd("team", false);
        pv.Owner.SetCustomProperties(temp);
        if (pv.IsMine)
        {
            Hashtable th = new();
            th.Add("kills", kills);
            th.Add("deaths", deaths);
            pv.Owner.CustomProperties.TryAdd("kills", kills);
            pv.Owner.CustomProperties.TryAdd("deaths", deaths);
            pv.Owner.SetCustomProperties(th);
            
        }
        spawnpointCamera = FindObjectOfType<SpawnpointCamera>();
        roomManager = FindObjectOfType<RoomManager>();
        cmm = FindObjectOfType<CurrentMatchManager>();
        cmm.RefreshPlayerList();
        if (pv.Owner.IsMasterClient)
        {

        }
        if (pv.IsMine)
        {
            //cmm.AddPlayer(this);
            cmm.localClientPlayer = this;
            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["gameStarted"]) cmm.UpdatePlayerKillOnClient();
        }
        else
        {
            //cmm.AddPlayer(this);
            cameraObject.gameObject.SetActive(false);
        }
        settingsMenu.SettingsMenuAwakeFunction();
        ToggleSettingsMenu(true);
        ToggleSettingsMenu(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!pv.IsMine)
        {
            //cameraObject.gameObject.SetActive(false);
            //PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("selectedMainWeaponIndex", out object selectedMainWeaponIndex);
            //Debug.Log(selectedMainWeaponIndex);
            Debug.Log((int)PhotonNetwork.LocalPlayer.CustomProperties["selectedSecondWeaponIndex"]);
            slotHolderScript.slotWeaponData[0] = FindWeaponDataFromIndex((int)pv.Owner.CustomProperties["selectedMainWeaponIndex"]);
            slotHolderScript.slotWeaponData[1] = FindWeaponDataFromIndex((int)pv.Owner.CustomProperties["selectedSecondWeaponIndex"]);
            deathUI.SetActive(false);
            hasRespawned = true;
            CloseMenu();
            CloseLoadoutMenu();
            randomPlayerColor = Color.red;
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties["roomMode"].ToString() == "Team Deathmatch" && pv.Owner.IsMasterClient)
            {
                cmm.DistributeTeams();
                cmm.TeamDeathmatchKillLogic(IsTeam);
            }
            //Debug.Log("Field of View in Player Preferences: " + PlayerPrefs.GetFloat("Field Of View"));
            settingsMenu.SettingsMenuAwakeFunction();
            //PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("selectedMainWeaponIndex", out object selectedMainWeaponIndex);
            //Debug.Log(selectedMainWeaponIndex);
            //Debug.Log((int)PhotonNetwork.LocalPlayer.CustomProperties["selectedSecondWeaponIndex"]);
            slotHolderScript.slotWeaponData[0] = FindWeaponDataFromIndex((int)PhotonNetwork.LocalPlayer.CustomProperties["selectedMainWeaponIndex"]);
            slotHolderScript.slotWeaponData[1] = FindWeaponDataFromIndex((int)PhotonNetwork.LocalPlayer.CustomProperties["selectedSecondWeaponIndex"]);
            //CreateController();
            OnJoiningOngoingRoom();
            //randomPlayerColor = Random.ColorHSV();
        }
        //if (!pv.IsMine) return;
        openedInventory = false;
        CloseMenu();
        CloseLoadoutMenu();

        //respawnCountdown = 8;
    }
    public void RetreiveIsTeamValue()
    {
        pv.RPC(nameof(RPC_GetIsTeamValue), RpcTarget.All);
    }
    [PunRPC]
    void RPC_GetIsTeamValue()
    {
        IsTeam = (bool)pv.Owner.CustomProperties["team"];
    }
    void OnJoiningOngoingRoom()
    {
        cameraObject.fieldOfView = PlayerPrefs.GetFloat("Field Of View");
        respawning = true;
        secondCount = 0;
        deathGUICanvas.alpha = 0f;
        deathGUICanvas.gameObject.SetActive(true);

        respawnButton.interactable = true;
        respawnUI.redeployButton.interactable = false;
        respawnCountdown = 4;
        hasRespawned = false;
        temp = 0;
        returnTemp = 0f;
        deathUI.SetActive(true);
        deathCountdown.text = "Waiting for Respawn";
        Cursor.lockState = CursorLockMode.None;
        if (randomSpawnpoint || spawnpoint == null) spawnpointUI.ChooseSpawnpoint(spawnpointUI.RandomSelectSpawnpoint());
        spawnpointUI.ChooseSpawnpoint(selectedSPIndex);
        
        transform.position = spawnpoint.position;
        transform.rotation = spawnpoint.rotation;
    }
    void CreateController()
    {
        RetreiveIsTeamValue();
        audioListener.enabled = false;
        respawning = true;
        respawnButton.interactable = false;
        respawnUI.redeployButton.interactable = true;
        deathUI.SetActive(false);
        
        Debug.Log("Instantiating Player Controller");

        if(spawnpoint == null) spawnpointUI.ChooseSpawnpoint(spawnpointUI.RandomSelectSpawnpoint());

        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnpoint.position, spawnpoint.rotation, 0, new object[] {pv.ViewID});
        controller.GetComponent<PlayerControllerManager>().playerManager = this;
        playerUI = controller.GetComponent<UIManager>();
        player = controller.GetComponent<PlayerControllerManager>();
        loadoutMenu.ui = controller.GetComponent<UIManager>();
        returnTemp = 2;
        if(!openedOptions && player.pv.IsMine) Cursor.lockState = CursorLockMode.Locked;
        deathGUICanvas.alpha = 0f;
        deathGUICanvas.gameObject.SetActive(false);

        settingsMenu.SettingsMenuAwakeFunction();
        //Player Related Settings
        string json = File.ReadAllText(Application.persistentDataPath + "/SettingsOptions.json");
        SettingsOptionsJSON jsonData = JsonUtility.FromJson<SettingsOptionsJSON>(json);

        controller.GetComponent<PlayerStats>().SetPlayerSensitivity(jsonData.MouseSensitivity);
        controller.GetComponent<PlayerStats>().SetPlayerFOV(jsonData.FieldOfView);
        controller.GetComponent<PlayerControllerManager>().SetBodyMaterialColor(randomPlayerColor);
        controller.GetComponent<PlayerControllerManager>().IsTeam = IsTeam;
        cmm.RefreshAllHostileIndicators();
    }
    public void Die()
    {
        if (pv.IsMine) SynchronizeValues(kills, deaths);
        audioListener.enabled = true;
        streakKills = 0;
        cameraObject.fieldOfView = PlayerPrefs.GetFloat("Field Of View");
        respawning = true;
        secondCount = 0;
        deathGUICanvas.alpha = 0f;
        deathGUICanvas.gameObject.SetActive(true);

        transform.position = controller.transform.position;
        transform.rotation = controller.transform.rotation;

        deaths++;
        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        PhotonNetwork.Destroy(controller);
        respawnButton.interactable = true;
        respawnUI.redeployButton.interactable = false;
        Debug.Log("Player " + player.pv.Owner.NickName + " was Killed");
        respawnCountdown = 4;
        hasRespawned = false;
        temp = 0;
        returnTemp = 0f;
        deathUI.SetActive(true);
        deathCountdown.text = "Waiting for Respawn";
        Cursor.lockState = CursorLockMode.None;
        if (randomSpawnpoint || spawnpoint == null) spawnpointUI.ChooseSpawnpoint(spawnpointUI.RandomSelectSpawnpoint());
        spawnpointUI.ChooseSpawnpoint(selectedSPIndex);
        cmm.RefreshAllHostileIndicators();
    }
    public void SynchronizeValues(int kills, int deaths)
    {
        pv.RPC(nameof(RPC_SynchronizeValues), RpcTarget.All, kills, deaths);
    }
    [PunRPC]
    void RPC_SynchronizeValues(int kills, int deaths)
    {
        this.kills = kills;
        this.deaths = deaths;
    }
    IEnumerator DelayedCompleteCountdown(float duration)
    {
        respawnCountdown = 0;
        temp = 0;
        secondCount = 0;
        hasRespawned = true;
        respawning = false;
        deathCountdown.text = "Waiting for Respawn";
        skipCountdownIndicator.SetActive(false);
        CloseLoadoutMenu();
        yield return new WaitForSeconds(duration);
        CompleteCountdown();
    }
    public void RedeployPlayer()
    {
        if (!pv.IsMine) return;
        respawnUI.redeployButton.interactable = false;
        //RespawnPlayer();
        Die();
    }
    public void RespawnPlayer()
    {
        if (!pv.IsMine) return;
        respawnButton.interactable = false;
        hasRespawned = false;
        //CompleteCountdown();
        StartCoroutine(DelayedCompleteCountdown(2f));
    }

    public void SetSpawnPositionReference(Transform obj, int index)
    {
        spawnpoint = obj;
        Debug.Log("Spawnpoint Setted to " + obj.name);
        selectedSPIndex = index;
    }
    public void DisconnectPlayer()
    {
        if (!pv.IsMine) return;
        StartCoroutine(DisconnectAndLoad());
    }
    IEnumerator DisconnectAndLoad()
    {
        if (!pv.IsMine) yield return null;
        //PhotonNetwork.Disconnect();
        PhotonNetwork.LeaveRoom();
        //while (PhotonNetwork.IsConnected)
        while (PhotonNetwork.InRoom)
            yield return null;
        cmm.RemovePlayer(this);
        SceneManager.LoadScene(0);
        roomManager.SelfDestruction();
        Debug.Log("Self Destruction Occured");
        MainMenuUIManager.instance.CloseMainMenu();
        MainMenuUIManager.instance.CloseLoadingMenu();
        MainMenuUIManager.instance.CloseFindRoomMenu();
        MainMenuUIManager.instance.CloseLoadingMenu();
        //MainMenuUIManager.instance.CloseMultiplayerMenu();
        MainMenuUIManager.instance.CloseRoomMenu();
        MainMenuUIManager.instance.CloseSettingsMenu();
        MainMenuUIManager.instance.CloseUpdateLogsMenu();
        MainMenuUIManager.instance.CloseCreateRoomMenu();
        MainMenuUIManager.instance.CloseLoadoutSelectionMenu();
        MainMenuUIManager.instance.CloseCosmeticsMenu();
        Debug.Log("Loaded Scene from Player Manager");
    }
    private void Update()
    {
        if (!pv.IsMine) return;
        //Developer Console
        if (Launcher.Instance.startKey == PhotonNetwork.LocalPlayer.NickName)
        {
            if (Input.GetKeyDown("o"))
            {
                GetKill("TestName", 0);
            }
        }
        if (secondFill < 1f)
        {
            secondFill += Time.deltaTime;
        }
        else
        {
            secondFill = 0f;
            secondCount++;
        }
        if ((Input.GetKeyDown(KeyCode.Escape) && !cmm.gameEnded))
        {
            if (loadoutMenu.openedSelectionMenu)
            {
                loadoutMenu.CloseSelectionMenu();
            }
            else if (openedLoadoutMenu)
            {
                CloseLoadoutMenu();
            }
            else if (openedSettingsSection)
            {
                ToggleSettingsMenu(false);
                ToggleButtonHolder(true);
            }
            else if (openedOptions)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
        if (Input.GetKeyDown(KeyCode.Return) && !openedOptions)
        {
            if (openedLoadoutMenu)
            {
                CloseLoadoutMenu();
            }else if (loadoutMenu.openedSelectionMenu)
            {
                loadoutMenu.CloseSelectionMenu();
            }
            else
            {
                OpenLoadoutMenu();
            }
        }
        if(returnTemp <= 0f && hasRespawned && !respawning)
        {
            //CreateController();
        }
        if (respawning)
        {
            cameraObject.fieldOfView = Mathf.Lerp(cameraObject.fieldOfView, 60f, Time.deltaTime * 2);
            if (returnTemp >= 2f)
            {
                if (!hasRespawned)
                {
                    deathGUICanvas.alpha = Mathf.Lerp(deathGUICanvas.alpha, 1f, Time.deltaTime * 2);
                    transform.position = Vector3.Slerp(transform.position, spawnpointCamera.transform.position, Time.deltaTime * 3);
                    transform.rotation = Quaternion.Slerp(transform.rotation, spawnpointCamera.transform.rotation, Time.deltaTime * 3);
                }
            }
            else
            {
                returnTemp += secondCount;
                secondCount = 0;
            }
        }
        else
        {
            if (hasRespawned)
            {
                deathGUICanvas.alpha = Mathf.Lerp(deathGUICanvas.alpha, 0f, Time.deltaTime * 5);
                transform.position = Vector3.Slerp(transform.position, spawnpoint.position, Time.deltaTime * 3);
                transform.rotation = Quaternion.Slerp(transform.rotation, spawnpoint.rotation, Time.deltaTime * 3);
            }
            if (returnTemp > 0f)
            {
                returnTemp -= secondCount;
                secondCount = 0;
            }
        }
        if (hasRespawned) return;
        if (openedLoadoutMenu) return;
        temp += Time.deltaTime;
        /*
        if (Input.GetKeyDown("f"))
        {
            CompleteCountdown();
            return;
        }*/
        //if(respawnCountdown >=)
        if(temp >= 1f)
        {
            if(respawnCountdown <= 0)
            {
                if (!randomSpawnpoint)
                {
                    respawnButton.interactable = spawnpoint == null ? false : true;
                }

                //CompleteCountdown();
                deathCountdown.text = "Respawn";
                //respawnButton.interactable = true;
                return;
            }
            else
            {
                respawnButton.interactable = false;
            }
            deathCountdown.text = "Respawn Available in " + respawnCountdown.ToString();
            respawnCountdown--;
            temp = 0;
        }
    }
    public void CompleteCountdown()
    {
        respawnCountdown = 0;
        temp = 0;
        secondCount = 0;
        hasRespawned = true;
        respawning = false;
        deathCountdown.text = "Waiting for Respawn";
        skipCountdownIndicator.SetActive(false);
        CloseLoadoutMenu();
        CreateController();
    }
    #region Menus
    public void OpenLoadoutMenu()
    {
        //if (!pv.IsMine) return;
        //Debug.Log("Opened Loadout UI ");
        openedLoadoutMenu = true;
        loadoutMenu.gameObject.SetActive(true);
        slotHolderScript.RefreshLoadoutSlotInfo();
        Cursor.lockState = CursorLockMode.None;
    }
    public void CloseLoadoutMenu()
    {
        //if (!pv.IsMine) return;
        //Debug.Log("Closed Loadout UI ");
        openedLoadoutMenu = false;
        loadoutMenu.gameObject.SetActive(false);
        if (controller != null) Cursor.lockState = CursorLockMode.Locked;
    }
    public void ToggleButtonHolder(bool value)
    {
        // (!pv.IsMine) return;
        //Debug.Log("Toggled Button Holder ");
        //Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        enabledButtonHolder = value;
        buttonHolder.SetActive(value);
    }
    public void OnChangedSensitivityValue(float value)
    {
        if(controller != null) controller.GetComponent<PlayerStats>().SetPlayerSensitivity(value);
    }
    public void OnChangedFOVValue(float value)
    {
        if (controller != null) controller.GetComponent<PlayerStats>().SetPlayerFOV(value);
    }
    public void ToggleSettingsMenu(bool value)
    {
        if (!value)
        {
            SettingsOptionsJSON data = new SettingsOptionsJSON();
            data.Volume = settingsMenu.volumeSlider.value;
            data.FieldOfView = settingsMenu.fieldOfViewSlider.value;
            data.Fullscreen = settingsMenu.fullscreenToggle.isOn;
            data.MouseSensitivity = settingsMenu.sensitivitySlider.value;
            data.QualityIndex = settingsMenu.qualityDropdown.value;
            data.ResolutionIndex = settingsMenu.resolutionDropdown.value;

            Debug.Log("Persistent Data Path: " + Application.persistentDataPath + "/SettingsOptions.json");
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(Application.persistentDataPath + "/SettingsOptions.json", json);
            if (controller != null) controller.GetComponent<PlayerStats>().SetPlayerSensitivity(data.MouseSensitivity);
            if (controller != null) controller.GetComponent<PlayerStats>().SetPlayerFOV(data.FieldOfView);
            Debug.LogWarning("Writing Settings Options To Files...");
        }
        else
        {
            if (!File.Exists(Application.persistentDataPath + "/SettingsOptions.json")) settingsMenu.WriteSettingsOptionsToJSON();
            string json = File.ReadAllText(Application.persistentDataPath + "/SettingsOptions.json");
            Debug.LogWarning("Reading Settings Options To Files...");
            SettingsOptionsJSON jsonData = JsonUtility.FromJson<SettingsOptionsJSON>(json);
            settingsMenu.SetFieldOfView(jsonData.FieldOfView);
            settingsMenu.SetVolume(jsonData.Volume);
            settingsMenu.SetFullscreen(jsonData.Fullscreen);
            settingsMenu.SetSensitivity(jsonData.MouseSensitivity);
            settingsMenu.SetQuality(jsonData.QualityIndex);
            settingsMenu.SetResolution(jsonData.ResolutionIndex);
            if (controller != null)
            {
                controller.GetComponent<PlayerStats>().SetPlayerSensitivity(PlayerPrefs.GetFloat("Mouse Sensitivity"));
                controller.GetComponent<PlayerStats>().SetPlayerFOV(jsonData.FieldOfView);
            }
        }
        openedSettingsSection = value;
        settingsMenu.gameObject.SetActive(value);

        //Set Local Player Properties According to Settings Menu
    }
    public void OpenMenu()
    {
        //if (!pv.IsMine) return;
        openedOptions = true;
        //Debug.Log("Opened Options UI ");
        optionsUI.SetActive(openedOptions);
        ToggleButtonHolder(openedOptions);
        ToggleSettingsMenu(!openedOptions);
        Cursor.lockState = CursorLockMode.None;
    }
    public void CloseMenu()
    {
        //if (!pv.IsMine) return;
        openedOptions = false;
        //Debug.Log("Closed Options UI ");
        optionsUI.SetActive(openedOptions);
        ToggleButtonHolder(openedOptions);
        ToggleSettingsMenu(openedOptions);
        if (controller != null) Cursor.lockState = CursorLockMode.Locked;
    }
    #endregion
    public void LeaveGame()
    {
        Die();
        player.playerManager.CloseMenu();
        player.playerManager.CloseLoadoutMenu();
        player.playerManager.ToggleButtonHolder(true);
        player.playerManager.ToggleSettingsMenu(false);
        //SceneManager.LoadScene(0);
        Launcher.Instance.LeaveRoom();
        //PhotonNetwork.LoadLevel(0);
        PhotonNetwork.Destroy(gameObject);
    }
    public void QuitGame()
    {
        Launcher.Instance.QuitApplication();
    }

    public void GetKill(string killedPlayerName, int withWeaponIndex)
    {
        //if (!pv.IsMine) return;
        pv.RPC(nameof(RPC_GetKill), pv.Owner, killedPlayerName, withWeaponIndex);
    }

    [PunRPC]
    void RPC_GetKill(string killedPlayerName, int withWeaponIndex)
    {
        kills++;
        recordKills++;
        streakKills++;
        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        player.ui.InvokeHitmarker(UIManager.HitmarkerType.Killmarker);
        player.sfx.InvokeHitmarkerAudio(UIManager.HitmarkerType.Killmarker);
        InstantiateKillIcon(false, killedPlayerName, 150 + (streakKills > 1 ? 150 * (streakKills - 1) / 4 : 0));
        totalGainedXP += 150 + (streakKills > 1 ? 150 * (streakKills - 1) / 4 : 0);
        InstantiateKillMSG(killedPlayerName, pv.Owner.NickName, withWeaponIndex);
        MinimapDotIdentifier[] tempget;
        tempget = FindObjectsOfType<MinimapDotIdentifier>();
        controller.GetComponent<PlayerControllerManager>().allMinimapDots.Clear();
        for (int i = 0; i < tempget.Length; i++)
        {
            controller.GetComponent<PlayerControllerManager>().allMinimapDots.Add(tempget[i].gameObject);
        }
        controller.GetComponent<PlayerControllerManager>().DisableAllMinimapDots();
        controller.GetComponent<PlayerControllerManager>().playerMinimapDot.SetActive(true);

        if (pv.IsMine)
        {
            SynchronizeValues(kills, deaths);
            //UserDatabase.Instance.AddUserLevelXP(150 + (streakKills > 1 ? 150 * (streakKills - 1) / 2 : 0));
        }
        //cmm.OnPlayerKillUpdate();
        //pv.RPC(nameof(RPC_InstantiateMessageOnKill), RpcTarget.All, killedPlayerName, pv.Owner.NickName, withWeaponIndex);
        //Debug.Log("Killed " + killedPlayerName + " with " + withWeaponIndex);
    }
    public void InstantiateKillMSG(string killedName, string killerName, int weaponIndex)
    {
        pv.RPC(nameof(RPC_InstantiateMessageOnKill), RpcTarget.All, killedName, killerName, weaponIndex);
    }
    [PunRPC]
    public void RPC_InstantiateMessageOnKill(string killedName, string killerName, int weaponIndex)
    {
        Debug.LogWarning("Instantiating Message: " + killedName + " " + killerName + " " + weaponIndex);
        GameObject temp = Instantiate(InGameUI.instance.killMSGPrefab, InGameUI.instance.killMSGHolder);
        temp.GetComponent<KillMessageItem>().SetInfo(killedName, killerName, InGameUI.instance.FindWeaponIcon(weaponIndex));
        Debug.Log(killedName + " was killed by " + killerName + " using weapon with an index of " + weaponIndex);
        Destroy(temp, 15f);
    }

    public void InstantiateKillIcon(bool isSpecialKill, string killedPlayerName, int killedPoints)
    {
        Debug.Log("Invoking Kill Icon ");
        GameObject temp1 = Instantiate(killStatsHUD.killIconItemPrefab, killStatsHUD.killIconHolder);
        GameObject temp2 = Instantiate(killStatsHUD.statsCounterItemPrefab, killStatsHUD.statsCounterHolder);
        if (isSpecialKill)
        {
            temp1.GetComponent<KillIconItem>().SetInfo(killStatsHUD.killIconSkull, killStatsHUD.specialKillColor, killStatsHUD.specialKillColorCross);
            temp2.GetComponent<TextStatItem>().SetInfo("Killed " + killedPlayerName, killedPoints + 50);
        }
        else
        {
            temp1.GetComponent<KillIconItem>().SetInfo(killStatsHUD.killIconSkull, killStatsHUD.normalKillColor, killStatsHUD.normalKillColorCross);
            temp2.GetComponent<TextStatItem>().SetInfo("Killed " + killedPlayerName, killedPoints);
        }
        Destroy(temp1, 2f);
        Destroy(temp2, 3f);
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.pv.Owner == player);
    }
}
