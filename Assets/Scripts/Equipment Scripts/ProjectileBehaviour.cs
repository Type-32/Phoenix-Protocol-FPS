using System.Net.NetworkInformation;
using System.Transactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ProjectileBehaviour : MonoBehaviourPun, IPunObservable
{
    public PhotonView pv;
    private GameObject explosionEffect;
    private Rigidbody body;
    private bool targetHit;
    private float damage;
    private bool explosionOnImpact;
    private float explosionDelay;
    private bool doesExplosion;
    private float range;
    private float explosionForce;
    private int GlobalEquipmentIndex;
    float time;
    public void SetData(float damage, bool doesExplosion, bool explosionOnImpact, float explosionDelay, float explosionForce, GameObject obj, float range, int equipmentIndex)
    {
        this.explosionOnImpact = explosionOnImpact;
        this.damage = damage;
        this.explosionDelay = explosionDelay;
        this.doesExplosion = doesExplosion;
        explosionEffect = obj;
        this.range = range;
        time = explosionDelay;
        GlobalEquipmentIndex = equipmentIndex;
    }
    void Start()
    {
        body = GetComponent<Rigidbody>();
        time = explosionDelay;
    }
    void OnCollisionEnter(Collision other)
    {
        if (targetHit) return;
        else targetHit = true;
        CheckExplosion(doesExplosion, explosionOnImpact);
    }
    void CheckExplosion(bool check, bool explodeOnImpact)
    {
        if (!check) return;
        else
        {
            if (explodeOnImpact)
            {
                Explode();
            }
            else
            {
                if (time <= 0) Explode();
            }
        }
    }
    public virtual void Explode()
    {
        RaycastHit c;
        Quaternion rot;
        bool flag = false;
        if (Physics.Raycast(transform.position, Vector3.down, out c, 3f))
        {
            rot = Quaternion.LookRotation(-c.normal, Vector3.up);
            flag = true;
        }
        else
        {
            flag = false;
        }
        GameObject tmp = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(tmp, 8f);
        Collider[] includedObjects = Physics.OverlapSphere(transform.position, range);
        for (int i = 0; i < includedObjects.Length; i++)
        {
            if (includedObjects[i].gameObject == gameObject)
            {
                //leave blank
            }
            else
            {
                if (includedObjects[i].GetComponent<IDamagable>() != null)
                {
                    includedObjects[i].GetComponent<IDamagable>().TakeDamage(damage, false, transform.position, transform.rotation, GlobalEquipmentIndex, false);
                }
                if (includedObjects[i].GetComponent<Rigidbody>() != null)
                {
                    Vector3 objectPos = includedObjects[i].transform.position;
                    Vector3 forceDirection = (objectPos - transform.position).normalized;
                    includedObjects[i].GetComponent<Rigidbody>().AddForceAtPosition(forceDirection * explosionForce + Vector3.up * explosionForce, transform.position, ForceMode.Impulse);
                }
            }
        }
        Destroy(gameObject);
    }
    void FixedUpdate()
    {
        time -= time > 0 ? Time.fixedDeltaTime : 0f;
        CheckExplosion(doesExplosion, explosionOnImpact);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
