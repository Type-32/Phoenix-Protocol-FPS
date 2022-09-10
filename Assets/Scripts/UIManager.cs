using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Script and Function References")]
    public PlayerControllerManager player;
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


    private const int xpBase = 500;
    private int xpLimit = 0;
    public float hitmarkerTimePassed = 0;
    private float hitmarkerTimeLimit = 0;

    private float healthAlphaDuration = 0f;
    float passedTime = 0f;

    public struct ReturnHitmarkerData
    {
        public bool isHit;
        public bool isKilled;
        public void SetValues(bool hit, bool killed)
        {
            isHit = hit;
            isKilled = killed;
        }
    };
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        //interactionIndicator = FindObjectOfType<InteractionIndicatorScript>().gameObject;
    }

    void Update()
    {
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
    public enum HitmarkerType
    {
        Killmarker,
        HeavyHitmarker,
        Hitmarker,
        ArmorBreakMarker,
        None
    }
    public void InvokeHitmarker(HitmarkerType type)
    {
        switch (type)
        {
            case HitmarkerType.Killmarker:
                anim.SetTrigger("Killmarker");
                break;
            case HitmarkerType.Hitmarker:
                anim.SetTrigger("Hitmarker");
                break;
            default:
                anim.SetTrigger("Hitmarker");
                break;
        }
    }
}
