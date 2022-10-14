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
    Hashtable infoGroup;
    public RoomInfo roomInfo;
    public FullRoomInformation fullRoomInfo;
    public void SetUp(RoomInfo info)
    {
        roomInfo = info;
        roomName.text = info.Name;
        //roomIcon.sprite = Launcher.Instance.mapItemInfo[info.CustomProperties[0]]
        Debug.Log("Getting Map Info Index: " + (((int)info.CustomProperties["roomMapIndex"]) - 1));
        roomIcon.sprite = Launcher.Instance.mapItemInfo[((int)info.CustomProperties["roomMapIndex"]) - 1].mapIcon;
        roomMode.text = (string)info.CustomProperties["roomMode"];
        roomHostName.text = (string)info.CustomProperties["roomHostName"];

        fullRoomInfo.Combine(roomInfo, roomName.text, roomIcon.sprite, roomMode.text, roomHostName.text);
        //roomInfo.CustomProperties.Add("roomIcon", roomIcon.sprite);
        //roomInfo.CustomProperties.Add("roomMode", roomMode.text);
        //roomInfo.CustomProperties.Add("roomHostName", roomHostName.text);
    }
    public void OnClick()
    {
        Launcher.Instance.JoinRoom(roomInfo);
    }
    public struct FullRoomInformation
    {
        private RoomInfo roomInfo;
        private string roomName;
        private Sprite roomIcon;
        private string roomMode;
        private string roomHostName;

        internal void Combine(RoomInfo _roomInfo, string _roomName, Sprite _roomIcon, string _roomMode, string _roomHostName)
        {
            this.roomInfo = _roomInfo;
            this.roomInfo.CustomProperties.Add("roomname", _roomName);
            this.roomInfo.CustomProperties.Add("roomicon", _roomIcon);
            this.roomInfo.CustomProperties.Add("roommode", _roomMode);
            this.roomInfo.CustomProperties.Add("roomhostname", _roomHostName);
        }
        internal void SetInfo(RoomInfo _roomInfo, string _roomName, Sprite _roomIcon, string _roomMode, string _roomHostName)
        {
            this.roomInfo = _roomInfo;
            this.roomName = _roomName;
            this.roomIcon = _roomIcon;
            this.roomMode = _roomMode;
            this.roomHostName = _roomHostName;
        }
        internal void SetInfoFromRoomInfo(RoomInfo _roomInfo)
        {
            this.roomInfo = _roomInfo;
            this.roomName = (string)this.roomInfo.CustomProperties["roomName"];
            this.roomIcon = (Sprite)this.roomInfo.CustomProperties["roomIcon"];
            this.roomMode = (string)this.roomInfo.CustomProperties["roomMode"];
            this.roomHostName = (string)this.roomInfo.CustomProperties["roomHostName"];
        }
    }

}
