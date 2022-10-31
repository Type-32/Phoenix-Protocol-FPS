using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CurrentMatchManager : MonoBehaviourPun
{
    [SerializeField] private PhotonView pv;
    private InGameUI internalUI;
    public List<PlayerManager> players = new();
    public bool gameStarted = false;
    public bool gameEnded = false;
    public MainMenuUIManager.Gamemodes roomGamemode;
    public float totalTime = 0f;

    [Space]
    [Header("FFA")]
    public PlayerManager topPlayer;
    public PlayerManager localClientPlayer;
    // Start is called before the first frame update
    private void Awake()
    {
        internalUI = FindObjectOfType<InGameUI>();
        //pv = GetComponent<PhotonView>();
        //photonView.ViewID = 998;
    }
    private void Start()
    {
        internalUI.ToggleMatchEndUI(false);
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
        UpdateTopPlayerHUD(topPlayer.kills, topPlayer.pv.Owner.NickName);
    }
    public void OnPlayerKillUpdate()
    {
        if (roomGamemode != MainMenuUIManager.Gamemodes.FFA) return;
        int temp = -100000;
        for(int i = 0; i < players.Count; i++)
        {
            if(players[i].kills >= temp)
            {
                topPlayer = players[i];
                temp = players[i].kills;
            }
        }
        UpdateTopPlayerHUD(topPlayer.kills, topPlayer.pv.Owner.NickName);
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
        if(topPlayer.kills >= 30)
        {
            FFAWin(topPlayer.pv.Owner.NickName);
        }
    }
    public void FFAWin(string winnerName)
    {
        pv.RPC(nameof(RPC_FFAWinMatch), RpcTarget.All, winnerName);
    }
    [PunRPC]
    public void RPC_FFAWinMatch(string winnerName)
    {
        gameStarted = false;
        gameEnded = true;
        Cursor.lockState = CursorLockMode.None;
        internalUI.ToggleMatchEndUI(true);
        internalUI.SetMatchEndMessage(winnerName + " Won the match!");
        StartCoroutine(QuitEveryPlayer(3f));
    }
    public IEnumerator QuitEveryPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        for(int i = 0; i < players.Count; i++)
        {
            players[i].DisconnectPlayer();
        }
    }
    public void UpdateTopPlayerHUD(int kill, string name)
    {
        pv.RPC(nameof(RPC_UpdateTopPlayerHUD), RpcTarget.All, kill, name);
    }
    public void FindForPhotonView(int viewID)
    {

    }
    public void RefreshPlayerList()
    {
        pv.RPC(nameof(RPC_RefreshPlayerList), RpcTarget.All);
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
    void RPC_FindForPhotonView()
    {

    }
    [PunRPC]
    void RPC_UpdateTopPlayerHUD(int kill, string name)
    {
        internalUI.topPlayerName.text = name;
        internalUI.topPlayerScore.text = kill.ToString();
    }
}
