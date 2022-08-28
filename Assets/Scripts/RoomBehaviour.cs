using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    public GameObject[] walls;
    public bool[] testStatus;
    // Start is called before the first frame update
    void Start()
    {
        UpdateRoom(testStatus);
    }

    void UpdateRoom(bool[] status)
    {
        for(int i = 0; i < status.Length; i++)
        {
            walls[i].SetActive(false);
        }
    }
}
