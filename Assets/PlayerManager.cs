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
    bool hasRespawned = false;
    bool respawning = false;
    float temp;
    float secondFill = 0f;
    float secondCount = 0f;
    float returnTemp = 2f;
    int respawnCountdown = 4;
    public GameObject grenadeExplosionEffect, flashbangExplosionEffect, smokeScreenEffect;

    [Space]
    [Header("UI")]
    public GameObject loadout;
    public LoadoutMenu loadoutMenu;
    [SerializeField] CanvasGroup deathGUICanvas;
    [SerializeField] CanvasGroup deathInfoCanvas;
    [SerializeField] Text killStatus, killerUsername;

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
    public GameObject deadBodyPrefab;

    int selectedSPIndex = 0;
    Vector3 tempVelocity;
    //float fov = 60f;
    private Color randomPlayerColor;
    public RoomManager roomManager;
    public CurrentMatchManager cmm;
    PlayerControllerManager tr;

    public WeaponData FindWeaponDataFromIndex(int index)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            if (index == i) return GlobalDatabase.singleton.allWeaponDatas[i];
        }
        return null;
    }
    public EquipmentData FindEquipmentDataFromIndex(int index)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allEquipmentDatas.Count; i++)
        {
            if (index == i) return GlobalDatabase.singleton.allEquipmentDatas[i];
        }
        return null;
    }
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        /*
        Hashtable temp = new();
        temp.Add("team", false);
        pv.Owner.CustomProperties.TryAdd("team", false);
        pv.Owner.SetCustomProperties(temp);*/
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
            slotHolderScript.slotWeaponData[0] = FindWeaponDataFromIndex((int)pv.Owner.CustomProperties["selectedMainWeaponIndex"]);
            slotHolderScript.slotWeaponData[1] = FindWeaponDataFromIndex((int)pv.Owner.CustomProperties["selectedSecondWeaponIndex"]);
            slotHolderScript.slotEquipmentData[0] = FindEquipmentDataFromIndex((int)pv.Owner.CustomProperties["selectedEquipmentIndex1"]);
            slotHolderScript.slotEquipmentData[1] = FindEquipmentDataFromIndex((int)pv.Owner.CustomProperties["selectedEquipmentIndex2"]);
            deathUI.SetActive(false);
            hasRespawned = true;
            CloseMenu();
            CloseLoadoutMenu();
            randomPlayerColor = Color.red;
            deathInfoCanvas.alpha = 0f;
            deathInfoCanvas.gameObject.SetActive(false);
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties["roomMode"].ToString() == "Team Deathmatch" && pv.Owner.IsMasterClient)
            {
                StartCoroutine(DelayedInit(0.2f));
            }
            StartCoroutine(DelayedSyncIsTeam(0.25f));
            //Debug.Log("Field of View in Player Preferences: " + PlayerPrefs.GetFloat("Field Of View"));
            settingsMenu.SettingsMenuAwakeFunction();
            //PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("selectedMainWeaponIndex", out object selectedMainWeaponIndex);
            //Debug.Log(selectedMainWeaponIndex);
            //Debug.Log((int)PhotonNetwork.LocalPlayer.CustomProperties["selectedSecondWeaponIndex"]);
            slotHolderScript.slotWeaponData[0] = FindWeaponDataFromIndex((int)PhotonNetwork.LocalPlayer.CustomProperties["selectedMainWeaponIndex"]);
            slotHolderScript.slotWeaponData[1] = FindWeaponDataFromIndex((int)PhotonNetwork.LocalPlayer.CustomProperties["selectedSecondWeaponIndex"]);
            slotHolderScript.slotEquipmentData[0] = FindEquipmentDataFromIndex((int)pv.Owner.CustomProperties["selectedEquipmentIndex1"]);
            slotHolderScript.slotEquipmentData[1] = FindEquipmentDataFromIndex((int)pv.Owner.CustomProperties["selectedEquipmentIndex2"]);
            //CreateController();
            OnJoiningOngoingRoom();
            //randomPlayerColor = Random.ColorHSV();
            deathInfoCanvas.alpha = 0f;
        }
        //if (!pv.IsMine) return;
        openedInventory = false;
        CloseMenu();
        CloseLoadoutMenu();

        //respawnCountdown = 8;
    }
    IEnumerator DelayedInit(float amount)
    {
        yield return new WaitForSeconds(amount);
        cmm.RefreshPlayerList();
        cmm.DistributeTeams();
        cmm.TeamDeathmatchKillLogic(0, IsTeam);
    }
    IEnumerator DelayedSyncIsTeam(float amount)
    {
        yield return new WaitForSeconds(amount);
        //RetreiveIsTeamValue();
    }
    public void SetPlayerIsTeamState(bool IsTeam, bool synchronize)
    {
        if (synchronize)
        {
            pv.RPC(nameof(RPC_SetTeamState), RpcTarget.All, IsTeam);
        }
        else
        {
            this.IsTeam = IsTeam;
            Hashtable tp = new();
            tp.Add("team", IsTeam);
            pv.Owner.SetCustomProperties(tp);
        }
    }
    [PunRPC]
    void RPC_SetTeamState(bool state)
    {
        IsTeam = state;
        Hashtable tp = new();
        tp.Add("team", state);
        pv.Owner.SetCustomProperties(tp);
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
    public void InstantiateExplosionEffect(Vector3 _pos, Quaternion _rot)
    {
        pv.RPC(nameof(RPC_InstantiateExplosionEffect), RpcTarget.All, _pos, _rot);
    }
    [PunRPC]
    void RPC_InstantiateExplosionEffect(Vector3 _pos, Quaternion _rot)
    {
        GameObject tmp = Instantiate(grenadeExplosionEffect, _pos, _rot);
        Destroy(tmp, 5f);
        Debug.Log("Calling Explosion Effect instantiation");
    }
    public void InstantiateSmokeScreenEffect(Vector3 _pos, Quaternion _rot)
    {
        pv.RPC(nameof(RPC_InstantiateSmokeScreenEffect), RpcTarget.All, _pos, _rot);
    }
    [PunRPC]
    void RPC_InstantiateSmokeScreenEffect(Vector3 _pos, Quaternion _rot)
    {
        GameObject tmp = Instantiate(smokeScreenEffect, _pos, _rot);
        Destroy(tmp, 15f);
        Debug.Log("Calling Smoke Screen Effect instantiation");
    }
    public void InstantiateFlashEffect(Vector3 _pos, Quaternion _rot)
    {
        pv.RPC(nameof(RPC_InstantiateFlashEffect), RpcTarget.All, _pos, _rot);
    }
    [PunRPC]
    void RPC_InstantiateFlashEffect(Vector3 _pos, Quaternion _rot)
    {
        GameObject tmp = Instantiate(flashbangExplosionEffect, _pos, _rot);
        Destroy(tmp, 3f);
        Debug.Log("Calling Flash Effect instantiation");
    }
    void CreateController()
    {
        //if(PhotonNetwork.CurrentRoom.CustomProperties["roomMode"].ToString() == "Team Deathmatch") RetreiveIsTeamValue();
        audioListener.enabled = false;
        respawning = true;
        respawnButton.interactable = false;
        respawnUI.redeployButton.interactable = true;
        deathUI.SetActive(false);

        Debug.Log("Instantiating Player Controller");

        if (spawnpoint == null) spawnpointUI.ChooseSpawnpoint(spawnpointUI.RandomSelectSpawnpoint());

        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { pv.ViewID });
        controller.GetComponent<PlayerControllerManager>().playerManager = this;
        playerUI = controller.GetComponent<UIManager>();
        player = controller.GetComponent<PlayerControllerManager>();
        loadoutMenu.ui = controller.GetComponent<UIManager>();
        returnTemp = 2;
        if (!openedOptions && player.pv.IsMine) Cursor.lockState = CursorLockMode.Locked;
        deathGUICanvas.alpha = 0f;
        deathGUICanvas.gameObject.SetActive(false);
        deathInfoCanvas.alpha = 0f;
        deathInfoCanvas.gameObject.SetActive(false);

        settingsMenu.SettingsMenuAwakeFunction();
        //Player Related Settings
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "SettingsOptions.json"));
        SettingsOptionsJSON jsonData = JsonUtility.FromJson<SettingsOptionsJSON>(json);

        controller.GetComponent<PlayerStats>().SetPlayerSensitivity(jsonData.MouseSensitivity);
        controller.GetComponent<PlayerStats>().SetPlayerFOV(jsonData.FieldOfView);
        controller.GetComponent<PlayerControllerManager>().SetBodyMaterialColor(randomPlayerColor);
        controller.GetComponent<PlayerControllerManager>().IsTeam = IsTeam;
        cmm.RefreshAllHostileIndicators();
    }
    private int trackingViewID = -1;
    [PunRPC]
    void RPC_InstantiateDeadBody(Vector3 position, Quaternion rotation)
    {
        Destroy(Instantiate(deadBodyPrefab, position, rotation), 3f);
    }
    public void Die(bool isSuicide, int ViewID, string killer = null)
    {
        if (killer != null)
        {
            killStatus.text = "Killed By";
            killerUsername.text = killer;
        }
        else
        {
            killStatus.text = "You have";
            killerUsername.text = "Suicided";
        }
        if (isSuicide)
        {
            killStatus.text = "You have";
            killerUsername.text = "Suicided";
            if (PhotonNetwork.CurrentRoom.CustomProperties["roomMode"].ToString() != "Team Deathmatch") InstantiateKillMSG(pv.Owner.NickName, pv.Owner.NickName, (int)pv.Owner.CustomProperties["weaponIndex"], ((int)pv.Owner.CustomProperties["weaponIndex"] < 2 ? true : false));
            else TDM_InstantiateKillMSG(pv.Owner.NickName, pv.Owner.NickName, (int)pv.Owner.CustomProperties["weaponIndex"], (bool)pv.Owner.CustomProperties["team"], ((int)pv.Owner.CustomProperties["weaponIndex"] < 2 ? true : false));
        }
        if (trackingViewID != -1 && !isSuicide)
        {
            PlayerControllerManager[] tmp = FindObjectsOfType<PlayerControllerManager>();
            for (int i = 0; i < tmp.Length; i++)
            {
                if (tmp[i].pv.ViewID == trackingViewID)
                {
                    tr = tmp[i];
                }
            }
        }
        if (pv.IsMine) SynchronizeValues(kills, deaths);
        pv.RPC(nameof(RPC_InstantiateDeadBody), RpcTarget.All, controller.transform.position, controller.transform.rotation);
        audioListener.enabled = true;
        streakKills = 0;
        cameraObject.fieldOfView = PlayerPrefs.GetFloat("Field Of View");
        respawning = true;
        secondCount = 0;
        deathGUICanvas.alpha = 0f;
        deathInfoCanvas.alpha = 1f;
        deathGUICanvas.gameObject.SetActive(true);
        deathInfoCanvas.gameObject.SetActive(true);
        trackingViewID = ViewID;

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
        cmm.RefreshAllSupplyIndicators();
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
        Die(true, -1);
    }
    public void RespawnPlayer()
    {
        if (!pv.IsMine) return;
        respawnButton.interactable = false;
        hasRespawned = false;
        //CompleteCountdown();
        StartCoroutine(DelayedCompleteCountdown(1.5f));
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
                GetKill("TestName", 0, true);
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
            }
            else if (loadoutMenu.openedSelectionMenu)
            {
                loadoutMenu.CloseSelectionMenu();
            }
            else
            {
                OpenLoadoutMenu();
            }
        }
        if (returnTemp <= 0f && hasRespawned && !respawning)
        {
            //CreateController();
        }
        if (respawning)
        {
            cameraObject.fieldOfView = Mathf.Lerp(cameraObject.fieldOfView, 60f, Time.deltaTime * 2);
            if (returnTemp >= 1.5f)
            {
                if (!hasRespawned)
                {
                    deathInfoCanvas.alpha = Mathf.Lerp(deathInfoCanvas.alpha, 0f, Time.deltaTime);
                    deathGUICanvas.alpha = Mathf.Lerp(deathGUICanvas.alpha, 1f, Time.deltaTime * 2);
                    transform.position = Vector3.Slerp(transform.position, spawnpointCamera.transform.position, Time.deltaTime * 3);
                    transform.rotation = Quaternion.Slerp(transform.rotation, spawnpointCamera.transform.rotation, Time.deltaTime * 3);
                }
            }
            else
            {
                //if (trackingViewID != -1) transform.LookAt(tr.transform, Vector3.up);
                returnTemp += secondCount;
                secondCount = 0;
            }
        }
        else
        {
            if (hasRespawned)
            {
                deathGUICanvas.alpha = Mathf.Lerp(deathGUICanvas.alpha, 0f, Time.deltaTime * 5);
                deathInfoCanvas.alpha = Mathf.Lerp(deathInfoCanvas.alpha, 0f, Time.deltaTime);
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
        if (temp >= 1f)
        {
            if (respawnCountdown <= 0)
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
        if (controller != null) controller.GetComponent<PlayerStats>().SetPlayerSensitivity(value);
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
            data.QualityIndex = settingsMenu.qualityDropdown.selectedItemIndex;
            data.ResolutionIndex = settingsMenu.resolutionDropdown.selectedItemIndex;

            Debug.Log("Persistent Data Path: " + Path.Combine(Application.persistentDataPath, "SettingsOptions.json"));
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "SettingsOptions.json"), json);
            if (controller != null) controller.GetComponent<PlayerStats>().SetPlayerSensitivity(data.MouseSensitivity);
            if (controller != null) controller.GetComponent<PlayerStats>().SetPlayerFOV(data.FieldOfView);
            Debug.LogWarning("Writing Settings Options To Files...");
        }
        else
        {
            if (!File.Exists(Path.Combine(Application.persistentDataPath, "SettingsOptions.json"))) settingsMenu.WriteSettingsOptionsToJSON();
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "SettingsOptions.json"));
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
        Die(true, -1);
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

    public void GetKill(string killedPlayerName, int withWeaponIndex, bool isWeapon)
    {
        //if (!pv.IsMine) return;
        pv.RPC(nameof(RPC_GetKill), pv.Owner, killedPlayerName, withWeaponIndex, isWeapon);
    }

    [PunRPC]
    void RPC_GetKill(string killedPlayerName, int withWeaponIndex, bool isWeapon)
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
        if (PhotonNetwork.CurrentRoom.CustomProperties["roomMode"].ToString() != "Team Deathmatch") InstantiateKillMSG(killedPlayerName, pv.Owner.NickName, withWeaponIndex, isWeapon);
        else TDM_InstantiateKillMSG(killedPlayerName, pv.Owner.NickName, withWeaponIndex, (bool)pv.Owner.CustomProperties["team"], isWeapon);
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
    public void InstantiateKillMSG(string killedName, string killerName, int weaponIndex, bool isWeapon)
    {
        pv.RPC(nameof(RPC_InstantiateMessageOnKill), RpcTarget.All, killedName, killerName, weaponIndex, isWeapon);
    }
    public void TDM_InstantiateKillMSG(string killedName, string killerName, int weaponIndex, bool killedIsTeam, bool isWeapon)
    {
        pv.RPC(nameof(RPC_TDM_InstantiateMessageOnKill), RpcTarget.All, killedName, killerName, weaponIndex, killedIsTeam, isWeapon);
    }
    [PunRPC]
    public void RPC_InstantiateMessageOnKill(string killedName, string killerName, int weaponIndex, bool isWeapon)
    {
        Debug.LogWarning("Instantiating Message: " + killedName + " " + killerName + " " + weaponIndex);
        GameObject temp = Instantiate(InGameUI.instance.killMSGPrefab, InGameUI.instance.killMSGHolder);
        temp.GetComponent<KillMessageItem>().SetInfo(killedName, killerName, (isWeapon ? InGameUI.instance.FindWeaponIcon(weaponIndex) : InGameUI.instance.FindEquipmentIcon(weaponIndex)));
        if (PhotonNetwork.CurrentRoom.CustomProperties["roomMode"].ToString() == "Free For All")
        {
            if (pv.Owner.NickName == killerName)
            {
                temp.GetComponent<KillMessageItem>().SetKilledColor(Color.red);
            }
            else
            {
                temp.GetComponent<KillMessageItem>().SetKilledColor(Color.red);
                temp.GetComponent<KillMessageItem>().SetKillerColor(Color.red);
            }
        }
        Debug.Log(killedName + " was killed by " + killerName + " using weapon with an index of " + weaponIndex);
        Destroy(temp, 15f);
    }
    [PunRPC]
    public void RPC_TDM_InstantiateMessageOnKill(string killedName, string killerName, int weaponIndex, bool killerIsTeam, bool isWeapon)
    {
        GameObject temp = Instantiate(InGameUI.instance.killMSGPrefab, InGameUI.instance.killMSGHolder);
        temp.GetComponent<KillMessageItem>().SetInfo(killedName, killerName, (isWeapon ? InGameUI.instance.FindWeaponIcon(weaponIndex) : InGameUI.instance.FindEquipmentIcon(weaponIndex)));
        if (cmm.localClientPlayer.IsTeam == killerIsTeam)
        {
            temp.GetComponent<KillMessageItem>().SetKilledColor(Color.red);
        }
        else
        {
            temp.GetComponent<KillMessageItem>().SetKillerColor(Color.red);
        }
        //Debug.LogWarning("Instantiating Message: " + killedName + " " + killerName + " " + weaponIndex);
        Debug.Log(killedName + " was killed by " + killerName + " using weapon with an index of " + weaponIndex + " on team " + (killerIsTeam ? "Red" : "Blue"));
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
