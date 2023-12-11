using System;
using UnityEngine;


class ClientPacketHandler
{
    public static Action<PacketSimple> OnPacketSimple;
    public static void PacketExampleHandler(PacketSession session, IPacket packet) {
        PacketExample pkt = packet as PacketExample;
        ServerSession serverSession = session as ServerSession;
        Debug.Log("[Client] PacketExampleHandler");
    }

    public static void PacketSimpleHandler(PacketSession session, IPacket packet)
    {
        PacketSimple pkt = packet as PacketSimple;
        ServerSession serverSession = session as ServerSession;
        Debug.Log("[Client] PacketSimpleHandler");
        Loom.QueueOnMainThread(()=>
        {
            OnPacketSimple?.Invoke(pkt);
        });
    }
}