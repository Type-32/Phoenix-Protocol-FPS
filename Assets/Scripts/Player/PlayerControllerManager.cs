using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
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
    public GameObject playerDeathEffect;
    //public Camera cameraView;

    [Space]
    [Header("Ground Masks")]
    public LayerMask groundMask;

    [Space]
    [Header("Volume Effects")]
    [SerializeField] Volume playerVolumeEffect, playerHurtEffect, armorHurtEffect, nightVisionEffect;

    [Space]
    [Header("Multiplayer")]
    public PhotonView pv;
    public PlayerManager playerManager;
    public List<GameObject> playerDeathLoots = new();

    private bool hasArmor = false;
    private float timePassedAfterDamageTaken = 5f;
    public bool usingStreakGifts = false;
    public GameObject playerMinimapDot;
    public List<GameObject> allMinimapDots = new();
    public bool hidePlayerHUD = false;

    [SerializeField] int weaponIndex1;
    [SerializeField] int weaponIndex2;

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
            weaponIndex1 = (int)pv.Owner.CustomProperties["selectedMainWeaponIndex"];
            weaponIndex2 = (int)pv.Owner.CustomProperties["selectedSecondWeaponIndex"];
            recoilScript = FindObjectOfType<Recoil>();
            DerivePlayerStatsToHUDInitialize();
            playerHead.SetActive(false);
            playerBody.SetActive(false);
            playerFeet1.SetActive(false);
            playerFeet2.SetActive(false);
            nightVisionEffect.gameObject.SetActive(stats.enableNightVision);
            MinimapDotIdentifier[] tempget;
            tempget = FindObjectsOfType<MinimapDotIdentifier>();
            for (int i = 0; i < tempget.Length; i++)
            {
                allMinimapDots.Add(tempget[i].gameObject);
            }
            DisableAllMinimapDots();
            playerMinimapDot.SetActive(true);
            ui.hud.GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            weaponIndex1 = (int)pv.Owner.CustomProperties["selectedMainWeaponIndex"];
            weaponIndex2 = (int)pv.Owner.CustomProperties["selectedSecondWeaponIndex"];
            Destroy(ui.gameObject);
            playerHead.SetActive(true);
            playerBody.SetActive(true);
            playerFeet1.SetActive(true);
            playerFeet2.SetActive(true);
            nightVisionEffect.gameObject.SetActive(stats.enableNightVision);
            playerMinimapDot.SetActive(false);
        }
    }
    private void Update()
    {
        if (!pv.IsMine) return;
        if (transform.position.y < -35) Die();
        DerivePlayerStatsToHUD();
        PlayerGUIReference();
        CrosshairNametagDetect();
        if (Input.GetKeyDown("l"))
        {
            if (hidePlayerHUD)
            {
                hidePlayerHUD = false;
                ui.hud.GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                hidePlayerHUD = true;
                ui.hud.GetComponent<CanvasGroup>().alpha = 0;
            }
        }
        if (playerManager.recordKills >= 3)
        {
            ui.streakBackground.value = Mathf.Lerp(ui.streakBackground.value, 1f, Time.deltaTime * 3);
            ui.streakHUDAlpha.alpha = Mathf.Lerp(ui.streakHUDAlpha.alpha, 1f, Time.deltaTime * 3);
        }
        else
        {
            ui.streakBackground.value = Mathf.Lerp(ui.streakBackground.value, 0f, Time.deltaTime * 3);
            ui.streakHUDAlpha.alpha = Mathf.Lerp(ui.streakHUDAlpha.alpha, 0.2f, Time.deltaTime * 3);
        }

        if (timePassedAfterDamageTaken < 5f) timePassedAfterDamageTaken += Time.deltaTime;
        else
        {
            if(stats.health < stats.healthLimit) stats.health += 1f;
        }

        if (playerManager.openedLoadoutMenu)
        {
            stats.playerMovementEnabled = false;
            stats.mouseMovementEnabled = false;
        }else if (playerManager.openedOptions)
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
    public void ChangePlayerHitbox(Vector3 center, float radius, float height)
    {
        pv.RPC(nameof(RPC_ChangePlayerHitbox), RpcTarget.All, center, radius, height);
    }
    public void EnableAllMinimapDots()
    {
        MinimapDotIdentifier[] tempget;
        tempget = FindObjectsOfType<MinimapDotIdentifier>();
        for (int i = 0; i < tempget.Length; i++)
        {
            tempget[i].gameObject.SetActive(true);
        }
        //OperateAllMinimapDots(false);
        playerMinimapDot.SetActive(true);
    }
    public void DisableAllMinimapDots()
    {
        MinimapDotIdentifier[] tempget;
        tempget = FindObjectsOfType<MinimapDotIdentifier>();
        for (int i = 0; i < tempget.Length; i++)
        {
            tempget[i].gameObject.SetActive(false);
        }
        //OperateAllMinimapDots(false);
        playerMinimapDot.SetActive(true);
    }
    public void SetMouseSensitivity(float value)
    {
        stats.mouseSensitivity = value;
        controls.aimingMouseSensitivity = stats.mouseSensitivity * 0.8f;
    }

    public bool TakeDamage(float amount, bool bypassArmor, Vector3 targetPos, Quaternion targetRot)
    {
        bool tempflag = false;
        stats.health -= amount;
        pv.RPC(nameof(RPC_TakeDamage), pv.Owner, amount, bypassArmor, targetPos, targetRot);
        /*
        if (stats.health <= 0)//Lag compensation here for visual player death lag, but it didn't work
        {
            tempflag = true;
            ClientFakeDeath();
            InvokePlayerDeathEffects();
        }*/
        //if (stats.health - amount <= 0) Destroy(gameObject);
        return tempflag;
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
    public IEnumerator UseStreakGift(float duration, int cost)
    {
        EnableAllMinimapDots();
        usingStreakGifts = true;
        playerManager.recordKills -= cost;
        yield return new WaitForSeconds(duration);
        usingStreakGifts = false;
        DisableAllMinimapDots();
    }
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
        ui.healthText.text = ((int)stats.health).ToString();
        ui.armorText.text = ((int)stats.armor).ToString();
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
    private void CrosshairNametagDetect()
    {
        RaycastHit hit;
        if(Physics.Raycast(holder.transform.position, holder.transform.forward, out hit, stats.playerNametagDistance))
        {
            PhotonView _pv = hit.collider.GetComponent<PhotonView>();
            if(_pv != null)
            {
                ui.nametagIndicatorObject.SetActive(true);
                ui.nametagIndicator.text = _pv.Owner.NickName;
            }
            else
            {
                ui.nametagIndicatorObject.SetActive(false);
            }
        }
        else
        {
            ui.nametagIndicatorObject.SetActive(false);
        }
    }
    private void PlayerGUIReference()
    {
        if (ui == null) return;
        if (playerManager.openedInventory) { stats.mouseMovementEnabled = false; stats.playerMovementEnabled = false; }
        else { stats.mouseMovementEnabled = true; stats.playerMovementEnabled = true; }
    }
    public void ClientFakeDeath()
    {
        capsuleCollider.enabled = false;
        playerHead.SetActive(false);
        playerBody.SetActive(false);
        playerFeet1.SetActive(false);
        playerFeet2.SetActive(false);
        gameObject.SetActive(false);
    }
    public void SpawnDeathLoot()
    {
        if (playerDeathLoots.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, playerDeathLoots.Count - 1);
            pv.RPC(nameof(RPC_SpawnDeathLoot), RpcTarget.All, transform.position, randomIndex);
        }
    }
    public void Die()
    {

        InvokePlayerDeathEffects();
        SpawnDeathLoot();
        DisableAllMinimapDots();
        usingStreakGifts = false;
        playerManager.Die();
        Debug.Log("Player " + stats.playerName + " was Killed");
        return;
    }
    public void ToggleNightVision()
    {
        if (stats.enableNightVision)
        {
            stats.enableNightVision = false;
        }
        else
        {
            stats.enableNightVision = true;
        }
        nightVisionEffect.gameObject.SetActive(stats.enableNightVision);
    }
    [PunRPC]
    void RPC_ChangePlayerHitbox(Vector3 center, float radius, float height)
    {
        body.center = capsuleCollider.center = center;
        body.radius = capsuleCollider.radius = radius;
        body.height = capsuleCollider.height = height;
    }
    [PunRPC]
    void RPC_SpawnDeathLoot(Vector3 pos, int randomIndex)
    {
        pos.y += 1.5f;
        Instantiate(playerDeathLoots[randomIndex], pos, transform.rotation);
    }
    Transform tempTransform;
    [PunRPC]
    void RPC_TakeDamage(float amount, bool bypassArmor, Vector3 targetPos, Quaternion targetRot, PhotonMessageInfo info)
    {
        Debug.Log("Took Damage " + amount + " from " + info.Sender.NickName + " using " + ((int)info.Sender.CustomProperties["weaponIndex"] == 0 ? (int)info.Sender.CustomProperties["selectedMainWeaponIndex"] : (int)info.Sender.CustomProperties["selectedSecondWeaponIndex"]).ToString());
        //tempTransform.position = transform.position;
        //tempTransform.rotation = transform.rotation;
        //tempTransform.position = targetPos;
        //tempTransform.rotation = targetRot;
        UIManager.ValueTransform tmp;
        tmp.position = targetPos;
        tmp.rotation = targetRot;
        UIManager.CreateIndicator(tmp);
        //Core Take Damage Functions
        recoilScript.RecoilFire(0.4f, 0.8f, 4, 0.12f, 0, 5, 12, 5, 12);
        if (bypassArmor)
        {
            stats.health -= amount;
            timePassedAfterDamageTaken = 0f;
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
                    timePassedAfterDamageTaken = 0f;
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
            int tm = (int)info.Sender.CustomProperties["weaponIndex"] == 0 ? (int)info.Sender.CustomProperties["selectedMainWeaponIndex"] : (int)info.Sender.CustomProperties["selectedSecondWeaponIndex"];
            PlayerManager.Find(info.Sender).GetKill(pv.Owner.NickName, tm);
            Die();
        }
        return;
    }
    public void CallShootRPCDecals(RaycastHit hit)
    {
        //Debug.LogWarning("Invoking Shoot RPC...");
        pv.RPC(nameof(RPC_Shoot), RpcTarget.All, hit.point, hit.normal);
    }
    public void InvokeGunEffects()
    {
        //Debug.LogWarning("Invoking Gun Effects RPC...");
        /*
        if (holder.weaponIndex == 0)
        {
            if ((int)pv.Owner.CustomProperties["SMWA_BarrelIndex1"] == -1) holder.weaponSlots[holder.weaponIndex].gun.audio.PlayGunSound(false);
            else holder.weaponSlots[holder.weaponIndex].gun.audio.PlayGunSound(true);
        }
        else
        {
            if ((int)pv.Owner.CustomProperties["SMWA_BarrelIndex2"] == -1) holder.weaponSlots[holder.weaponIndex].gun.audio.PlayGunSound(false);
            else holder.weaponSlots[holder.weaponIndex].gun.audio.PlayGunSound(true);
        }

        if (holder.weaponIndex == 0)
        {
            if ((int)pv.Owner.CustomProperties["SMWA_BarrelIndex1"] == -1) holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.Play();
        }
        else
        {
            if ((int)pv.Owner.CustomProperties["SMWA_BarrelIndex2"] == -1) holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.Play();
        }*/
        pv.RPC(nameof(RPC_InvokeGunEffects), RpcTarget.All, pv.ViewID);
    }
    public void InvokePlayerDeathEffects()
    {
        //Debug.LogWarning("Invoking PLayer Death Effects RPC...");
        pv.RPC(nameof(RPC_InvokePlayerDeathEffects), RpcTarget.All);
    }
    public void SynchronizePlayerState(bool value, int stateIndex)
    {
        pv.RPC(nameof(RPC_ChangePlayerState), RpcTarget.All, value, stateIndex);
    }
    [PunRPC]
    public void RPC_ChangePlayerState(bool value, int ind)
    {
        /*
       0. Sprinting
       1. Crouching
       2. Sliding
         */
        switch (ind)
        {
            case 0:
                stats.isSprinting = value;
                break;
            case 1:
                stats.isCrouching = value;
                break;
            case 2:
                stats.isSliding = value;
                break;
            case 3:
                stats.isWalking = value;
                break;
            default:
                break;
        }
    }
    [PunRPC]
    public void RPC_InvokePlayerDeathEffects()
    {
        GameObject obj = Instantiate(playerDeathEffect, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation);
        Destroy(obj, 5f);
    }
    [PunRPC]
    public void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        //Debug.LogWarning("Finding Colliders...");
        if (colliders.Length != 0)
        {
            //Debug.LogWarning("Colliders Found");
            GameObject bulletImpactObject = Instantiate(holder.weaponSlots[holder.weaponIndex].bulletImpactPrefab, hitPosition + hitNormal * 0.01f, Quaternion.LookRotation(-hitNormal, Vector3.up));
            Destroy(bulletImpactObject, 5f);
            bulletImpactObject.transform.SetParent(colliders[0].transform);
            if (colliders[0].GetComponent<PlayerControllerManager>() != null) Destroy(bulletImpactObject);
            //bulletImpactObject.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x,transform.rotation.y, Random.Range(0f, 90f)));
        }
    }
    [PunRPC]
    public void RPC_InvokeGunEffects(int viewID)
    {
        if (pv.ViewID != viewID) return;
        if (holder.weaponIndex == 0)
        {
            if ((int)pv.Owner.CustomProperties["SMWA_BarrelIndex1"] == -1) holder.weaponSlots[holder.weaponIndex].gun.audio.PlayGunSound(false);
            else holder.weaponSlots[holder.weaponIndex].gun.audio.PlayGunSound(true);
        }
        else
        {
            if ((int)pv.Owner.CustomProperties["SMWA_BarrelIndex2"] == -1) holder.weaponSlots[holder.weaponIndex].gun.audio.PlayGunSound(false);
            else holder.weaponSlots[holder.weaponIndex].gun.audio.PlayGunSound(true);
        }

        if (holder.weaponIndex == 0)
        {
            if ((int)pv.Owner.CustomProperties["SMWA_BarrelIndex1"] == -1) holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.Play();
        }
        else
        {
            if ((int)pv.Owner.CustomProperties["SMWA_BarrelIndex2"] == -1) holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.Play();
        }
    }
}
