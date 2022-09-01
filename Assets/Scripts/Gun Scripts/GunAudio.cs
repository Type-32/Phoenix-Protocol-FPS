using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAudio : MonoBehaviour
{
    public GunManager gun;

    public void PlayGunSound()
    {
        if (gun.stats.weaponData.fireClips.Count != 0) gun.fireSoundSource.clip = gun.stats.weaponData.fireClips.Count - 1 > 0 ? gun.stats.weaponData.fireClips[Random.Range(0, gun.stats.weaponData.fireClips.Count - 1)] : gun.stats.weaponData.fireClips[0];
        if (gun.stats.weaponData.mechClips.Count != 0) gun.mechSoundSource.clip = gun.stats.weaponData.mechClips.Count - 1 > 0 ? gun.stats.weaponData.mechClips[Random.Range(0, gun.stats.weaponData.mechClips.Count - 1)] : gun.stats.weaponData.mechClips[0];
        if (gun.stats.weaponData.bassClips.Count != 0) gun.bassSoundSource.clip = gun.stats.weaponData.bassClips.Count - 1 > 0 ? gun.stats.weaponData.bassClips[Random.Range(0, gun.stats.weaponData.bassClips.Count - 1)] : gun.stats.weaponData.bassClips[0];
        if (gun.bassSoundSource.clip == null) Debug.LogWarning("No Gunfire Bass Sound Source Present for Gun " + gun.stats.weaponData.itemName);
        if (gun.mechSoundSource.clip == null) Debug.LogWarning("No Gunfire Mech Sound Source Present for Gun " + gun.stats.weaponData.itemName);
        if (gun.fireSoundSource.clip == null) Debug.LogWarning("No Gunfire Fire Sound Source Present for Gun " + gun.stats.weaponData.itemName);
        if (gun.stats.weaponData.bassClips.Count != 0) gun.bassSoundSource.Play();
        if (gun.stats.weaponData.fireClips.Count != 0) gun.fireSoundSource.Play();
        if (gun.stats.weaponData.mechClips.Count != 0) gun.mechSoundSource.Play();
    }
}
