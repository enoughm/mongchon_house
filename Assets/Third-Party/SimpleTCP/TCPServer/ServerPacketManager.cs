using ServerCore;
using System;
using System.Collections.Generic;

public class ServerPacketManager {

    #region Singleton
    static ServerPacketManager instance = new ServerPacketManager();
    public static ServerPacketManager Instance { get { return instance; } }
    #endregion

    ServerPacketManager() {
        Register();
    }
    
    Dictionary<int, Func<PacketSession, ArraySegment<byte>, IPacket>> makeFunc = new Dictionary<int, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    Dictionary<int, Action<PacketSession, IPacket>> handler = new Dictionary<int, Action<PacketSession, IPacket>>();

    public void Register() {
         makeFunc.Add((int)PacketID.Example, MakePacket<PacketExample>);
        handler.Add((int)PacketID.Example, ServerPacketHandler.PacketExampleHandler);

         makeFunc.Add((int)PacketID.Simple, MakePacket<PacketSimple>);
        handler.Add((int)PacketID.Simple, ServerPacketHandler.PacketSimpleHandler);
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null) {
        int count = 0;
        int size = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
        count += sizeof(int);
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;


        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if (makeFunc.TryGetValue(id, out func)) {
            IPacket packet = func.Invoke(session, buffer);
            if (onRecvCallback != null) {
                onRecvCallback.Invoke(session, packet);
            }
            else {
                HandlePacket(session, packet);
            }
        }
    }

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new() {
        T pkt = new T();
        pkt.Read(buffer);
        return pkt;
    }

    public void HandlePacket(PacketSession session, IPacket packet) {
        Action<PacketSession, IPacket> action = null;
        if (handler.TryGetValue(packet.PacketId, out action))
            action.Invoke(session, packet);
    }
}
