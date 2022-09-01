using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] Text roomName;
    [SerializeField] Image roomIcon;
    [SerializeField] Text roomMode;
    [SerializeField] Text roomHostName;
    public RoomInfo roomInfo;
    public void SetUp(RoomInfo info)
    {
        roomInfo = info;
        roomName.text = info.Name;
    }
    public void OnClick()
    {
        Launcher.Instance.JoinRoom(roomInfo);
    }
}
