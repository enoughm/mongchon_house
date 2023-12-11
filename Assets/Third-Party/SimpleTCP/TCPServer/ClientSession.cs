using System;
using System.Net;
using System.Threading;
using ServerCore;
using UnityEngine;

public class ClientSession : PacketSession
{
    public int SessionId { get; set; }
    public GameRoom Room { get; set; }
    
    public override void OnConnected(EndPoint endPoint)
    {
        Debug.Log($"[Server] OnConnected: {endPoint}");
        TCPServerManager.Room.Enter(this);
    }
    
    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        Debug.Log($"[Server] OnRecvPacket");
        ServerPacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnSend(int numOfBytes)
    {
        Debug.Log($"[Server] Transferred bytes: {numOfBytes}");
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        SessionManager.Instance.Remove(this);
        if (Room != null)
        {
            Room.Leave(this);
            Room = null;
        }
        Debug.Log($"[Server] OnDisconnected: {endPoint}");
    }
}