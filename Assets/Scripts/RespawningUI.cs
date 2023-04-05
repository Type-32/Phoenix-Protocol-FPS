using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawningUI : MonoBehaviour
{
    public PlayerManager playerManager;
    public Button redeployButton;
    void Awake()
    {

    }

    // Update is called once per frame
    /*
    void Update()
    {
        //if (sp.setSpawnpoint == null) respawnButton.interactable = false;
        //else if (!dmc.playerIsDead) respawnButton.interactable = false;
        //else respawnButton.interactable = true;
    }*/
    /*
    public void OnRespawnButtonPress()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //dmc.deathCam.RespawnPlayer();
        playerManager.CloseLoadoutMenu();
        playerManager.RespawnPlayer();
    }*/
}
