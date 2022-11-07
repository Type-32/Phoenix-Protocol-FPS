using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    public int currentSceneIndex = 0;
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
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
        if (scene.buildIndex == 1)//Inside game Scene
        {
            //PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }else if (scene.buildIndex == 2)
        {
            //PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }else if (scene.buildIndex == 3)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
        else if (scene.buildIndex == 0)
        {
            if (MainMenuUIManager.instance.queuedPopups.Count > 0)
            {
                for (int i = 0; i < MainMenuUIManager.instance.queuedPopups.Count; i++)
                {
                    if (MainMenuUIManager.instance.queuedPopups[i].queueType == MainMenuUIManager.PopupQueue.OnMainMenuLoad)
                    {
                        MainMenuUIManager.instance.AddPopup(MainMenuUIManager.instance.queuedPopups[i].title, MainMenuUIManager.instance.queuedPopups[i].content);
                        MainMenuUIManager.instance.queuedPopups.Remove(MainMenuUIManager.instance.queuedPopups[i]);
                        Debug.Log("Removing used queued popup");
                    }
                }
                MainMenuUIManager.instance.queuedPopups.Clear();
            }
        }
        else
        {
            CloseAllMenus();
        }
        currentSceneIndex = scene.buildIndex;
    }
    public void CloseAllMenus()
    {
        MainMenuUIManager.instance.CloseMainMenu();
        MainMenuUIManager.instance.CloseLoadingMenu();
        MainMenuUIManager.instance.CloseFindRoomMenu();
        MainMenuUIManager.instance.CloseLoadingMenu();
        MainMenuUIManager.instance.CloseMultiplayerMenu();
        MainMenuUIManager.instance.CloseRoomMenu();
        MainMenuUIManager.instance.CloseSettingsMenu();
        MainMenuUIManager.instance.CloseUpdateLogsMenu();
        MainMenuUIManager.instance.CloseCreateRoomMenu();
        MainMenuUIManager.instance.CloseLoadoutSelectionMenu();
        MainMenuUIManager.instance.CloseCosmeticsMenu();
        //MainMenuUIManager.instance.OpenMultiplayerMenu();
        //CloseMainMenuDelayed(0.5f);
        Debug.Log("Loaded Scene from Room Manager");
    }
    public IEnumerator CloseMainMenuDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        MainMenuUIManager.instance.CloseMainMenu();
    }
    public void SelfDestruction()
    {
        RoomManager temp = Instantiate(gameObject).GetComponent<RoomManager>();
        //temp.CloseAllMenus();
        //StartCoroutine(temp.CloseMainMenuDelayed(0.2f));
        Destroy(gameObject);
    }
}
