using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : Equipment
{
    [HideInInspector] public PlayerControllerManager player;
    [HideInInspector] public EquipmentHolder holder;
    [Header("Scripts")]
    public EquipmentStats stats;
    public EquipmentUI ui;
    public EquipmentCore core;
    public EquipmentLogic logic;
    public EquipmentAnimation animate;
    public GameObject projectile;

    [Space, Header("Equipment Body")]
    [HideInInspector] public MouseLookScript fpsCam;
    public GameObject equipmentVisual;
    public GameObject handsVisual;
    public GameObject thirdPersonHandsVisual;
    public GameObject swayModel;
    public GameObject recoilModel;
    [HideInInspector] public Recoil camRecoil;
    [HideInInspector] public int inEquipmentState;
    public void SetFirstPersonViewHandsMaterial(Material mat)
    {
        MeshRenderer[] tmp = handsVisual.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < tmp.Length; i++)
        {
            tmp[i].material = mat;
        }
    }
    public void SetThirdPersonViewHandsMaterial(Material mat)
    {
        MeshRenderer[] tmp = thirdPersonHandsVisual.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < tmp.Length; i++)
        {
            tmp[i].material = mat;
        }
    }
    public override void Use()
    {
        DeterminatesFunction();
        if (!stats.interactionEnabled) return;

        animate.CoreAnimations();
        logic.EquipmentGeneralLogic();
        ui.UIFunctions();
        core.EquipmentCoreFunc();
    }
    public override void InitializeStart()
    {
        camRecoil = GetComponentInParent<Recoil>();
        holder = GetComponentInParent<EquipmentHolder>();
        stats.InitializeEquipmentStats();
        animate.InitializeValues();
    }
    public override void InitializeAwake()
    {
        ui.EquipmentUIAwake();
    }
    private void OnEnable()
    {
        fpsCam = GetComponentInParent<MouseLookScript>();
    }
    public void DeterminatesFunction()
    {
        if (ui.ui.player.playerManager.openedLoadoutMenu)
        {
            stats.interactionEnabled = false;
        }
        else if (ui.ui.player.playerManager.openedOptions)
        {
            stats.interactionEnabled = false;
        }
        else
        {
            stats.interactionEnabled = true;
        }
    }
    public void SelfDestruct()
    {
        Debug.Log("Equipment Destructed ");
        Destroy(gameObject);
    }
}
