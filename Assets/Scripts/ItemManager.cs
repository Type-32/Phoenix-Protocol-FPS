using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public ItemAnimation anim;
    public ItemStats stats;
    public PlayerManager player;
    private void Start()
    {
        player = FindObjectOfType<PlayerManager>();
    }
    // Start is called before the first frame update

}
