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
    GameObject controller;
    [SerializeField] GameObject deathUI;
    [SerializeField] Text deathCountdown;
    [SerializeField] Camera cameraObject;
    [SerializeField] UIManager playerUI;
    [SerializeField] PlayerControllerManager player;
    [SerializeField] GameObject skipCountdownIndicator;
    [SerializeField] GameObject spawnpointUISelection;
    public LoadoutSlotHolder slotHolderScript;
    public ChoiceHolderScript choiceHolderScript;
    [SerializeField] bool hasRespawned = false;
    [SerializeField] bool respawning = false;
    [SerializeField] float temp;
    [SerializeField] float secondFill = 0f;
    [SerializeField] float secondCount = 0f;
    [SerializeField] float returnTemp = 2f;
    [SerializeField] int respawnCountdown = 8;

    [Space]
    [Header("UI")]
    public GameObject loadout;
    public LoadoutMenu loadoutMenu;

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

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        spawnpointCamera = FindObjectOfType<SpawnpointCamera>();
        if (pv.IsMine)
        {
            CreateController();
        }
        else
        {
            cameraObject.gameObject.SetActive(false);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!pv.IsMine)
        {
            cameraObject.gameObject.SetActive(false);
        }
        //if (!pv.IsMine) return;
        openedInventory = false;
        CloseMenu();
        CloseLoadoutMenu();
        deathCountdown.text = "Waiting for Respawn";

        respawnCountdown = 8;
        hasRespawned = true;
        deathUI.SetActive(false);
    }
    void CreateController()
    {
        respawning = true;
        respawnUI.respawnButton.interactable = false;
        respawnUI.redeployButton.interactable = true;
        deathUI.SetActive(false);
        spawnpoint = SpawnManager.Instance.GetRandomSpawnpoint();
        Debug.Log("Instantiating Player Controller");
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnpoint.position, spawnpoint.rotation, 0, new object[] {pv.ViewID});
        controller.GetComponent<PlayerControllerManager>().playerManager = this;
        playerUI = controller.GetComponent<UIManager>();
        player = controller.GetComponent<PlayerControllerManager>();
        loadoutMenu.ui = controller.GetComponent<UIManager>();
        returnTemp = 2;

        //Player Related Settings
        player.stats.mouseSensitivity = PlayerPrefs.GetFloat("Mouse Sensitivity");
    }
    public void Die()
    {
        respawning = true;
        secondCount = 0;

        transform.position = controller.transform.position;
        transform.rotation = controller.transform.rotation;

        deaths++;
        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        PhotonNetwork.Destroy(controller);
        respawnUI.respawnButton.interactable = true;
        respawnUI.redeployButton.interactable = false;
        Debug.Log("Player " + player.pv.Owner.NickName + " was Killed");
        respawnCountdown = 8;
        hasRespawned = false;
        temp = 0;
        returnTemp = 0f;
        deathUI.SetActive(true);
        deathCountdown.text = "Waiting for Respawn";
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
        respawnUI.respawnButton.interactable = false;
        hasRespawned = false;
        CompleteCountdown();
    }
    private void Update()
    {
        if (!pv.IsMine) return;
        if (secondFill < 1f)
        {
            secondFill += Time.deltaTime;
        }
        else
        {
            secondFill = 0f;
            secondCount++;
        }
        if ((Input.GetKeyDown(KeyCode.Escape)))
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
            CreateController();
        }
        if (respawning)
        {
            if (returnTemp >= 2f)
            {
                if (!hasRespawned)
                {
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
            if (returnTemp > 0f)
            {
                returnTemp -= secondCount;
                secondCount = 0;
            }
            else
            {
                if (hasRespawned)
                {
                    transform.position = Vector3.Slerp(transform.position, spawnpoint.position, Time.deltaTime * 3);
                    transform.rotation = Quaternion.Slerp(transform.rotation, spawnpoint.rotation, Time.deltaTime * 3);
                }
            }
        }
        if (hasRespawned) return;
        if (openedLoadoutMenu) return;
        temp += Time.deltaTime;
        if (Input.GetKeyDown("f"))
        {
            CompleteCountdown();
            return;
        }
        if(temp >= 1f)
        {
            if(respawnCountdown <= 0)
            {
                CompleteCountdown();
                return;
            }
            else
            {
                skipCountdownIndicator.SetActive(true);
            }
            deathCountdown.text = "Respawning in " + respawnCountdown.ToString();
            respawnCountdown--;
            temp = 0;
        }
    }
    void CompleteCountdown()
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
        Debug.Log("Opened Loadout UI ");
        openedLoadoutMenu = true;
        loadoutMenu.gameObject.SetActive(true);
        slotHolderScript.RefreshLoadoutSlotInfo();
        Cursor.lockState = CursorLockMode.None;
    }
    public void CloseLoadoutMenu()
    {
        //if (!pv.IsMine) return;
        Debug.Log("Closed Loadout UI ");
        openedLoadoutMenu = false;
        loadoutMenu.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ToggleButtonHolder(bool value)
    {
        // (!pv.IsMine) return;
        Debug.Log("Toggled Button Holder ");
        //Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        enabledButtonHolder = value;
        buttonHolder.SetActive(value);
    }
    public void ToggleSettingsMenu(bool value)
    {
        //if (!pv.IsMine) return;
        Debug.Log("Toggled Settings Menu ");
        //Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        openedSettingsSection = value;
        settingsMenu.gameObject.SetActive(value);

        //Set Local Player Properties According to Settings Menu
        player.stats.mouseSensitivity = PlayerPrefs.GetFloat("Mouse Sensitivity");
    }
    public void OpenMenu()
    {
        //if (!pv.IsMine) return;
        openedOptions = true;
        Debug.Log("Opened Options UI ");
        optionsUI.SetActive(openedOptions);
        ToggleButtonHolder(openedOptions);
        ToggleSettingsMenu(!openedOptions);
        Cursor.lockState = CursorLockMode.None;
    }
    public void CloseMenu()
    {
        //if (!pv.IsMine) return;
        openedOptions = false;
        Debug.Log("Closed Options UI ");
        Cursor.lockState = CursorLockMode.Locked;
        optionsUI.SetActive(openedOptions);
        ToggleButtonHolder(openedOptions);
        ToggleSettingsMenu(openedOptions);
        Cursor.lockState = CursorLockMode.Locked;
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

    public void GetKill()
    {
        pv.RPC(nameof(RPC_GetKill), pv.Owner);
    }
    [PunRPC]
    void RPC_GetKill()
    {
        kills++;
        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        player.ui.InvokeHitmarker(UIManager.HitmarkerType.Killmarker);
        player.sfx.InvokeHitmarkerAudio(UIManager.HitmarkerType.Killmarker);
    }
    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.pv.Owner == player);
    }
}
