using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCosmeticManager : MonoBehaviour
{
    public PlayerControllerManager player;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public bool ApplyAppearance(PlayerCosmeticData data)
    {
        if (data.appliedPart == PlayerCosmeticData.CosmeticPart.Head)
        {

        }
        return false;
    }
}
