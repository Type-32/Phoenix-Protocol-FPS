using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillStatsHUD : MonoBehaviour
{
    [Header("References")]
    public PlayerManager manager;
    public Transform killIconHolder;
    public Transform statsCounterHolder;
    public GameObject killIconItemPrefab;
    public GameObject statsCounterItemPrefab;

    [Space]
    [Header("Colors")]
    public Color specialKillColor;
    public Color specialKillColorCross;
    public Color normalKillColor;
    public Color normalKillColorCross;

    [Space]
    [Header("Icons")]
    public Sprite killIconSkull;
    private void Start()
    {
        if (!manager.pv.IsMine) gameObject.SetActive(false);
    }

}
