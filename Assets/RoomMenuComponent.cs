using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InfoTypes.InRoomPreview;

public class RoomMenuComponent : MonoBehaviour
{
    [SerializeField] Image mapIcon, primaryIcon, secondaryIcon, EquipmentIcon1, EquipmentIcon2;
    [SerializeField] Text mapText, gamemodeText, maxPlayersText, roomCodeText, visibilityText, primaryText, secondaryText;
    private void SetMapView(MapPreviewInfo mpi)
    {
        mapIcon.sprite = mpi.mapIcon;
        mapText.text = mpi.mapName;
    }
    private void SetStatisticsView(StatisticsPreviewInfo spi)
    {
        gamemodeText.text = spi.gamemode;
        maxPlayersText.text = spi.maxPlayers.ToString();
        roomCodeText.text = "Room Code " + spi.roomCode.ToString();
        visibilityText.text = spi.visibility ? "Public" : "Private";
    }
    private void SetLoadoutView(LoadoutPreviewInfo lpi)
    {
        primaryText.text = lpi.w_Name1;
        secondaryText.text = lpi.w_Name2;
        primaryIcon.sprite = lpi.w_Icon1;
        secondaryIcon.sprite = lpi.w_Icon2;
        EquipmentIcon1.sprite = lpi.e_Icon1;
        EquipmentIcon2.sprite = lpi.e_Icon2;
    }
    public void SetRoomInfoPreview(MapPreviewInfo mpi, StatisticsPreviewInfo spi, LoadoutPreviewInfo lpi)
    {
        SetMapView(mpi);
        SetStatisticsView(spi);
        SetLoadoutView(lpi);
    }
}
