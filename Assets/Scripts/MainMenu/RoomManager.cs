using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    [HideInInspector] public CurrentMatchManager cmm;
    public int currentSceneIndex = 0;
    public bool loadEnterAnim = true;
    public Animator mapPreviewLoading;
    public Canvas canvas;
    [SerializeField] Text mapNameText, gamemodeText;
    [SerializeField] Image mapPreviewImage;
    private IEnumerator SetLoadingScreenState_IEN(bool state, int duration)
    {
        yield return new WaitForSeconds(duration);
        mapPreviewLoading.SetBool("FinishedLoad", !state);
    }
    public void SetLoadingScreenState(bool state, int duration)
    {
        StartCoroutine(SetLoadingScreenState_IEN(state, duration));
    }
    public void SetLoadingPreview(MapItemInfo itemInfo, bool showScreen)
    {
        SetLoadingScreenState(showScreen, 0);
        mapPreviewImage.sprite = itemInfo.mapIcon;
        mapNameText.text = itemInfo.mapName;
        gamemodeText.text = $"{(string)PhotonNetwork.CurrentRoom.CustomProperties["roomMode"]}  -  {((int)PhotonNetwork.CurrentRoom.MaxPlayers).ToString()} Players Maximum";
    }
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        canvas.gameObject.SetActive(true);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 0)
        {
            CheckQueue();
            if (!loadEnterAnim)
            {
                MenuManager.instance.enterAnimMenu.SetActive(false);
                Debug.Log("Main Scene Loaded");
            }
            else
            {
                MenuManager.instance.enterAnimMenu.SetActive(true);
            }
        }
        if (scene.buildIndex == 1)//Inside game Scene
        {
            SetLoadingScreenState(false, 1);
            cmm = FindObjectOfType<CurrentMatchManager>();
            GameObject temp = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            cmm.localClientPlayer = temp.GetComponent<PlayerManager>();
        }
        else if (scene.buildIndex == 2)
        {
            SetLoadingScreenState(false, 1);
            cmm = FindObjectOfType<CurrentMatchManager>();
            GameObject temp = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            cmm.localClientPlayer = temp.GetComponent<PlayerManager>();
        }
        else if (scene.buildIndex == 3)
        {
            SetLoadingScreenState(false, 1);
            cmm = FindObjectOfType<CurrentMatchManager>();
            GameObject temp = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            cmm.localClientPlayer = temp.GetComponent<PlayerManager>();
        }
        else if (scene.buildIndex == 4)
        {
            SetLoadingScreenState(false, 1);
            cmm = FindObjectOfType<CurrentMatchManager>();
            GameObject temp = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            cmm.localClientPlayer = temp.GetComponent<PlayerManager>();
        }
        else if (scene.buildIndex == 5)
        {
            SetLoadingScreenState(false, 1);
            cmm = FindObjectOfType<CurrentMatchManager>();
            GameObject temp = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            cmm.localClientPlayer = temp.GetComponent<PlayerManager>();
        }
        else
        {
            CloseAllMenus();
        }
        currentSceneIndex = scene.buildIndex;
    }
    public void CloseAllMenus()
    {
        MenuManager.instance.CloseCurrentMenu();
        //MainMenuUIManager.instance.OpenMultiplayerMenu();
        //CloseMainMenuDelayed(0.5f);
        Debug.Log("Loaded Scene from Room Manager");
    }
    public IEnumerator CloseMainMenuDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        MenuManager.instance.CloseMainMenu();
    }
    public void SelfDestruction()
    {
        RoomManager temp = Instantiate(gameObject).GetComponent<RoomManager>();
        //temp.CloseAllMenus();
        //StartCoroutine(temp.CloseMainMenuDelayed(0.2f));
        Destroy(gameObject);
    }
    public void CheckQueue()
    {
        if (MenuManager.instance.queuedModalWindows.Count > 0)
        {
            for (int i = 0; i < MenuManager.instance.queuedModalWindows.Count; i++)
            {
                if (MenuManager.instance.queuedModalWindows[i].queueType == MenuManager.PopupQueue.OnMainMenuLoad)
                {
                    MenuManager.instance.AddModalWindow(MenuManager.instance.queuedModalWindows[i].title, MenuManager.instance.queuedModalWindows[i].content);
                    MenuManager.instance.queuedModalWindows.Remove(MenuManager.instance.queuedModalWindows[i]);
                    Debug.Log("Removing queued modal window from queue list");
                }
            }
            MenuManager.instance.queuedModalWindows.Clear();
        }
        if (MenuManager.instance.queuedNotifications.Count > 0)
        {
            for (int i = 0; i < MenuManager.instance.queuedNotifications.Count; i++)
            {
                if (MenuManager.instance.queuedNotifications[i].queueType == MenuManager.PopupQueue.OnMainMenuLoad)
                {
                    MenuManager.instance.AddNotification(MenuManager.instance.queuedNotifications[i].title, MenuManager.instance.queuedNotifications[i].content);
                    MenuManager.instance.queuedNotifications.Remove(MenuManager.instance.queuedNotifications[i]);
                    Debug.Log("Removing queued notification from queue list");
                }
            }
            MenuManager.instance.queuedNotifications.Clear();
        }
    }
}
