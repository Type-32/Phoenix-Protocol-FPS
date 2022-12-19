using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] Text playerName;
    [SerializeField] Text playerLevel;
    Player player;
    public void SetUp(Player _player)
    {
        player = _player;
        playerName.text = _player.NickName;
        playerLevel.text = ((int)_player.CustomProperties["userLevel"]).ToString();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }
    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
