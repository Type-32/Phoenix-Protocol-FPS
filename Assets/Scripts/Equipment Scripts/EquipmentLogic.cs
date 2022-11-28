using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentLogic : MonoBehaviour
{
    public EquipmentManager equip;
    // Update is called once per frame
    /*
    void Update()
    {
        if (!equip.stats.equipInteractionEnabled) return;
    }*/
    public void EquipmentGeneralLogic()
    {
        EquipmentMovementLogic();
    }
    void EquipmentMovementLogic()
    {
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)) equip.stats.isWalking = true;
        else equip.stats.isWalking = false;

        if (Input.GetKey("left shift") && !equip.stats.isHolding && !equip.player.stats.isCrouching && equip.stats.isWalking) equip.stats.isSprinting = true;
        else equip.stats.isSprinting = false;
    }
}
