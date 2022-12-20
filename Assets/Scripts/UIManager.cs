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
    public GameObject targetIndicatorHUD;
    public RectTransform hostileTIHolder, friendlyTIHolder, objectiveTIHolder, supplyTIHolder;
    public CanvasGroup hostileTIGroup, friendlyTIGroup, objectiveTIGroup, supplyTIGroup;
    public GameObject targetIndicatorPrefab;

    [Space]
    [Header("HUD Stats")]
    public Slider healthBar;
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
    public Image hitmarker1, hitmarker2, hitmarker3, hitmarker4;
    public Image hitmarker5, hitmarker6, hitmarker7, hitmarker8;
    public Image hitmarker9, hitmarker10, hitmarker11, hitmarker12;

    public static Action<ValueTransform> CreateIndicator = delegate { };
    public static Func<ValueTransform, bool> CheckIfObjectInSight = null;
    private Dictionary<ValueTransform, HurtIndicatorBehavior> Indicators = new();
    public List<TargetIndicator> supplyTargetIndicators = new();
    public List<TargetIndicator> objectiveTargetIndicators = new();
    public List<TargetIndicator> friendlyTargetIndicators = new();
    public List<TargetIndicator> hostileTargetIndicators = new();
    public struct ValueTransform
    {
        public Vector3 position;
        public Quaternion rotation;
    }
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
    void Create(ValueTransform target)
    {
        if (Indicators.ContainsKey(target))
        {
            Indicators[target].Restart();
            return;
        }
        HurtIndicatorBehavior newIndicator = Instantiate(hurtIndicatorPrefab, hurtIndicatorHolder).GetComponent<HurtIndicatorBehavior>();
        newIndicator.RegisterData(target, player.transform, new Action(() => { Indicators.Remove(target); }));
        Indicators.Add(target, newIndicator);
    }
    bool InSight(ValueTransform t)
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


    void Update()
    {
        if (!player.pv.IsMine) return;
        if (objectiveTargetIndicators.Count > 0)
        {
            for (int i = 0; i < objectiveTargetIndicators.Count; i++)
            {
                objectiveTargetIndicators[i].UpdateTargetIndicator();
            }
        }
        if (friendlyTargetIndicators.Count > 0)
        {
            for (int i = 0; i < friendlyTargetIndicators.Count; i++)
            {
                friendlyTargetIndicators[i].UpdateTargetIndicator();
            }
        }
        if (hostileTargetIndicators.Count > 0)
        {
            for (int i = 0; i < hostileTargetIndicators.Count; i++)
            {
                hostileTargetIndicators[i].UpdateTargetIndicator();
            }
        }
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

    }
    public enum HitmarkerType
    {
        Killmarker,
        HeavyHitmarker,
        Hitmarker,
        ArmorBreakMarker,
        None
    }
    public enum TargetIndicatorType
    {
        None,
        Objective,
        Hostile,
        Friendly,
        Supply
    }
    public void AddTargetIndicator(GameObject target, TargetIndicatorType type, Color color)
    {
        switch (type)
        {
            case TargetIndicatorType.None:
                return;
            case TargetIndicatorType.Hostile:
                TargetIndicator indicator1 = Instantiate(targetIndicatorPrefab, hostileTIHolder).GetComponent<TargetIndicator>();
                indicator1.InitializeIndicator(target, type, color, player.fpsCam.playerMainCamera, hostileTIHolder);
                hostileTargetIndicators.Add(indicator1);
                break;
            case TargetIndicatorType.Friendly:
                TargetIndicator indicator2 = Instantiate(targetIndicatorPrefab, friendlyTIHolder).GetComponent<TargetIndicator>();
                indicator2.InitializeIndicator(target, type, color, player.fpsCam.playerMainCamera, friendlyTIHolder);
                friendlyTargetIndicators.Add(indicator2);
                break;
            case TargetIndicatorType.Objective:
                TargetIndicator indicator3 = Instantiate(targetIndicatorPrefab, objectiveTIHolder).GetComponent<TargetIndicator>();
                indicator3.InitializeIndicator(target, type, color, player.fpsCam.playerMainCamera, objectiveTIHolder);
                objectiveTargetIndicators.Add(indicator3);
                break;
            case TargetIndicatorType.Supply:
                TargetIndicator indicator4 = Instantiate(targetIndicatorPrefab, supplyTIHolder).GetComponent<TargetIndicator>();
                indicator4.InitializeIndicator(target, type, color, player.fpsCam.playerMainCamera, supplyTIHolder);
                supplyTargetIndicators.Add(indicator4);
                break;
        }
    }
    public void RemoveTargetIndicator(TargetIndicator indicator)
    {
        if (objectiveTargetIndicators.Contains(indicator))
        {
            objectiveTargetIndicators.Remove(indicator);
            Destroy(indicator.gameObject);
        }
        if (hostileTargetIndicators.Contains(indicator))
        {
            hostileTargetIndicators.Remove(indicator);
            Destroy(indicator.gameObject);
        }
        if (friendlyTargetIndicators.Contains(indicator))
        {
            friendlyTargetIndicators.Remove(indicator);
            Destroy(indicator.gameObject);
        }
    }
    public void InvokeHitmarker(HitmarkerType type, Color color = new Color())
    {
        switch (type)
        {
            case HitmarkerType.Killmarker:
                anim.SetTrigger("Killmarker");
                if (color != new Color())
                {
                    hitmarker5.color = color;
                    hitmarker6.color = color;
                    hitmarker7.color = color;
                    hitmarker8.color = color;
                    hitmarker9.color = color;
                    hitmarker10.color = color;
                    hitmarker11.color = color;
                    hitmarker12.color = color;
                }
                else
                {
                    hitmarker5.color = Color.red;
                    hitmarker6.color = Color.red;
                    hitmarker7.color = Color.red;
                    hitmarker8.color = Color.red;
                    hitmarker9.color = Color.red;
                    hitmarker10.color = Color.red;
                    hitmarker11.color = Color.red;
                    hitmarker12.color = Color.red;
                }
                break;
            case HitmarkerType.Hitmarker:
                anim.SetTrigger("Hitmarker");
                if (color != new Color())
                {
                    hitmarker1.color = color;
                    hitmarker2.color = color;
                    hitmarker3.color = color;
                    hitmarker4.color = color;
                }
                else
                {
                    hitmarker1.color = Color.white;
                    hitmarker2.color = Color.white;
                    hitmarker3.color = Color.white;
                    hitmarker4.color = Color.white;
                }
                break;
            default:
                anim.SetTrigger("Hitmarker");
                break;
        }
    }
}
