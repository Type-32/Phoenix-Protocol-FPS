using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardItemPrefab;
    Dictionary<Player, ScoreboardItem> scoreboardItems = new Dictionary<Player, ScoreboardItem>();
    public List<ScoreboardItem> LocalScoreboardItems = new();
    public List<Player> LocalPlayerDatas = new();
    [SerializeField] CanvasGroup canvasGroup;
    public CurrentMatchManager matchManager;
    private void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
            //matchManager.tmp.Add(player);
        }
    }
    private void Awake()
    {
        matchManager = FindObjectOfType<CurrentMatchManager>();
    }

    public void AddScoreboardItem(Player player)
    {
        matchManager = FindObjectOfType<CurrentMatchManager>();
        ScoreboardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreboardItem>();
        item.Initialize(player, this);
        scoreboardItems[player] = item;
        LocalScoreboardItems.Add(item);
        LocalPlayerDatas.Add(player);
    }
    public void RemoveScoreboardItem(Player player)
    {
        LocalScoreboardItems.Remove(scoreboardItems[player]);
        LocalPlayerDatas.Remove(player);
        Destroy(scoreboardItems[player].gameObject);
        scoreboardItems.Remove(player);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreboardItem(otherPlayer);
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, 8 * Time.deltaTime);
        }
        else
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, 8 * Time.deltaTime);
        }
    }
    public void FindForMostKills()
    {
        int temp = -10000;
        string tempname = "";
        //ScoreboardItem tempItem = new();
        for (int i = 0; i < LocalScoreboardItems.Count; i++)
        {
            if (int.Parse(LocalScoreboardItems[i].killsText.text) >= temp)
            {
                temp = int.Parse(LocalScoreboardItems[i].killsText.text);
                tempname = LocalScoreboardItems[i].usernameText.text;
                //tempItem = LocalScoreboardItems[i];
            }
        }
        matchManager.UpdateTopPlayerHUD(temp, tempname);
        //matchManager.FindForPlayerID(tempItem.player.UserId);
    }
}
