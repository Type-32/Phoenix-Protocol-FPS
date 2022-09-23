using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCamLookScript : MonoBehaviour
{
    public UIManager ui;
    public DeathMenuController dmc;
    public RespawningUI spUI;
    public bool isRespawning = false;
    private int maxRespawnDelay = 3;
    public int respawnDelay;
    float temp = 0f;
    private void Awake()
    {
        ui = FindObjectOfType<UIManager>();
        //ui.deathMenu.SetActive(true);
        dmc = FindObjectOfType<DeathMenuController>();
        spUI = FindObjectOfType<RespawningUI>();
        respawnDelay = maxRespawnDelay;
        dmc.respawnDeathIndicator.GetComponent<CanvasGroup>().alpha = 0f;
        dmc.loadoutDeathIndicator.GetComponent<CanvasGroup>().alpha = 0f;
        dmc.deathCam = this;
        dmc.playerIsDead = true;
    }
    private void Start()
    {
        spUI = FindObjectOfType<RespawningUI>();
    }
    private void Update()
    {
        if (isRespawning)
        {
            return;
        }
        ui.hud.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(ui.hud.GetComponent<CanvasGroup>().alpha, 0f, Time.deltaTime * 8);
        dmc.loadoutDeathIndicator.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(dmc.loadoutDeathIndicator.GetComponent<CanvasGroup>().alpha, 1f, 5 * Time.deltaTime);
        if (respawnDelay > 0) temp += Time.deltaTime;
        else
        {
            dmc.respawnDeathIndicator.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(dmc.respawnDeathIndicator.GetComponent<CanvasGroup>().alpha, 1f, 5 * Time.deltaTime);
        }
        if(temp >= 1f)
        {
            temp = 0f;
            respawnDelay--;
        }
        //if(Input.GetKeyDown("f")) RespawnPlayer();
    }
    public void RespawnPlayer()
    {
        spUI = FindObjectOfType<RespawningUI>();
        isRespawning = true;
        ui.hud.GetComponent<CanvasGroup>().alpha = 1f;
        dmc.playerIsDead = false;
        dmc.respawnDeathIndicator.GetComponent<CanvasGroup>().alpha = 0f;
        dmc.loadoutDeathIndicator.GetComponent<CanvasGroup>().alpha = 0f;
        dmc.deathCam = null;
        Destroy(gameObject);
    }
}
