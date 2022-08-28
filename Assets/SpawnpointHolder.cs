using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnpointHolder : MonoBehaviour
{
    public List<SpawnpointScript> spawnpoints = new List<SpawnpointScript>();
    public List<SpawnpointUIPoint> spawnpointUIPoints = new List<SpawnpointUIPoint>();
    public Camera spawnpointCamera;
    public GameObject spawnpointUIPrefab;
    public GameObject spawnpointUIHolder;

    private void Start()
    {
        GetAllSpawnpoints();
        InstantiateSpawnpointUI();
    }
    public void GetAllSpawnpoints()
    {
        SpawnpointScript[] sp = FindObjectsOfType<SpawnpointScript>();
        for (int i = 0; i < sp.Length; i++)
        {
            spawnpoints.Add(sp[i]);
        }
    }
    public void GetAllSpawnpointUI()
    {
        SpawnpointUIPoint[] sp = FindObjectsOfType<SpawnpointUIPoint>();
        for (int i = 0; i < sp.Length; i++)
        {
            spawnpointUIPoints.Add(sp[i]);
        }
    }
    public void InstantiateSpawnpointUI()
    {
        for(int i = 0;i< spawnpoints.Count; i++)
        {
            GameObject temp = Instantiate(spawnpointUIPrefab, spawnpointUIHolder.transform);
            LinkSpawnpoint(spawnpoints[i], temp.GetComponent<SpawnpointUIPoint>(), i+1);
            spawnpointUIPoints.Add(temp.GetComponent<SpawnpointUIPoint>());
        }
    }
    public void LinkSpawnpoint(SpawnpointScript obj, SpawnpointUIPoint ui, int index)
    {
        ui.spawnpointRef = obj.gameObject;
        ui.spawnpointText.text = index.ToString();
    }
}
