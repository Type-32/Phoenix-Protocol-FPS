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
    [SerializeField] Text roomName, mapText, gamemodeText, maxPlayersText, roomCodeText, visibilityText, primaryText, secondaryText, downedStateText;
    void Start()
    {
        FileOps<UserDataJSON>.OperatedFile += DetectIfLoadoutModified;
    }
    public void SetMapView(MapPreviewInfo mpi)
    {
        if (mapIcon != null) mapIcon.sprite = mpi.mapIcon;
        if (mapText != null) mapText.text = mpi.mapName;
    }
    public void SetStatisticsView(StatisticsPreviewInfo spi)
    {
        if (gamemodeText != null) gamemodeText.text = spi.gamemode;
        if (maxPlayersText != null) maxPlayersText.text = spi.maxPlayers.ToString() + " Players Maximum";
        if (roomCodeText != null) roomCodeText.text = "Room Code " + spi.roomCode.ToString();
        if (visibilityText != null) visibilityText.text = spi.visibility ? "Public" : "Private";
        if (downedStateText != null) downedStateText.text = spi.allowPlayerDownedState ? "Allows Downed Players" : "Disabled Downed Players";
    }
    public void SetLoadoutView(LoadoutPreviewInfo lpi)
    {
        if (primaryText != null) primaryText.text = lpi.w_Name1;
        if (secondaryText != null) secondaryText.text = lpi.w_Name2;
        if (primaryIcon != null) primaryIcon.sprite = lpi.w_Icon1;
        if (secondaryIcon != null) secondaryIcon.sprite = lpi.w_Icon2;
        if (EquipmentIcon1 != null) EquipmentIcon1.sprite = lpi.e_Icon1;
        if (EquipmentIcon2 != null) EquipmentIcon2.sprite = lpi.e_Icon2;
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
