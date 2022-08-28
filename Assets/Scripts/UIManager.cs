using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Script and Function References")]
    public PlayerManager player;
    public Animator anim;
    public GameObject gui;
    public GameObject hud;

    [Space]
    [Header("HUD Elements")]
    public GameObject crosshair;
    public GameObject interactionIndicator;

    [Space]
    [Header("HUD Stats")]
    public Slider healthBar;
    public Slider armorBar;
    public QuantityStatsHUD quantityHUD;

    [Space]
    [Header("Inventory")]
    public GameObject inventory;
    public GameObject loadout;

    [Space]
    [Header("Loadouts")]
    public LoadoutMenu loadoutMenu;

    [Space]
    [Header("Options Elements")]
    public GameObject optionsUI;
    public GameObject deathMenu;
    public Slider mouseSensitivitySlider;

    private const int xpBase = 500;
    private int xpLimit = 0;
    public float hitmarkerTimePassed = 0;
    private float hitmarkerTimeLimit = 0;

    private float healthAlphaDuration = 0f;
    float passedTime = 0f;

    public bool openedOptions = false;
    public bool openedInventory = false;
    public bool openedLoadoutMenu = false;
    void Start()
    {
        openedOptions = false;
        openedInventory = false;
        optionsUI.SetActive(false);
        CloseMenu();
        Cursor.lockState = CursorLockMode.Locked;
        loadoutMenu.gameObject.SetActive(openedLoadoutMenu);
        //interactionIndicator = FindObjectOfType<InteractionIndicatorScript>().gameObject;
    }

    void Update()
    {
        if(player != null)
        {
            if (player.stats.health > 0)
            {
                if (deathMenu.activeSelf) deathMenu.SetActive(false);
            }
        }
        if ((Input.GetKeyDown(KeyCode.Escape)))
        {
            if (openedOptions)
            {
                CloseMenu();
            }else if (openedLoadoutMenu)
            {
                CloseLoadoutMenu();
            }else if (loadoutMenu.openedSelectionMenu)
            {
                loadoutMenu.CloseSelectionMenu();
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
            else
            {
                OpenLoadoutMenu();
            }
        }
        passedTime += Time.deltaTime;
        if (healthAlphaDuration <= 0f)
        {
            healthBar.gameObject.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(healthBar.gameObject.GetComponent<CanvasGroup>().alpha, 0.3f, 5 * Time.deltaTime);
        }
        else
        {
            healthBar.gameObject.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(healthBar.gameObject.GetComponent<CanvasGroup>().alpha, 1f, 5 * Time.deltaTime);
            if (passedTime >= 1) { healthAlphaDuration -= 1; passedTime = 0; }
        }

    }
    public void OpenMenu()
    {
        Debug.Log("Opened Options UI ");
        Cursor.lockState = CursorLockMode.None;
        openedOptions = true;
        optionsUI.SetActive(true);
    }
    public void CloseMenu()
    {
        Debug.Log("Closed Options UI ");
        Cursor.lockState = CursorLockMode.Locked;
        openedOptions = false;
        optionsUI.SetActive(false);
    }
    public enum HitmarkerType
    {
        Killmarker,
        HeavyHitmarker,
        Hitmarker,
        ArmorBreakMarker,
        None
    }
    public void OpenLoadoutMenu()
    {
        Debug.Log("Opened Loadout UI ");
        Cursor.lockState = CursorLockMode.None;
        openedLoadoutMenu = true;
        loadoutMenu.gameObject.SetActive(true);
        loadoutMenu.slotHolderScript.RefreshLoadoutSlotInfo();
    }
    public void CloseLoadoutMenu()
    {
        Debug.Log("Closed Loadout UI ");
        Cursor.lockState = CursorLockMode.Locked;
        openedLoadoutMenu = false;
        loadoutMenu.gameObject.SetActive(false);
    }
}
