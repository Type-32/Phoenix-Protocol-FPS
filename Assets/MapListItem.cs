using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class MapListItem : MonoBehaviour
{
    [SerializeField] Text mapName;
    [SerializeField] Image mapIcon;
    [SerializeField] Text mapPlayers;
    [SerializeField] Text roomHostName;
    public RoomInfo roomInfo;
    public void OnClick()
    {
        
    }
}
