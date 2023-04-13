using System;
using System.Collections;
using System.Collections.Generic;
using PrototypeLib.OnlineServices.PUNMultiplayer.ConfigurationKeys;
using UnityEngine;
using UnityEngine.UI;

public enum AttachmentTypes
{
    Sight,
    Underbarrel,
    Barrel,
    Leftbarrel,
    Rightbarrel,
    Upbarrel,
    None
}
public class GunAttachments : MonoBehaviour
{
    public GunManager gun;
    public GameObject attachmentHolder;

    [Space]
    public GunAttachmentItem[] attachmentsArray;
    private Camera[] cameras;
    private RawImage[] images;
    //private List<GameObject> barrelAttachments = new List<GameObject>();
    //private List<GameObject> underbarrelAttachments = new List<GameObject>();
    //private List<GameObject> leftbarrelAttachments = new List<GameObject>();
    //private List<GameObject> rightbarrelAttachments = new List<GameObject>();
    //private List<GameObject> sightAttachments = new List<GameObject>();

    private void SwitchedItem(int index)
    {
        if(gun.player.pv.IsMine && gameObject.activeInHierarchy && index < 2) SetTrail(index);
    }

    void Start()
    {
        if (gun == null) return;
        attachmentsArray = attachmentHolder.GetComponentsInChildren<GunAttachmentItem>();
        if (!gun.player.pv.IsMine)
        {
            cameras = attachmentHolder.GetComponentsInChildren<Camera>();
            images = attachmentHolder.GetComponentsInChildren<RawImage>();
            for (int i = 0; i < cameras.Length; i++) Destroy(cameras[i].gameObject);
            for (int i = 0; i < images.Length; i++) Destroy(images[i].gameObject);
        }
        for (int i = 0; i < attachmentsArray.Length; i++) attachmentsArray[i].gameObject.SetActive(false);
        EnableGunCustomizations(gun.player.holder.weaponIndex);
        gun.player.holder.OnItemSwitch += SwitchedItem;
    }
    public void CheckEnabledSightAimingPosition(int index)
    {
        if (index == 0)
        {
            for (int i = 0; i < attachmentsArray.Length; i++)
            {
                if (attachmentsArray[i].dataGlobalIndex == (int)gun.player.pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Sight, 1)])
                {
                    gun.animate.animate.SetFloat("Blend", attachmentsArray[i].dataGlobalIndex == -1f ? 0f : attachmentsArray[i].dataGlobalIndex);
                    //gun.animate.animate.SetInteger("Blend", attachmentsArray[i].dataGlobalIndex == -1 ? 0 : attachmentsArray[i].dataGlobalIndex);
                }
            }
        }
        else
        {
            for (int i = 0; i < attachmentsArray.Length; i++)
            {
                if (attachmentsArray[i].dataGlobalIndex == (int)gun.player.pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Sight, 2)])
                {
                    gun.animate.animate.SetFloat("Blend", attachmentsArray[i].dataGlobalIndex == -1f ? 0f : attachmentsArray[i].dataGlobalIndex);
                    //gun.animate.animate.SetInteger("Blend", attachmentsArray[i].dataGlobalIndex == -1 ? 0 : attachmentsArray[i].dataGlobalIndex);
                }
            }
        }
    }
    public void EnableGunCustomizations(int selectedWeaponSlot)
    {
        int[] attachments = new int[6] { (int)gun.player.pv.Owner.CustomProperties[$"SMWA_BarrelIndex{selectedWeaponSlot + 1}"], (int)gun.player.pv.Owner.CustomProperties[$"SMWA_SightIndex{selectedWeaponSlot + 1}"], (int)gun.player.pv.Owner.CustomProperties[$"SMWA_UnderbarrelIndex{selectedWeaponSlot + 1}"], (int)gun.player.pv.Owner.CustomProperties[$"SMWA_LeftbarrelIndex{selectedWeaponSlot + 1}"], (int)gun.player.pv.Owner.CustomProperties[$"SMWA_RightbarrelIndex{selectedWeaponSlot + 1}"], (int)gun.player.pv.Owner.CustomProperties[$"SMWA_AppearanceIndex{selectedWeaponSlot + 1}"] };
        SetCustomization(attachments[0], attachments[1], attachments[2], attachments[3], attachments[4], attachments[5], selectedWeaponSlot);
    }
    public void SetCustomization(int barrel, int sight, int underbarrel, int leftbarrel, int rightbarrel, int appearance, int selectedSlot)
    {
        for (int i = 0; i < attachmentsArray.Length; i++)
        {
            if (attachmentsArray[i].dataGlobalIndex == barrel) attachmentsArray[i].gameObject.SetActive(true);
            if (attachmentsArray[i].dataGlobalIndex == sight)
            {
                attachmentsArray[i].gameObject.SetActive(true);
                if (gun != null) gun.animate.animate.SetFloat("Blend", (float)attachmentsArray[i].dataGlobalIndex == -1f ? 0f : (float)attachmentsArray[i].dataGlobalIndex);
                //gun.animate.animate.SetInteger("Blend", attachmentsArray[i].dataGlobalIndex == -1 ? 0 : attachmentsArray[i].dataGlobalIndex);
            }
            if (attachmentsArray[i].dataGlobalIndex == underbarrel) attachmentsArray[i].gameObject.SetActive(true);
            if (attachmentsArray[i].dataGlobalIndex == leftbarrel) attachmentsArray[i].gameObject.SetActive(true);
            if (attachmentsArray[i].dataGlobalIndex == rightbarrel) attachmentsArray[i].gameObject.SetActive(true);
        }
        SetTrail(selectedSlot);
    }

    private void SetTrail(int weaponIndex)
    {
        int appearance = (int)gun.player.pv.Owner.CustomProperties[$"SMWA_AppearanceIndex{weaponIndex + 1}"];
        if (appearance != -1)
        {
            if (gun != null)
            {
                Debug.LogWarning("On Set Trail Color -----------");
                if (GlobalDatabase.Instance.allWeaponAppearanceDatas[appearance].overrideDefaultTrailerColor)
                {
                    gun.player.local_trailMaterial.SetColor("_Color",
                        GlobalDatabase.Instance.allWeaponAppearanceDatas[appearance].trailColor);
                    gun.player.local_trailMaterial.SetColor("_EmissionColor",
                        GlobalDatabase.Instance.allWeaponAppearanceDatas[appearance].trailColor);
                }
                else
                {
                    gun.player.local_trailMaterial.SetColor("_Color",
                        GlobalDatabase.Instance.DefaultTrailColor);
                    gun.player.local_trailMaterial.SetColor("_EmissionColor",
                        GlobalDatabase.Instance.DefaultTrailColor);
                }
                gun.gunVisual.GetComponent<MeshFilter>().mesh = GlobalDatabase.Instance.allWeaponAppearanceDatas[appearance].mesh;
            }
        }
        else
        {
            if (gun != null){
                gun.player.local_trailMaterial.SetColor("_Color",
                    GlobalDatabase.Instance.DefaultTrailColor);
                gun.player.local_trailMaterial.SetColor("_EmissionColor",
                    GlobalDatabase.Instance.DefaultTrailColor);
            }
        }
    }
}
