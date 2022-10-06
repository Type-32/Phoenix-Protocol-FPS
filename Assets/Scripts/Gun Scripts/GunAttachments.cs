using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public enum AttachmentTypes
    {
        Sight,
        Underbarrel,
        Barrel,
        Sidebarrel_Left,
        Sidebarrel_Right,
        Sidebarrel_Up,
        None
    }

    void Start()
    {
        attachmentsArray = attachmentHolder.GetComponentsInChildren<GunAttachmentItem>();
        if (!gun.player.pv.IsMine)
        {
            cameras = attachmentHolder.GetComponentsInChildren<Camera>();
            images = attachmentHolder.GetComponentsInChildren<RawImage>();
            for (int i = 0; i < cameras.Length; i++) Destroy(cameras[i].gameObject);
            for (int i = 0; i < images.Length; i++) Destroy(images[i].gameObject);
        }
        for (int i = 0; i < attachmentsArray.Length; i++) attachmentsArray[i].gameObject.SetActive(false);
        EnableGunAttachments(gun.player.holder.weaponIndex);
    }
    public void CheckEnabledSightAimingPosition(int index)
    {
        if(index == 0)
        {
            for (int i = 0; i < attachmentsArray.Length; i++)
            {
                if (attachmentsArray[i].dataGlobalIndex == (int)gun.player.pv.Owner.CustomProperties["SMWA_SightIndex1"])
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
                if (attachmentsArray[i].dataGlobalIndex == (int)gun.player.pv.Owner.CustomProperties["SMWA_SightIndex2"])
                {
                    gun.animate.animate.SetFloat("Blend", attachmentsArray[i].dataGlobalIndex == -1f ? 0f : attachmentsArray[i].dataGlobalIndex);
                    //gun.animate.animate.SetInteger("Blend", attachmentsArray[i].dataGlobalIndex == -1 ? 0 : attachmentsArray[i].dataGlobalIndex);
                }
            }
        }
    }
    public void EnableGunAttachments(int selectedWeaponSlot)
    {
        if(selectedWeaponSlot == 0)
        {
            for (int i = 0; i < attachmentsArray.Length; i++)
            {
                if (attachmentsArray[i].dataGlobalIndex == (int)gun.player.pv.Owner.CustomProperties["SMWA_BarrelIndex1"]) attachmentsArray[i].gameObject.SetActive(true);
                if (attachmentsArray[i].dataGlobalIndex == (int)gun.player.pv.Owner.CustomProperties["SMWA_SightIndex1"])
                {
                    attachmentsArray[i].gameObject.SetActive(true);
                    gun.animate.animate.SetFloat("Blend", (float)attachmentsArray[i].dataGlobalIndex == -1f ? 0f : (float)attachmentsArray[i].dataGlobalIndex);
                    //gun.animate.animate.SetInteger("Blend", attachmentsArray[i].dataGlobalIndex == -1 ? 0 : attachmentsArray[i].dataGlobalIndex);
                }
                if (attachmentsArray[i].dataGlobalIndex == (int)gun.player.pv.Owner.CustomProperties["SMWA_UnderbarrelIndex1"]) attachmentsArray[i].gameObject.SetActive(true);
                if (attachmentsArray[i].dataGlobalIndex == (int)gun.player.pv.Owner.CustomProperties["SMWA_LeftbarrelIndex1"]) attachmentsArray[i].gameObject.SetActive(true);
                if (attachmentsArray[i].dataGlobalIndex == (int)gun.player.pv.Owner.CustomProperties["SMWA_RightbarrelIndex1"]) attachmentsArray[i].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < attachmentsArray.Length; i++)
            {
                if (attachmentsArray[i].dataGlobalIndex == (int)gun.player.pv.Owner.CustomProperties["SMWA_BarrelIndex2"]) attachmentsArray[i].gameObject.SetActive(true);
                if (attachmentsArray[i].dataGlobalIndex == (int)gun.player.pv.Owner.CustomProperties["SMWA_SightIndex2"])
                {
                    attachmentsArray[i].gameObject.SetActive(true);
                    gun.animate.animate.SetFloat("Blend", (float)attachmentsArray[i].dataGlobalIndex == -1f ? 0f : (float)attachmentsArray[i].dataGlobalIndex);
                    //gun.animate.animate.SetInteger("Blend", attachmentsArray[i].dataGlobalIndex == -1 ? 0 : attachmentsArray[i].dataGlobalIndex);
                }
                if (attachmentsArray[i].dataGlobalIndex == (int)gun.player.pv.Owner.CustomProperties["SMWA_UnderbarrelIndex2"]) attachmentsArray[i].gameObject.SetActive(true);
                if (attachmentsArray[i].dataGlobalIndex == (int)gun.player.pv.Owner.CustomProperties["SMWA_LeftbarrelIndex2"]) attachmentsArray[i].gameObject.SetActive(true);
                if (attachmentsArray[i].dataGlobalIndex == (int)gun.player.pv.Owner.CustomProperties["SMWA_RightbarrelIndex2"]) attachmentsArray[i].gameObject.SetActive(true);
            }
        }
    }
}
