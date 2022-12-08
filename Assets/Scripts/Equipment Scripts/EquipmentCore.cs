using System.Runtime.InteropServices.ComTypes;
using System.ComponentModel;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class EquipmentCore : MonoBehaviour
{
    public EquipmentManager equip;
    public bool throwState = false;
    public bool throwAvailable = true;
    public IEnumerator ReturnToOriginalEquipment(float time)
    {
        yield return new WaitForSeconds(time);
        equip.holder.EquipItem(equip.holder.previousIndex);
    }
    public void EquipmentCoreFunc()
    {
        if (((Input.GetButton("Fire1") || Input.GetKey(KeyCode.Slash)) || (equip.inEquipmentState == 0 ? Input.GetKey(KeyCode.E) : Input.GetKey(KeyCode.G))) && equip.stats.interactionEnabled && equip.stats.count > 0 && throwAvailable && !equip.stats.isSliding && !equip.stats.isSprinting)
        {
            throwState = true;
        }
        if (((Input.GetButtonUp("Fire1") || Input.GetKeyUp(KeyCode.Slash)) || (equip.inEquipmentState == 0 ? Input.GetKeyUp(KeyCode.E) : Input.GetKeyUp(KeyCode.G))) && equip.stats.interactionEnabled && equip.stats.count > 0 && throwState)
        {
            Throw();
            equip.animate.animate.SetTrigger("isThrowing");
            StartCoroutine(ReturnToOriginalEquipment(0.25f));
        }
    }
    public void Throw()
    {
        throwState = throwAvailable = false;
        RaycastHit ht;
        Physics.Raycast(equip.fpsCam.transform.position, equip.fpsCam.transform.forward, out ht, 2f);
        GameObject projectile = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", equip.stats.photonGameObjectString), equip.stats.attackPoint.position, equip.fpsCam.transform.rotation);
        projectile.GetComponent<ProjectileBehaviour>().pv = equip.player.pv;
        projectile.GetComponent<ProjectileBehaviour>().SetData(equip.stats.equipmentData.damage, equip.stats.equipmentData.isExplosive, equip.stats.equipmentData.explosionOnImpact, equip.stats.equipmentData.explosionDelay, equip.stats.equipmentData.influenceForce, equip.stats.explosionEffect, equip.stats.equipmentData.areaOfInfluence, equip.stats.equipmentData.GlobalEquipmentIndex, equip.stats.equipmentData.effectString, equip.stats.equipmentData.dealDamage);
        Rigidbody projBody = projectile.GetComponent<Rigidbody>();
        Vector3 forceDir = equip.fpsCam.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(equip.fpsCam.transform.position, equip.fpsCam.transform.forward, out hit, 500f))
        {
            forceDir = (hit.point - equip.stats.attackPoint.position).normalized;
        }
        Vector3 additionalForce = forceDir * equip.stats.throwForce + transform.up * equip.stats.throwUpwardForce;
        projBody.AddForce(additionalForce, ForceMode.Impulse);
        equip.stats.count--;
        Invoke(nameof(ResetThrow), equip.stats.recoveryTime);
    }
    void ResetThrow()
    {
        throwAvailable = true;
    }
}
