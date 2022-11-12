using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CurrentMatchManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private PhotonView pv;
    private InGameUI internalUI;
    public List<PlayerManager> players = new();
    public List<Player> punPlayers = new();
    public bool gameStarted = false;
    public bool gameEnded = false;
    public MainMenuUIManager.Gamemodes roomGamemode;
    public float totalTime = 0f;
    public int maxKillLimit;
    public Scoreboard scoreboard;

    [Space]
    [Header("FFA")]
    public Player punTopPlayer;
    public PlayerManager topPlayer;
    public PlayerManager localClientPlayer;

    [Space, Header("TDM")]
    public List<PlayerManager> teamBlue = new();
    public List<PlayerManager> teamRed = new();
    public int teamBluePoints = 0;
    public int teamRedPoints = 0;

    // Start is called before the first frame update
    private void Awake()
    {
        maxKillLimit = (int)PhotonNetwork.CurrentRoom.CustomProperties["maxKillLimit"];
        internalUI = FindObjectOfType<InGameUI>();
        scoreboard = FindObjectOfType<Scoreboard>();
        //pv = GetComponent<PhotonView>();
        //photonView.ViewID = 998;
    }
    private void Start()
    {
        internalUI.ToggleMatchEndUI(false);
        internalUI.ToggleMatchEndStats(false, 0f);
        gameStarted = true;
        gameEnded = false;
        switch (PhotonNetwork.CurrentRoom.CustomProperties["roomMode"])
        {
            case "Free For All":
                roomGamemode = MainMenuUIManager.Gamemodes.FFA;
                internalUI.ToggleFFA_UI(true);
                internalUI.ToggleTDM_UI(false);
                internalUI.ToggleKOTH_UI(false);
                internalUI.ToggleDZ_UI(false);
                break;
            case "Team Deathmatch":
                roomGamemode = MainMenuUIManager.Gamemodes.TDM;
                internalUI.ToggleFFA_UI(false);
                internalUI.ToggleTDM_UI(true);
                internalUI.ToggleKOTH_UI(false);
                internalUI.ToggleDZ_UI(false);
                break;
            case "King of the Hills":
                roomGamemode = MainMenuUIManager.Gamemodes.KOTH;
                internalUI.ToggleFFA_UI(false);
                internalUI.ToggleTDM_UI(false);
                internalUI.ToggleKOTH_UI(true);
                internalUI.ToggleDZ_UI(false);
                break;
            case "Drop Zones":
                roomGamemode = MainMenuUIManager.Gamemodes.DZ;
                internalUI.ToggleFFA_UI(false);
                internalUI.ToggleTDM_UI(false);
                internalUI.ToggleKOTH_UI(false);
                internalUI.ToggleDZ_UI(true);
                break;
        }
        topPlayer = FindObjectOfType<PlayerManager>();
        OnPlayerKillUpdate();
        //UpdateTopPlayerHUD(topPlayer.kills, topPlayer.pv.Owner.NickName);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        punPlayers.Add(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        punPlayers.Remove(newPlayer);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        /*
        for(int i = 0; i < punPlayers.Count; i++)
        {
            if(punPlayers[i] == targetPlayer)
            {
                //if (changedProps.ContainsKey("kills")) OnPlayerKillReceiveCallback();
                //else return;
            }
        }*/
    }
    //public override void On
    public void OnPlayerKillReceiveCallback()
    {
        punPlayers = scoreboard.LocalPlayerDatas;
        if (roomGamemode != MainMenuUIManager.Gamemodes.FFA) return;
        int temp = -100000;
        if (punTopPlayer == null) punTopPlayer = punPlayers[Random.Range(0, punPlayers.Count - 1)];
        for (int i = 0; i < players.Count; i++)
        {
            if ((int)punPlayers[i].CustomProperties["kills"] >= temp)
            {
                punTopPlayer = punPlayers[i];
                temp = (int)punPlayers[i].CustomProperties["kills"];
            }
        }
        UpdateTopPlayerHUD((int)punTopPlayer.CustomProperties["kills"], punTopPlayer.NickName);
        for(int i = 0; i < players.Count; i++)
        {
            if(punTopPlayer.UserId == players[i].pv.Owner.UserId) topPlayer = players[i];
        }
        Debug.Log("Received Pun Kill Update");
    }
    public void OnPlayerKillUpdate()
    {
        if (roomGamemode != MainMenuUIManager.Gamemodes.FFA) return;
        int temp = -100000;
        for(int i = 0; i < players.Count; i++)
        {
            if((int)players[i].pv.Owner.CustomProperties["kills"] >= temp)
            {
                topPlayer = players[i];
                temp = (int)players[i].pv.Owner.CustomProperties["kills"];
            }
        }
        UpdateTopPlayerHUD((int)topPlayer.pv.Owner.CustomProperties["kills"], topPlayer.pv.Owner.NickName);
        Debug.Log("Called Kill Update");
    }
    public void UpdatePlayerKillOnClient()
    {
        if (roomGamemode != MainMenuUIManager.Gamemodes.FFA) return;
        int temp = -100000;
        for (int i = 0; i < players.Count; i++)
        {
            if ((int)players[i].pv.Owner.CustomProperties["kills"] >= temp)
            {
                topPlayer = players[i];
                temp = (int)players[i].pv.Owner.CustomProperties["kills"];
            }
        }
        internalUI.topPlayerName.text = topPlayer.pv.Owner.NickName;
        internalUI.topPlayerScore.text = ((int)topPlayer.pv.Owner.CustomProperties["kills"]).ToString();
    }
    public void OnPlayerListUpdate(List<PlayerManager> playerList)
    {
        players = playerList;
    }
    public void AddPlayer(PlayerManager player)
    {
        if(!players.Contains(player)) players.Add(player);
        OnPlayerListUpdate(players);
    }
    public void RemovePlayer(PlayerManager player)
    {
        if(players.Contains(player)) players.Remove(player);
        OnPlayerListUpdate(players);
    }
    private void Update()
    {
        if (gameEnded)
        {
            return;
        }
        if (!gameStarted) return;
        if (roomGamemode == MainMenuUIManager.Gamemodes.FFA)
        {
            FreeForAllFunctions();
        }
        else if(roomGamemode == MainMenuUIManager.Gamemodes.TDM)
        {

        }else if(roomGamemode == MainMenuUIManager.Gamemodes.KOTH)
        {

        }else if(roomGamemode == MainMenuUIManager.Gamemodes.DZ)
        {

        }
    }
    void FreeForAllFunctions()
    {
        if(topPlayer.kills >= maxKillLimit)
        {
            gameStarted = false;
            gameEnded = true;
            FFAWin(topPlayer.pv.Owner.NickName);
        }
    }
    void TeamDeathmatchFunctions()
    {
        
    }
    public void FFAWin(string winnerName)
    {
        pv.RPC(nameof(RPC_FFAWinMatch), RpcTarget.All, winnerName);
    }
    public void RefreshAllHostileIndicators()
    {
        pv.RPC(nameof(RPC_RefreshHostileIndicators), RpcTarget.All);
    }
    [PunRPC]
    void RPC_RefreshHostileIndicators()
    {
        if(localClientPlayer.controller != null)
        {
            PlayerControllerManager plr = localClientPlayer.controller.GetComponent<PlayerControllerManager>();
            for (int i = 0; i < plr.ui.hostileTargetIndicators.Count; i++)
            {
                plr.ui.RemoveTargetIndicator(plr.ui.hostileTargetIndicators[i]);
            }
            PlayerControllerManager[] plrArray = FindObjectsOfType<PlayerControllerManager>();
            foreach (PlayerControllerManager p in plrArray)
            {
                if (p == plr) continue;
                plr.ui.AddTargetIndicator(p.gameObject, UIManager.TargetIndicatorType.Hostile, Color.red);
            }
        }
    }
    [PunRPC]
    public void RPC_FFAWinMatch(string winnerName)
    {
        gameStarted = false;
        gameEnded = true;
        Cursor.lockState = CursorLockMode.None;
        internalUI.ToggleMatchEndUI(true);
        internalUI.SetMatchEndMessage(winnerName + " Won the match!");
        //StartCoroutine(QuitEveryPlayer(3f));
        if(localClientPlayer.controller != null)
        {
            localClientPlayer.controller.GetComponent<PlayerControllerManager>().Die();
        }
        localClientPlayer.CloseMenu();
        internalUI.ToggleMatchEndStats(true, 2f);
        int gainedCoins = (int)(localClientPlayer.totalGainedXP * (5f / 6f));
        internalUI.SetMatchEndStats(localClientPlayer.pv.Owner.NickName, (int)localClientPlayer.pv.Owner.CustomProperties["kills"], (int)localClientPlayer.pv.Owner.CustomProperties["deaths"], localClientPlayer.totalGainedXP, gainedCoins, UserDatabase.Instance.GetUserXPLevelValue(), UserDatabase.Instance.GetUserXPValue());
        UserDatabase.Instance.AddUserLevelXP(localClientPlayer.totalGainedXP);
        UserDatabase.Instance.AddUserCurrency(gainedCoins);
    }
    public void QuitFromMatchEnd()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if(players[i].pv.IsMine) players[i].DisconnectPlayer();
        }
    }
    public IEnumerator QuitEveryPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        for(int i = 0; i < players.Count; i++)
        {
            players[i].DisconnectPlayer();
        }
    }
    public void UpdateTeamDeathmatchHUD(int blueKills, int redKills)
    {

    }
    public void UpdateTopPlayerHUD(int kill, string name)
    {
        pv.RPC(nameof(RPC_UpdateTopPlayerHUD), RpcTarget.All, kill, name);
    }
    public void FindForPlayerID(string id)
    {
        pv.RPC(nameof(RPC_FindForPlayerID), RpcTarget.All, id);
    }
    public void RefreshPlayerList()
    {
        pv.RPC(nameof(RPC_RefreshPlayerList), RpcTarget.All);
    }
    public void DistributeTeams()
    {
        if (players.Count >= 2)
        {
            int blue = players.Count / 2;
            int red = players.Count - blue;
            List<PlayerManager> tempPlayers = players;
            for (int i = 0; i < blue; i++)
            {
                PlayerManager chosen = tempPlayers[Random.Range(0, tempPlayers.Count - 1)];
                teamBlue.Add(chosen);
                chosen.pv.Owner.CustomProperties.Add("team", true);
                tempPlayers.Remove(chosen);
                chosen.RetreiveIsTeamValue();
            }
            for (int i = 0; i < red; i++)
            {
                PlayerManager chosen = tempPlayers[Random.Range(0, tempPlayers.Count - 1)];
                teamRed.Add(chosen);
                chosen.pv.Owner.CustomProperties.Add("team", false);
                tempPlayers.Remove(chosen);
                chosen.RetreiveIsTeamValue();
            }
            string[] blueClientIDs = new string[teamBlue.Count];
            string[] redClientIDs = new string[teamRed.Count];
            for (int i = 0; i < teamBlue.Count; i++)
            {
                blueClientIDs[i] = teamBlue[i].pv.Owner.UserId;
            }
            for (int i = 0; i < teamRed.Count; i++)
            {
                redClientIDs[i] = teamRed[i].pv.Owner.UserId;
            }
            SynchronizeBlueTeamMembers(blueClientIDs);
            SynchronizeRedTeamMembers(redClientIDs);
        }
    }
    public void SynchronizeBlueTeamMembers(string[] clientIDs)
    {
        pv.RPC(nameof(RPC_SyncBlueClientIDs), RpcTarget.All, clientIDs);
    }
    [PunRPC]
    void RPC_UpdateTeamDeathmatchHUD(int blueKills, int redKills)
    {

    }
    [PunRPC]
    void RPC_SyncBlueClientIDs(string[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            teamBlue.Add(Client_FindForPlayerID(ids[i]));
        }
    }
    public void SynchronizeRedTeamMembers(string[] clientIDs)
    {
        pv.RPC(nameof(RPC_SyncBlueClientIDs), RpcTarget.All, clientIDs);
    }
    [PunRPC]
    void RPC_SyncRedClientIDs(string[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            teamRed.Add(Client_FindForPlayerID(ids[i]));
        }
    }
    PlayerManager Client_FindForPlayerID(string userID)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if(players[i].pv.Owner.UserId == userID)
            {
                return players[i];
            }
        }
        return null;
    }
    [PunRPC]
    void RPC_RefreshPlayerList()
    {
        players.Clear();
        PlayerManager[] tmp = FindObjectsOfType<PlayerManager>();
        for (int i = 0; i < tmp.Length; i++)
        {
            players.Add(tmp[i]);
        }
        OnPlayerListUpdate(players);
    }
    [PunRPC]
    void RPC_FindForPlayerID(string id)
    {
        PlayerManager[] tmp = FindObjectsOfType<PlayerManager>();
        for(int i = 0; i < tmp.Length; i++)
        {
            if (tmp[i].pv.Owner.UserId == id) topPlayer = tmp[i];
        }
    }
    [PunRPC]
    void RPC_UpdateTopPlayerHUD(int kill, string name)
    {
        internalUI.topPlayerName.text = name;
        internalUI.topPlayerScore.text = kill.ToString();
    }
}
