using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.MUIP;

public class LoadoutPreviewUI : MonoBehaviour
{
    //public Button[] buttons;
    [SerializeField] LoadoutSelectionScript loadoutSelection;
    public Text[] texts;
    public Image[] images;
    public ButtonManager[] buttons;
    public Transform[] attachPreviewHolders;
    public GameObject attachPreviewPrefab;
    public Image customizeWeaponIcon;
    public Sprite nullWeaponIcon;

    public void SetWeaponSlotInfo(int index, WeaponData weaponData)
    {
        Debug.Log("Setting WeaponSlot Preview");
        string temp = "";
        switch (index)
        {
            case 0:
                temp = "PRIMARY - ";
                break;
            case 1:
                temp = "SECONDARY - ";
                break;
        }
        if (weaponData != null)
        {
            texts[index <= 1 ? index : 0].text = temp + weaponData.itemName;
            images[index <= 1 ? index : 0].sprite = weaponData.itemIcon;
            buttons[index <= 1 ? index : 0].gameObject.SetActive(true);
            attachPreviewHolders[index <= 1 ? index : 0].gameObject.SetActive(true);
            loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[index] = weaponData;
            if (index <= 1)
            {
                if (index == 0)
                {
                    loadoutSelection.selectedMainWeaponIndex = Launcher.Instance.FindGlobalWeaponIndex(weaponData);
                }
                else
                {
                    loadoutSelection.selectedSecondWeaponIndex = Launcher.Instance.FindGlobalWeaponIndex(weaponData);
                }
            }
        }
        else
        {
            texts[index <= 1 ? index : 0].text = temp + "None";
            images[index <= 1 ? index : 0].sprite = nullWeaponIcon;
            buttons[index <= 1 ? index : 0].gameObject.SetActive(false);
            //attachPreviewHolders[index <= 1 ? index : 0].gameObject.SetActive(false);
            loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[index] = null;
        }
        SetPreviewAttachments(index, weaponData);
        loadoutSelection.WriteLoadoutDataToJSON();
    }
    public void SetPreviewAttachments(int index, WeaponData weaponData)
    {
        Debug.Log("Setting Attachments Preview");
        int tmp = (index <= 1 ? index : 0);
        Transform[] ar = attachPreviewHolders[tmp].GetComponentsInChildren<Transform>();
        for (int i = 0; i < ar.Length; i++)
        {
            if (ar[i] != attachPreviewHolders[tmp])
            {
                Destroy(ar[i].gameObject);
            }
        }
        if (loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedBarrelIndex[tmp] != -1)
        {
            LAPreview i = Instantiate(attachPreviewPrefab, attachPreviewHolders[tmp]).GetComponentInChildren<LAPreview>();
            i.SetIcon(FindAttachmentIcon(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedBarrelIndex[tmp]));
        }
        if (loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelLeftIndex[tmp] != -1)
        {
            LAPreview i = Instantiate(attachPreviewPrefab, attachPreviewHolders[tmp]).GetComponentInChildren<LAPreview>();
            i.SetIcon(FindAttachmentIcon(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelLeftIndex[tmp]));
        }
        if (loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelRightIndex[tmp] != -1)
        {
            LAPreview i = Instantiate(attachPreviewPrefab, attachPreviewHolders[tmp]).GetComponentInChildren<LAPreview>();
            i.SetIcon(FindAttachmentIcon(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelRightIndex[tmp]));
        }
        if (loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSightIndex[tmp] != -1)
        {
            LAPreview i = Instantiate(attachPreviewPrefab, attachPreviewHolders[tmp]).GetComponentInChildren<LAPreview>();
            i.SetIcon(FindAttachmentIcon(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSightIndex[tmp]));
        }
        if (loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedUnderbarrelIndex[tmp] != -1)
        {
            LAPreview i = Instantiate(attachPreviewPrefab, attachPreviewHolders[tmp]).GetComponentInChildren<LAPreview>();
            i.SetIcon(FindAttachmentIcon(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedUnderbarrelIndex[tmp]));
        }
    }
    public void SetPreviewInfo(LoadoutData data)
    {
        SetWeaponSlotInfo(0, data.weaponData[0]);
        SetWeaponSlotInfo(1, data.weaponData[1]);
    }
    public void SetSelectionMenuSlotIndex(int index)
    {
        loadoutSelection.forSelectedSlot = index;
    }
    Sprite FindAttachmentIcon(int index)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponAttachmentDatas.Count; i++)
        {
            if (i == index) return GlobalDatabase.singleton.allWeaponAttachmentDatas[i].attachmentIcon;
        }
        return null;
    }
    public void OnCustomizeButtonPress(int index)
    {
        SetSelectionMenuSlotIndex(index);
        loadoutSelection.customButtonsHolder.SetAllIcons(loadoutSelection.forSelectedSlot);
        loadoutSelection.loadoutCustomization.RemoveAttachmentUIItems();
        loadoutSelection.loadoutCustomization.AttachmentUIItemInstantiation();
        loadoutSelection.ToggleCustomizationMenu(true);
        loadoutSelection.ToggleCustomizeButtonsUI(true);
        //loadoutSelection.DisablePreview();
        //loadoutSelection.CloseLoadoutButtonsVisual();
        customizeWeaponIcon.sprite = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[index].itemIcon;
    }
    public void QuitCustomizationUI()
    {
        loadoutSelection.ToggleCustomizationMenu(false);
        loadoutSelection.ToggleCustomizeButtonsUI(false);
        loadoutSelection.EnablePreview();
        //loadoutSelection.OpenLoadoutButtonsVisual();
        SetPreviewInfo(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex]);
    }
}
