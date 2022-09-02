using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    PhotonView pv;
    GameObject controller;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (pv.IsMine)
        {
            CreateController();
        }
    }
    void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetRandomSpawnpoint();
        Debug.Log("Instantiating Player Controller");
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnpoint.position, spawnpoint.rotation, 0, new object[] {pv.ViewID});
    }
    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}
