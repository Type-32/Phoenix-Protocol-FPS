using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunAttachments : MonoBehaviour
{
    public GunManager gun;
    public GameObject attachmentsUI;
    [SerializeField] private Text barrelText;
    [SerializeField] private Text underbarrelText;
    [SerializeField] private Text sightText;

    [Space]
    public GameObject[] attachmentsArray;
    public List<GameObject> barrelAttachments = new List<GameObject>();
    public List<GameObject> underbarrelAttachments = new List<GameObject>();
    public List<GameObject> sightAttachments = new List<GameObject>();
    private GameObject nullAttachment = null;
    void Start()
    {
        //barrelAttachments[0] = nullAttachment;
        //underbarrelAttachments[0] = nullAttachment;
        //sightAttachments[0] = nullAttachment;
        UpdateDistributedAttachments();
        SelectBarrelAttachment();
        SelectUnderbarrelAttachment();
        SelectSightAttachment();
    }
    void Update()
    {
        if (!gun.stats.isAttaching)
        {
            if (attachmentsUI.activeSelf)
            {
                attachmentsUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                gun.core.enableGunCoreFunc = true;
            }
            return;
        }
        if (!attachmentsUI.activeSelf)
        {
            attachmentsUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            gun.core.enableGunCoreFunc = false;
        }
    }
    public void BarrelIndexIncrement()
    {
        if (gun.stats.selectedBarrelIndex >= barrelAttachments.Count - 1) gun.stats.selectedBarrelIndex = 0;
        else gun.stats.selectedBarrelIndex++;
        SelectBarrelAttachment();
    }
    public void UnderbarrelIndexIncrement()
    {
        if (gun.stats.selectedUnderbarrelIndex >= underbarrelAttachments.Count - 1) gun.stats.selectedUnderbarrelIndex = 0;
        else gun.stats.selectedUnderbarrelIndex++;
        SelectUnderbarrelAttachment();
    }
    public void SightIndexIncrement()
    {
        if (gun.stats.selectedSightIndex >= sightAttachments.Count - 1) gun.stats.selectedSightIndex = 0;
        else gun.stats.selectedSightIndex++;
        SelectSightAttachment();
    }
    void UpdateDistributedAttachments()
    {
        int tempBarrelIndex = 1, tempUnderbarrelIndex = 1, tempSightIndex = 1;
        for(int i = 0; i < attachmentsArray.Length; i++)
        {/*
            if (attachmentsArray[i].GetComponent<AttachmentScript>().attachmentSO.attachmentType == WeaponAttachmentSO.WeaponAttachmentTypes.Barrel)
            {
                barrelAttachments.Add(attachmentsArray[i].gameObject);
                tempBarrelIndex++;
            }else if(attachmentsArray[i].GetComponent<AttachmentScript>().attachmentSO.attachmentType == WeaponAttachmentSO.WeaponAttachmentTypes.Underbarrel)
            {
                underbarrelAttachments.Add(attachmentsArray[i].gameObject);
                tempUnderbarrelIndex++;
            }
            else if (attachmentsArray[i].GetComponent<AttachmentScript>().attachmentSO.attachmentType == WeaponAttachmentSO.WeaponAttachmentTypes.Sight)
            {
                sightAttachments.Add(attachmentsArray[i].gameObject);
                tempSightIndex++;
            }
            */
        }
    }
    void SelectBarrelAttachment()
    {
        int tmpIndex = 0;
        foreach (GameObject attachment in barrelAttachments)
        {
            if (tmpIndex == gun.stats.selectedBarrelIndex)
            {
                barrelAttachments[tmpIndex].gameObject.SetActive(true);
                //Debug.Log(barrelAttachments[tmpIndex].GetComponent<AttachmentScript>().attachmentSO.attachmentName + " attachment is enabled\n" + "Index " + tmpIndex + "\n");
            }
            else
            {
                barrelAttachments[tmpIndex].gameObject.SetActive(false);
                //Debug.Log(barrelAttachments[tmpIndex].GetComponent<AttachmentScript>().attachmentSO.attachmentName + " attachment is disabled\n" + "Index" + tmpIndex + "\n");
            }
            tmpIndex++;
        }
    }
    void SelectUnderbarrelAttachment()
    {
        int tmpIndex = 0;
        foreach (GameObject attachment in underbarrelAttachments)
        {
            if (tmpIndex == gun.stats.selectedUnderbarrelIndex)
            {
                underbarrelAttachments[tmpIndex].gameObject.SetActive(true);
                //Debug.Log(underbarrelAttachments[tmpIndex].GetComponent<AttachmentScript>().attachmentSO.attachmentName + " attachment is enabled\n" + "Index" + tmpIndex + "\n");
            }
            else
            {
                underbarrelAttachments[tmpIndex].gameObject.SetActive(false);
                //Debug.Log(underbarrelAttachments[tmpIndex].GetComponent<AttachmentScript>().attachmentSO.attachmentName + " attachment is disabled\n" + "Index" + tmpIndex + "\n");
            }
            tmpIndex++;
        }
    }
    void SelectSightAttachment()
    {
        int tmpIndex = 0;
        foreach (GameObject attachment in sightAttachments)
        {
            if (tmpIndex == gun.stats.selectedSightIndex)
            {
                sightAttachments[tmpIndex].gameObject.SetActive(true);
                //Debug.Log(sightAttachments[tmpIndex].GetComponent<AttachmentScript>().attachmentSO.attachmentName + " attachment is enabled\n" + "Index" + tmpIndex + "\n");
            }
            else
            {
                sightAttachments[tmpIndex].gameObject.SetActive(false);
                //Debug.Log(sightAttachments[tmpIndex].GetComponent<AttachmentScript>().attachmentSO.attachmentName + " attachment is disabled\n" + "Index" + tmpIndex + "\n");
            }
            tmpIndex++;
        }
        gun.core.UpdateSightFOV();
    }
}
