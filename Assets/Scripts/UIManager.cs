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
    public CanvasGroup crosshairGroup;
    public RectTransform reticle;
    public GameObject FFA_UI, TDM_UI, CTF_UI, DZ_UI;

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
    public float restingSize = 60f;
    public float walkSize = 140f;
    public float jumpSize = 180f;
    public float slideSize = 220f;
    public float crouchSize = 50f;
    private float currentSize;
    private float targetSize;
    [SerializeField] Image topReticle, bottomReticle, leftReticle, rightReticle;

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

    [Space, Header("Gamemodes HUD")]
    [SerializeField] Text ffa_prompt;
    [SerializeField] Text ffa_topName;
    [SerializeField] Text ffa_topKills;

    [Space, Header("Sway")]
    [SerializeField]
    GameObject ui_sway, ui_base;

    [Space, Header("UI Sway")]
    [SerializeField] float swayIntensity, maxSwayIntensity, smoothness;
    private Vector3 initPos, targetPos;

    void Awake()
    {
        initPos = ui_base.transform.localPosition;
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
        SetReticleColor(Color.white);
        Cursor.lockState = CursorLockMode.Locked;
        currentSize = restingSize;
        //interactionIndicator = FindObjectOfType<InteractionIndicatorScript>().gameObject;
    }

    void Update()
    {
        if (!player.pv.IsMine) return;
        UISway();
        UpdatePosition();
        if (objectiveTargetIndicators.Count > 0)
        {
            for (int i = 0; i < objectiveTargetIndicators.Count; i++)
            {
                if (objectiveTargetIndicators[i] == null)
                {
                    objectiveTargetIndicators.Remove(objectiveTargetIndicators[i]);
                }
                objectiveTargetIndicators[i].UpdateTargetIndicator();
            }
        }
        if (friendlyTargetIndicators.Count > 0)
        {
            for (int i = 0; i < friendlyTargetIndicators.Count; i++)
            {
                if (friendlyTargetIndicators[i] == null)
                {
                    friendlyTargetIndicators.Remove(friendlyTargetIndicators[i]);
                }
                friendlyTargetIndicators[i].UpdateTargetIndicator();
            }
        }
        if (hostileTargetIndicators.Count > 0)
        {
            for (int i = 0; i < hostileTargetIndicators.Count; i++)
            {
                if (hostileTargetIndicators[i] == null)
                {
                    hostileTargetIndicators.Remove(hostileTargetIndicators[i]);
                }
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
        if (player.stats.isSliding)
        {
            targetSize = Mathf.Lerp(targetSize, slideSize, Time.deltaTime * 10f);
        }
        else if (player.stats.isJumping || !player.stats.onGround)
        {
            targetSize = Mathf.Lerp(targetSize, jumpSize, Time.deltaTime * 10f);
        }
        else if (player.stats.isWalking)
        {
            targetSize = Mathf.Lerp(targetSize, walkSize, Time.deltaTime * 10f);
        }
        else if (player.stats.isCrouching)
        {
            targetSize = Mathf.Lerp(targetSize, crouchSize, Time.deltaTime * 10f);
        }
        else
        {
            targetSize = Mathf.Lerp(targetSize, restingSize, Time.deltaTime * 10f);
        }
        currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * 6f);
        reticle.sizeDelta = new Vector2(currentSize, currentSize);
        if (player.stats.isSprinting)
        {
            crosshairGroup.alpha = Mathf.Lerp(crosshairGroup.alpha, 0f, Time.deltaTime * 5f);
        }
        else
        {
            crosshairGroup.alpha = Mathf.Lerp(crosshairGroup.alpha, 1f, Time.deltaTime * 5f);
        }
    }
    float timer = 0f;
    private void UISway()
    {
        if (!player.stats.mouseMovementEnabled) return;
        timer += Time.deltaTime * (player.stats.isSprinting ? 22f : 0f);
        Vector3 finalPos = new Vector3(Mathf.Clamp((-player.fpsCam.mouseX - (player.stats.onGround & player.stats.isSprinting ? (Mathf.Cos(timer / 2) * 0.8f) : 0f)) * swayIntensity, -maxSwayIntensity, maxSwayIntensity), Mathf.Clamp((-Input.GetAxis("Mouse Y") + player.body.velocity.y - (player.stats.isJumping & player.stats.onGround ? 3f : 0f) - (player.stats.onGround & player.stats.isSprinting ? Mathf.Sin(timer) : 0f)) * swayIntensity, -maxSwayIntensity, maxSwayIntensity), 0f);
        ui_base.transform.localPosition = Vector3.Lerp(ui_base.transform.localPosition, finalPos + initPos, Time.deltaTime * smoothness);
    }
    private void UpdatePosition()
    {
        targetPos = Vector3.Lerp(targetPos, initPos, Time.deltaTime * smoothness);
        ui_base.transform.localPosition = Vector3.Lerp(ui_base.transform.localPosition, targetPos, Time.deltaTime * smoothness);
    }

    public void AddReticleSize(float amount)
    {
        targetSize += amount;
    }
    public void SetReticleColor(Color color)
    {
        topReticle.color = color;
        bottomReticle.color = color;
        leftReticle.color = color;
        rightReticle.color = color;
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
