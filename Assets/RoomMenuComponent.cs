using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InfoTypes.InRoomPreview;
using UserConfiguration;
using PrototypeLib.Modules.FileOperations.IO;
public class RoomMenuComponent : MonoBehaviour
{
    [SerializeField] Image mapIcon, primaryIcon, secondaryIcon, EquipmentIcon1, EquipmentIcon2;
    [SerializeField] Text roomName, mapText, gamemodeText, maxPlayersText, roomCodeText, visibilityText, primaryText, secondaryText;
    void Start()
    {
        FileOps<UserDataJSON>.OperatedFile += DetectIfLoadoutModified;
    }
    public void SetMapView(MapPreviewInfo mpi)
    {
        mapIcon.sprite = mpi.mapIcon;
        mapText.text = mpi.mapName;
    }
    public void SetStatisticsView(StatisticsPreviewInfo spi)
    {
        gamemodeText.text = spi.gamemode;
        maxPlayersText.text = spi.maxPlayers.ToString() + " Players Maximum";
        roomCodeText.text = "Room Code " + spi.roomCode.ToString();
        visibilityText.text = spi.visibility ? "Public" : "Private";
    }
    public void SetLoadoutView(LoadoutPreviewInfo lpi)
    {
        primaryText.text = lpi.w_Name1;
        secondaryText.text = lpi.w_Name2;
        primaryIcon.sprite = lpi.w_Icon1;
        secondaryIcon.sprite = lpi.w_Icon2;
        EquipmentIcon1.sprite = lpi.e_Icon1;
        EquipmentIcon2.sprite = lpi.e_Icon2;
    }
    public void SetRoomInfoPreview(string roomName, MapPreviewInfo mpi, StatisticsPreviewInfo spi, LoadoutPreviewInfo lpi)
    {
        this.roomName.text = roomName;
        SetMapView(mpi);
        SetStatisticsView(spi);
        SetLoadoutView(lpi);
    }
    private void DetectIfLoadoutModified(string strPath, UserDataJSON tmp)
    {
        if (strPath == UserSystem.UserDataPath)
        {
            SetLoadoutView(new LoadoutPreviewInfo(GlobalDatabase.Instance.allWeaponDatas[tmp.LoadoutData.Slots[tmp.LoadoutData.SelectedSlot].Weapon1], GlobalDatabase.Instance.allWeaponDatas[tmp.LoadoutData.Slots[tmp.LoadoutData.SelectedSlot].Weapon2], GlobalDatabase.Instance.allEquipmentDatas[tmp.LoadoutData.Slots[tmp.LoadoutData.SelectedSlot].Equipment1], GlobalDatabase.Instance.allEquipmentDatas[tmp.LoadoutData.Slots[tmp.LoadoutData.SelectedSlot].Equipment2]));
        }
    }
}
