using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Spawnpoint Data", menuName = "New Spawnpoint Data", order = 1)]
public class SpawnpointData : ScriptableObject
{
    public GameObject spawnpointObject;
    public Sprite spawnpointIcon;
    public string spawnpointName;
    public string spawnpointSubscript;
}
