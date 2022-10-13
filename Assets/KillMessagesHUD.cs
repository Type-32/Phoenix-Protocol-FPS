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
        if(!playerManager.pv.IsMine) gameObject.SetActive(false);
    }

}
