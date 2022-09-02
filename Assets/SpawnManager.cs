using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    SpawnpointScript[] spawnpoints;
    private void Awake()
    {
        Instance = this;
        spawnpoints = GetComponentsInChildren<SpawnpointScript>();
    }
    public Transform GetRandomSpawnpoint()
    {
        return spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
    }
}
