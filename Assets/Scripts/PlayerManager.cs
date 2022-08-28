using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerManager : MonoBehaviour
{

    [Header("Script Control")]
    public PlayerControls controls;
    public PlayerStats stats;
    public PlayerSounds sfx;
    public UIManager ui;
    public MouseLookScript cam;
    public EquipmentHolder holder;
    //public GadgetUsageScript gadgetFunc;

    [Space]
    [Header("Character Customization")]
    public GameObject playerHead;
    public GameObject playerBody;
    public GameObject playerFeet1, playerFeet2;
    public Material playerHeadMaterial, playerBodyMaterial, playerFeetMaterial;


    [Space]
    [Header("References")]
    public CharacterController body;
    public CapsuleCollider capsuleCollider;
    public GameObject fpsCam;
    public GameObject deathCam;
    public Animator cameraAnim;
    public Recoil recoilScript;
    public Transform groundCheck;

    [Space]
    [Header("Ground Masks")]
    public LayerMask groundMask;

    [Space]
    [Header("Volume Effects")]
    public Volume playerVolumeEffect, playerHurtEffect, armorHurtEffect;

    private bool hasArmor = false;

    private void Awake()
    {
        ui = FindObjectOfType<UIManager>();
        //if(playerHeadMat != null) gameObject.GetComponent<MeshRenderer>playerHeadMat
    }
    private void Start()
    {
        recoilScript = FindObjectOfType<Recoil>();
        DerivePlayerStatsToHUDInitialize();

        for(int i = 0; i < ui.loadoutMenu.slotHolderScript.slotWeaponData.Length; i++)
        {
            holder.InstantiateWeapon(ui.loadoutMenu.slotHolderScript.slotWeaponData[i]);
        }
    }
    private void Update()
    {
        if (transform.position.y < -35 || Input.GetKeyDown("i")) TakeDamageFromPlayer(5, false);
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
    public void TakeDamageFromPlayer(float amount, bool bypassArmor)
    {
        //ui.ShowHealthBar(2f);
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
                if(hasArmor) sfx.InvokeArmorDamagedAudio();
                hasArmor = false;
                if(stats.armor - amount < 0)
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

    private void TakeHitEffect()
    {

    }
    public void Die()
    {
        Debug.Log("Death Here ");
        GameObject tmp = Instantiate(deathCam, fpsCam.transform.position, fpsCam.transform.rotation);
        tmp.gameObject.GetComponentInChildren<Camera>().fieldOfView = fpsCam.GetComponent<Camera>().fieldOfView;
        //tmp.gameObject.GetComponent<Rigidbody>().velocity = body.velocity;
        //ui.anim.SetTrigger("PlayerKilled");
        Cursor.lockState = CursorLockMode.None;
        GameManager.instance.RemovePlayer(gameObject);
        return;
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
}
