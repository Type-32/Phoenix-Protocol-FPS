using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;
    public Transform killMSGHolder;
    public GameObject killMSGPrefab;
    private void Awake()
    {
        instance = this;
    }
    public Sprite FindWeaponIcon(int index)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            if (i == index) return GlobalDatabase.singleton.allWeaponDatas[i].itemIcon;
        }
        return null;
    }

}
