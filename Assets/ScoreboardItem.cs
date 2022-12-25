using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    public Text usernameText;
    public Text killsText;
    public Text deathsText;
    public Text pingText;
    public Scoreboard scoreboard;
    public Player player;
    private int cachedPlayerKill;
    public void Initialize(Player player, Scoreboard scbd)
    {
        this.player = player;
        scoreboard = scbd;
        usernameText.text = player.NickName;
        cachedPlayerKill = 0;
        pingText.text = Random.Range(30, 110).ToString();
        UpdateStats();
    }
    void UpdateStats()
    {
        if (scoreboard.matchManager.roomMode == MenuManager.Gamemodes.FFA)
        {
            if (player.CustomProperties.TryGetValue("kills", out object kills))
            {
                killsText.text = kills.ToString();
                scoreboard.FindForMostKills();
            }
            if (player.CustomProperties.TryGetValue("deaths", out object deaths))
            {
                deathsText.text = deaths.ToString();
            }
        }
        else if (scoreboard.matchManager.roomMode == MenuManager.Gamemodes.TDM)
        {
            if (player.CustomProperties.TryGetValue("kills", out object kills))
            {
                killsText.text = kills.ToString();
                player.CustomProperties.TryGetValue("team", out object team);
                int res = (int)kills - cachedPlayerKill;
                if (res >= 1) { scoreboard.matchManager.TeamDeathmatchKillLogic(res, (bool)team); cachedPlayerKill = (int)kills; }
            }
            if (player.CustomProperties.TryGetValue("deaths", out object deaths))
            {
                deathsText.text = deaths.ToString();
            }
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == player)
        {
            if (changedProps.ContainsKey("kills") || changedProps.ContainsKey("deaths"))
            {
                UpdateStats();
            }
        }
    }
}
