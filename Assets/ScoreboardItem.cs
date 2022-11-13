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
    public void Initialize(Player player, Scoreboard scbd)
    {
        this.player = player;
        scoreboard = scbd;
        usernameText.text = player.NickName;
        UpdateStats();
    }
    void UpdateStats()
    {
        if(scoreboard.matchManager.roomMode == MainMenuUIManager.Gamemodes.FFA)
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
        }else if (scoreboard.matchManager.roomMode == MainMenuUIManager.Gamemodes.TDM)
        {
            if(player.CustomProperties.TryGetValue("team", out object team))
            {
                scoreboard.matchManager.TeamDeathmatchKillLogic((bool)team);
            }
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(targetPlayer == player)
        {
            if (changedProps.ContainsKey("kills") || changedProps.ContainsKey("deaths")) 
            {
                UpdateStats();
            }
        }
    }
}
