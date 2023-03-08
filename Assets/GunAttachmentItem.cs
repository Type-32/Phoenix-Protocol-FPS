using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAttachmentItem : MonoBehaviour
{
    public WeaponAttachmentData data;
    public int dataGlobalIndex;

    private void Awake()
    {
        dataGlobalIndex = FindIndexFromData(data);
    }
    public int FindIndexFromData(WeaponAttachmentData _data)
    {
        if (_data == null) return -1;
        for (int i = 0; i < GlobalDatabase.Instance.allWeaponAttachmentDatas.Count; i++)
        {
            if (GlobalDatabase.Instance.allWeaponAttachmentDatas[i] == _data) return i;
        }
        return -1;
    }
}
