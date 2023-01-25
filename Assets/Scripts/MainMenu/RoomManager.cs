using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    [HideInInspector] public CurrentMatchManager cmm;
    public int currentSceneIndex = 0;
    public bool loadEnterAnim = true;
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
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
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
            if (!loadEnterAnim)
            {
                MenuManager.instance.enterAnimMenu.SetActive(false);
                Debug.Log("Main Scene Loaded");
            }
        }
        if (scene.buildIndex == 1)//Inside game Scene
        {
            //PhotonNetwork.CurrentRoom.IsVisible = false;
            cmm = FindObjectOfType<CurrentMatchManager>();
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
        else if (scene.buildIndex == 2)
        {
            //PhotonNetwork.CurrentRoom.IsVisible = false;
            cmm = FindObjectOfType<CurrentMatchManager>();
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
        else if (scene.buildIndex == 3)
        {
            cmm = FindObjectOfType<CurrentMatchManager>();
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
        else if (scene.buildIndex == 4)
        {
            cmm = FindObjectOfType<CurrentMatchManager>();
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
        else if (scene.buildIndex == 5)
        {
            cmm = FindObjectOfType<CurrentMatchManager>();
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
        else if (scene.buildIndex == 0)
        {
            CheckQueue();
        }
        else
        {
            CloseAllMenus();
        }
        currentSceneIndex = scene.buildIndex;
    }
    public void CloseAllMenus()
    {
        MenuManager.instance.CloseMainMenu();
        MenuManager.instance.CloseLoadingMenu();
        MenuManager.instance.CloseFindRoomMenu();
        MenuManager.instance.CloseLoadingMenu();
        MenuManager.instance.CloseRoomMenu();
        MenuManager.instance.CloseSettingsMenu();
        MenuManager.instance.CloseUpdateLogsMenu();
        MenuManager.instance.CloseCreateRoomMenu();
        MenuManager.instance.CloseLoadoutSelectionMenu();
        MenuManager.instance.CloseCosmeticsMenu();
        MenuManager.instance.OpenMainMenu();
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
