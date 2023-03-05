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
    [SerializeField] Image mapIcon, mapSelected;
    public int mapIndex;
    private void Awake()
    {
        holder = MapListItemHolder.Instance;
        holder.OnSelectMapItem += OnSelectedInvoked;
    }
    public void SetInfo(MapItemInfo info, int index)
    {
        mapName.text = info.mapName;
        mapIcon.sprite = info.mapIcon;
        mapSelected.gameObject.SetActive(true);
        mapIndex = index;
    }
    public void OnClick()
    {
        holder.SetSelectedMap(mapIndex);
    }
    private void OnSelectedInvoked(bool active, int id)
    {
        if (id == -1)
        {
            mapSelected.gameObject.SetActive(active);
        }
        else if (id == mapIndex)
        {
            mapSelected.gameObject.SetActive(active);
        }
    }
}
