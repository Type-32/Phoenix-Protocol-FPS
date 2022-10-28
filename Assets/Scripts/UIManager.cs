using System.Collections;
using System.Collections.Generic;
using System;
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
    public GameObject nametagIndicatorObject;
    public Transform hurtIndicatorHolder;
    public GameObject hurtIndicatorPrefab;

    [Space]
    [Header("HUD Stats")]
    public Slider healthBar;
    public Slider armorBar;
    public Slider streakBackground;
    public Text healthText;
    public Text armorText;
    public Text nametagIndicator;
    public QuantityStatsHUD quantityHUD;
    public CanvasGroup healthBarAlpha;
    public CanvasGroup streakHUDAlpha;

    [Space]
    [Header("Inventory")]
    public GameObject inventory;
    public float hitmarkerTimePassed = 0;
    public static Action<Transform> CreateIndicator = delegate { };
    public static Func<Transform, bool> CheckIfObjectInSight = null;
    private Dictionary<Transform, HurtIndicatorBehavior> Indicators = new Dictionary<Transform, HurtIndicatorBehavior>();

    private void OnEnable()
    {
        CreateIndicator += Create;
        CheckIfObjectInSight += InSight;
    }
    private void OnDisable()
    {
        CreateIndicator -= Create;
        CheckIfObjectInSight -= InSight;
    }
    void Create(Transform target)
    {
        if (Indicators.ContainsKey(target))
        {
            Indicators[target].Restart();
            return;
        }
        HurtIndicatorBehavior newIndicator = Instantiate(hurtIndicatorPrefab, hurtIndicatorHolder).GetComponent<HurtIndicatorBehavior>();
        newIndicator.RegisterData(target, player.transform, new Action( () => { Indicators.Remove(target); }));
        Indicators.Add(target, newIndicator);
    }
    bool InSight(Transform t)
    {
        Vector3 screenpoint = player.fpsCam.playerMainCamera.WorldToViewportPoint(t.position);
        return screenpoint.z > 0 && screenpoint.x > 0 && screenpoint.x < 1 && screenpoint.y > 0 && screenpoint.y < 1;
    }

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
        if (!player.pv.IsMine) return;
        Cursor.lockState = CursorLockMode.Locked;
        
        //interactionIndicator = FindObjectOfType<InteractionIndicatorScript>().gameObject;
    }

    /*
    void Update()
    {
        if (!player.pv.IsMine) return;
        //passedTime += Time.deltaTime;
        //if (healthAlphaDuration <= 0f)
        //{
        //    healthBarAlpha.alpha = Mathf.Lerp(healthBarAlpha.alpha, 0.6f, 5 * Time.deltaTime);
        //}
        //else
        //{
        //    healthBarAlpha.alpha = Mathf.Lerp(healthBarAlpha.alpha, 1f, 5 * Time.deltaTime);
        //    if (passedTime >= 1) { healthAlphaDuration -= 1; passedTime = 0; }
        //}

    }*/
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
