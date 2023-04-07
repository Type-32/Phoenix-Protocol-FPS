using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RMI : MonoBehaviour
{
    public GameObject roomManagerPrefab;
    void Awake()
    {
        if (FindObjectsOfType<RoomManager>().Length < 1)
        {
            Instantiate(roomManagerPrefab);
        }
    }
}
