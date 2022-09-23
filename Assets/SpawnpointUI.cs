using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointUI : MonoBehaviour
{
    public PlayerManager playerManager;
    public SpawnpointListHolder spawnpointListHolder;
    [SerializeField] GameObject spawnpointItemPrefab;
    public List<SpawnpointListItem> spawnpointItemsUI = new List<SpawnpointListItem>();
    string[] characterList = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    // Start is called before the first frame update
    void Start()
    {
    }
    private void Awake()
    {
        for (int i = 0; i < SpawnManager.Instance.spawnpoints.Length; i++)
        {
            InstantiateSpawnpointListItems(characterList[i], i);
        }
    }

    public void InstantiateSpawnpointListItems(string captial, int index)
    {
        SpawnpointListItem temp = Instantiate(spawnpointItemPrefab, spawnpointListHolder.transform).GetComponent<SpawnpointListItem>();
        spawnpointItemsUI.Add(temp);
        temp.SetCapitalCharacter(captial);
        temp.SetTrackingObject(SpawnManager.Instance.spawnpoints[index].gameObject);
        temp.SetIndex(index);
    }
    public void ToggleRandomRespawnPoint(bool value)
    {
        playerManager.randomSpawnpoint = value;
        if (playerManager.spawnpoint == null && value) playerManager.spawnpoint = SpawnManager.Instance.GetRandomSpawnpoint(); 
    }
    public void ChooseSpawnpoint(int index)
    {
        spawnpointItemsUI[index].OnClickSpawnpoint();
    }
    public int RandomSelectSpawnpoint()
    {
        return Random.Range(0, spawnpointItemsUI.Count - 1);
    }
    public void DeselectEverySpawnpoint()
    {
        for(int i = 0; i < spawnpointItemsUI.Count; i++)
        {
            spawnpointItemsUI[i].OnDeselectSpawnpoint();
        }
    }
}
