using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutCustomButtonsHolder : MonoBehaviour
{
    [SerializeField] LoadoutSelectionScript loadoutSelection;
    public List<GameObject> buttons = new List<GameObject>();

    public Image sightIcon;
    public Image barrelIcon;
    public Image underbarrelIcon;
    public Image leftbarrelIcon;
    public Image rightbarrelIcon;
    public Sprite nullIcon;
    public int forSelectedSlot = 0;
    public void SetIcon(int index, Sprite sprite)
    {
        switch (index)
        {
            case 0:
                sightIcon.sprite = sprite;
                break;
            case 1:
                barrelIcon.sprite = sprite;
                break;
            case 2:
                underbarrelIcon.sprite = sprite;
                break;
            case 3:
                leftbarrelIcon.sprite = sprite;
                break;
            case 4:
                rightbarrelIcon.sprite = sprite;
                break;
        }
    }
    public Sprite FindIconFromAttachmentIndex(int index)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponAttachmentDatas.Count; i++)
        {
            if (index == i) return GlobalDatabase.singleton.allWeaponAttachmentDatas[i].attachmentIcon;
        }
        return nullIcon;
    }
    private void Start()
    {
        SetAllIcons(0);
    }
    public void SetAllIcons(int index)
    {
        SetIcon(0, FindIconFromAttachmentIndex(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSightIndex[index]));
        SetIcon(1, FindIconFromAttachmentIndex(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedBarrelIndex[index]));
        SetIcon(2, FindIconFromAttachmentIndex(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedUnderbarrelIndex[index]));
        SetIcon(3, FindIconFromAttachmentIndex(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelLeftIndex[index]));
        SetIcon(4, FindIconFromAttachmentIndex(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelRightIndex[index]));
    }
    public void OnClickButton(int index)
    {
        loadoutSelection.ToggleCustomizeSelectionUI(true);
        loadoutSelection.ToggleCustomizeButtonsUI(false);
        loadoutSelection.loadoutCustomization.AttachmentSelectionUIToggler(index, true);
        //SetAllIcons(loadoutSelection.forSelectedSlot);
    }
    public void OnClickClearButton()
    {
        SetIcon(0, nullIcon);
        SetIcon(1, nullIcon);
        SetIcon(2, nullIcon);
        SetIcon(3, nullIcon);
        SetIcon(4, nullIcon);
        loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].SetNullAttachment(GunAttachments.AttachmentTypes.Sight, loadoutSelection.forSelectedSlot);
        loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].SetNullAttachment(GunAttachments.AttachmentTypes.Barrel, loadoutSelection.forSelectedSlot);
        loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].SetNullAttachment(GunAttachments.AttachmentTypes.Sidebarrel_Right, loadoutSelection.forSelectedSlot);
        loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].SetNullAttachment(GunAttachments.AttachmentTypes.Sidebarrel_Left, loadoutSelection.forSelectedSlot);
        loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].SetNullAttachment(GunAttachments.AttachmentTypes.Sidebarrel_Up, loadoutSelection.forSelectedSlot);
        loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].SetNullAttachment(GunAttachments.AttachmentTypes.Underbarrel, loadoutSelection.forSelectedSlot);
        OnClearCustomization(loadoutSelection.forSelectedSlot);
    }
    public void OnClearCustomization(int forIndex)
    {
        loadoutSelection.loadoutPreviewUI.ClearPreviewAttachments(forIndex);
    }
}
