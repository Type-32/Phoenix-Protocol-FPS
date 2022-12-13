using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAudio : MonoBehaviour
{
    public GunManager gun;

    public void PlayGunSound(bool isSilenced)
    {
        if (gun.stats.weaponData.fireClips.Count != 0 && !isSilenced) gun.fireSoundSource.clip = gun.stats.weaponData.fireClips.Count - 1 > 0 ? gun.stats.weaponData.fireClips[Random.Range(0, gun.stats.weaponData.fireClips.Count - 1)] : gun.stats.weaponData.fireClips[0];
        if (gun.stats.weaponData.silencedFireClips.Count != 0 && isSilenced) gun.fireSoundSource.clip = gun.stats.weaponData.silencedFireClips.Count - 1 > 0 ? gun.stats.weaponData.silencedFireClips[Random.Range(0, gun.stats.weaponData.silencedFireClips.Count - 1)] : gun.stats.weaponData.silencedFireClips[0];
        if (gun.stats.weaponData.mechClips.Count != 0) gun.mechSoundSource.clip = gun.stats.weaponData.mechClips.Count - 1 > 0 ? gun.stats.weaponData.mechClips[Random.Range(0, gun.stats.weaponData.mechClips.Count - 1)] : gun.stats.weaponData.mechClips[0];
        if (gun.stats.weaponData.bassClips.Count != 0) gun.bassSoundSource.clip = gun.stats.weaponData.bassClips.Count - 1 > 0 ? gun.stats.weaponData.bassClips[Random.Range(0, gun.stats.weaponData.bassClips.Count - 1)] : gun.stats.weaponData.bassClips[0];
        if (gun.bassSoundSource.clip == null) Debug.LogWarning("No Gunfire Bass Sound Source Present for Gun " + gun.stats.weaponData.itemName);
        if (gun.mechSoundSource.clip == null) Debug.LogWarning("No Gunfire Mech Sound Source Present for Gun " + gun.stats.weaponData.itemName);
        if (gun.fireSoundSource.clip == null) Debug.LogWarning("No Gunfire Fire Sound Source Present for Gun " + gun.stats.weaponData.itemName);
        if (gun.stats.weaponData.bassClips.Count != 0) gun.bassSoundSource.Play();
        if (gun.stats.weaponData.fireClips.Count != 0 && !isSilenced) gun.fireSoundSource.Play();
        if (gun.stats.weaponData.silencedFireClips.Count != 0 && isSilenced) gun.fireSoundSource.Play();
        if (gun.stats.weaponData.mechClips.Count != 0) gun.mechSoundSource.Play();
    }
    public void PlayNPCGunSound(bool isSilenced)
    {
        if (gun.stats.weaponData.NPC_FireClips.Count != 0 && !isSilenced) gun.fireSoundSource.clip = gun.stats.weaponData.NPC_FireClips.Count - 1 > 0 ? gun.stats.weaponData.NPC_FireClips[Random.Range(0, gun.stats.weaponData.NPC_FireClips.Count - 1)] : gun.stats.weaponData.NPC_FireClips[0];
        if (gun.stats.weaponData.NPC_SilencedFireClips.Count != 0 && isSilenced) gun.fireSoundSource.clip = gun.stats.weaponData.NPC_SilencedFireClips.Count - 1 > 0 ? gun.stats.weaponData.NPC_SilencedFireClips[Random.Range(0, gun.stats.weaponData.NPC_SilencedFireClips.Count - 1)] : gun.stats.weaponData.NPC_SilencedFireClips[0];
        if (gun.stats.weaponData.mechClips.Count != 0) gun.mechSoundSource.clip = gun.stats.weaponData.mechClips.Count - 1 > 0 ? gun.stats.weaponData.mechClips[Random.Range(0, gun.stats.weaponData.mechClips.Count - 1)] : gun.stats.weaponData.mechClips[0];
        if (gun.stats.weaponData.bassClips.Count != 0) gun.bassSoundSource.clip = gun.stats.weaponData.bassClips.Count - 1 > 0 ? gun.stats.weaponData.bassClips[Random.Range(0, gun.stats.weaponData.bassClips.Count - 1)] : gun.stats.weaponData.bassClips[0];
        if (gun.bassSoundSource.clip == null) Debug.LogWarning("No Gunfire Bass Sound Source Present for Gun " + gun.stats.weaponData.itemName);
        if (gun.mechSoundSource.clip == null) Debug.LogWarning("No Gunfire Mech Sound Source Present for Gun " + gun.stats.weaponData.itemName);
        if (gun.fireSoundSource.clip == null) Debug.LogWarning("No Gunfire Fire Sound Source Present for Gun " + gun.stats.weaponData.itemName);
        if (gun.stats.weaponData.bassClips.Count != 0) gun.bassSoundSource.Play();
        if (gun.stats.weaponData.NPC_FireClips.Count != 0 && !isSilenced) gun.fireSoundSource.Play();
        if (gun.stats.weaponData.NPC_SilencedFireClips.Count != 0 && isSilenced) gun.fireSoundSource.Play();
        if (gun.stats.weaponData.mechClips.Count != 0) gun.mechSoundSource.Play();
    }
    public void PlayRechamberSound()
    {
        if (gun.stats.weaponData.rechamberClips.Count != 0) gun.mechSoundSource.clip = gun.stats.weaponData.rechamberClips.Count - 1 > 0 ? gun.stats.weaponData.rechamberClips[Random.Range(0, gun.stats.weaponData.rechamberClips.Count - 1)] : gun.stats.weaponData.rechamberClips[0];
        if (gun.mechSoundSource.clip == null) Debug.LogWarning("No Gunfire Mech Sound Source Present for Gun " + gun.stats.weaponData.itemName);
        if (gun.mechSoundSource.clip != null)
        {
            AudioSource temp = Instantiate(gun.mechSoundSource, gun.mechSoundSource.transform.position, gun.mechSoundSource.transform.rotation, gun.recoilModel.transform).GetComponent<AudioSource>();
            temp.clip = gun.mechSoundSource.clip;
            temp.Play();
            Destroy(temp.gameObject, temp.clip.length);
        }
    }

    #region Single Sounds
    public void PlayRechamberStart()
    {
        if (gun.stats.weaponData.rechamberStart != null)
        {
            gun.mechSoundSource.clip = gun.stats.weaponData.rechamberStart;
            AudioSource temp = Instantiate(gun.mechSoundSource, gun.mechSoundSource.transform.position, gun.mechSoundSource.transform.rotation, gun.recoilModel.transform).GetComponent<AudioSource>();
            temp.clip = gun.mechSoundSource.clip;
            temp.Play();
            Destroy(temp.gameObject, temp.clip.length);
        }
    }
    public void PlayRechamberEnd()
    {
        if (gun.stats.weaponData.rechamberEnd != null)
        {
            gun.mechSoundSource.clip = gun.stats.weaponData.rechamberEnd;
            AudioSource temp = Instantiate(gun.mechSoundSource, gun.mechSoundSource.transform.position, gun.mechSoundSource.transform.rotation, gun.recoilModel.transform).GetComponent<AudioSource>();
            temp.clip = gun.mechSoundSource.clip;
            temp.Play();
            Destroy(temp.gameObject, temp.clip.length);
        }
    }
    public void PlayPullBolt()
    {
        if (gun.stats.weaponData.pullBolt != null)
        {
            gun.mechSoundSource.clip = gun.stats.weaponData.pullBolt;
            AudioSource temp = Instantiate(gun.mechSoundSource, gun.mechSoundSource.transform.position, gun.mechSoundSource.transform.rotation, gun.recoilModel.transform).GetComponent<AudioSource>();
            temp.clip = gun.mechSoundSource.clip;
            temp.Play();
            Destroy(temp.gameObject, temp.clip.length);
        }
    }
    public void PlayPullMagazine()
    {
        if (gun.stats.weaponData.pullMagazine != null)
        {
            gun.mechSoundSource.clip = gun.stats.weaponData.pullMagazine;
            AudioSource temp = Instantiate(gun.mechSoundSource, gun.mechSoundSource.transform.position, gun.mechSoundSource.transform.rotation, gun.recoilModel.transform).GetComponent<AudioSource>();
            temp.clip = gun.mechSoundSource.clip;
            temp.Play();
            Destroy(temp.gameObject, temp.clip.length);
        }
    }
    public void PlayRecoverBolt()
    {
        if (gun.stats.weaponData.recoverBolt != null)
        {
            gun.mechSoundSource.clip = gun.stats.weaponData.recoverBolt;
            AudioSource temp = Instantiate(gun.mechSoundSource, gun.mechSoundSource.transform.position, gun.mechSoundSource.transform.rotation, gun.recoilModel.transform).GetComponent<AudioSource>();
            temp.clip = gun.mechSoundSource.clip;
            temp.Play();
            Destroy(temp.gameObject, temp.clip.length);
        }
    }
    public void PlayInsertMagazine()
    {
        if (gun.stats.weaponData.insertMagazine != null)
        {
            gun.mechSoundSource.clip = gun.stats.weaponData.insertMagazine;
            AudioSource temp = Instantiate(gun.mechSoundSource, gun.mechSoundSource.transform.position, gun.mechSoundSource.transform.rotation, gun.recoilModel.transform).GetComponent<AudioSource>();
            temp.clip = gun.mechSoundSource.clip;
            temp.Play();
            Destroy(temp.gameObject, temp.clip.length);
        }
    }
    public void PlayFullBolt()
    {
        if (gun.stats.weaponData.fullBolt != null)
        {
            gun.mechSoundSource.clip = gun.stats.weaponData.fullBolt;
            AudioSource temp = Instantiate(gun.mechSoundSource, gun.mechSoundSource.transform.position, gun.mechSoundSource.transform.rotation, gun.recoilModel.transform).GetComponent<AudioSource>();
            temp.clip = gun.mechSoundSource.clip;
            temp.Play();
            Destroy(temp.gameObject, temp.clip.length);
        }
    }
    public void PlaySlapGun()
    {
        if (gun.stats.weaponData.slapGun != null)
        {
            gun.mechSoundSource.clip = gun.stats.weaponData.slapGun;
            AudioSource temp = Instantiate(gun.mechSoundSource, gun.mechSoundSource.transform.position, gun.mechSoundSource.transform.rotation, gun.recoilModel.transform).GetComponent<AudioSource>();
            temp.clip = gun.mechSoundSource.clip;
            temp.Play();
            Destroy(temp.gameObject, temp.clip.length);
        }
    }
    public void PlayInsertRound()
    {
        if (gun.stats.weaponData.insertRound != null)
        {
            gun.mechSoundSource.clip = gun.stats.weaponData.insertRound;
            AudioSource temp = Instantiate(gun.mechSoundSource, gun.mechSoundSource.transform.position, gun.mechSoundSource.transform.rotation, gun.recoilModel.transform).GetComponent<AudioSource>();
            temp.clip = gun.mechSoundSource.clip;
            temp.Play();
            Destroy(temp.gameObject, temp.clip.length);
        }
    }
    public void PlayAimIn()
    {
        if (gun.stats.weaponData.aimIn != null)
        {
            gun.mechSoundSource.clip = gun.stats.weaponData.aimIn;
            AudioSource temp = Instantiate(gun.mechSoundSource, gun.mechSoundSource.transform.position, gun.mechSoundSource.transform.rotation, gun.recoilModel.transform).GetComponent<AudioSource>();
            temp.clip = gun.mechSoundSource.clip;
            temp.Play();
            Destroy(temp.gameObject, temp.clip.length);
        }
    }
    public void PlayAimOut()
    {
        if (gun.stats.weaponData.aimOut != null)
        {
            gun.mechSoundSource.clip = gun.stats.weaponData.aimOut;
            AudioSource temp = Instantiate(gun.mechSoundSource, gun.mechSoundSource.transform.position, gun.mechSoundSource.transform.rotation, gun.recoilModel.transform).GetComponent<AudioSource>();
            temp.clip = gun.mechSoundSource.clip;
            temp.Play();
            Destroy(temp.gameObject, temp.clip.length);
        }
    }
    public void PlayRaiseGun()
    {
        if (gun.stats.weaponData.aimOut != null)
        {
            gun.mechSoundSource.clip = gun.stats.weaponData.raiseGun;
            AudioSource temp = Instantiate(gun.mechSoundSource, gun.mechSoundSource.transform.position, gun.mechSoundSource.transform.rotation, gun.recoilModel.transform).GetComponent<AudioSource>();
            temp.clip = gun.mechSoundSource.clip;
            temp.Play();
            Destroy(temp.gameObject, temp.clip.length);
        }
    }
    #endregion
}
