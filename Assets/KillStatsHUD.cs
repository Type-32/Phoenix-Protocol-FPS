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
    [SerializeField] Color specialKillColor;
    [SerializeField] Color specialKillColorCross;
    [SerializeField] Color normalKillColor;
    [SerializeField] Color normalKillColorCross;

    [Space]
    [Header("Icons")]
    public Sprite killIconSkull;
    private void Start()
    {
        if (!manager.pv.IsMine) gameObject.SetActive(false);
    }

    public void InstantiateOnKill(bool isSpecialKill, string killedPlayerName, int killedPoints)
    {
        GameObject temp1 = Instantiate(killIconItemPrefab, killIconHolder);
        GameObject temp2 = Instantiate(statsCounterItemPrefab, statsCounterHolder);
        if (isSpecialKill)
        {
            temp1.GetComponent<KillIconItem>().SetInfo(killIconSkull, specialKillColor, specialKillColorCross);
            temp2.GetComponent<TextStatItem>().SetInfo("Killed " + killedPlayerName, killedPoints + 50);
        }
        else
        {
            temp1.GetComponent<KillIconItem>().SetInfo(killIconSkull, normalKillColor, normalKillColorCross);
            temp2.GetComponent<TextStatItem>().SetInfo("Killed " + killedPlayerName, killedPoints);
        }
        Destroy(temp1, 2f);
        Destroy(temp2, 2f);
    }
}
