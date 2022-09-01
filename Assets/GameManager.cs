using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject playerPrefab;
    //public GameObject playerHolder;
    public List<GameObject> playerList = new List<GameObject>();
    private UIManager ui;
    public enum Gamemode
    {
        TeamDeathmatch,
        FreeForAll,
        DropZones,
        Zombies,
        None
    }
    public bool gameStarted = false;
    public struct TDMDataPackage
    {
        [Tooltip("Match Duration Counter is written in seconds.")] public int matchDuration;
        public int teamScoreLimit;
        public int scorePerKill;
        public int maxTotalPlayerCountInMatch;
        public int playerCountBlue;
        public int playerCountRed;
        public int respawnDuration;

        public void GenerateTeamMaxPlayerCount()
        {
            float temp = maxTotalPlayerCountInMatch / 2.0f;
            playerCountBlue = (int)temp;
            playerCountRed = maxTotalPlayerCountInMatch - playerCountBlue;
        }

    };
    public struct FFADataPackage
    {
        [Tooltip("Match Duration Counter is written in seconds.")] public int matchDuration;
        public int playerScoreLimit;
        public int scorePerKill;
        public int maxTotalPlayerCountInMatch;
        public int respawnDuration;
    };
    public struct DZTeamsDataPackage
    {
        public List<PlayerControllerManager> playersInTeam;
        [Range(1, 4)] public int maxPlayerCountPerTeam;
        public int teamIndex;
        bool AddPlayerToTeam(PlayerControllerManager plr)
        {
            if(maxPlayerCountPerTeam >= playersInTeam.Count)
            {
                Debug.LogWarning("Failed adding ");
                return false;
            }
            Debug.Log("Added Player ");
            return true;
        }
    };
    public struct DZDataPackage
    {
        [Tooltip("Gas Shrinking Duration is written in seconds.")] public int gasShrinkDuration;
        public List<DZTeamsDataPackage> teamsInMatch;
        public int maxTotalPlayerCountInMatch;
    };
    public struct ZombiesDataPackage
    {
        [Tooltip("Amount of waves per a Boss wave")] public int wavesPerBossWave;
        [Range(1, 6)] public int maxTotalPlayerCountInMatch;
    };
    public struct MatchDataPackage
    {
        [Header("Universal Data")]
        public Gamemode selectedGamemode;
        public TDMDataPackage teamDeathmatchData;
        public FFADataPackage freeForAllData;
        public DZDataPackage dropZonesData;
        public ZombiesDataPackage zombiesData;
        public List<PlayerControllerManager> matchPlayers;
    };
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        ui = FindObjectOfType<UIManager>();
    }
    public void StartGameMode(MatchDataPackage data)
    {
        if (data.selectedGamemode == Gamemode.None)
        {
            Debug.LogWarning("Selected Gamemode Cannot be Null! ");
            return;
        }
        else
        {
            if (data.selectedGamemode == Gamemode.TeamDeathmatch)
            {

            }
            gameStarted = true;
            Debug.Log("Game is Starting... ");
        }
    }
    public void SpawnPlayer(Transform spawnPosition)
    {
        GameObject temp = Instantiate(playerPrefab, Vector3.zero, spawnPosition.rotation);
        temp.transform.position = spawnPosition.position;
        playerList.Add(temp);
    }
    public void RemovePlayer(GameObject player)
    {
        playerList.Remove(player);
        Destroy(player);
    }
}
