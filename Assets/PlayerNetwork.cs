using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkVariable<PlayerNetworkData> netState = new NetworkVariable<PlayerNetworkData>(writePerm: NetworkVariableWritePermission.Owner);
    private Vector3 vel;
    private float rotVel;
    private float cheapInterpolationTime = 0.1f;

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            netState.Value = new PlayerNetworkData()
            {
                Position = transform.position,
                Rotation = transform.rotation.eulerAngles
            };// If the view is owner, then set the network updated transform from own transform
        }
        else
        {
            // If the view is not owner, then set the transform from the network updated transform
            transform.position = Vector3.SmoothDamp(transform.position, netState.Value.Position, ref vel, cheapInterpolationTime);
            transform.rotation = Quaternion.Euler(
                Mathf.SmoothDampAngle(transform.rotation.eulerAngles.x, netState.Value.Rotation.x, ref rotVel, cheapInterpolationTime),
                Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, netState.Value.Rotation.y, ref rotVel, cheapInterpolationTime),
                Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, netState.Value.Rotation.z, ref rotVel, cheapInterpolationTime)
                );
        }
    }
    struct PlayerNetworkData : INetworkSerializable
    {
        private float x, y, z;
        private short xRot, yRot, zRot;
        private short yScale;
        internal Vector3 Position
        {
            get => new Vector3(x, y, z);
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }
        internal Vector3 Rotation
        {
            get => new Vector3(xRot, yRot, zRot);
            set
            {
                yRot = (short)value.y;
                xRot = (short)value.x;
                zRot = (short)value.z;
            }
        }
        internal Vector3 Scale
        {
            get => new Vector3(1, yScale, 1);
            set => yScale = (short)value.y;
        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref x);
            serializer.SerializeValue(ref y);
            serializer.SerializeValue(ref z);
            serializer.SerializeValue(ref xRot);
            serializer.SerializeValue(ref yRot);
            serializer.SerializeValue(ref zRot);
            serializer.SerializeValue(ref yScale);
        }
    }
}
