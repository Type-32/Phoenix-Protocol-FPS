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
using UserConfiguration;
using UnityEngine.Rendering;
using PrototypeLib.Modules.FileOperations.IO;
using UnityEngine.Rendering.Universal;
using PrototypeLib.OnlineServices.PUNMultiplayer.ConfigurationKeys;
using Unity.VisualScripting;

public class PlayerManager : MonoBehaviour
{
    public PhotonView pv;
    public GameObject controller;
    [SerializeField] Collider managerCollider;
    [SerializeField] Rigidbody managerRigidbody;
    [SerializeField] Volume hurtVolume;
    [SerializeField] GameObject deathUI;
    [SerializeField] Text deathCountdown;
    public Camera cameraObject;
    [SerializeField] UIManager playerUI;
    [SerializeField] PlayerControllerManager player;
    [SerializeField] GameObject skipCountdownIndicator;
    [SerializeField] GameObject spawnpointUISelection;
    [SerializeField] Button respawnButton;
    public MatchLoadoutManager matchLoadoutManager;
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
    [SerializeField] CanvasGroup deathGUICanvas;
    [SerializeField] CanvasGroup deathInfoCanvas;
    [SerializeField] Text killStatus, killerUsername;
    public GameObject FFA_UI, TDM_UI, CTF_UI, DZ_UI;
    [SerializeField] Image prevIcon1, prevIcon2, prevIcon3, prevIcon4;

    [Space]
    public bool openedLoadoutMenu = false;
    public bool openedOptions = false;
    public bool openedSettingsSection = false;
    public bool openedLoadoutSwapMenu = false;
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
    public GameObject loadoutSwapMenu;
    public GameObject loadoutSwapPrefab;
    public Transform loadoutSwapPrefabHolder;
    
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
    public Sprite skullIcon;

    int selectedSPIndex = 0;
    Vector3 tempVelocity;
    //float fov = 60f;
    private Color randomPlayerColor;
    public RoomManager roomManager;
    public CurrentMatchManager cmm;
    PlayerControllerManager tr;

    public WeaponData FindWeaponDataFromIndex(int index)
    {
        for (int i = 0; i < GlobalDatabase.Instance.allWeaponDatas.Count; i++)
        {
            if (index == i) return GlobalDatabase.Instance.allWeaponDatas[i];
        }
        return null;
    }
    public EquipmentData FindEquipmentDataFromIndex(int index)
    {
        for (int i = 0; i < GlobalDatabase.Instance.allEquipmentDatas.Count; i++)
        {
            if (index == i) return GlobalDatabase.Instance.allEquipmentDatas[i];
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
        managerCollider.enabled = false;
        managerRigidbody.useGravity = false;
        managerRigidbody.isKinematic = false;
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
            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.GameStarted]) cmm.UpdatePlayerKillOnClient();
            matchLoadoutManager = GetComponent<MatchLoadoutManager>();
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
            deathUI.SetActive(false);
            hasRespawned = true;
            CloseMenu();
            randomPlayerColor = Color.red;
            deathInfoCanvas.alpha = 0f;
            deathInfoCanvas.gameObject.SetActive(false);
            FFA_UI.SetActive(false);
            TDM_UI.SetActive(false);
            CTF_UI.SetActive(false);
            DZ_UI.SetActive(false);
            if (CurrentMatchManager.Instance.roomMode == MenuManager.Gamemodes.FFA)
            {
                FFA_UI.SetActive(true);
            }
            else if (CurrentMatchManager.Instance.roomMode == MenuManager.Gamemodes.TDM)
            {
                TDM_UI.SetActive(true);
            }
            else if (CurrentMatchManager.Instance.roomMode == MenuManager.Gamemodes.CTF)
            {
                CTF_UI.SetActive(true);
            }
            else if (CurrentMatchManager.Instance.roomMode == MenuManager.Gamemodes.DZ)
            {
                DZ_UI.SetActive(true);
            }

            cameraObject.enabled = true;
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomMode].ToString() == "Team Deathmatch" && pv.Owner.IsMasterClient)
            {
                StartCoroutine(DelayedInit(0.2f));
            }
            StartCoroutine(DelayedSyncIsTeam(0.25f));
            settingsMenu.SettingsMenuAwakeFunction();
            SetWeaponPreview(GlobalDatabase.Instance.allWeaponDatas[(int)pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponIndex(1)]].itemIcon, GlobalDatabase.Instance.allWeaponDatas[(int)pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponIndex(2)]].itemIcon, GlobalDatabase.Instance.allEquipmentDatas[(int)pv.Owner.CustomProperties[LoadoutKeys.SelectedEquipmentIndex(1)]].itemIcon,GlobalDatabase.Instance.allEquipmentDatas[(int)pv.Owner.CustomProperties[LoadoutKeys.SelectedEquipmentIndex(2)]].itemIcon);
            OnJoiningOngoingRoom();
            deathInfoCanvas.alpha = 0f;
            randomPlayerColor = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255), 1);
            LoadoutDataJSON temp = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath).LoadoutData;
            for (int i = 0; i < temp.Slots.Count; i++)
            {
                RespawnLoadoutItemScript item = Instantiate(loadoutSwapPrefab, loadoutSwapPrefabHolder).GetComponent<RespawnLoadoutItemScript>();
                item.SetInfo(temp.Slots[i], this);
            }
        }
        openedInventory = false;
        CloseMenu();
        ToggleLoadoutSwapMenu(false);
    }

    public void SetWeaponPreview(Sprite w1, Sprite w2, Sprite e1, Sprite e2)
    {
        prevIcon1.sprite = w1;
        prevIcon2.sprite = w2;
        prevIcon3.sprite = e1;
        prevIcon4.sprite = e2;
    }
    IEnumerator DelayedInit(float amount)
    {
        yield return new WaitForSeconds(amount);
        CurrentMatchManager.Instance.RefreshPlayerList();
        CurrentMatchManager.Instance.DistributeTeams();
        CurrentMatchManager.Instance.TeamDeathmatchKillLogic(0, IsTeam);
    }
    IEnumerator DelayedSyncIsTeam(float amount)
    {
        yield return new WaitForSeconds(amount);
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
        cameraObject.enabled = true;
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
        cameraObject.gameObject.TryGetComponent(out UniversalAdditionalCameraData cameraData);
        if (cameraData)
        {
            cameraData.renderPostProcessing = false;
        }
        audioListener.enabled = false;
        respawning = true;
        respawnButton.interactable = false;
        deathUI.SetActive(false);
        cameraObject.enabled = true;
        Debug.Log("Instantiating Player Controller");

        if (spawnpoint == null) spawnpointUI.ChooseSpawnpoint(spawnpointUI.RandomSelectSpawnpoint());
        if ((bool)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RandomRespawn]) spawnpointUI.ChooseSpawnpoint(spawnpointUI.RandomSelectSpawnpoint());

        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { pv.ViewID });
        controller.GetComponent<PlayerControllerManager>().playerManager = this;
        playerUI = controller.GetComponent<UIManager>();
        player = controller.GetComponent<PlayerControllerManager>();
        returnTemp = 2;
        if (!openedOptions && player.pv.IsMine) Cursor.lockState = CursorLockMode.Locked;
        deathGUICanvas.alpha = 0f;
        deathGUICanvas.gameObject.SetActive(false);
        deathInfoCanvas.alpha = 0f;
        deathInfoCanvas.gameObject.SetActive(false);

        settingsMenu.SettingsMenuAwakeFunction();

        //Player Related Settings
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, UserSystem.SettingsOptionsKey));
        SettingsOptionsJSON jsonData = JsonUtility.FromJson<SettingsOptionsJSON>(json);

        controller.GetComponent<PlayerStats>().SetPlayerSensitivity(jsonData.MouseSensitivity);
        controller.GetComponent<PlayerStats>().SetPlayerFOV(jsonData.FieldOfView);
        controller.GetComponent<PlayerControllerManager>().IsTeam = IsTeam;
        cmm.RefreshAllHostileIndicators();
    }
    private int trackingViewID = -1;
    [PunRPC]
    void RPC_InstantiateDeadBody(Vector3 position, Quaternion rotation)
    {
        Destroy(Instantiate(deadBodyPrefab, position, rotation), 3f);
    }
    public void SetDeathUI(bool isSuicide, int ViewID, string killer = null)
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
            //if (PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomMode].ToString() != "Team Deathmatch") InstantiateKillMSG(pv.Owner.NickName, pv.Owner.NickName, -1, ((int)pv.Owner.CustomProperties["weaponIndex"] < 2 ? true : false));
            //else TDM_InstantiateKillMSG(pv.Owner.NickName, pv.Owner.NickName, -1, (bool)pv.Owner.CustomProperties["team"], ((int)pv.Owner.CustomProperties["weaponIndex"] < 2 ? true : false));
        }
    }
    IEnumerator DelayedControllerDestroy(float value)
    {
        yield return new WaitForSeconds(value);
        PhotonNetwork.Destroy(controller);
    }
    public void Downed(bool isSuicide, int ViewID, string killer = null, float delayObjectDestroy = 3f)
    {
        Debug.Log("Die() Invoked");
        SetDeathUI(isSuicide, ViewID, killer);
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
        //pv.RPC(nameof(RPC_InstantiateDeadBody), RpcTarget.All, controller.transform.position, controller.transform.rotation);
        audioListener.enabled = true;
        streakKills = 0;
        cameraObject.fieldOfView = FileOps<SettingsOptionsJSON>.ReadFile(UserSystem.SettingsOptionsPath).FieldOfView;
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
        if (pv.IsMine) SynchronizeValues(kills, deaths);
        hurtVolume.weight = 1f;
        if (cameraObject != null)
        {
            cameraObject.gameObject.TryGetComponent(out UniversalAdditionalCameraData cameraData);
            if (cameraData)
            {
                cameraData.renderPostProcessing = true;
            }
        }
    }
    public void Die(bool isSuicide, int ViewID, string killer = null, float delayObjectDestroy = 3f)
    {
        cameraObject.enabled = true;
        Downed(isSuicide, ViewID, killer, delayObjectDestroy);
        StartCoroutine(DelayedControllerDestroy(delayObjectDestroy));
        //PhotonNetwork.Destroy(controller);
        respawnButton.interactable = true;
        Debug.Log("Player " + player.pv.Owner.NickName + " was Killed");
        respawnCountdown = 7;
        hasRespawned = false;
        temp = 0;
        returnTemp = 0f;
        deathUI.SetActive(true);
        deathCountdown.text = "Waiting For Deployment";
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
        deathCountdown.text = "Waiting For Deployment";
        skipCountdownIndicator.SetActive(false);
        yield return new WaitForSeconds(duration);
        CompleteCountdown();
    }
    public void RedeployPlayer()
    {
        if (!pv.IsMine) return;
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
        //cmm.RemovePlayer(this);
        SceneManager.LoadScene(0);
        //roomManager.SelfDestruction();
        //Debug.Log("Self Destruction Occured");
        //Debug.Log("Loaded Scene from Player Manager");
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
            if (openedSettingsSection)
            {
                ToggleSettingsMenu(false);
                ToggleButtonHolder(true);
            }
            else if (openedOptions)
            {
                CloseMenu();
            }
            else if (openedLoadoutSwapMenu)
            {
                ToggleLoadoutSwapMenu(false);
            }
            else
            {
                OpenMenu();
            }
        }
        if (returnTemp <= 0f && hasRespawned && !respawning)
        {
            //CreateController();
        }
        if (respawning)
        {
            cameraObject.fieldOfView = Mathf.Lerp(cameraObject.fieldOfView, 60f, Time.deltaTime * 2);
            if (returnTemp >= 3f)
            {
                if (!hasRespawned)
                {
                    deathInfoCanvas.alpha = Mathf.Lerp(deathInfoCanvas.alpha, 0f, Time.deltaTime * 2);
                    deathGUICanvas.alpha = Mathf.Lerp(deathGUICanvas.alpha, 1f, Time.deltaTime * 5);
                    transform.position = Vector3.Slerp(transform.position, spawnpointCamera.transform.position, Time.deltaTime * 3);
                    transform.rotation = Quaternion.Slerp(transform.rotation, spawnpointCamera.transform.rotation, Time.deltaTime * 3);
                    managerCollider.enabled = false;
                    managerRigidbody.isKinematic = false;
                    managerRigidbody.useGravity = false;
                    hurtVolume.weight = Mathf.Lerp(hurtVolume.weight, 0f, Time.deltaTime * 2);
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
                deathInfoCanvas.alpha = Mathf.Lerp(deathInfoCanvas.alpha, 0f, Time.deltaTime * 3);
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
                deathCountdown.text = "Deploy";
                //respawnButton.interactable = true;
                return;
            }
            else
            {
                respawnButton.interactable = false;
            }
            deathCountdown.text = "Deploy in " + respawnCountdown.ToString();
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
        deathCountdown.text = "Waiting For Deployment";
        skipCountdownIndicator.SetActive(false);
        CreateController();
    }
    #region Menus
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
            FileOps<SettingsOptionsJSON>.WriteFile(data, UserSystem.SettingsOptionsPath);
            if (controller != null) controller.GetComponent<PlayerStats>().SetPlayerSensitivity(data.MouseSensitivity);
            if (controller != null) controller.GetComponent<PlayerStats>().SetPlayerFOV(data.FieldOfView);
            Debug.LogWarning("Writing Settings Options To Files...");
        }
        else
        {
            if (!File.Exists(Path.Combine(Application.persistentDataPath, UserSystem.SettingsOptionsKey))) settingsMenu.WriteSettingsOptionsToJSON();
            string json = File.ReadAllText(Path.Combine(Application.persistentDataPath, UserSystem.SettingsOptionsKey));
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

    public void ToggleLoadoutSwapMenu(bool state)
    {
        openedLoadoutSwapMenu = state;
        loadoutSwapMenu.SetActive(state);
    }
    #endregion
    public void LeaveGame()
    {
        Die(true, -1);
        player.playerManager.CloseMenu();
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
        if (PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomMode].ToString() != "Team Deathmatch") InstantiateKillMSG(killedPlayerName, pv.Owner.NickName, withWeaponIndex, isWeapon);
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
    public void RPC_InstantiateMessageOnKill(string killedName, string killerName, int weaponIndex, bool isWeapon, PhotonMessageInfo info)
    {
        GameObject temp = Instantiate(InGameUI.instance.killMSGPrefab, InGameUI.instance.killMSGHolder);
        KillMessageItem msg = temp.GetComponent<KillMessageItem>();
        msg.SetInfo(killedName, killerName, (weaponIndex != -1 ? (isWeapon ? InGameUI.instance.FindWeaponIcon(weaponIndex) : InGameUI.instance.FindEquipmentIcon(weaponIndex)) : skullIcon));
        if (PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomMode].ToString() == "Free For All")
        {
            if (PhotonNetwork.LocalPlayer == info.Sender && PhotonNetwork.LocalPlayer == pv.Owner)
            {
                Debug.LogWarning("You killed a baddie");
                msg.SetKilledColor(Color.red);
            }
            else if (PhotonNetwork.LocalPlayer != info.Sender && PhotonNetwork.LocalPlayer != pv.Owner)
            {
                Debug.LogWarning("You are killed by a baddie");
                msg.SetKillerColor(Color.red);
            }
            else if (PhotonNetwork.LocalPlayer != info.Sender && PhotonNetwork.LocalPlayer == pv.Owner)
            {
                Debug.LogWarning("You probably are killed by a baddie");
                msg.SetKillerColor(Color.red);
            }
            else
            {
                Debug.LogWarning("Unknown is killed by a baddie");
                msg.SetKilledColor(Color.red);
                msg.SetKillerColor(Color.red);
            }
        }
        Debug.Log(killedName + " was killed by " + killerName + " using weapon with an index of " + weaponIndex);
        Destroy(temp, 15f);
    }
    [PunRPC]
    public void RPC_TDM_InstantiateMessageOnKill(string killedName, string killerName, int weaponIndex, bool killerIsTeam, bool isWeapon, PhotonMessageInfo info)
    {
        GameObject temp = Instantiate(InGameUI.instance.killMSGPrefab, InGameUI.instance.killMSGHolder);
        KillMessageItem msg = temp.GetComponent<KillMessageItem>();
        msg.SetInfo(killedName, killerName, (weaponIndex != -1 ? (isWeapon ? InGameUI.instance.FindWeaponIcon(weaponIndex) : InGameUI.instance.FindEquipmentIcon(weaponIndex)) : skullIcon));
        if (cmm.localClientPlayer.IsTeam == killerIsTeam)
        {
            msg.SetKilledColor(Color.red);
        }
        else
        {
            msg.SetKillerColor(Color.red);
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
