using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFunction : MonoBehaviour
{
    public ItemManager item;
    public PlayerManager player;
    private float aimAlpha = 0f;
    private float fireAlpha = 0f;
    private float runAlpha = 0f;
    [HideInInspector] public int ammo = 0;
    [HideInInspector] public int[] magArray;
    private int maxAmmo = 0;
    private int magCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerManager>();
        ammo = item.stats.itemData.maxAmmoPerMag;
        maxAmmo = item.stats.itemData.maxAmmoPerMag;
        magCount = item.stats.itemData.magazineCount;
        for (int i = 0; i < magCount; i++)
        {
            magArray[i] = maxAmmo;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (item.stats.itemData.itemType != ItemData.ItemType.GunItem) return;
        if (item.stats.itemData.gunType == ItemData.GunType.Pistol)
        {
            PistolTypeFunc();
        }
    }
    void AnimationFunc()
    {
        //if (fireAlpha < 1f) fireAlpha += 0.1f;
        if (fireAlpha > 0f) fireAlpha -= 0.1f;
        if (fireAlpha < 0f) fireAlpha = 0f;
        if (fireAlpha > 1f) fireAlpha = 1f;
        if (runAlpha < 0f) runAlpha = 0f;
        if (runAlpha > 1f) runAlpha = 1f;

        item.anim.animate.SetFloat("fireAlpha", fireAlpha);
        item.anim.animate.SetFloat("runAlpha", runAlpha);
        item.anim.animate.SetBool("isAiming", player.stats.isAiming);
        item.anim.animate.SetBool("isRunning", player.stats.isSprinting);
    }
    void PistolTypeFunc()
    {
        AnimationFunc();
        if(Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }
    void Fire()
    {
        Debug.Log(item.stats.itemData.itemName + " Fired by player ");
        fireAlpha += 0.9f;
        item.anim.animate.Play("Fire");
    }
}
