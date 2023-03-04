using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] Text roomName;
    [SerializeField] Image roomIcon;
    [SerializeField] Text roomMode;
    [SerializeField] Text roomHostName;
    [SerializeField] Text roomMapName;
    [SerializeField] Text roomCode;
    Hashtable infoGroup;
    public RoomInfo roomInfo;
    public void SetUp(RoomInfo info)
    {
        roomInfo = info;
        roomName.text = info.Name;
        roomIcon.sprite = Launcher.Instance.mapItemInfo[((int)info.CustomProperties["roomMapIndex"]) - 1].mapIcon;
        roomMapName.text = Launcher.Instance.mapItemInfo[((int)info.CustomProperties["roomMapIndex"]) - 1].mapName;
        roomMode.text = (string)info.CustomProperties["roomMode"];
        roomCode.text = ((int)info.CustomProperties["roomCode"]).ToString();
        roomHostName.text = (string)info.CustomProperties["roomHostName"];
        //Debug.Log("Getting Map Info Index: " + (((int)info.CustomProperties["roomMapIndex"]) - 1));
    }
    public void OnClick() => Launcher.Instance.JoinRoom(roomInfo);
}
