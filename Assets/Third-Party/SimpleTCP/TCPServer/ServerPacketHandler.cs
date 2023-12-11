using System;
using UnityEngine;

class ServerPacketHandler
{
    public static Action<PacketSimple> onPacketSimple;
    
    public static void PacketExampleHandler(PacketSession session, IPacket packet) {
        PacketExample pkt = packet as PacketExample;
        ClientSession clientSession = session as ClientSession;
        Debug.Log("[Server] PacketExampleHandler");
    }

    public static void PacketSimpleHandler(PacketSession session, IPacket packet)
    {
        PacketSimple pkt = packet as PacketSimple;
        ClientSession clientSession = session as ClientSession;
        Debug.Log("[Server] PacketSimpleHandler");
        
        Loom.QueueOnMainThread(() =>
        {
            onPacketSimple?.Invoke(pkt);
        });
    }
}