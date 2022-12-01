using System.IO;
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
    string effectFileString = "";

    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;
    public void SetData(float damage, bool doesExplosion, bool explosionOnImpact, float explosionDelay, float explosionForce, GameObject obj, float range, int equipmentIndex, string effectString)
    {
        this.explosionOnImpact = explosionOnImpact;
        this.damage = damage;
        this.explosionDelay = explosionDelay;
        this.doesExplosion = doesExplosion;
        explosionEffect = obj;
        this.range = range;
        time = explosionDelay;
        GlobalEquipmentIndex = equipmentIndex;
        effectFileString = effectString;
    }
    void Start()
    {
        body = GetComponent<Rigidbody>();
        time = explosionDelay;
        if (pv == null)
        {
            pv = GetComponent<PhotonView>();
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (!pv.IsMine) return;
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
        InstantiateExplosionEffect(transform.position, Quaternion.Euler(Vector3.up.x, Vector3.up.y, Vector3.up.z));
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
        PhotonNetwork.Destroy(gameObject);
    }
    void FixedUpdate()
    {
        if (!pv.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
        }
        else
        {
            time -= time > 0 ? Time.fixedDeltaTime : 0f;
            CheckExplosion(doesExplosion, explosionOnImpact);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
        }
    }
    public void InstantiateExplosionEffect(Vector3 _pos, Quaternion _rot)
    {
        PlayerManager[] li = FindObjectsOfType<PlayerManager>();
        for (int i = 0; i < li.Length; i++)
        {
            if (li[i].pv.ViewID == pv.ViewID)
            {
                li[i].InstantiateExplosionEffect(_pos, _rot);
            }
        }
    }
}
