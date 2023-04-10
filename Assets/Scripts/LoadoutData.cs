using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loadout Data", menuName = "Project Phoenix/Loadouts/New Loadout Data", order = 1)]
public class LoadoutData : ScriptableObject
{
    public string loadoutName = "Custom Loadout";
    public int loadoutIndex = 0;
    public WeaponData[] weaponData = new WeaponData[2];
    public EquipmentData[] equipmentData = new EquipmentData[2];

    public WeaponAttachmentData[] selectedBarrel;
    public int[] selectedBarrelIndex = { 0, 0 };

    public WeaponAttachmentData[] selectedSight;
    public int[] selectedSightIndex = { 0, 0 };

    public WeaponAttachmentData[] selectedUnderbarrel;
    public int[] selectedUnderbarrelIndex = { 0, 0 };

    public WeaponAttachmentData[] selectedSidebarrelLeft;
    public int[] selectedSidebarrelLeftIndex = { 0, 0 };

    public WeaponAttachmentData[] selectedSidebarrelRight;
    public int[] selectedSidebarrelRightIndex = { 0, 0 };

    public WeaponAppearanceMeshData[] selectedAppearanceData;
    public int[] selectedAppearanceDataIndex = { -1, -1 };

    public struct BarrelSelection
    {
        WeaponAttachmentData object1;
        WeaponAttachmentData object2;
        public bool CheckExistence(int index)
        {
            switch (index)
            {
                case 0:
                    if (object1 == null) return false;
                    break;
                case 1:
                    if (object2 == null) return false;
                    break;
            }
            return true;
        }
    }

    public struct UnderbarrelSelection
    {
        WeaponAttachmentData object1;
        WeaponAttachmentData object2;
        public bool CheckExistence(int index)
        {
            switch (index)
            {
                case 0:
                    if (object1 == null) return false;
                    break;
                case 1:
                    if (object2 == null) return false;
                    break;
            }
            return true;
        }
    }
    public struct SightSelection
    {
        WeaponAttachmentData object1;
        WeaponAttachmentData object2;
        public bool CheckExistence(int index)
        {
            switch (index)
            {
                case 0:
                    if (object1 == null) return false;
                    break;
                case 1:
                    if (object2 == null) return false;
                    break;
            }
            return true;
        }
    }

    public struct SidebarrelSelection
    {
        SidebarrelData object1;
        SidebarrelData object2;
        public bool CheckDataExistence(int index)
        {
            if (index == 0)
            {
                for (int i = 0; i <= 2; i++)
                {
                    bool check = false;
                    check = object1.CheckExistence(i);
                    if (check) return true;
                }
            }
            else
            {
                for (int i = 0; i <= 2; i++)
                {
                    bool check = false;
                    check = object2.CheckExistence(i);
                    if (check) return true;
                }
            }
            return false;
        }
    }
    struct SidebarrelData
    {
        WeaponAttachmentData left;
        WeaponAttachmentData right;
        public bool CheckExistence(int index)
        {
            switch (index)
            {
                case 0:
                    if (left == null) return false;
                    break;
                case 1:
                    if (right == null) return false;
                    break;
            }
            return true;
        }
    }

    public bool isDefault = false;
    public bool allowMainWeaponsOnBothSlots = false;

    public void InitializeArrayData()
    {
        for (int i = 0; i < 2; i++)
        {
            selectedBarrel.SetValue(null, i);
            selectedSight.SetValue(null, i);
            selectedUnderbarrel.SetValue(null, i);
            selectedSidebarrelLeft.SetValue(null, i);
            selectedSidebarrelRight.SetValue(null, i);
            selectedAppearanceData.SetValue(null, i);

            selectedBarrelIndex.SetValue(-1, i);
            selectedSightIndex.SetValue(-1, i);
            selectedUnderbarrelIndex.SetValue(-1, i);
            selectedSidebarrelLeftIndex.SetValue(-1, i);
            selectedSidebarrelRightIndex.SetValue(-1, i);
            selectedAppearanceDataIndex.SetValue(-1, i);

            weaponData.SetValue(null, i);
        }
    }


    public void SetWeaponSlot(WeaponData data, int slotIndex)
    {
        weaponData[slotIndex] = data;
    }
    public void SetAttachment(WeaponAttachmentData data, AttachmentTypes type, int slotIndex)
    {
        switch (type)
        {
            case AttachmentTypes.Sight:
                selectedSight[slotIndex] = data;
                selectedSightIndex[slotIndex] = FindAttachmentGlobalIndex(data);
                break;
            case AttachmentTypes.Barrel:
                selectedBarrel[slotIndex] = data;
                selectedBarrelIndex[slotIndex] = FindAttachmentGlobalIndex(data);
                break;
            case AttachmentTypes.Underbarrel:
                selectedUnderbarrel[slotIndex] = data;
                selectedUnderbarrelIndex[slotIndex] = FindAttachmentGlobalIndex(data);
                break;
            case AttachmentTypes.Rightbarrel:
                selectedSidebarrelRight[slotIndex] = data;
                selectedSidebarrelRightIndex[slotIndex] = FindAttachmentGlobalIndex(data);
                break;
            case AttachmentTypes.Leftbarrel:
                selectedSidebarrelLeft[slotIndex] = data;
                selectedSidebarrelLeftIndex[slotIndex] = FindAttachmentGlobalIndex(data);
                break;
        }
    }
    public void SetNullAttachment(AttachmentTypes type, int slotIndex)
    {
        switch (type)
        {
            case AttachmentTypes.Sight:
                selectedSightIndex[slotIndex] = -1;
                break;
            case AttachmentTypes.Barrel:
                selectedBarrelIndex[slotIndex] = -1;
                break;
            case AttachmentTypes.Underbarrel:
                selectedUnderbarrelIndex[slotIndex] = -1;
                break;
            case AttachmentTypes.Rightbarrel:
                selectedSidebarrelRightIndex[slotIndex] = -1;
                break;
            case AttachmentTypes.Leftbarrel:
                selectedSidebarrelLeftIndex[slotIndex] = -1;
                break;
        }
    }
    public void SetNullAppearance(int slotIndex)
    {
        selectedAppearanceDataIndex[slotIndex] = -1;
    }
    public int FindAttachmentGlobalIndex(WeaponAttachmentData data)
    {
        for (int i = 0; i < GlobalDatabase.Instance.allWeaponAttachmentDatas.Count; i++)
        {
            if (data == GlobalDatabase.Instance.allWeaponAttachmentDatas[i])
            {
                return i;
            }
        }
        return -1;
    }
    public void SetAppearance(WeaponAppearanceMeshData data, int slotIndex)
    {
        selectedAppearanceData[slotIndex] = data;
        selectedAppearanceDataIndex[slotIndex] = data.WeaponAppearanceMeshDataIndex;
    }
}
