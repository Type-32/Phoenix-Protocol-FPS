using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class MapListItem : MonoBehaviour
{
    MapListItemHolder holder;
    [SerializeField] Text mapName;
    [SerializeField] Image mapIcon;
    [SerializeField] Text mapPlayers;
    [SerializeField] Text mapVersion;
    public int mapIndex;
    private void Awake()
    {
        holder = MapListItemHolder.Instance;
    }
    public void SetInfo(MapItemInfo info, int index)
    {
        mapName.text = info.mapName;
        mapIcon.sprite = info.mapIcon;
        mapPlayers.text = "Recommended Players: " + info.recommendedPlayers.ToString();
        mapVersion.text = "Map Version V" + info.mapVersion;
        mapIndex = index;
    }
    public void OnClick()
    {
        holder.SetSelectedMap(mapIndex, mapName.text);
    }
}
