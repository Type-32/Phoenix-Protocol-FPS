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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.pv.IsMine) return;
        var lagDistance = realPosition - transform.position;
        if(lagDistance.magnitude > 5f)
        {
            //transform.position = realPosition;
            lagDistance = Vector3.zero;
        }

        transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
        transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
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
}
