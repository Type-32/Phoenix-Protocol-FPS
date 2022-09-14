using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Item Info", menuName = "New Map Item Info", order = 1)]
public class MapItemInfo : ScriptableObject
{
    public string mapName;
    public string mapVersion;
    public int recommendedPlayers;
    public Sprite mapIcon;
    //public int mapIndex;
}
