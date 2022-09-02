using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Photon.Pun;
using Photon.Realtime;

public class PlayerControllerManager : MonoBehaviourPunCallbacks, IDamagable
{

    [Header("Script Control")]
    public PlayerControls controls;
    public PlayerStats stats;
    public PlayerSounds sfx;
    public UIManager ui;
    public MouseLookScript cam;
    public EquipmentHolder holder;
    public IDamagable damagable;
    //public GadgetUsageScript gadgetFunc;

    [Space]
    [Header("Character Customization")]
    [SerializeField] GameObject playerHead, playerBody, playerFeet1, playerFeet2;
    [SerializeField] Material playerHeadMaterial, playerBodyMaterial, playerFeetMaterial;


    [Space]
    [Header("References")]
    public CharacterController body;
    public CapsuleCollider capsuleCollider;
    public MouseLookScript fpsCam;
    public GameObject deathCam;
    public Animator cameraAnim;
    public Recoil recoilScript;
    public Transform groundCheck;

    [Space]
    [Header("Ground Masks")]
    public LayerMask groundMask;

    [Space]
    [Header("Volume Effects")]
    [SerializeField] Volume playerVolumeEffect, playerHurtEffect, armorHurtEffect;

    [Space]
    [Header("Multiplayer")]
    public PhotonView pv;
    PlayerManager playerManager;

    private bool hasArmor = false;

    private void Awake()
    {
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
        pv = GetComponent<PhotonView>();
        //ui = FindObjectOfType<UIManager>();
        //if(playerHeadMat != null) gameObject.GetComponent<MeshRenderer>playerHeadMat
    }
    private void Start()
    {
        if (pv.IsMine)
        {
            recoilScript = FindObjectOfType<Recoil>();
            DerivePlayerStatsToHUDInitialize();
        }
        else
        {
            Destroy(ui.gameObject);
        }
    }
    private void Update()
    {
        if (!pv.IsMine) return;
        if (transform.position.y < -35) Die();
        DerivePlayerStatsToHUD();
        PlayerGUIReference();

        if (ui.openedLoadoutMenu)
        {
            stats.playerMovementEnabled = false;
            stats.mouseMovementEnabled = false;
        }else if (ui.openedOptions)
        {
            stats.playerMovementEnabled = false;
            stats.mouseMovementEnabled = false;
        }
        else
        {
            stats.playerMovementEnabled = true;
            stats.mouseMovementEnabled = true;
        }
    }
    public void GetPickupsForPlayer()
    {
        RaycastHit detectRay;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out detectRay, 3f))
        {
            Pickup temp = detectRay.collider.GetComponent<Pickup>();
            if(temp != null && temp.itemData != null)
            {
                
            }
        }
    }
    public void SetMouseSensitivity(float value)
    {
        stats.mouseSensitivity = value;
        controls.aimingMouseSensitivity = stats.mouseSensitivity * 0.8f;
    }
    public void TakeDamage(float amount, bool bypassArmor)
    {
        pv.RPC("RPC_TakeDamage", RpcTarget.All, amount, bypassArmor);//Running the function on everyone's computer
        //ui.ShowHealthBar(2f);
        
    }
    
    #region Body Materials
    public void SetBodyMaterialColor(Color color)
    {
        playerBodyMaterial.color = color;
    }
    public void SetFeetMaterialColor(Color color)
    {
        playerFeetMaterial.color = color;
    }
    public void SetHeadMaterialColor(Color color)
    {
        playerHeadMaterial.color = color;
    }
    #endregion
    
    private void TakeHitEffect()
    {

    }
    private void DerivePlayerStatsToHUD()
    {
        if (ui == null) return;
        ui.healthBar.value = Mathf.Lerp(ui.healthBar.value, stats.health, 8 * Time.deltaTime);
        //ui.healthBarFill.color = Color.Lerp(ui.healthBarFill.color, (stats.health >= 50f) ? Color.green : (stats.health < 50f && stats.health >= 30f) ? Color.yellow : Color.red, 5 * Time.deltaTime);
        //ui.healthText.text = ((int)stats.health).ToString();
        ui.armorBar.value = Mathf.Lerp(ui.armorBar.value, stats.armor, 8 * Time.deltaTime);
        playerVolumeEffect.weight = 1f - (stats.health / 100f);
        playerHurtEffect.weight = Mathf.Lerp(playerHurtEffect.weight, 0f, 8 * Time.deltaTime);
        armorHurtEffect.weight = Mathf.Lerp(armorHurtEffect.weight, 0f, 8 * Time.deltaTime);
    }
    private void DerivePlayerStatsToHUDInitialize()
    {
        if (ui == null) return;
        ui.healthBar.maxValue = stats.healthLimit;
        ui.armorBar.maxValue = stats.armorLimit;
        stats.stress = 0;
    }
    private void PlayerGUIReference()
    {
        if (ui == null) return;
        if (ui.openedInventory) { stats.mouseMovementEnabled = false; stats.playerMovementEnabled = false; }
        else { stats.mouseMovementEnabled = true; stats.playerMovementEnabled = true; }
    }
    public void Die()
    {
        playerManager.Die();
        Debug.Log("Player " + stats.playerName + " was Killed");
        return;
    }
    [PunRPC]
    void RPC_TakeDamage(float amount, bool bypassArmor)
    {
        if(!pv.IsMine) return; //PV Check to make sure it only runs on victim's computer
        Debug.Log("Took Damage " + amount);

        //Core Take Damage Functions
        recoilScript.RecoilFire(0.4f, 0.8f, 4, 0.12f, 0, 5, 12, 5, 12);
        if (bypassArmor)
        {
            stats.health -= amount;
            playerHurtEffect.weight = 1f;
            sfx.InvokePlayerHurtAudio();
        }
        else
        {
            if (stats.armor - amount <= 0)
            {
                if (hasArmor) sfx.InvokeArmorDamagedAudio();
                hasArmor = false;
                if (stats.armor - amount < 0)
                {
                    float temp = stats.armor - amount;
                    stats.armor = 0f;
                    stats.health += temp;
                }
                else
                {
                    stats.armor = 0f;
                }
                playerHurtEffect.weight = 1f;
            }
            else
            {
                stats.armor -= amount;
                hasArmor = true;
                armorHurtEffect.weight = 1f;
                sfx.InvokePlayerHurtAudio();
            }

        }
        stats.totalAbsorbedDamage += amount;
        if (stats.health <= 0f)
        {
            Die();
            return;
        }
    }
}
