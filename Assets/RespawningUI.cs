using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawningUI : MonoBehaviour
{
    public UIManager ui;
    public SpawnpointHolder sp;
    public DeathMenuController dmc;
    public Button respawnButton;
    // Start is called before the first frame update
    void Awake()
    {
        ui = FindObjectOfType<UIManager>();
        sp = FindObjectOfType<SpawnpointHolder>();
        dmc = FindObjectOfType<DeathMenuController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sp.setSpawnpoint == null) respawnButton.interactable = false;
        else if (!dmc.playerIsDead) respawnButton.interactable = false;
        else respawnButton.interactable = true;
    }
    public void OnRespawnButtonPress()
    {
        Cursor.lockState = CursorLockMode.Locked;
        dmc.deathCam.RespawnPlayer();
        ui.CloseLoadoutMenu();
    }
}
