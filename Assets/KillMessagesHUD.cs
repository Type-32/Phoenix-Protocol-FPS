using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class KillMessagesHUD : MonoBehaviour
{
    public PlayerManager playerManager;
    public Transform killMessagesHolder;
    public GameObject killMessagesItemPrefab;
    private void Start()
    {
        //if(!playerManager.pv.IsMine) gameObject.SetActive(false);
    }
    public Sprite FindWeaponIcon(int index)
    {
        for(int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            if (i == index) return GlobalDatabase.singleton.allWeaponDatas[i].itemIcon;
        }
        return null;
    }

}
