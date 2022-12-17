using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using Photon.Realtime;

public class NetworkPlayer : MonoBehaviourPun, IPunObservable
{
    [SerializeField] PlayerControllerManager player;
    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;

    Vector3 camRealPos = Vector3.zero;
    Quaternion camRealRot = Quaternion.identity;
    void FixedUpdate()
    {
        if (!player.pv.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
            player.fpsCam.gameObject.transform.rotation = Quaternion.Lerp(player.fpsCam.gameObject.transform.rotation, camRealRot, 0.1f);
            /*
            if (player.fpsCam.gameObject.transform.localRotation.x <= 0.3f && player.fpsCam.gameObject.transform.localRotation.x >= -0.2f)
            {
                Debug.LogWarning("x: " + player.fpsCam.gameObject.transform.localRotation.x + " y: " + player.fpsCam.gameObject.transform.localRotation.y + " z: " + player.fpsCam.gameObject.transform.localRotation.z);
                player.fpsCam.gameObject.transform.rotation = Quaternion.Lerp(player.fpsCam.gameObject.transform.rotation, camRealRot, 0.1f);
            }
            else
            {
                Debug.LogWarning("Is Rotating Body Joint");
                player.bodyRotatePoint.transform.rotation = Quaternion.Lerp(player.bodyRotatePoint.transform.rotation, Quaternion.Euler((camRealRot.x - player.fpsCam.gameObject.transform.rotation.x), 0f, 0f), 0.1f);
            }*/
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            //stream.SendNext(player.holder.transform.position);
            stream.SendNext(player.fpsCam.gameObject.transform.rotation);
        }
        else
        {
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            //camRealPos = (Vector3)stream.ReceiveNext();
            camRealRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
