using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Threading.Tasks;
using PrototypeLib.OnlineServices.PUNMultiplayer.ConfigurationKeys;

public class CurrentMatchManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private PhotonView pv;
    public static CurrentMatchManager Instance;
    private InGameUI internalUI;
    public List<PlayerManager> players = new();
    public List<Player> punPlayers = new();
    public bool gameStarted = false;
    public bool gameEnded = false;
    public MenuManager.Gamemodes roomMode;
    public float totalTime = 0f;
    public int maxKillLimit;
    public Scoreboard scoreboard;
    public bool allowDownedState = false;

    [Space]
    [Header("FFA")]
    public Player punTopPlayer;
    public PlayerManager topPlayer, localClientPlayer, masterClientPlayer;

    [Space, Header("TDM")]
    public List<PlayerManager> teamBlue = new();
    public List<PlayerManager> teamRed = new();
    public int teamBluePoints = 0;
    public int teamRedPoints = 0;

    [Space, Header("DZ")]
    public int dropTimes = 1;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        allowDownedState = (bool)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.AllowDownedState];
        maxKillLimit = (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.MaxKillLimit];
        internalUI = FindObjectOfType<InGameUI>();
        scoreboard = FindObjectOfType<Scoreboard>();
        //pv = GetComponent<PhotonView>();
        //photonView.ViewID = 998;
    }
    private async void Start()
    {
        internalUI.ToggleMatchEndUI(false);
        internalUI.TDM_ToggleMatchEndUI(false, false, false);
        internalUI.ToggleMatchEndStats(false, 0f);
        gameEnded = false;
        switch (PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomMode])
        {
            case "Free For All":
                roomMode = MenuManager.Gamemodes.FFA;
                internalUI.ToggleFFA_UI(true);
                internalUI.ToggleTDM_UI(false);
                internalUI.ToggleCTF_UI(false);
                internalUI.ToggleDZ_UI(false);
                break;
            case "Team Deathmatch":
                roomMode = MenuManager.Gamemodes.TDM;
                internalUI.ToggleFFA_UI(false);
                internalUI.ToggleTDM_UI(true);
                internalUI.ToggleCTF_UI(false);
                internalUI.ToggleDZ_UI(false);
                break;
            case "Capture The Flag":
                roomMode = MenuManager.Gamemodes.CTF;
                internalUI.ToggleFFA_UI(false);
                internalUI.ToggleTDM_UI(false);
                internalUI.ToggleCTF_UI(true);
                internalUI.ToggleDZ_UI(false);
                break;
            case "Drop Zones":
                roomMode = MenuManager.Gamemodes.DZ;
                internalUI.ToggleFFA_UI(false);
                internalUI.ToggleTDM_UI(false);
                internalUI.ToggleCTF_UI(false);
                internalUI.ToggleDZ_UI(true);
                break;
        }
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            await WaitForPlayerList();
            Debug.Log("Player is master client.");
        }
        OnPlayerKillUpdate();
        //UpdateTopPlayerHUD(topPlayer.kills, topPlayer.pv.Owner.NickName);
    }
    public async Task WaitForPlayerList()
    {
        Debug.Log("Waiting for Instances to be instantiated...");
        int rot = 1;
        while (players.Count != PhotonNetwork.CurrentRoom.PlayerCount || players.Count < PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log($"Finding Players... {players.Count}/{PhotonNetwork.CurrentRoom.PlayerCount}, Attempt {rot}");
            rot++;
            await Task.Delay(1000);
            photonView.RPC(nameof(RPC_RetrieveAllPlayerManagers), RpcTarget.All);
        }
        RoomManager.Instance.SetLoadingScreenStateRPC(false, 2);
        gameStarted = true;
    }
    [PunRPC]
    void RPC_RetrieveAllPlayerManagers()
    {
        PlayerManager[] t = FindObjectsOfType<PlayerManager>();
        players.Clear();
        foreach (PlayerManager i in t)
        {
            players.Add(i);
            if (i.pv.ControllerActorNr == PhotonNetwork.CurrentRoom.MasterClientId)
            {
                masterClientPlayer = i;
                topPlayer = i;
            }
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        punPlayers.Add(newPlayer);
        //players.Add(FindPlayerManager(newPlayer));
    }
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        punPlayers.Remove(newPlayer);
        players.Remove(FindPlayerManager(newPlayer));
    }
    public PlayerManager FindPlayerManager(Player player)
    {
        PlayerManager[] tpm = FindObjectsOfType<PlayerManager>();
        PlayerManager res = null;
        foreach (PlayerManager i in tpm)
        {
            if (player == i.pv.Owner)
            {
                res = i;
                break;
            }
        }
        return res;
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
    public void OnPlayerKillUpdate()
    {
        if (roomMode != MenuManager.Gamemodes.FFA) return;
        int temp = -100000;
        for (int i = 0; i < players.Count; i++)
        {
            if ((int)players[i].pv.Owner.CustomProperties["kills"] >= temp)
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
        if (roomMode != MenuManager.Gamemodes.FFA) return;
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
    public void TeamDeathmatchKillLogic(int amount, bool team)
    {
        TDM_AddPoint(amount, team);
    }
    void TDM_AddPoint(int amount, bool team)
    {
        if (localClientPlayer.pv.Owner.IsMasterClient)
        {
            if (localClientPlayer.IsTeam == true)
            {
                if (team == localClientPlayer.IsTeam)
                {
                    teamBluePoints += amount;
                }
                else
                {
                    teamRedPoints += amount;
                }
            }
            else
            {
                if (team == localClientPlayer.IsTeam)
                {
                    teamRedPoints += amount;
                }
                else
                {
                    teamBluePoints += amount;
                }
            }
        }
        else
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].pv.Owner.IsMasterClient)
                {
                    if (players[i].IsTeam == true)
                    {
                        if (team == players[i].IsTeam)
                        {
                            teamBluePoints += amount;
                        }
                        else
                        {
                            teamRedPoints += amount;
                        }
                    }
                    else
                    {
                        if (team == players[i].IsTeam)
                        {
                            teamRedPoints += amount;
                        }
                        else
                        {
                            teamBluePoints += amount;
                        }
                    }
                    break;
                }
            }
        }
        pv.RPC(nameof(TDM_AddPointSynchronize), RpcTarget.All, teamBluePoints, teamRedPoints);
    }
    [PunRPC]
    void TDM_AddPointSynchronize(int blueAmount, int redAmount)
    {
        if (localClientPlayer.IsTeam)
        {
            internalUI.UpdateTDMData(blueAmount, redAmount, (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.MaxKillLimit]);
        }
        else
        {
            internalUI.UpdateTDMData(redAmount, blueAmount, (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.MaxKillLimit]);
        }
    }
    private void FixedUpdate()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;
        if (gameEnded)
        {
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        if (!gameStarted) return;
        if (roomMode == MenuManager.Gamemodes.FFA)
        {
            FreeForAllFunctions();
        }
        else if (roomMode == MenuManager.Gamemodes.TDM)
        {
            TeamDeathmatchFunctions();
        }
        else if (roomMode == MenuManager.Gamemodes.CTF)
        {

        }
        else if (roomMode == MenuManager.Gamemodes.DZ)
        {
            DropZonesFunctions();
        }
    }
    void FreeForAllFunctions()
    {
        if (topPlayer.kills >= maxKillLimit)
        {
            gameStarted = false;
            gameEnded = true;
            FFAWin(topPlayer.pv.Owner.NickName);
        }
    }
    void TeamDeathmatchFunctions()
    {
        if (teamBluePoints >= maxKillLimit && teamBluePoints >= teamRedPoints)
        {
            TDMWin(true);
        }
        else if (teamRedPoints >= maxKillLimit && teamRedPoints >= teamBluePoints)
        {
            TDMWin(false);
        }
    }
    void DropZonesFunctions()
    {

    }
    public void TDMWin(bool winnerIsTeam)
    {
        pv.RPC(nameof(RPC_TDMWinMatch), RpcTarget.All, winnerIsTeam);
    }
    public void FFAWin(string winnerName)
    {
        pv.RPC(nameof(RPC_FFAWinMatch), RpcTarget.All, winnerName);
    }
    public void RefreshAllHostileIndicators()
    {
        pv.RPC(nameof(RPC_RefreshHostileIndicators), RpcTarget.All);
    }
    public void RefreshAllSupplyIndicators()
    {
        pv.RPC(nameof(RPC_RefreshSupplyIndicators), RpcTarget.All);
    }
    [PunRPC]
    void RPC_RefreshHostileIndicators()
    {
        if (localClientPlayer.controller != null)
        {
            PlayerControllerManager plr = localClientPlayer.controller.GetComponent<PlayerControllerManager>();
            for (int i = 0; i < plr.ui.hostileTargetIndicators.Count; i++) plr.ui.RemoveTargetIndicator(plr.ui.hostileTargetIndicators[i]);
            PlayerControllerManager[] plrArray = FindObjectsOfType<PlayerControllerManager>();
            foreach (PlayerControllerManager p in plrArray)
            {
                if (p == plr) continue;
                plr.ui.AddTargetIndicator(p.gameObject, UIManager.TargetIndicatorType.Hostile, Color.red);
            }
        }
    }
    [PunRPC]
    void RPC_RefreshSupplyIndicators()
    {
        if (localClientPlayer.controller != null)
        {
            PlayerControllerManager plr = localClientPlayer.controller.GetComponent<PlayerControllerManager>();
            for (int i = 0; i < plr.ui.supplyTargetIndicators.Count; i++) plr.ui.RemoveTargetIndicator(plr.ui.supplyTargetIndicators[i]);
            Pickup[] plrArray = FindObjectsOfType<Pickup>();
            foreach (Pickup p in plrArray)
            {
                if (p == plr) continue;
                plr.ui.AddTargetIndicator(p.gameObject, UIManager.TargetIndicatorType.Supply, Color.white);
            }
        }
    }
    [PunRPC]
    void RPC_TDMWinMatch(bool winnerIsTeam)
    {
        gameStarted = false;
        gameEnded = true;
        Cursor.lockState = CursorLockMode.None;
        internalUI.TDM_ToggleMatchEndUI(true, winnerIsTeam, localClientPlayer.IsTeam);
        internalUI.UIAnimator.SetBool("MatchEnded", true);
        internalUI.SetMatchEndMessage((winnerIsTeam ? "Blue Team" : "Red Team") + " Won the match!");
        //StartCoroutine(QuitEveryPlayer(3f));
        if (localClientPlayer.controller != null)
        {
            localClientPlayer.controller.GetComponent<PlayerControllerManager>().Die(true, -1);
        }
        localClientPlayer.CloseMenu();
        int gainedCoins = (int)(localClientPlayer.totalGainedXP * (5f / 6f));
        internalUI.SetMatchEndStats(localClientPlayer.pv.Owner.NickName, (int)localClientPlayer.pv.Owner.CustomProperties["kills"], (int)localClientPlayer.pv.Owner.CustomProperties["deaths"], localClientPlayer.totalGainedXP, gainedCoins, UserDatabase.Instance.GetUserXPLevelValue(), UserDatabase.Instance.GetUserXPValue());
        UserDatabase.Instance.AddUserLevelXP(localClientPlayer.totalGainedXP);
        UserDatabase.Instance.AddUserCurrency(gainedCoins);
    }
    [PunRPC]
    public void RPC_FFAWinMatch(string winnerName)
    {
        gameStarted = false;
        gameEnded = true;
        localClientPlayer.CloseMenu();
        if (localClientPlayer.controller != null)
        {
            localClientPlayer.controller.GetComponent<PlayerControllerManager>().SetPlayerControlState(false);
        }
        Cursor.lockState = CursorLockMode.None;
        internalUI.ToggleMatchEndUI(true);
        internalUI.UIAnimator.SetBool("MatchEnded", true);
        internalUI.SetMatchEndMessage(winnerName + " Won the match!");
        //StartCoroutine(QuitEveryPlayer(3f));
        int gainedCoins = (int)(localClientPlayer.totalGainedXP * (5f / 6f));
        internalUI.SetMatchEndStats(localClientPlayer.pv.Owner.NickName, (int)localClientPlayer.pv.Owner.CustomProperties["kills"], (int)localClientPlayer.pv.Owner.CustomProperties["deaths"], localClientPlayer.totalGainedXP, gainedCoins, UserDatabase.Instance.GetUserXPLevelValue(), UserDatabase.Instance.GetUserXPValue());
        UserDatabase.Instance.AddUserLevelXP(localClientPlayer.totalGainedXP);
        UserDatabase.Instance.AddUserCurrency(gainedCoins);
    }
    public void QuitFromMatchEnd()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].pv.IsMine) players[i].DisconnectPlayer();
        }
    }
    public IEnumerator QuitEveryPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < players.Count; i++)
        {
            players[i].DisconnectPlayer();
        }
    }
    public void UpdateTeamDeathmatchHUD(int blueKills, int redKills)
    {
        pv.RPC(nameof(RPC_UpdateTeamDeathmatchHUD), RpcTarget.All, blueKills, redKills);
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
        List<PlayerManager> tmp = players;
        Debug.Log("Teams Distributed");
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            RefreshPlayerList();
            Debug.Log("Executing Command");
            int blue = tmp.Count / 2;
            int red = tmp.Count - blue;
            Debug.Log("Blue Count: " + blue + "\n PlayerCount: " + tmp.Count);
            Debug.Log("Red Count: " + red + "\n PlayerCount: " + tmp.Count);
            for (int i = 0; i < blue; i++)
            {
                int rnd = Random.Range(0, tmp.Count - 1);
                tmp[rnd].SetPlayerIsTeamState(true, true);
                SynchronizeBlueTeamMembers(tmp[rnd].GetComponent<PhotonView>().ViewID);
                tmp.Remove(tmp[rnd]);
            }
            for (int i = 0; i < red; i++)
            {
                int rnd = Random.Range(0, tmp.Count - 1);
                tmp[rnd].SetPlayerIsTeamState(false, true);
                SynchronizeRedTeamMembers(tmp[rnd].GetComponent<PhotonView>().ViewID);
                tmp.Remove(tmp[rnd]);
            }
            UpdateTeamDeathmatchHUD(0, 0);
        }
    }
    public void SynchronizeBlueTeamMembers(int clientIDs)
    {
        pv.RPC(nameof(RPC_SyncBlueClientIDs), RpcTarget.All, clientIDs);
    }
    [PunRPC]
    void RPC_UpdateTeamDeathmatchHUD(int blueKills, int redKills)
    {
        internalUI.UpdateTDMData(blueKills, redKills, maxKillLimit);
    }
    [PunRPC]
    void RPC_SyncBlueClientIDs(int ids)
    {
        PlayerManager tp = Client_FindForPlayerID(ids);
        //if (tp.pv.Owner.IsLocal) return;
        teamBlue.Add(tp);
    }
    public void SynchronizeRedTeamMembers(int clientIDs)
    {
        pv.RPC(nameof(RPC_SyncRedClientIDs), RpcTarget.All, clientIDs);
    }
    [PunRPC]
    void RPC_SyncRedClientIDs(int ids)
    {
        PlayerManager tp = Client_FindForPlayerID(ids);
        //if (tp.pv.Owner.IsLocal) return;
        teamRed.Add(tp);
    }
    PlayerManager Client_FindForPlayerID(int userID)
    {
        PlayerManager[] tps = FindObjectsOfType<PlayerManager>();
        for (int i = 0; i < tps.Length; i++)
        {
            if (tps[i].pv.ViewID == userID)
            {
                return tps[i];
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
    }
    [PunRPC]
    void RPC_FindForPlayerID(string id)
    {
        PlayerManager[] tmp = FindObjectsOfType<PlayerManager>();
        for (int i = 0; i < tmp.Length; i++)
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
