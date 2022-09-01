using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public ItemAnimation anim;
    public ItemStats stats;
    public PlayerControllerManager player;
    private void Start()
    {
        player = FindObjectOfType<PlayerControllerManager>();
    }
    // Start is called before the first frame update

}
