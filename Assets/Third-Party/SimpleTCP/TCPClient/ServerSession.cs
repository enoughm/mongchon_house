using System;
using System.Net;
using ServerCore;
using UnityEngine;


public class ServerSession : PacketSession
{
    public override void OnConnected(EndPoint endPoint)
    {
        Debug.Log($"[Client] OnConnected: {endPoint}");
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        Debug.Log($"[Client] OnRecvPacket");
        ClientPacketManager.Instance.OnRecvPacket(this, buffer, (s, p) => PacketQueue.Instance.Push(p));
    }

    public override void OnSend(int numOfBytes)
    {
        Debug.Log($"[Client] OnSend Transferred bytes: {numOfBytes}");
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Debug.Log($"[Client] OnDisconnected: {endPoint}");
    }
}